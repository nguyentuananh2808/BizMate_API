using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.Common.Enums
{
    public enum ProductUnit
    {
        [Display(Name = "Cái")]
        Piece = 1,

        [Display(Name = "Hộp")]
        Box = 2,

        [Display(Name = "Thùng")]
        Carton = 3,

        [Display(Name = "Kg")]
        Kilogram = 4,

        [Display(Name = "Lít")]
        Liter = 5
    }

}
