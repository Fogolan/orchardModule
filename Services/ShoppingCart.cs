using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using SimpleCommerce.Models;

namespace SimpleCommerce.Services
{
    public class ShoppingCart : IShoppingCart {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        public IEnumerable<ShoppingCartItem> Items { get { return ItemsInternal.AsReadOnly(); } }

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        public List<ShoppingCartItem> ItemsInternal
        {
            get
            {
                var items = (List<ShoppingCartItem>) HttpContext.Session["ShoppingCart"];
                if (items == null) {
                    items = new List<ShoppingCartItem>();
                    HttpContext.Session["ShoppingCart"] = items;
                }
                return items;
            }
        }

        public ShoppingCart(IWorkContextAccessor workContextAccessor, IContentManager contentManager) {
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
        }

        public void Add(int productId, int quantity = 3) {
            var item = Items.SingleOrDefault(x => x.ProductId == productId);
            if (item == null) {
                item = new ShoppingCartItem(productId, quantity);
                ItemsInternal.Add(item);
            }
            else {
                item.Quantity += quantity;
            }
        }

        public void Remove(int productId) {
            var item = Items.SingleOrDefault(x => x.ProductId == productId);

            if (item != null)
                ItemsInternal.Remove(item);
        }

        public ProductPart GetProduct(int productId) {
            return _contentManager.Get<ProductPart>(productId);
        }

        public IEnumerable<ProductQuantity> GetProducts()
        {
            var ids = Items.Select(x => x.ProductId).ToList();
            var productParts = _contentManager.GetMany<ProductPart>(ids, VersionOptions.Latest, QueryHints.Empty).ToArray();

            var query = from item in Items
                from productPart in productParts
                where productPart.Id == item.ProductId
                select new ProductQuantity
                {
                    ProductPart = productPart,
                    Quantity = item.Quantity
                };

            return query;
        }

        public float Total() {
            return Items.Select(x => GetProduct(x.ProductId).UnitPrice * x.Quantity).Sum();
        }

        public int ItemCount() {
            return Items.Sum(x => x.Quantity);
        }

        public void UpdateItems() {
            ItemsInternal.RemoveAll(x => x.Quantity == 0);
        }

        public void AddRange(IEnumerable<ShoppingCartItem> items)
        {
            foreach (var item in items)
            {
                Add(item.ProductId, item.Quantity);
            }
        }

        public void Clear() {
            ItemsInternal.Clear();
            UpdateItems();
        }
    }
}