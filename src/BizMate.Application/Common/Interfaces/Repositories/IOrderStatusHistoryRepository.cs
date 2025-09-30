namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IOrderStatusHistoryRepository
    {
        Task UpdateOrderStatus(Guid orderId, Guid newStatusId, Guid userId, string userName, string? note = null);
    }

}
