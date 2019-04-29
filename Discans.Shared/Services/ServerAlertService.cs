using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class ServerAlertService
    {
        private readonly AppDbContext dbContext;

        public ServerAlertService(AppDbContext dbContext) => 
            this.dbContext = dbContext;

        public async Task<IList<ServerAlert>> Get(ulong serverId) => 
            await dbContext
                .ServerAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId)
                .ToListAsync();
        
        public async Task Create(ulong serverId, Manga manga)
        {
            var serverAlert = await dbContext
                .ServerAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId &&
                            x.Manga.MangaSiteId == manga.MangaSiteId &&
                            x.Manga.MangaSite == manga.MangaSite)
                .FirstOrDefaultAsync();

            if (serverAlert == null)
            {
                manga.ServerAlerts.Add(new ServerAlert(serverId, manga));
                dbContext.Mangas.Update(manga);
            }
        }

        public async Task Remove(ulong serverId, string mangaSiteId, MangaSite mangaSite)
        {
            var alert = await dbContext
                .ServerAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId &&
                            x.Manga.MangaSiteId == mangaSiteId &&
                            x.Manga.MangaSite == mangaSite)
                .FirstOrDefaultAsync();

            if (alert != null)
                dbContext.ServerAlerts.Remove(alert);
        }

        public async Task Remove(ulong serverId)
        {
            var alerts = await dbContext
                .ServerAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId)
                .ToListAsync();

            dbContext.ServerAlerts.RemoveRange(alerts);
        }
    }
}
