using Discans.Shared.Models;
using Discans.Shared.ViewModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var result = default(string);

            var baseAddress = new Uri("https://unionmangas.top/i");
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            cookieContainer.Add(baseAddress, new Cookie("__cfduid", "d3692fd2f6b7a4f4377c37a71dab479921562685336"));

            using (var httpClient = new HttpClient(handler))
            {

                httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("origin", "https://unionmangas.top/i");
                httpClient.DefaultRequestHeaders.Add("accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
                httpClient.DefaultRequestHeaders.Add("accept", "*/*");

                result = httpClient.GetAsync(baseAddress)
                    .GetAwaiter().GetResult()
                    .Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();
            }

            var document = new HtmlDocument();
            document.LoadHtml(result);

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
