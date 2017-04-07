using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using SimpleCommerce.Models;

namespace SimpleCommerce.Drivers
{
    public class ProductDriver : ContentPartDriver<ProductPart>
    {
        protected override DriverResult Display(
            ProductPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(ContentShape("Parts_Product",
                () => shapeHelper.Parts_Product(
                    Sku: part.Sku,
                    Price: part.UnitPrice)),
                    ContentShape("Parts_Product_AddButton", () => 
                                   shapeHelper.Parts_Product_AddButton(ProductId: part.Id))
            );
        }

        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Product_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Product",
                    Model: part,
                    Prefix: Prefix));
        }

        protected override DriverResult Editor(
            ProductPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}