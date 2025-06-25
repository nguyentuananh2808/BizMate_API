using MediatR;

namespace BizMate.Application.UserCases.Product.Commands.DeleteProduct
{
    public class DeleteProductRequest : IRequest<DeleteProductResponse>
    {
        public Guid Id { get; set; }

        public DeleteProductRequest(Guid id)
        {
            Id = id;
        }
    }
}
