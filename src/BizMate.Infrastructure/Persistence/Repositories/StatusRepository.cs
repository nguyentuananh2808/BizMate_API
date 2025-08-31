using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
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
            return await _context.Statuses.Where(s => s.Code == code && s.Group == group && !s.IsDeleted && !s.IsActive)
                  .Select(s => s.Id)
                  .FirstOrDefaultAsync();
        }

        public async Task<Status?> GetIdById(Guid id, CancellationToken cancellation)
        {
            return await _context.Statuses
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted && !s.IsActive, cancellation);
        }

        public async Task<IEnumerable<Status>> GetStatusesOfGroup(string group)
        {
            return await _context.Statuses.Where(s => s.Group == group && !s.IsActive && !s.IsDeleted)
                .ToListAsync();
        }
    }
}
