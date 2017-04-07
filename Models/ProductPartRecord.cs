using Orchard.ContentManagement.Records;

namespace SimpleCommerce.Models
{
    public class ProductPartRecord : ContentPartRecord
    {
        public virtual float UnitPrice { get; set; }
        public virtual string Sku { get; set; }
    }
}