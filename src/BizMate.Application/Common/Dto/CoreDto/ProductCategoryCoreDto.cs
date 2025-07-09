namespace BizMate.Application.Common.Dto.CoreDto
{
    public class ProductCategoryCoreDto
    {
        public string ProductCategoryCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid StoreId { get; set; }
    }
}
