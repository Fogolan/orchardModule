using System.Collections.Generic;
using Orchard;
using SimpleCommerce.Models;

namespace SimpleCommerce.Services
{
    public interface IShoppingCart : IDependency
    {
        IEnumerable<ShoppingCartItem> Items { get; }
        void Add(int productId, int quantity = 1);
        void Remove(int productId);
        ProductPart GetProduct(int productId);
        IEnumerable<ProductQuantity> GetProducts();
        float Total();
        int ItemCount();
        void UpdateItems();
        void Clear();
        void AddRange(IEnumerable<ShoppingCartItem> items);
    }
}