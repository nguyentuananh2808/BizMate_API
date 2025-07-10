using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.CreateProductCategory
{
    public class CreateProductCategoryResponseViewModel
    {
        public ProductCategoryCoreDto ProductCategoryCoreDto { get; set; }

        public CreateProductCategoryResponseViewModel(ProductCategoryCoreDto productCategoryCoreDto)
        {
            ProductCategoryCoreDto = productCategoryCoreDto;
        }
    }
}
