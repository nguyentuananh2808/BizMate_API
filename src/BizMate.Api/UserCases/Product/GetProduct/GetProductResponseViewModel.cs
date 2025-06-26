using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.GetProduct
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
