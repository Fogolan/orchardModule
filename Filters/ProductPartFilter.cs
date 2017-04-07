using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;
using SimpleCommerce.Models;

namespace SimpleCommerce.Filters
{
    public class ProductPartFilter : IFilterProvider
    {
        public Localizer T { get; set; }

        public ProductPartFilter() {
            T = NullLocalizer.Instance;
        }
        public void Describe(DescribeFilterContext describe) {
            describe.For(
                "Content",
                T("Content"),
                T("Content"))

                .Element(
                    "ProductParts",
                    T("Product Parts"),
                    T("Product parts"),
                    ApplyFilter,
                    DisplayFilter
                );
        }

        private void ApplyFilter(FilterContext context) {
            context.Query = context.Query.Join(x => x.ContentPartRecord(typeof(ProductPartRecord)));
        }

        private LocalizedString DisplayFilter(FilterContext context) {
            return T("Content with ProductPart");
        }
    }
}