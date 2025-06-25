using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.Common.Dto.Identity
{
    public enum InventoryType
    {
        [Display(Name = "Phiếu nhâp")]
        Import = 1,

        [Display(Name = "Phiếu xuất")]
        Export = 2,
    }
}
