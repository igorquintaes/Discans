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
        private readonly UnionMangasCrawlerService unionMangas;
        private readonly InfoAnimeCrawlerService infoAnime;

        public IMangaSiteCrawlerService SiteCrawler { get; private set; }

        public CrawlerService(
            MangaUpdatesCrawlerService mangaUpdates, 
            TuMangaCrawlerService tuManga,
            UnionMangasCrawlerService unionMangas,
            InfoAnimeCrawlerService infoAnimeCrawlerService)
        {
            this.mangaUpdates = mangaUpdates;
            this.tuManga = tuManga;
            this.unionMangas = unionMangas;
            this.infoAnime = infoAnimeCrawlerService;
        }

        public async Task<bool> LoadPageAsync(string link)
        {
            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri)
             || link.StartsWith("<"))
                return false;

            switch (uri.Host.Replace("www.", ""))
            {
                case "mangaupdates.com":
                    var mangaUpdatesDocument = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
                    var mangaUpdatesNodes = mangaUpdatesDocument.DocumentNode.SelectNodes("//span[contains(@class, 'releasestitle')]");
                    if (mangaUpdatesNodes?.Count != 1)
                        return false;

                    mangaUpdates.SetDocument(mangaUpdatesDocument);
                    SiteCrawler = mangaUpdates;
                    return true;

                case "tmofans.com":
                    var tuMangaDocument = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
                    var tuMangaNodes = tuMangaDocument.DocumentNode.SelectNodes("//div[@id='app']//div[@class='card chapters']");
                    if (tuMangaNodes?.Count != 1)
                        return false;

                    tuManga.SetDocument(tuMangaDocument);
                    SiteCrawler = tuManga;
                    return true;

                case "unionmangas.top":
                    var unionMangasDocument = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
                    var unionMangasNodes = unionMangasDocument.DocumentNode.SelectNodes("//div[@class='row lancamento-linha']");
                    if (unionMangasNodes?.Count == 0)
                        return false;

                    unionMangas.SetDocument(unionMangasDocument);
                    SiteCrawler = unionMangas;
                    return true;

                case "infoanime.com.br":
                    var infoAnimeDocument = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
                    var infoAnimeNodes = infoAnimeDocument.DocumentNode.SelectNodes("//*[@class='grid_i_nome titulo1']");
                    if (infoAnimeNodes?.Count == 0)
                        return false;

                    infoAnime.SetDocument(infoAnimeDocument);
                    infoAnime.MangaUrl = link;
                    SiteCrawler = infoAnime;
                    return true;


                default:
                    return false;
            }
        }
    }
}
