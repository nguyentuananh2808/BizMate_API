using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

public class StoreRepository : IStoreRepository
{
    private readonly AppDbContext _context;
    private readonly QueryFactory _queryFactory;

    public StoreRepository(AppDbContext context, QueryFactory queryFactory)
    {
        _context = context;
        _queryFactory = queryFactory;
    }

    public async Task<Store?> GetByNameAsync(string storeName, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Name.ToLower() == storeName.ToLower() && !s.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Store store, CancellationToken cancellationToken = default)
    {
        _context.Stores.Add(store);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string storeName, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .AnyAsync(s => s.Name.ToLower() == storeName.ToLower() && !s.IsDeleted, cancellationToken);
    }

    public async Task<List<Store>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Where(s => !s.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store != null)
        {
            store.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<Store> Stores, int TotalCount)> SearchStoresWithPaging(string? keyword, int pageIndex, int pageSize)
    {
        var query = _queryFactory.Query("Stores").Where("IsDeleted", false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query.WhereRaw(@"LOWER(""Name"") LIKE ?", $"%{keyword.ToLower()}%");
        }

        var totalCount = await query.Clone().CountAsync<int>();

        var results = await query
            .Offset((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .GetAsync<Store>();

        return (results.ToList(), totalCount);
    }
}
