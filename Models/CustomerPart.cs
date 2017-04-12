using System;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Users.Models;

namespace SimpleCommerce.Models
{
    public class CustomerPart : ContentPart<CustomerPartRecord>
    {
        public string FirstName
        {
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        public string LastName
        {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }

        public string Title
        {
            get { return Record.Title; }
            set { Record.Title = value; }
        }

        public DateTime CreatedUtc
        {
            get { return Record.CreatedUtc; }
            set { Record.CreatedUtc = value; }
        }

        public IUser User
        {
            get { return this.As<UserPart>(); }
        }
    }
}