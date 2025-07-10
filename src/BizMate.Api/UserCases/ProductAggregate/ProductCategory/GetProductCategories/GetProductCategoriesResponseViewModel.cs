using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.GetProductCategories
{
    public class GetProductCategoriesResponseViewModel
    {
        public IEnumerable<ProductCategoryCoreDto> ProductCategories { get; set; }
        public int TotalCount { get; }
        public GetProductCategoriesResponseViewModel(IEnumerable<ProductCategoryCoreDto> productCategories, int totalCount)
        {
            ProductCategories = productCategories;
            TotalCount = totalCount;
        }
    }
}
