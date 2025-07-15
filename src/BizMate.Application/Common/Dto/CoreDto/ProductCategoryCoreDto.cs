using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class ProductCategoryCoreDto : BaseEntity
    {
        public string Name { get; set; } = default!;
    }
}
