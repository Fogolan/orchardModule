﻿using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SimpleCommerce.Models;

namespace SimpleCommerce.Handlers
{
    public class AddressPartHandler : ContentHandler
    {
        public AddressPartHandler(IRepository<AddressPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}