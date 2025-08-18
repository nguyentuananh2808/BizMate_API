using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class DealerLevelRepository : IDealerLevelRepository
    {
        private readonly AppDbContext _context;

        public DealerLevelRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(DealerLevel dealerLevel, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(dealerLevel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dealerLevel = await _context.DealerLevels.FindAsync(id);
            if (dealerLevel is not null && !dealerLevel.IsDeleted)
            {
                dealerLevel.IsDeleted = true;
                dealerLevel.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<DealerLevel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.DealerLevels
          .Where(p => !p.IsDeleted && p.Id == id)
          .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<DealerLevel>> SearchDealerLevels(Guid storeId, string? name, QueryFactory queryFactory)
        {
            var query = queryFactory.Query("DealerLevels as d")
              .Where("d.StoreId", storeId)
              .Where("d.IsDeleted", false)
              .Where("d.IsActive", false);
            if (name != null)
                query.Where("Name", name);


            var result = await query.GetAsync<DealerLevel>();
            return result.ToList();
        }

        public async Task<(List<DealerLevelCoreDto> DealerLevels, int TotalCount)> SearchDealerLevelsWithPaging(Guid storeId, string? keyword, int pageIndex, int pageSize, bool? isActive, QueryFactory queryFactory)
        {
            var baseQuery = queryFactory.Query("DealerLevels as d")
                .Where("d.StoreId", storeId)
                .Where("d.IsDeleted", false);

            if (isActive.HasValue)
            {
                baseQuery.Where("d.IsActive", isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = $"%{keyword.ToLower()}%";
                baseQuery.WhereRaw(@"LOWER(d.""Name"") LIKE ? OR LOWER(d.""Code"") LIKE ?", kw, kw);
            }


            var totalQuery = baseQuery.Clone();
            var totalCount = await totalQuery.CountAsync<int>();

            var results = await baseQuery
                .Offset((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .GetAsync<DealerLevelCoreDto>();

            return (results.ToList(), totalCount);
        }

        public async Task UpdateAsync(DealerLevel dealerLevel, CancellationToken cancellationToken = default)
        {
            _context.DealerLevels.Update(dealerLevel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
