using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
                return;
            }

            var userLocalizer = await context.UserLocalizer.FirstOrDefaultAsync(x => x.UserId == userId);
            userLocalizer.Language = language;
            context.UserLocalizer.Update(userLocalizer);
            Languages[userId] = language;
        }
    }
}
