using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.GetProductCategory
{
    public class GetProductCategoryResponseViewModel
    {
        public ProductCategoryCoreDto ProductCategory { get; set; }
        public GetProductCategoryResponseViewModel(ProductCategoryCoreDto productCategory)
        {
            ProductCategory = productCategory;
        }
    }
}
