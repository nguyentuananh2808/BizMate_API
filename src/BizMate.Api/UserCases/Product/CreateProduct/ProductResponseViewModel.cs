using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.CreateProduct
{
    public class ProductResponseViewModel
    {
        public ProductCoreDto Product { get; set; }
        public ProductResponseViewModel(ProductCoreDto product)
        {
            Product = product;
        }
    }
}
