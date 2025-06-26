using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Product>> SearchProducts(Guid storeId, Guid? supplierId, string? name,
        QueryFactory queryFactory)
    {
        var query = queryFactory.Query("Products as p")
            .Where("p.StoreId", storeId)
            .Where("p.IsDeleted", false);

        if (supplierId.HasValue)
            query.Where("p.SupplierId", supplierId.Value);

        if (!string.IsNullOrWhiteSpace(name))
            query.WhereRaw(@"LOWER(p.""Name"") LIKE ?", $"%{name.ToLower()}%");

        var result = await query.GetAsync<Product>();
        return result.ToList();
    }

    public async Task<(List<Product> Products, int TotalCount)> SearchProductsWithPaging(
        Guid storeId,
        string? keyword,
        int pageIndex,
        int pageSize,
        QueryFactory queryFactory)
    {
        var baseQuery = queryFactory.Query("Products as p")
            .Where("p.StoreId", storeId)
            .Where("p.IsDeleted", false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            baseQuery.WhereRaw(@"LOWER(p.""Name"") LIKE ?", $"%{keyword.ToLower()}%");
        }

        var totalQuery = baseQuery.Clone();
        var totalCount = await totalQuery.CountAsync<int>();

        var results = await baseQuery
            .Offset((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .GetAsync<Product>();

        return (results.ToList(), totalCount);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        var entry = _context.Entry(product);
        entry.Property(nameof(BaseEntity.RowVersion)).OriginalValue = product.RowVersion;
        product.RowVersion++;

        try
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null && !product.IsDeleted)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
