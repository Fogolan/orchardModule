using Orchard.ContentManagement;

namespace SimpleCommerce.Models
{
    public class AddressPart : ContentPart<AddressPartRecord>
    {

        public int CustomerId
        {
            get { return Record.CustomerId; }
            set { Record.CustomerId = value; }
        }

        public string Type
        {
            get { return Record.Type; }
            set { Record.Type = value; }
        }
    }
}