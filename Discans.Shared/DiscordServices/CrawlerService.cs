using Discans.Shared.DiscordServices.CrawlerSites;
using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace Discans.Shared.DiscordServices
{
    public class CrawlerService
    {
        private readonly MangaUpdatesCrawlerService mangaUpdates;
        private readonly TuMangaCrawlerService tuManga;

        public CrawlerService(
            MangaUpdatesCrawlerService mangaUpdates, 
            TuMangaCrawlerService tuManga)
        {
            this.mangaUpdates = mangaUpdates;
            this.tuManga = tuManga;
        }

        public async Task<(bool, IMangaSiteCrawlerService)> LoadPageAsync(string link)
        {
            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri)
             || link.StartsWith("<"))
                return (false, default);

            switch (uri.Host.Replace("www.", ""))
            {
                case "mangaupdates.com":
                    var mangaUpdatesDocument = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
                    var mangaUpdatesNodes = mangaUpdatesDocument.DocumentNode.SelectNodes("//span[contains(@class, 'releasestitle')]");
                    if (mangaUpdatesNodes?.Count != 1)
                        return (false, default);

                    mangaUpdates.SetDocument(mangaUpdatesDocument);
                    return (true, mangaUpdates);
                case "tmofans.com":
                    var tuMangaDocument = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
                    var tuMangaNodes = tuMangaDocument.DocumentNode.SelectNodes("//div[@id='app']//div[@class='card chapters']");
                    if (tuMangaNodes?.Count != 1)
                        return (false, default);

                    tuManga.SetDocument(tuMangaDocument);
                    return (true, tuManga);
                default:
                    return (false, default);
            }
        }
    }
}
