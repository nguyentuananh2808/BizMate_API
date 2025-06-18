using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizMate.Domain.Entities
{
    public class InventoryReceiptDetail
    {
        public Guid Id { get; set; }

        public Guid InventoryReceiptId { get; set; }
        public InventoryReceipt InventoryReceipt { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
    }

}
