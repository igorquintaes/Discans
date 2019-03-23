using Discans.Shared.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.DiscordServices
{
    public class MangaUpdatesCrawlerService
    {
        private HtmlDocument document;

        public async Task<bool> LoadPageAsync(string link)
        {
            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri)
             || link.StartsWith("<"))
                return false;

            if (uri.Host != "www.mangaupdates.com")
                return false;

            var document = await new HtmlWeb().LoadFromWebAsync(uri.AbsoluteUri);
            var nodes = document.DocumentNode.SelectNodes("//span[contains(@class, 'releasestitle')]");
            if (nodes?.Count != 1)
                return false;

            this.document = document;
            return true;
        }

        public void LoadPage(string link) => 
            document = new HtmlWeb().Load(link);

        public IEnumerable<Manga> GetLastReleases()
        {
            document = new HtmlWeb().Load("https://www.mangaupdates.com/releases.html");
            var mangas = new List<Manga>();
            var tables = document.DocumentNode.SelectNodes("//*[@id='main_content']//div[@class='alt p-1']//div[@class='row no-gutters']");

            foreach (var table in tables)
            {
                var releasesCount = table.SelectNodes("div[@class='col-6 pbreak']/a/..").Count;
                for (var count = 1; count <= releasesCount; count++)
                {
                    mangas.Add(new Manga(
                        id: Convert.ToInt32(table.SelectSingleNode($"(div[@class='col-6 pbreak']/a)[{count}]")
                            .GetAttributeValue("href", "")
                            .Split("id=")
                            .Last()
                            .Trim()),
                        name: HtmlEntity.DeEntitize(table
                            .SelectSingleNode($"(div[@class='col-6 pbreak']/a/..)[{count}]")
                            .InnerText.Trim()),
                        lastRelease: HtmlEntity.DeEntitize(table
                            .SelectSingleNode($"((div[@class='col-6 pbreak']/a/..)[{count}]/following-sibling::div)[1]")
                            .InnerText.Trim())));
                }
            }

            return mangas;
        }

        public int GetMangaId() => 
            int.Parse(document.DocumentNode
                .SelectNodes("//*[contains(@href, 'stats.html?period=week&amp;series=')]")
                .Single()
                .GetAttributeValue("href", null)
                .Replace("stats.html?period=week&amp;series=", "§")
                .Split('§')
                .Last());

        public string GetMangaName() =>
            HtmlEntity.DeEntitize(document.DocumentNode
                .SelectNodes("//span[contains(@class, 'releasestitle')]")
                .Single()
                .InnerText);

        public string LastRelease()
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
