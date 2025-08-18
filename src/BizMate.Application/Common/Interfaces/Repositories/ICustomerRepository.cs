using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Customer>> SearchCustomers(Guid storeId, string? name, string? phone, QueryFactory queryFactory);
        Task AddAsync(Customer Customer, CancellationToken cancellationToken = default);
        Task UpdateAsync(Customer Customer, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(List<CustomerCoreDto> Customers, int TotalCount)> SearchCustomersWithPaging(Guid storeId, string? keyword, int pageIndex, int pageSize, bool? isActive, QueryFactory queryFactory);

    }
}
