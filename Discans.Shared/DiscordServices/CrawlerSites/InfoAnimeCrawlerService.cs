using System;
using System.Collections.Generic;
using System.Linq;
using Discans.Shared.Models;
using Discans.Shared.ViewModels;
using HtmlAgilityPack;

namespace Discans.Shared.DiscordServices.CrawlerSites
{
    public class InfoAnimeCrawlerService : IMangaSiteCrawlerService
    {
        public MangaSite MangaSite => MangaSite.InfoAnime;
        public string MangaUrl { get; set; }

        private HtmlDocument document;

        internal void SetDocument(HtmlDocument document) =>
            this.document = document;

        public string GetLastChapter() =>
            document.DocumentNode
                .SelectNodes("(//table)[1]//tr[./td]/td[3]")
                .Select(x => Convert.ToInt32(x.InnerText.Trim()))
                .Max()
                .ToString();

        public string GetMangaId() =>
            MangaUrl
                .Replace("dados?", "ª")
                .Split("ª")
                .Last();

        public string GetMangaName() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("//*[@class='grid_i_nome titulo1']")
                .InnerText)
            .Trim();

        public IEnumerable<MangaRelease> LastMangaReleases()
        {
            var document = new HtmlWeb().Load("https://www.infoanime.com.br");
            var mangas = new List<MangaRelease>();
            var nodes = document.DocumentNode.SelectNodes("//tr[./td]").ToList();
            for (var i = 0; i < nodes.Count; i++)
            {
                var mangaSiteId = HtmlEntity.DeEntitize(document.DocumentNode
                    .SelectSingleNode($"//td[2]/a")
                    .GetAttributeValue("href", "")
                    .Replace("dados?", ""));

                var lastRelease = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode("//td[4]")
                        .InnerText)
                    .Trim();

                var scanName = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode("//td[5]/a")
                        .InnerText)
                    .Trim();

                var infoAnimesScanLink = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode("//td[5]/a")
                        .GetAttributeValue("href", ""))
                    .Trim();

                var scanLink = HtmlEntity.DeEntitize(new HtmlWeb()
                        .Load($"https://www.infoanime.com.br/{infoAnimesScanLink}")
                        .DocumentNode.SelectSingleNode("//a[./img]")
                         ?.GetAttributeValue("href", ""))
                    ?.Trim();

                if (string.IsNullOrWhiteSpace(scanLink))
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
    }
}
