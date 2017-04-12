using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Mvc;
using Orchard.Themes;
using SimpleCommerce.Models;
using SimpleCommerce.Services;
using SimpleCommerce.ViewModels;

namespace SimpleCommerce.Controllers
{
    public class ShoppingCartController : Controller {
        private readonly IOrchardServices _services;
        private readonly IShoppingCart _shoppingCart;

        public ShoppingCartController(IShoppingCart shoppingCart, IOrchardServices services) {
            _shoppingCart = shoppingCart;
            _services = services;
        }
        [HttpPost]
        public ActionResult Add(int id) {
            _shoppingCart.Add(id, 1);
            return RedirectToAction($"Index");
        }

        [Themed]
        public ActionResult Index() {
            var shape = _services.New.ShoppingCart(
                Products: _shoppingCart.GetProducts().Select(p => _services.New.ShoppingCartItem(
                    ProductPart: p.ProductPart,
                    Quantity: p.Quantity,
                    Title: _services.ContentManager.GetItemMetadata(p.ProductPart).DisplayText)
                    ).ToList(),
                Total: _shoppingCart.Total()
            );

            return new ShapeResult(this, shape);
        }

        [HttpPost]
        public ActionResult Update(string command, UpdateShoppingCartItemViewModel[] items)
        {
            UpdateShoppingCart(items);

            if (Request.IsAjaxRequest())
                return Json(true);

            switch (command)
            {
                case "Checkout":
                    return RedirectToAction("SignupOrLogin", "Checkout");
                case "ContinueShopping":
                    break;
                case "Update":
                    break;
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetItems() {
            var products = _shoppingCart.GetProducts();

            var json = new {
                items = (from item in products
                         select new {
                            id = item.ProductPart.Id,
                            title = _services.ContentManager.GetItemMetadata(item.ProductPart).DisplayText ?? "(No TitlePart attached)",
                            unitPrice = item.ProductPart.UnitPrice,
                            quantity = item.Quantity
                         }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private void UpdateShoppingCart(IEnumerable<UpdateShoppingCartItemViewModel> items) {
            _shoppingCart.Clear();
            if(items == null)
                return;
            _shoppingCart.AddRange(items
                .Where(item => !item.IsRemoved)
                .Select(item => new ShoppingCartItem(item.ProductId, item.Quantity < 0 ? 0 : item.Quantity)));
        }
    }
}