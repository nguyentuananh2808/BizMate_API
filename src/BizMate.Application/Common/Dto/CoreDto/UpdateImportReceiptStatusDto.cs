namespace BizMate.Application.Common.Dto.CoreDto
{
    public class UpdateImportReceiptStatusDto
    {
        public Guid Id { get; set; }
        public Guid StatusId { get; set; }
        public Guid StoreId { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid RowVersion { get; set; }
    }
}
