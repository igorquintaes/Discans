using Discans.Attributes;
using Discans.Extensions;
using Discans.Resources;
using Discans.Resources.Modules.Alert;
using Discans.Shared.Database;
using Discans.Shared.DiscordServices;
using Discans.Shared.Services;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Modules.Alert
{
    public class ServerAlertModule : ModuleBase<SocketCommandContext>
    {
        private readonly MangaService mangaService;
        private readonly ServerAlertService serverAlertService;
        private readonly UserAlertService userAlertService;
        private readonly CrawlerService crawlerService;
        private readonly AppDbContext dbContext;
        private static LocaledResourceManager resourceManager;

        public const string AlertCommand = "server-alert";
        public const string AlertRemoveCommand = "server-alert-remove";
        public const string AlertRemoveAllConfirmCommand = "server-alert-remove-all-confirm";
        public const string AlertRemoveAllCommand = "server-alert-remove-all";
        public const string AlertListCommand = "server-alert-list";
        public const string AlertListAllCommand = "server-alert-list all";

        public ServerAlertModule(
            MangaService mangaService,
            ServerAlertService serverAlertService,
            UserAlertService userAlertService,
            CrawlerService crawlerService,
            AppDbContext dbContext)
        {
            this.mangaService = mangaService;
            this.serverAlertService = serverAlertService;
            this.userAlertService = userAlertService;
            this.crawlerService = crawlerService;
            this.dbContext = dbContext;

            resourceManager = resourceManager
                ?? new LocaledResourceManager(nameof(ServerAlertModuleResource),
                                              typeof(ServerAlertModuleResource).Assembly);
        }

        [Command(AlertCommand), Admin, ValidLink]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task ServerAlert(string link)
        {
            var mangaCrawlerService = crawlerService.SiteCrawler;
            var mangaId = mangaCrawlerService.GetMangaId();
            var mangaName = mangaCrawlerService.GetMangaName();
            var lastRelease = mangaCrawlerService.GetLastChapter();
            var manga = await mangaService.GetOrCreateIfNew(mangaId, lastRelease, mangaName, mangaCrawlerService.MangaSite);

            await serverAlertService.Create(Context.Guild.Id, manga);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(string.Format(resourceManager.GetString(
                nameof(ServerAlertModuleResource.ServerAlertSuccess)),
                manga.Name,
                manga.LastRelease,
                mangaCrawlerService.MangaSite.ToString()));
        }

        [Command(AlertRemoveCommand), Admin, ValidLink]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task ServerAlertRemove(string link)
        {
            var mangaCrawlerService = crawlerService.SiteCrawler;
            var mangaSiteId = mangaCrawlerService.GetMangaId();
            await serverAlertService.Remove(Context.Guild.Id, mangaSiteId, mangaCrawlerService.MangaSite);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(
                nameof(ServerAlertModuleResource.ServerAlertRemoveSuccess)));
        }

        [Command(AlertRemoveAllConfirmCommand), Admin]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task ServerAlertRemoveAllConfirm()
        {
            await serverAlertService.Remove(Context.Guild.Id);
            await userAlertService.Remove(Context.Guild.Id);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(
                nameof(ServerAlertModuleResource.ServerAlertRemoveAllConfirmSuccess)));
        }

        [Command(AlertRemoveAllCommand), Admin]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task ServerAlertRemoveAll() => 
            await ReplyAsync(resourceManager.GetString(
                nameof(ServerAlertModuleResource.ServerAlertRemoveAllMessage)));

        [Command(AlertListCommand)]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task ServerAlertList()
        {
            var alerts = await serverAlertService.Get(Context.Guild.Id);
            if (!alerts.Any())
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(ServerAlertModuleResource.ServerAlertListEmpty)));
                return;
            }

            var alertMessages = alerts.Select(x => string.Format(resourceManager.GetString(
                    nameof(ServerAlertModuleResource.ServerAlertListMessageItem)), 
                    x.Manga.Name, 
                    x.Manga.LastRelease,
                    x.Manga.MangaSite.ToString()));

            await ReplyAsync(resourceManager.GetString(nameof(ServerAlertModuleResource.ServerAlertListMessageHeader)));
            foreach (var alertChunck in alertMessages.ToList().ChunkList(10))
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }

        [Command(AlertListAllCommand)]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task ServerAlertListAll()
        {
            var serverAlerts = await serverAlertService.Get(Context.Guild.Id);
            var userAlerts = await userAlertService.GetServerAlerts(Context.Guild.Id);

            if (!serverAlerts.Any() && !userAlerts.Any())
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(ServerAlertModuleResource.ServerAlertListAllEmpty)));
                return;
            }

            var serverAlertMessages = serverAlerts.Select(x => string.Format(resourceManager.GetString(
                nameof(ServerAlertModuleResource.ServerAlertListAllMessageItem)),
                x.Manga.Name,
                Context.Guild.Name,
                x.Manga.LastRelease,
                x.Manga.MangaSite.ToString()));

            var userAlertMessages = userAlerts.GroupBy(x => x.Manga.Id).Select(x => string.Format(resourceManager.GetString(
                nameof(ServerAlertModuleResource.ServerAlertListAllMessageItem)),
                x.First().Manga.Name,
                string.Join(", ", x.Select(alert => Context.Guild.Users.FirstOrDefault(y => y.Id == alert.UserId)?.Username)
                                   .Where(y => y != null)),
                x.First().Manga.LastRelease,
                x.First().Manga.MangaSite.ToString()));
            
            await ReplyAsync(resourceManager.GetString(nameof(ServerAlertModuleResource.ServerAlertListAllMessageHeader)));
            foreach (var alertChunck in serverAlertMessages.Concat(userAlertMessages).ToList().ChunkList(7))
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }
    }
}
