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

        public ServerAlertService(AppDbContext context) => 
            this.dbContext = context;

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
                .Where(x => x.ServerId == serverId
                         && x.Manga.Id == manga.Id)
                .FirstOrDefaultAsync();

            if (serverAlert != null)
                return;

            serverAlert = new ServerAlert(serverId, manga);
            manga.ServerAlerts.Add(serverAlert);

            dbContext.Mangas.Update(manga);
        }

        public async Task Remove(ulong serverId, int mangaId)
        {
            var alert = await dbContext
                .ServerAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId
                         && x.Manga.Id == mangaId)
                .FirstOrDefaultAsync();

            dbContext.ServerAlerts.Remove(alert);
        }

        public async Task Remove(ulong serverId)
        {
            var alerts = await dbContext
                .ServerAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId)
                .ToListAsync();

            foreach (var alert in alerts)
            {
                dbContext.ServerAlerts.Remove(alert);
            }
        }
    }
}
