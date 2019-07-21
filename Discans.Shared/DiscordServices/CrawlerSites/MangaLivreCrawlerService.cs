using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Discans.Shared.Models;
using Discans.Shared.ViewModels;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Discans.Shared.DiscordServices.CrawlerSites
{
    public class MangaLivreCrawlerService : IMangaSiteCrawlerService
    {
        public MangaSite MangaSite => MangaSite.MangaLivre;
        public string MangaUrl { get; set; }

        private HtmlDocument document;

        internal void SetDocument(HtmlDocument document) =>
            this.document = document;

        public string GetLastChapter()
        {
            var jsonResult = default(string);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                var content = httpClient.GetAsync($"https://mangalivre.com/series/chapters_list.json?page=1&id_serie={GetMangaId()}").GetAwaiter().GetResult();
                if (!content.IsSuccessStatusCode)
                    throw new Exception(content.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                jsonResult = content.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            return (JObject.Parse(jsonResult) as dynamic).chapters[0].number.ToString();
        }

        public string GetMangaId() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("//*[contains(@class, 'score-rating')]")
                .GetAttributeValue("data-id-serie", ""));

        public string GetMangaName() => HtmlEntity.DeEntitize(
            document.DocumentNode
                .SelectSingleNode("//*[@id='series-data']//*[contains(@class, 'series-title')]//h1")
                .InnerText)
            .Trim();

        public IEnumerable<MangaRelease> LastMangaReleases()
        {
            var jsonResult = default (string);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                var getResult = httpClient.GetAsync("https://mangalivre.com/home/releases?page=1").GetAwaiter().GetResult();
                if (!getResult.IsSuccessStatusCode)
                    throw new Exception(getResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                jsonResult = getResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            var mangas = new List<MangaRelease>();
            foreach (var release in (JObject.Parse(jsonResult) as dynamic).releases)
            {
                var mangaSiteId = release.id_serie.ToString();
                var mangaName = release.name.ToString();
                var lastRelease = release.chapters[0].number.ToString();
                dynamic scanInfo = ((IDictionary<string, JToken>)release.chapters[0].scanlators).First(x => x.Key.StartsWith("scan-")).Value as dynamic;

                var scanName = scanInfo.name.ToString();
                var scanLink = $"https://mangalivre.com{scanInfo.link.ToString()}";

                var manga = new MangaRelease(
                    mangaSiteId: mangaSiteId,
                    name: mangaName,
                    lastRelease: lastRelease,
                    mangaSite: MangaSite,
                    scanName: scanName,
                    scanLink: scanLink);

                manga.MangaLink = $"https://mangalivre.com{release.chapters[0].url.ToString()}";
                mangas.Add(manga);                
            }

            return mangas;
        }
    }
}
