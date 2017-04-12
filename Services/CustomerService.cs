using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Models;
using SimpleCommerce.Models;

namespace SimpleCommerce.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IClock _clock;

        public CustomerService(IOrchardServices orchardServices, IMembershipService membershipService, IClock clock)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _clock = clock;
        }

        public CustomerPart CreateCustomer(string email, string password)
        {
            var customer = _orchardServices.ContentManager.New("Customer");
            var userPart = customer.As<UserPart>();
            var customerPart = customer.As<CustomerPart>();
            userPart.UserName = email;
            userPart.Email = email;
            userPart.NormalizedUserName = email.ToLowerInvariant();
            userPart.Record.HashAlgorithm = "SHA1";
            userPart.Record.RegistrationStatus = UserStatus.Approved;
            userPart.Record.EmailStatus = UserStatus.Approved;

            customerPart.CreatedUtc = _clock.UtcNow;

            _membershipService.SetPassword(userPart, password);

            _orchardServices.ContentManager.Create(customer);

            return customerPart;
        }

        public AddressPart GetAddress(int customerId, string addressType)
        {
            return _orchardServices.ContentManager.Query<AddressPart, AddressPartRecord>().Where(x => x.CustomerId == customerId && x.Type == addressType).List().FirstOrDefault();
        }

        public AddressPart CreateAddress(int customerId, string addressType) {
            return _orchardServices.ContentManager.Create<AddressPart>("Address", x => {
                x.Type = addressType;
                x.CustomerId = customerId;
            });
        }
    }
}