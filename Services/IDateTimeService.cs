using System;
using Orchard;

namespace SimpleCommerce.Services
{
    public interface IDateTimeService : IDependency
    {
        DateTime Now { get; }
    }
}
