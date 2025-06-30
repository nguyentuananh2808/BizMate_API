using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.GetProducts
{
    public class GetProductsResponseViewModel
    {
        public IEnumerable<ProductCoreDto> Products { get; set; }
        public int TotalCount { get; }
        public GetProductsResponseViewModel(IEnumerable<ProductCoreDto> products, int totalCount)
        {
            Products = products;
            TotalCount = totalCount;
        }
    }
}
