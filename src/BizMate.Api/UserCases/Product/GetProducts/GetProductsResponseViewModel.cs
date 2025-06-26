using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.GetProducts
{
    public class GetProductsResponseViewModel
    {
        public IEnumerable<ProductCoreDto> Products { get; set; }
        public GetProductsResponseViewModel(IEnumerable<ProductCoreDto> products)
        {
            Products = products;
        }
    }
}
