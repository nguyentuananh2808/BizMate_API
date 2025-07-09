using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct
{
    public class UpdateProductResponse : BaseResponse
    {
        public ProductCoreDto Prodduct { get; }
        public UpdateProductResponse(ProductCoreDto product, bool success = true, string message = null) : base(success, message)
        {
            Prodduct = product;
        }
        public UpdateProductResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
