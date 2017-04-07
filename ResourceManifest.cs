using Orchard.UI.Resources;

namespace SimpleCommerce
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("SimpleCommerce.Common").SetUrl("common.css");

            manifest.DefineStyle("SimpleCommerce.ShoppingCart").SetUrl("shoppingcart.css").SetDependencies("SimpleCommerce.Common");

            manifest.DefineStyle("SimpleCommerce.ShoppingCartWidget").SetUrl("shoppingcartwidget.css").SetDependencies("Webshop.Common");

            manifest.DefineScript("SimpleCommerce.ShoppingCart").SetUrl("shoppingcart.js").SetDependencies("jQuery", "jQuery_LinqJs", "ko");
        }
    }
}