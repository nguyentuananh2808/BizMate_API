using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

public class StockRepository : IStockRepository
{
    private readonly AppDbContext _context;
    private readonly QueryFactory _queryFactory;

    public StockRepository(AppDbContext context, QueryFactory queryFactory)
    {
        _context = context;
        _queryFactory = queryFactory;
    }

    public async Task<List<Stock>> GetByStoreAndProductAsync(Guid storeId, List<Guid> productIds)
    {
        return await _context.Stocks
            .Where(s => s.StoreId == storeId
                     && productIds.Contains(s.ProductId)
                     && !s.IsDeleted)
            .ToListAsync();
    }


    public async Task AddAsync(Stock stock)
    {
        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stock stock, CancellationToken cancellationToken)
    {
        _context.Stocks.Update(stock);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id)
    {
        var stock = await _context.Stocks.FindAsync(id);
        if (stock != null)
        {
            stock.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Stock>> SearchStocks(Guid storeId, string? productName)
    {
        var query = _queryFactory.Query("Stocks as s")
            .Join("Products as p", "s.ProductId", "p.Id")
            .Where("s.StoreId", storeId)
            .Where("s.IsDeleted", false);

        if (!string.IsNullOrWhiteSpace(productName))
        {
            query.WhereRaw(@"LOWER(p.""Name"") LIKE ?", $"%{productName.ToLower()}%");
        }

        var result = await query.Select("s.*").GetAsync<Stock>();
        return result.ToList();
    }

    public async Task<(List<Stock> Stocks, int TotalCount)> SearchStocksWithPaging(Guid storeId, string? productName, int pageIndex, int pageSize)
    {
        var query = _queryFactory.Query("Stocks as s")
            .Join("Products as p", "s.ProductId", "p.Id")
            .Where("s.StoreId", storeId)
            .Where("s.IsDeleted", false);

        if (!string.IsNullOrWhiteSpace(productName))
        {
            query.WhereRaw(@"LOWER(p.""Name"") LIKE ?", $"%{productName.ToLower()}%");
        }

        var totalQuery = query.Clone();
        var totalCount = await totalQuery.CountAsync<int>();

        var results = await query
            .Select("s.*")
            .Offset((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .GetAsync<Stock>();

        return (results.ToList(), totalCount);
    }
}
