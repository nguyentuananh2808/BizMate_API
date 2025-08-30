using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IOrderDetailRepository
    {

        Task<List<OrderDetail>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<OrderDetail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(OrderDetail detail, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default);

        Task UpdateAsync(OrderDetail detail, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    }
}
