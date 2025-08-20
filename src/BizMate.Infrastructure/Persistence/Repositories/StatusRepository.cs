using BizMate.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly AppDbContext _context;

        public StatusRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> GetIdByGroupAndCodeAsync(string code, string group)
        {
            return await _context.Statuses.Where(s => s.Code == code && s.Group == group)
                  .Select(s => s.Id)
                  .FirstOrDefaultAsync();
        }
    }
}
