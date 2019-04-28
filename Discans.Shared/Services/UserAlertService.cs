using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class UserAlertService
    {
        private readonly AppDbContext dbContext;

        public UserAlertService(AppDbContext dbContext) => 
            this.dbContext = dbContext;

        public async Task<IList<UserAlert>> GerUserServerAlerts(ulong serverId, ulong userId) => 
            await dbContext
                .UserAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId && 
                            x.UserId == userId)
                .ToListAsync();

        public async Task<IList<UserAlert>> GetServerAlerts(ulong serverId) => 
            await dbContext
                .UserAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId)
                .ToListAsync();

        public async Task Create(IEnumerable<ulong> userIds, ulong serverId, Manga manga)
        {
            var createdAlerts = await dbContext
                .UserAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId &&
                            x.Manga.MangaSiteId == manga.MangaSiteId &&
                            x.Manga.MangaSite == manga.MangaSite &&
                            userIds.Contains(x.UserId))
                .ToListAsync();

            userIds
                .Where(x => !createdAlerts.Select(y => y.UserId).Contains(x))
                .Select(x => new UserAlert(x, serverId, manga)).ToList()
                .ForEach(manga.UserAlerts.Add);

            dbContext.Mangas.Update(manga);
        }        

        public async Task Remove(ulong serverId)
        {
            var alerts = await dbContext
                .UserAlerts
                .Where(x => x.ServerId == serverId)
                .ToListAsync();

            dbContext.UserAlerts.RemoveRange(alerts);
        }

        public async Task Remove(ulong serverId, IEnumerable<ulong> userIds)
        {
            var alerts = await dbContext
                .UserAlerts
                .Where(x => x.ServerId == serverId && 
                            userIds.Contains(x.UserId))
                .ToListAsync();

            dbContext.UserAlerts.RemoveRange(alerts);
        }

        public async Task Remove(ulong serverId, IEnumerable<ulong> userIds, int mangaSiteId, MangaSite mangaSite)
        {
            var alerts = await dbContext
                .UserAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId &&
                            x.Manga.MangaSiteId == mangaSiteId &&
                            x.Manga.MangaSite == mangaSite &&
                            userIds.Contains(x.UserId))
                .ToListAsync();

            dbContext.UserAlerts.RemoveRange(alerts);
        }        
    }
}
