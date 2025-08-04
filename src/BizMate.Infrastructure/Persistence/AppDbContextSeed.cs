using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence
{
    public static class AppDbContextSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Statuses.AnyAsync())
            {
                var orderStatuses = new List<Status>
                {
                    new Status { Id = Guid.NewGuid(), Name = "Nháp", Code = "DRAFT", Group = "Order", IsActive = true },
                    new Status { Id = Guid.NewGuid(), Name = "Tạo mới", Code = "NEW", Group = "Order", IsActive = true },
                    new Status { Id = Guid.NewGuid(), Name = "Đang đóng hàng", Code = "PACKING", Group = "Order", IsActive = true },
                    new Status { Id = Guid.NewGuid(), Name = "Đã đóng hàng", Code = "PACKED", Group = "Order", IsActive = true },
                    new Status { Id = Guid.NewGuid(), Name = "Hủy", Code = "CANCELLED", Group = "Order", IsActive = true },
                    new Status { Id = Guid.NewGuid(), Name = "Hoàn thành", Code = "COMPLETED", Group = "Order", IsActive = true },
                };

                context.Statuses.AddRange(orderStatuses);
                await context.SaveChangesAsync();
            }
        }
    }
}
