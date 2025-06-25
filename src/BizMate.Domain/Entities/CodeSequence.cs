using System.ComponentModel.DataAnnotations;

namespace BizMate.Domain.Entities
{
    public class CodeSequence
    {
        [Key]
        public string Prefix { get; set; } = default!;  // "SP", "XK", "NK"
        public int LastNumber { get; set; }
    }
}
