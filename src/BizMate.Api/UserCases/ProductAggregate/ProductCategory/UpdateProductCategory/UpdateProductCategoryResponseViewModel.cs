using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.UpdateProductCategory
{
    public class UpdateProductCategoryResponseViewModel
    {
        public ProductCategoryCoreDto ProductCategory { get; set; }
        public UpdateProductCategoryResponseViewModel(ProductCategoryCoreDto productCategory)
        {
            ProductCategory = productCategory;
        }

    }
}
