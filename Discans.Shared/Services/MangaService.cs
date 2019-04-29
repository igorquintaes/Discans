using Discans.Shared.Database;
using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class MangaService
    {
        private readonly AppDbContext context;

        public MangaService(AppDbContext context) => 
            this.context = context;

        public async Task<IList<Manga>> GetAll() => 
            await context
                .Mangas
                .Include(x => x.PrivateAlerts)
                .Include(x => x.ServerAlerts)
                .Include(x => x.UserAlerts)
                .ToListAsync();

        public async Task<Manga> GetOrCreateIfNew(string mangaSiteId, string lastRelease, string name, MangaSite mangaSite)
        {
            var manga = await context.Mangas
                .Include(x => x.ServerAlerts)
                .Include(x => x.UserAlerts)
                .Include(x => x.PrivateAlerts)
                .FirstOrDefaultAsync(x => x.MangaSiteId == mangaSiteId);

            if (manga != null)
                return manga;

            manga = new Manga(mangaSiteId, name, lastRelease, mangaSite);
            context.Mangas.Add(manga);
            await context.SaveChangesAsync();
            return manga;
        }

        public async Task<Manga> UpdateLastRelease(int id, string lastRelease)
        {
            var manga = await context.Mangas.FindAsync(id);
            manga.UpdateLastRelase(lastRelease);

            context.Update(manga);
            await context.SaveChangesAsync();
            return manga;
        }
    }
}
