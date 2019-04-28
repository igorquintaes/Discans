using Discans.Shared.Models;
using System.Collections.Generic;

namespace Discans.Shared.DiscordServices
{
    public interface IMangaSiteCrawlerService
    {
        MangaSite MangaSite { get; }

        IEnumerable<Manga> LastMangaReleases();
        string GetLastChapter();
        string GetMangaName();
        int GetMangaId();
    }
}
