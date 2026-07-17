using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Queries.Products
{
    public class GetProductsRequest : SearchCore, IRequest<GetProductsResponse>
    {
        public bool? IsActive { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string? StockFilter { get; set; }
        public int? LowStockThreshold { get; set; }
    }
}
