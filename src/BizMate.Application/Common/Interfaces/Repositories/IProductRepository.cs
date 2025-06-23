using BizMate.Domain.Entities;
using SqlKata.Execution;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(Guid id);
        Task<List<Product>> SearchProducts(Guid storeId, Guid? supplierId, string? name, QueryFactory queryFactory);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product, uint originalRowVersion); 
        Task DeleteAsync(Guid id);
    }

}
