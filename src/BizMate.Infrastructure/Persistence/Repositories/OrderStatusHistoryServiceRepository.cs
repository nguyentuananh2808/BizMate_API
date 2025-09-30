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
        public async Task UpdateOrderStatus(Guid orderId, Guid newStatusId, Guid userId, string userName, string? note = null)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            var oldStatusId = order.StatusId;
            order.StatusId = newStatusId;

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
            await _dbContext.SaveChangesAsync();
        }

    }
}
