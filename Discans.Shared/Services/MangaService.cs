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

        public async Task<Manga> GetOrCreateIfNew(int id, string lastRelease, string name)
        {
            var manga = await context.Mangas
                .Include(x => x.ServerAlerts)
                .Include(x => x.UserAlerts)
                .Include(x => x.PrivateAlerts)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (manga != null)
                return manga;

            manga = new Manga(id, name, lastRelease);
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
