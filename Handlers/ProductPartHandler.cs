using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SimpleCommerce.Models;

namespace SimpleCommerce.Handlers
{
    public class ProductPartHandler : ContentHandler{
        public ProductPartHandler(IRepository<ProductPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}