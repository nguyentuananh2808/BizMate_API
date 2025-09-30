using BizMate.Application.Common.Dto.UserAggregate;
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

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && ids.Contains(p.Id) && !p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductCoreDto> GetByIdWithQuantityAsync(Guid id, QueryFactory queryFactory)
    {
        var result = await queryFactory.Query("Products as p")
               .LeftJoin("Stocks as s", j => j
                   .On("p.Id", "s.ProductId")
                   .On("s.StoreId", "p.StoreId"))
               .Where("p.Id", id)
               .Where("p.IsDeleted", false)
               .Select("p.*")
               .SelectRaw(@"COALESCE(s.""Quantity"", 0) as Quantity")
               .FirstOrDefaultAsync<ProductCoreDto>();

        return result;
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

    public async Task<(List<ProductCoreDto> Products, int TotalCount)> SearchProductsWithPaging(
    Guid storeId,
    string? keyword,
    int pageIndex,
    int pageSize,
    bool? isActive,
    QueryFactory queryFactory)
    {
        var baseQuery = queryFactory.Query("Products as p")
            .LeftJoin("Stocks as s", j => j
                .On("p.Id", "s.ProductId")
                .On("s.StoreId", "p.StoreId"))
            .LeftJoin("ProductCategories as pc", z => z
                .On("p.ProductCategoryId", "pc.Id"))
            .Where("p.StoreId", storeId)
            .Where("p.IsDeleted", false);

        if (isActive.HasValue)
        {
            baseQuery.Where("p.IsActive", isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalizedKeyword = keyword.ToLower().Replace("-", "").Trim();
            var kw = $"%{normalizedKeyword}%";

            string vietnameseChars = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ";
            string noSignChars = "aaaaaaaaaaaaaaaaaeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd";

            baseQuery.Where(q =>
                q.WhereRaw(
                    $@"TRANSLATE(LOWER(REPLACE(p.""Name"", '-', '')),
                '{vietnameseChars}',
                '{noSignChars}'
            ) LIKE ?", kw
                ).OrWhereRaw(
                    $@"TRANSLATE(LOWER(REPLACE(p.""Code"", '-', '')),
                '{vietnameseChars}',
                '{noSignChars}'
            ) LIKE ?", kw
                )
            );
        }


        baseQuery
            .Select("p.*")
            .SelectRaw(@"COALESCE(s.""Quantity"", 0) as Quantity")
            .SelectRaw(@"COALESCE(s.""Quantity"", 0) - COALESCE(s.""Reserved"", 0) as Available")
            .Select("pc.Name as ProductCategoryName");

        var totalQuery = baseQuery.Clone();
        var totalCount = await totalQuery.CountAsync<int>();

        var results = await baseQuery
            .Offset((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .GetAsync<ProductCoreDto>();

        return (results.ToList(), totalCount);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);

    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null && !product.IsDeleted)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
