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

        public UserAlertService(AppDbContext context) => 
            this.dbContext = context;

        public async Task<IList<UserAlert>> GerUserServerAlert(ulong serverId, ulong userId) => 
            await dbContext
                .UserAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId 
                    && x.UserId == userId)
                .ToListAsync();

        public async Task<IList<UserAlert>> GetServerAlert(ulong serverId) => 
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
                            x.Manga.Id == manga.Id &&
                            userIds.Contains(x.UserId))
                .ToListAsync();

            foreach (var userId in userIds.Where(x => !createdAlerts.Select(y => y.UserId).Contains(x)))
                manga.UserAlerts.Add(new UserAlert(userId, serverId, manga));

            dbContext.Mangas.Update(manga);
        }        

        public async Task Remove(ulong serverId)
        {
            var alerts = await dbContext
                .UserAlerts
                .Where(x => x.ServerId == serverId)
                .ToListAsync();

            foreach (var alert in alerts)
            {
                dbContext.UserAlerts.Remove(alert);
            }
        }

        public async Task Remove(ulong serverId, IEnumerable<ulong> userIds)
        {
            var alerts = await dbContext
                .UserAlerts
                .Where(x => x.ServerId == serverId
                         && userIds.Contains(x.UserId))
                .ToListAsync();

            foreach (var alert in alerts)
            {
                dbContext.UserAlerts.Remove(alert);
            }
        }

        public async Task Remove(ulong serverId, IEnumerable<ulong> userIds, int mangaId)
        {
            var alerts = await dbContext
                .UserAlerts
                .Include(x => x.Manga)
                .Where(x => x.ServerId == serverId
                         && x.Manga.Id == mangaId
                         && userIds.Contains(x.UserId))
                .ToListAsync();

            foreach (var talertack in alerts)
            {
                dbContext.UserAlerts.Remove(talertack);
            }
        }        
    }

    [Flags]
    public enum AlertCreateStatus : short
    {
        Success = 0b01,
        Error = 0b10
    }
}
