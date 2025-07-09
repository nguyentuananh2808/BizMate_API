using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<ProductCoreDto> GetByIdWithQuantityAsync(Guid id, QueryFactory queryFactory);
        Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Product>> SearchProducts(Guid storeId, Guid? supplierId, string? name, QueryFactory queryFactory);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(List<ProductCoreDto> Products, int TotalCount)> SearchProductsWithPaging(Guid storeId, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);
    }

}
