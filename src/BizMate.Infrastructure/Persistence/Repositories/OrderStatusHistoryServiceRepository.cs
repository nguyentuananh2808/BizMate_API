using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class OrderStatusHistoryServiceRepository : IOrderStatusHistoryRepository
    {
        private readonly AppDbContext _dbContext;
        public OrderStatusHistoryServiceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task AddOrderStatusHistory(Guid orderId, Guid oldStatusId, Guid newStatusId, Guid userId, string userName, string? note = null)
        {
            var history = new OrderStatusHistory
            {
                OrderId = orderId,
                OldStatusId = oldStatusId,
                NewStatusId = newStatusId,
                ChangedByUserId = userId,
                ChangedByUserName = userName,
                Note = note
            };

            _dbContext.OrderStatusHistories.Add(history);
            return Task.CompletedTask;
        }

    }
}
