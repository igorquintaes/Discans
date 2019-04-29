using Discans.Shared.Models;
using Discans.Shared.ViewModels;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace Discans.Shared.DiscordServices.CrawlerSites
{
    public class MangaUpdatesCrawlerService : IMangaSiteCrawlerService
    {
        private HtmlDocument document;

        public MangaSite MangaSite { get => MangaSite.MangaUpdates; }
        
        internal void SetDocument(HtmlDocument document) =>
            this.document = document;

        public IEnumerable<MangaRelease> LastMangaReleases()
        {
            var document = new HtmlWeb().Load("https://www.mangaupdates.com/releases.html");
            var mangas = new List<MangaRelease>();
            var tables = document.DocumentNode.SelectNodes("//*[@id='main_content']//div[@class='alt p-1']//div[@class='row no-gutters']");

            foreach (var table in tables)
            {
                var releasesCount = table.SelectNodes("div[@class='col-6 pbreak']/a/..").Count;
                for (var count = 1; count <= releasesCount; count++)
                {
                    mangas.Add(new MangaRelease(
                        mangaSiteId: table.SelectSingleNode($"(div[@class='col-6 pbreak']/a)[{count}]")
                            .GetAttributeValue("href", "")
                            .Split("id=").Last()
                            .Trim(),
                        name: default,
                        lastRelease: HtmlEntity.DeEntitize(table
                            .SelectSingleNode($"((div[@class='col-6 pbreak']/a/..)[{count}]/following-sibling::div)[1]")
                            .InnerText.Trim()),
                        mangaSite: MangaSite.MangaUpdates));
                }
            }

            return mangas;
        }

        public string GetMangaId() => 
            document.DocumentNode
                .SelectNodes("//*[contains(@href, 'stats.html?period=week&amp;series=')]")
                .Single()
                .GetAttributeValue("href", null)
                .Replace("stats.html?period=week&amp;series=", "§")
                .Split('§')
                .Last();

        public string GetMangaName() =>
            HtmlEntity.DeEntitize(document.DocumentNode
                .SelectNodes("//span[contains(@class, 'releasestitle')]")
                .Single()
                .InnerText);

        public string GetLastChapter()
        {
            var chaptersNodes = document.DocumentNode
                .SelectNodes("//*[text()='Latest Release(s)']/../following-sibling::div[@class='sContent'][1]")
                .FirstOrDefault();

            var lastRelease = HtmlEntity.DeEntitize(
                chaptersNodes
                    .InnerHtml
                    .Split("<br>")
                    .First()
                    .Split("by")
                    .First()
                    .Replace("<i>", "")
                    .Replace("</i>", "")
                    .Trim());

            if (lastRelease.Contains("N/A") ||
                lastRelease.Contains("Search for all releases of this series"))
                return "N/A";

            return lastRelease;
        }
    }
}
