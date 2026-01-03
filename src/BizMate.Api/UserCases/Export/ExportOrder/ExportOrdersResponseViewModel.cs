using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Export.ExportOrder
{
    public class ExportOrdersResponseViewModel
    {
        public IEnumerable<OrderCoreDto> ExportReceipts { get; set; }
        public ExportOrdersResponseViewModel(IEnumerable<OrderCoreDto> exportReceipts)
        {
            ExportReceipts = exportReceipts;
        }
    }
}
