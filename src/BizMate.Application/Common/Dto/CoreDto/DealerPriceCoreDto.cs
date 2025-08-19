namespace BizMate.Application.Common.Dto.CoreDto
{
    public class DealerPriceCoreDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid DealerLevelId { get; set; }
        public decimal Price { get; set; }
        public Guid RowVersion { get; set; }
    }
}
