using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.ProductItem.Queries.GetProductItems
{
    public class GetProductItemsRequest : IRequest<GetProductItemsResponse>
    {
        public Guid ProductId { get; set; }
        public int? Status { get; set; }
        public string? Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
