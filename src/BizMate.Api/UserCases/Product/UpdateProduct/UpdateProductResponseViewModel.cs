using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.UpdateProduct
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
