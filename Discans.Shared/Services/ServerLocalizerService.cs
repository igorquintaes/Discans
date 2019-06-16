using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class ServerLocalizerService
    {
        private readonly AppDbContext context;
        public static IDictionary<ulong, string> Languages;

        public ServerLocalizerService(AppDbContext context) =>
            this.context = context;

        public virtual async Task CreateOrUpdate(ulong serverId, string language)
        {
            if (!Languages.Any(x => x.Key == serverId))
            {
                context.ServerLocalizer.Add(new ServerLocalizer(serverId, language));
                Languages.Add(serverId, language);

                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
                return;
            }

            var serverLocalizer = await context.ServerLocalizer.FirstAsync(x => x.ServerId == serverId);
            serverLocalizer.UpdateLanguage(language);
            context.ServerLocalizer.Update(serverLocalizer);
            Languages[serverId] = language;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
        }
    }
}
