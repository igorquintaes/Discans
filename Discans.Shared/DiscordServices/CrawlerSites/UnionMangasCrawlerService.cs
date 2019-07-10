using Discans.Shared.Models;
using Discans.Shared.ViewModels;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Discans.Shared.DiscordServices.CrawlerSites
{
    public class UnionMangasCrawlerService : IMangaSiteCrawlerService
    {
        public MangaSite MangaSite => MangaSite.UnionMangas;

        private HtmlDocument document;

        internal void SetDocument(HtmlDocument document) =>
            this.document = document;

        public IEnumerable<MangaRelease> LastMangaReleases()
        {
            var document = new HtmlWeb().Load("https://unionmangas.top");
            var mangas = new List<MangaRelease>();
            var nodes = document.DocumentNode.SelectNodes("//div[@class='row' and @style='margin-bottom: 10px;' and .//a[@class='link-titulo']]").ToList();
            for (var i = 0; i < nodes.Count; i++)
            {
                var mangaSiteId = HtmlEntity.DeEntitize(document.DocumentNode
                    .SelectSingleNode($"(//div[@class='row' and @style='margin-bottom: 10px;' and .//a[@class='link-titulo']])[{i + 1}]//a[@class='link-titulo'][2]")
                    .GetAttributeValue("href", "")
                    .Split('/').Last());

                var lastRelease = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode($"((//div[@class='row' and @style='margin-bottom: 10px;' and .//a[@class='link-titulo']][{i + 1}])/following-sibling::*[1]//a)[1]")
                        .InnerText)
                    .Replace("Capítulo", "")
                    .Trim();

                var scanName = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode($"((//div[@class='row' and @style='margin-bottom: 10px;' and .//a[@class='link-titulo']][{i + 1}])/following-sibling::*[1]//a)[2]")
                        .InnerText)
                    .Trim();

                var scanLink = document.DocumentNode
                        .SelectSingleNode($"((//div[@class='row' and @style='margin-bottom: 10px;' and .//a[@class='link-titulo']][{i + 1}])/following-sibling::*[1]//a)[2]")
                        .GetAttributeValue("href", "").ToLower();

                if (scanLink.Contains("google.com") || scanLink.Contains("unionmangas.top"))
                    scanLink = $"https://www.google.com/search?q={scanName.Replace(" ", "+")}+{mangaSiteId.Replace("-", "+")}";

                mangas.Add(new MangaRelease(
                    mangaSiteId: mangaSiteId,
                    name: default,
                    lastRelease: lastRelease,
                    mangaSite: MangaSite.UnionMangas,
                    scanName: scanName,
                    scanLink: scanLink));
            }

            return mangas;
        }

        public string GetLastChapter() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("((//div[@class='row lancamento-linha'])[1]//a)[1]")
                .InnerText)
            .Replace("Cap.", "")
            .Trim();

        public string GetMangaName() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("(//div[@class='col-md-12']//h2)[1]")
                .InnerText)
            .Trim();

        public string GetMangaId() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("(//*[@id='share']/div)[1]")
                .GetAttributeValue("data-href", ""))
            .Split('/')
            .Last();
    }
}
