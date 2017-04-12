using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Layouts.Helpers;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;
using SimpleCommerce.Models;
using SimpleCommerce.Services;
using SimpleCommerce.ViewModels;

namespace SimpleCommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrchardServices _services;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerService _customerService;
        private readonly IMembershipService _membershipService;
        private readonly IShoppingCart _shoppingCart;
        private Localizer T { get; set; }

        public CheckoutController(IOrchardServices services, IAuthenticationService authenticationService, ICustomerService customerService, IMembershipService membershipService, IShoppingCart shoppingCart) {
            _services = services;
            _authenticationService = authenticationService;
            _customerService = customerService;
            _membershipService = membershipService;
            _shoppingCart = shoppingCart;
        }

        [Themed]
        public ActionResult SignupOrLogin()
        {
            if (_authenticationService.GetAuthenticatedUser() != null)
                return RedirectToAction("SelectAddress");

            return new ShapeResult(this, _services.New.Checkout_SignupOrLogin());
        }

        [Themed]
        public ActionResult Signup()
        {
            var shape = _services.New.Checkout_Signup();
            return new ShapeResult(this, shape);
        }

        [HttpPost]
        public ActionResult Signup(SignupViewModel signup)
        {
            if (!ModelState.IsValid)
                return new ShapeResult(this, _services.New.Checkout_Signup(Signup: signup));

            var customer = _customerService.CreateCustomer(signup.Email, signup.Password);
            customer.FirstName = signup.FirstName;
            customer.LastName = signup.LastName;
            customer.Title = signup.Title;

            _authenticationService.SignIn(customer.User, true);

            return RedirectToAction($"SelectAddress");
        }

        [Themed]
        public ActionResult Login()
        {
            var shape = _services.New.Checkout_Login();
            return new ShapeResult(this, shape);
        }

        [Themed, HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            var user = _membershipService.ValidateUser(login.Email, login.Password);

            if (user == null)
            {
                ModelState.AddModelError("Email", T("Incorrect username/password combination").ToString());
            }

            if (!ModelState.IsValid)
            {
                var shape = _services.New.Checkout_Login(Login: login);
                return new ShapeResult(this, shape);
            }

            _authenticationService.SignIn(user, login.CreatePersistentCookie);

            return RedirectToAction("SelectAddress");
        }

        [Themed]
        public ActionResult SelectAddress()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
                throw new OrchardSecurityException(T("Login required"));

            var customer = currentUser.ContentItem.As<CustomerPart>();
            var invoiceAddress = _customerService.GetAddress(customer.Id, "InvoiceAddress");
            var shippingAddress = _customerService.GetAddress(customer.Id, "ShippingAddress");

            var addressesViewModel = new AddressesViewModel
            {
                InvoiceAddress = MapAddress(invoiceAddress),
                ShippingAddress = MapAddress(shippingAddress)
            };

            var shape = _services.New.Checkout_SelectAddress(Addresses: addressesViewModel);
            if (string.IsNullOrWhiteSpace(addressesViewModel.InvoiceAddress.Name))
                addressesViewModel.InvoiceAddress.Name = $"{customer.Title} {customer.FirstName} {customer.LastName}";
            return new ShapeResult(this, shape);
        }

        [Themed, HttpPost]
        public ActionResult SelectAddress(AddressesViewModel addresses)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
                throw new OrchardSecurityException(T("Login required"));

            if (!ModelState.IsValid)
            {
                return new ShapeResult(this, _services.New.Checkout_SelectAddress(Addresses: addresses));
            }

            var customer = currentUser.ContentItem.As<CustomerPart>();
            MapAddress(addresses.InvoiceAddress, "InvoiceAddress", customer);
            MapAddress(addresses.ShippingAddress, "ShippingAddress", customer);

            return RedirectToAction("Summary");
        }

        private AddressViewModel MapAddress(AddressPart addressPart)
        {
            dynamic address = addressPart;
            var addressViewModel = new AddressViewModel();

            if (addressPart != null)
            {
                addressViewModel.Name = address.Name.Value;
                addressViewModel.AddressLine1 = address.AddressLine1.Value;
                addressViewModel.AddressLine2 = address.AddressLine2.Value;
                addressViewModel.Zipcode = address.Zipcode.Value;
                addressViewModel.City = address.City.Value;
                addressViewModel.Country = address.Country.Value;
            }

            return addressViewModel;
        }

        private AddressPart MapAddress(AddressViewModel source, string addressType, CustomerPart customerPart)
        {
            var addressPart = _customerService.GetAddress(customerPart.Id, addressType) ?? _customerService.CreateAddress(customerPart.Id, addressType);
            dynamic address = addressPart;

            address.Name.Value = source.Name.TrimSafe();
            address.AddressLine1.Value = source.AddressLine1.TrimSafe();
            address.AddressLine2.Value = source.AddressLine2.TrimSafe();
            address.Zipcode.Value = source.Zipcode.TrimSafe();
            address.City.Value = source.City.TrimSafe();
            address.Country.Value = source.Country.TrimSafe();

            return addressPart;
        }

        [Themed]
        public ActionResult Summary()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            if (user == null)
                throw new OrchardSecurityException(T("Login required"));

            dynamic invoiceAddress = _customerService.GetAddress(user.Id, "InvoiceAddress");
            dynamic shippingAddress = _customerService.GetAddress(user.Id, "ShippingAddress");
            dynamic shoppingCartShape = _services.New.ShoppingCart();

            var query = _shoppingCart.GetProducts().Select(x => _services.New.ShoppingCartItem(
                Product: x.ProductPart,
                Quantity: x.Quantity,
                Title: _services.ContentManager.GetItemMetadata(x.ProductPart).DisplayText
            ));

            shoppingCartShape.ShopItems = query.ToArray();
            shoppingCartShape.Total = _shoppingCart.Total();

            return new ShapeResult(this, _services.New.Checkout_Summary(
                ShoppingCart: shoppingCartShape,
                InvoiceAddress: invoiceAddress,
                ShippingAddress: shippingAddress
            ));
        }
    }
}