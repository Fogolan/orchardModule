using Orchard;
using SimpleCommerce.Models;

namespace SimpleCommerce.Services
{
    public interface ICustomerService : IDependency {
        CustomerPart CreateCustomer(string email, string password);
        AddressPart GetAddress(int customerId, string addressType);
        AddressPart CreateAddress(int customerId, string addressType);
    }
}
