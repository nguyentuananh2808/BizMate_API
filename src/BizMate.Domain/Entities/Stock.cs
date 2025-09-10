namespace BizMate.Domain.Entities
{
    public class Stock : Base
    {

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        /// <summary>
        /// Tổng tồn kho thực tế trong kho
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Số lượng đã giữ cho đơn hàng nhưng chưa xuất
        /// </summary>
        public int Reserved { get; set; }

        /// <summary>
        /// Số lượng khả dụng = Quantity - Reserved
        /// </summary>
        public int Available => Quantity - Reserved;
    }
}
