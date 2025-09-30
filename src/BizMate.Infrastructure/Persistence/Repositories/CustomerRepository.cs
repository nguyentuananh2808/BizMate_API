using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is not null && !customer.IsDeleted)
            {
                customer.IsDeleted = true;
                customer.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
          .Where(p => !p.IsDeleted && p.Id == id)
          .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Customer>> SearchCustomers(Guid storeId, string? phone, string? name, QueryFactory queryFactory)
        {
            var query = queryFactory.Query("Customers as c")
                .Where("c.StoreId", storeId)
                .Where("c.IsDeleted", false)
                .Where("c.IsActive", false);
            //if (name != null)
            //    query.Where("Name", name);

            if (phone != null)
                query.Where("Phone", phone);

            var result = await query.GetAsync<Customer>();
            return result.ToList();

        }
        public async Task<(List<CustomerCoreDto> Customers, int TotalCount)> SearchCustomersWithPaging(
     Guid storeId, string? keyword, int pageIndex, int pageSize, bool? isActive, QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("Customers as p")
                .Where("p.StoreId", storeId)
                .Where("p.IsDeleted", false);

            if (isActive.HasValue)
            {
                baseQuery.Where("p.IsActive", isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywords = keyword
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                baseQuery.Where(q =>
                {
                    foreach (var word in keywords)
                    {
                        var kwPrefix = $"{word}%";   // match từ đầu
                        var kwLike = $"%{word}%";  // match ở bất kỳ đâu

                        q.OrWhereRaw(@"LOWER(p.""Name"") LIKE ?", kwPrefix);
                        q.OrWhereRaw(@"LOWER(p.""Name"") LIKE ?", kwLike);
                    }
                    return q;
                });

                // Ưu tiên Name bắt đầu bằng keyword trước
                baseQuery.OrderByRaw(@"
                    CASE 
                        WHEN LOWER(p.""Name"") LIKE ? THEN 0
                        ELSE 1
                    END, p.""Name"" ASC",
                            keyword.ToLower() + "%"
                        );
                }
                else
                {
                    baseQuery.OrderBy("p.Name");
                }

            var totalQuery = baseQuery.Clone();
            var totalCount = await totalQuery.CountAsync<int>();

            var results = await baseQuery
                .Offset((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .GetAsync<CustomerCoreDto>();

            return (results.ToList(), totalCount);
        }

        public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> HasCustomersWithDealerLevelAsync(Guid dealerLevelId, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AnyAsync(c => !c.IsDeleted && c.DealerLevelId == dealerLevelId, cancellationToken);
        }

    }
}
