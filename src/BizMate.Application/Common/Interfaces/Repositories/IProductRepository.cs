using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<ProductCoreDto> GetByIdWithQuantityAsync(Guid id, QueryFactory queryFactory);
        Task<Product> GetByIdAsync(Guid id);
        Task<List<Product>> GetByIdsAsync(List<Guid> ids);
        Task<List<Product>> SearchProducts(Guid storeId, Guid? supplierId, string? name, QueryFactory queryFactory);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<(List<ProductCoreDto> Products, int TotalCount)> SearchProductsWithPaging(Guid storeId, string? keyword, int pageIndex, int pageSize, QueryFactory queryFactory);
    }

}
