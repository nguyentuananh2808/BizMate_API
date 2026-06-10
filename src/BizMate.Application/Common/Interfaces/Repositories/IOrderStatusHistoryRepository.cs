namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IOrderStatusHistoryRepository
    {
        Task AddOrderStatusHistory(Guid orderId, Guid oldStatusId, Guid newStatusId, Guid userId, string userName, string? note = null);
    }

}
