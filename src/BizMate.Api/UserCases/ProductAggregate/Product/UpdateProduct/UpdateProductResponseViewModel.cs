using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.ProductAggregate.Product.UpdateProduct
{
    public class UpdateProductResponseViewModel
    {
        public ProductCoreDto Product { get; set; }
        public UpdateProductResponseViewModel(ProductCoreDto product)
        {
            Product = product;
        }
    }
}
