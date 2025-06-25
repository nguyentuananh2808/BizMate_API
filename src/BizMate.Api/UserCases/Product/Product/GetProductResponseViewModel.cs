using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.Product
{
    public class GetProductResponseViewModel
    {
        public ProductCoreDto Product { get; set; }
        public GetProductResponseViewModel(ProductCoreDto product)
        {
            Product = product;
        }
    }
}
