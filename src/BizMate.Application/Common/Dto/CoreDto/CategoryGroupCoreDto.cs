using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class CategoryGroupCoreDto : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int ProductCategoryCount { get; set; }
        public List<Guid> ProductCategoryIds { get; set; } = new();
        public List<ProductCategoryCoreDto> ProductCategories { get; set; } = new();
    }
}
