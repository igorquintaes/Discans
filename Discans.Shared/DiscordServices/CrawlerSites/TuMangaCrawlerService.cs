using System;
using System.Collections.Generic;
using System.Linq;
using Discans.Shared.Models;
using HtmlAgilityPack;

namespace Discans.Shared.DiscordServices.CrawlerSites
{
    public class TuMangaCrawlerService : IMangaSiteCrawlerService
    {
        public MangaSite MangaSite => MangaSite.TuManga;

        private HtmlDocument document;

        internal void SetDocument(HtmlDocument document) =>
            this.document = document;

        public string GetLastChapter() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("(//div[@id='app']//div[@class='card chapters']//a)[1]")
                .InnerText)
            .Trim().Replace(':', ' ')
            .Split(' ')[1];

        public int GetMangaId() => Convert.ToInt32(
            document.DocumentNode
                .SelectSingleNode("//a[contains(@href, 'https://tmofans.com/uploads/')]")
                .GetAttributeValue("href", null)
                .Split('/').Last());

        public string GetMangaName() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("//h2[@class='element-subtitle']")
                .InnerText)
            .Trim();

        public IEnumerable<Manga> LastMangaReleases()
        {
            var document = new HtmlWeb().Load("https://tmofans.com/?uploads_mode=list");
            var mangas = new List<Manga>();
            foreach (var release in document.DocumentNode.SelectNodes("//*[@id='latest_uploads']/..//tbody/tr"))
            {
                mangas.Add(new Manga(
                    mangaSiteId: Convert.ToInt32(release.SelectSingleNode("td[2]/a")
                        .GetAttributeValue("href", "")
                        .Split('/')
                        .Last()
                        .Trim()),
                    name: default,
                    lastRelease: HtmlEntity.DeEntitize(release.SelectSingleNode("td[3]/a")
                        .InnerText.Trim()),
                    mangaSite: MangaSite.TuManga));
            }

            return mangas;
        }
    }
}
