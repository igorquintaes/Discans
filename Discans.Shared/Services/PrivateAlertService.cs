using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class PrivateAlertService
    {
        private readonly AppDbContext dbContext;

        public PrivateAlertService(AppDbContext context) =>
            dbContext = context;

        public async Task Create(ulong userId, Manga manga)
        {
            var createdAlert = await dbContext
                .PrivateAlerts
                .Include(x => x.Manga)
                .FirstOrDefaultAsync(x => 
                    x.UserId == userId &&
                    x.Manga.Id == manga.Id);

            if (createdAlert == null)
            {
                manga.PrivateAlerts.Add(new PrivateAlert(userId, manga));
                dbContext.Mangas.Update(manga);
            }
        }

        public async Task Remove(ulong userId, int mangaId)
        {
            var alert = await dbContext
                .PrivateAlerts
                .Include(x => x.Manga)
                .FirstOrDefaultAsync(x => 
                    x.UserId == userId && 
                    x.Manga.Id == mangaId);

            if (alert != null)
                dbContext.PrivateAlerts.Remove(alert);
        }

        public async Task<IList<PrivateAlert>> GetAlerts(ulong userId) =>
            await dbContext
                .PrivateAlerts
                .Include(x => x.Manga)
                .Where(x => x.UserId == userId)
                .ToListAsync();
    }
}
