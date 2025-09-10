using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IDealerLevelRepository
    {
        Task<DealerLevel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<DealerLevel>> SearchDealerLevels(Guid storeId, string? name, QueryFactory queryFactory);
        Task AddAsync(DealerLevel DealerLevel, CancellationToken cancellationToken = default);
        Task UpdateAsync(DealerLevel DealerLevel, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(List<DealerLevelCoreDto> DealerLevels, int TotalCount)> SearchDealerLevelsWithPaging(Guid storeId, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);


    }
}
