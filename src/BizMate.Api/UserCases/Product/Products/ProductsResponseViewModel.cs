using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.Products
{
    public class ProductsResponseViewModel
    {
        public IEnumerable<ProductCoreDto> Products { get; set; }
        public ProductsResponseViewModel(IEnumerable<ProductCoreDto> products)
        {
            Products = products;
        }
    }
}
