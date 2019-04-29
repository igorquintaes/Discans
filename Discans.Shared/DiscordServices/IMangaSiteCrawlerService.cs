using Discans.Shared.Models;
using Discans.Shared.ViewModels;
using System.Collections.Generic;

namespace Discans.Shared.DiscordServices
{
    public interface IMangaSiteCrawlerService
    {
        MangaSite MangaSite { get; }

        IEnumerable<MangaRelease> LastMangaReleases();
        string GetLastChapter();
        string GetMangaName();
        string GetMangaId();
    }
}
