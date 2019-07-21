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
                    .SelectSingleNode($"(//tr[./td])[{i + 1}]/td[2]/a")
                    .GetAttributeValue("href", "")
                    .Replace("dados?", ""));

                var lastRelease = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode($"(//tr[./td])[{i + 1}]/td[4]")
                        .InnerText)
                    .Trim();

                var scanName = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode($"(//tr[./td])[{i + 1}]/td[5]/a")
                        .InnerText)
                    .Trim();

                var scanLink = HtmlEntity.DeEntitize(document.DocumentNode
                        .SelectSingleNode($"(//tr[./td])[{i + 1}]/td[5]/a")
                        .GetAttributeValue("href", ""))
                    .Trim();

                mangas.Add(new MangaRelease(
                    mangaSiteId: mangaSiteId,
                    name: default,
                    lastRelease: lastRelease,
                    mangaSite: MangaSite,
                    scanName: scanName,
                    scanLink: scanLink));
            }

            return mangas;
        }
    }
}
