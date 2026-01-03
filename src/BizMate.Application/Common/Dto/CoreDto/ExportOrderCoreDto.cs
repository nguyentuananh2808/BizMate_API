namespace BizMate.Application.Common.Dto.CoreDto
{
    public class ExportOrderCoreDto
    {
        public required IEnumerable<OrderCoreDto> Orders { get; set; }
    }
}
