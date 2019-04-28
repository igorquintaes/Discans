using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class ServerLocalizerService
    {
        private readonly AppDbContext context;
        public static IDictionary<ulong, string> Languages;

        public ServerLocalizerService(AppDbContext context) =>
            this.context = context;

        public async Task CreateOrUpdate(ulong serverId, string language)
        {
            if (!Languages.Any(x => x.Key == serverId))
            {
                context.ServerLocalizer.Add(new ServerLocalizer(serverId, language));
                Languages.Add(serverId, language);
                return;
            }

            var serverLocalizer = await context.ServerLocalizer.FirstOrDefaultAsync(x => x.ServerId == serverId);
            serverLocalizer.Language = language;
            context.ServerLocalizer.Update(serverLocalizer);
            Languages[serverId] = language;
        }
    }
}
