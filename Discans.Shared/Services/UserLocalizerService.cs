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
    public class UserLocalizerService
    {
        private readonly AppDbContext context;
        public static IDictionary<ulong, string> Languages;

        public UserLocalizerService(AppDbContext context) => 
            this.context = context;

        public async Task CreateOrUpdate(ulong userId, string language)
        {
            if (!Languages.Any(x => x.Key == userId))
            {
                context.UserLocalizer.Add(new UserLocalizer(userId, language));
                Languages.Add(userId, language);

                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
                return;
            }

            var userLocalizer = await context.UserLocalizer.FirstAsync(x => x.UserId == userId);
            userLocalizer.UpdateLanguage(language);
            context.UserLocalizer.Update(userLocalizer);
            Languages[userId] = language;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
        }
    }
}
