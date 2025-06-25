using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Product.Commands.DeleteProduct
{
    public class DeleteProductResponse : BaseResponse
    {
        public DeleteProductResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
