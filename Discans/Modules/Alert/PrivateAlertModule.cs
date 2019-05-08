using Discans.Extensions;
using Discans.Attributes;
using Discans.Resources.Modules.Alert;
using Discans.Shared.Database;
using Discans.Shared.DiscordServices;
using Discans.Shared.Services;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discans.Resources;

namespace Discans.Modules.Alert
{
    public class PrivateAlertModule : ModuleBase<SocketCommandContext>
    {
        private readonly MangaService mangaService;
        private readonly PrivateAlertService privateAlertService;
        private readonly CrawlerService crawlerService;
        private readonly AppDbContext dbContext;
        private readonly LocaledResourceManager<PrivateAlertModuleResource> resourceManager;

        public const string AlertCommand = "private-alert";
        public const string AlertRemoveCommand = "private-alert-remove";
        public const string AlertListCommand = "private-alert-list";

        public PrivateAlertModule(
            MangaService mangaService,
            PrivateAlertService privateAlertService,
            CrawlerService crawlerService,
            AppDbContext dbContext,
            LocaledResourceManager<PrivateAlertModuleResource> resourceManager)
        {
            this.mangaService = mangaService;
            this.privateAlertService = privateAlertService;
            this.crawlerService = crawlerService;
            this.dbContext = dbContext;
            this.resourceManager = resourceManager;
        }
        
        [Command(AlertCommand), ValidLink]
        [LocaledRequireContext(ContextType.DM)]
        public async Task PrivateAlert(string link)
        {
            var mangaCrawlerService = crawlerService.SiteCrawler;
            var mangaId = mangaCrawlerService.GetMangaId();
            var mangaName = mangaCrawlerService.GetMangaName();
            var lastRelease = mangaCrawlerService.GetLastChapter();
            var manga = await mangaService.GetOrCreateIfNew(mangaId, lastRelease, mangaName, mangaCrawlerService.MangaSite);

            await privateAlertService.Create(
                userId: Context.User.Id,
                manga: manga);

            await dbContext.SaveChangesAsync();
            var message = string.Format(
                resourceManager.GetString(nameof(PrivateAlertModuleResource.UserAlertSuccess)),
                $"{Consts.BotCommand}{AlertListCommand}",
                mangaName,
                lastRelease,
                mangaCrawlerService.MangaSite.ToString());

            await ReplyAsync(message);
        }

        [Command(AlertRemoveCommand), ValidLink]
        [LocaledRequireContext(ContextType.DM)]
        public async Task PrivateAlertRemove(string link)
        {
            var mangaCrawlerService = crawlerService.SiteCrawler;
            var mangaSiteId = mangaCrawlerService.GetMangaId();
            await privateAlertService.Remove(Context.User.Id, mangaSiteId, mangaCrawlerService.MangaSite);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(nameof(PrivateAlertModuleResource.UserAlertRemove)));
        }
               
        [Command(AlertListCommand)]
        [LocaledRequireContext(ContextType.DM)]
        public async Task PrivateAlertList()
        {
            var alerts = await privateAlertService.GetAlerts(Context.User.Id);
            if (!alerts.Any())
            {
                await ReplyAsync(resourceManager.GetString(nameof(PrivateAlertModuleResource.NoAlerts)));
                return;
            }

            var alertMessages = alerts
                .Select(x => string.Format(
                    nameof(PrivateAlertModuleResource.AlertListChunck),
                    x.Manga.Name,
                    x.Manga.LastRelease,
                    x.Manga.MangaSite.ToString()))
                .ToList()
                .ChunkList(10);

            await ReplyAsync(nameof(PrivateAlertModuleResource.AlertListTitle));
            foreach (var alertChunck in alertMessages)
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }
    }
}
