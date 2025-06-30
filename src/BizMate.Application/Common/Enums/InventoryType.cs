using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.Common.Enums
{
    public enum InventoryType
    {
        [Display(Name = "Phiếu nhâp")]
        Import = 1,

        [Display(Name = "Phiếu xuất")]
        Export = 2,
    }
}
