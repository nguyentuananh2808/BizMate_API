using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptRequest : IRequest<UpdateStatusImportReceiptResponse>
    {
        [Required]
        public Guid Id { get; set; } = default!;
        [Required]
        public string CodeStatus { get; set; } = default!;
        [Required]
        public Guid RowVersion { get; set; } = default!;
    }
}
