using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.OrderDetails
                .Where(d => d.OrderId == orderId)
                .ToListAsync(cancellationToken);
        }

        public async Task<OrderDetail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.OrderDetails
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task AddAsync(OrderDetail detail, CancellationToken cancellationToken = default)
        {
            await _context.OrderDetails.AddAsync(detail, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default)
        {
            await _context.OrderDetails.AddRangeAsync(details, cancellationToken);
        }

        public Task UpdateAsync(OrderDetail detail, CancellationToken cancellationToken = default)
        {
            _context.OrderDetails.Update(detail);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var detail = await _context.OrderDetails.FindAsync(new object[] { id }, cancellationToken);
            if (detail != null)
            {
                _context.OrderDetails.Remove(detail);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var details = await _context.OrderDetails
                .Where(d => ids.Contains(d.Id))
                .ToListAsync(cancellationToken);

            if (details.Any())
            {
                _context.OrderDetails.RemoveRange(details);
            }
        }
    }
}