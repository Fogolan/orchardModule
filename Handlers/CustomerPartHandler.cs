using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Users.Models;
using SimpleCommerce.Models;

namespace SimpleCommerce.Handlers
{
    public class CustomerPartHandler : ContentHandler
    {
        public CustomerPartHandler(IRepository<CustomerPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<UserPart>("Customer"));
        }
    }
}