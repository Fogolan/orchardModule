using System;

namespace SimpleCommerce.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now
        {
            get { return DateTime.UtcNow;}
        }
    }
}