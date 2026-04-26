using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ProductAggregate.ProductItem.GetProductItems
{
    public class GetProductItemsResponseViewModel
    {
        public IEnumerable<ProductItemCoreDto> ProductItems { get; }
        public int TotalCount { get; }

        public GetProductItemsResponseViewModel(
            IEnumerable<ProductItemCoreDto> productItems,
            int totalCount)
        {
            ProductItems = productItems;
            TotalCount = totalCount;
        }
    }
}
