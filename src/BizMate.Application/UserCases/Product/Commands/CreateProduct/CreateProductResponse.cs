using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Responses;


namespace BizMate.Application.UserCases.Product.Commands.CreateProduct
{
    public class CreateProductResponse : BaseResponse
    {
        public ProductCoreDto Prodduct { get; }
        public CreateProductResponse(ProductCoreDto product, bool success = true, string message = null) : base(success, message)
        {
            Prodduct = product;
        }
        public CreateProductResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }

}
