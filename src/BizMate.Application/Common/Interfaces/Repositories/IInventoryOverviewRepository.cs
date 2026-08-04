using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IInventoryOverviewRepository
    {
        Task<bool> CategoryGroupExistsAsync(
            Guid storeId,
            Guid categoryGroupId,
            bool includeInactive,
            CancellationToken cancellationToken = default);

        Task<List<InventoryOverviewRowDto>> GetOverviewRowsAsync(
            Guid storeId,
            Guid? categoryGroupId,
            string? keyword,
            bool includeInactive,
            CancellationToken cancellationToken = default);
    }
}
