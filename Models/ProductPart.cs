using Orchard.ContentManagement;

namespace SimpleCommerce.Models
{
    public class ProductPart : ContentPart<ProductPartRecord>
    {
        public float UnitPrice
        {
            get { return Record.UnitPrice; }
            set { Record.UnitPrice = value; }
        }

        public string Sku
        {
            get { return Record.Sku; }
            set { Record.Sku = value; }
        }
    }
}