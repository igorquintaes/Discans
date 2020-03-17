using Discans.Extensions;
﻿using Discans.Attributes;
using Discans.Resources;
using Discans.Resources.Modules.Alert;
using Discans.Shared.Database;
using Discans.Shared.DiscordServices;
using Discans.Shared.Services;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Modules.Alert
{
    public class UserAlertModule : ModuleBase<SocketCommandContext>
    {
        private readonly MangaService mangaService;
        private readonly UserAlertService userAlertService;
        private readonly CrawlerService crawlerService;
        private readonly AppDbContext dbContext;
        private readonly LocaledResourceManager<UserAlertModuleResource> resourceManager;

        public const string AlertCommand = "user-alert";
        public const string AlertRemoveCommand = "user-alert-remove";
        public const string AlertListCommand = "user-alert-list";

        public UserAlertModule(
            MangaService mangaService,
            UserAlertService userAlertService,
            CrawlerService crawlerService,
            AppDbContext context,
            LocaledResourceManager<UserAlertModuleResource> resourceManager)
        {
            this.mangaService = mangaService;
            this.userAlertService = userAlertService;
            this.crawlerService = crawlerService;
            this.dbContext = context;
            this.resourceManager = resourceManager;
        }

        [Command(AlertCommand), ValidLink]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task UserAlert(string link, params IGuildUser[] users)
        {
            if (!users.Any())
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.UserNotFound)));
                return;
            }

            if (!(Context.Message.Author as IGuildUser).Guild.Roles.Any(x => x.Permissions.Administrator)
                && users.Any(x => x.Id != Context.Message.Author.Id))
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.OnlyAdminAlertSomeoneElse)));
                return;
            }

            var mangaCrawlerService = crawlerService.SiteCrawler;
            users = users.GroupBy(x => x.Id).Select(x => x.First()).ToArray();
            var mangaId = mangaCrawlerService.GetMangaId();
            var mangaName = mangaCrawlerService.GetMangaName();
            var lastRelease = mangaCrawlerService.GetLastChapter();
            var manga = await mangaService.GetOrCreateIfNew(mangaId, lastRelease, mangaName, mangaCrawlerService.MangaSite);

            await userAlertService.Create(
                userIds: users.Select(x => x.Id),
                serverId: Context.Guild.Id,
                manga: manga);

            await dbContext.SaveChangesAsync();
            await ReplyAsync(string.Format(resourceManager.GetString(
                nameof(UserAlertModuleResource.UserAlertSuccess)),
                $"{Consts.BotCommand}{ServerAlertModule.AlertListAllCommand}",
                manga.Name,
                string.Join(", ", users.Select(x => x.Nickname ?? x.Username)),
                manga.LastRelease,
                mangaCrawlerService.MangaSite.ToString()));
        }

        [Command(AlertRemoveCommand), Priority(1), ValidLink]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task UserAlertRemove(string link, params IGuildUser[] users)
        {
            if (!users.Any())
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.UserNotFound)));
                return;
            }

            if (!(Context.Message.Author as IGuildUser).Guild.Roles.Any(x => x.Permissions.Administrator)
                && users.Any(x => x.Id != Context.Message.Author.Id))
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.OnlyAdminAlertSomeoneElse)));
                return;
            }

            var mangaId = crawlerService.SiteCrawler.GetMangaId();
            await userAlertService.Remove(Context.Guild.Id, users.Select(x => x.Id), mangaId, crawlerService.SiteCrawler.MangaSite);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(
                nameof(UserAlertModuleResource.UserAlertRemoveSuccess)));
        }

        [Command(AlertRemoveCommand), Priority(2)]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task UserAlertRemove(params IGuildUser[] users)
        {
            if (!users.Any())
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.UserNotFound)));
                return;
            }

            if (!(Context.Message.Author as IGuildUser).Guild.Roles.Any(x => x.Permissions.Administrator)
                && users.Any(x => x.Id != Context.Message.Author.Id))
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.OnlyAdminAlertSomeoneElse)));
                return;
            }

            var mangaSiteId = crawlerService.SiteCrawler.GetMangaId();
            await userAlertService.Remove(Context.Guild.Id, users.Select(x => x.Id), mangaSiteId, crawlerService.SiteCrawler.MangaSite);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(
                nameof(UserAlertModuleResource.UserAlertRemoveAllSuccess)));
        }

        [Command(AlertListCommand)]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task UserAlertList(IGuildUser user)
        {
            var alerts = await userAlertService.GetUserServerAlerts(Context.Guild.Id, user.Id);

            if (!alerts.Any())
            {
                await ReplyAsync(resourceManager.GetString(
                    nameof(UserAlertModuleResource.UserHasNoAlerts)));
                return;
            }

            var alertMessages = alerts.Select(x => string.Format(resourceManager.GetString(
                nameof(UserAlertModuleResource.UserAlertListMessageItem)),
                x.Manga.Name,
                x.Manga.LastRelease,
                x.Manga.MangaSite.ToString()));

            await ReplyAsync(resourceManager.GetString(nameof(UserAlertModuleResource.UserAlertListHeader)));
            foreach (var alertChunck in alertMessages.ToList().ChunkList(10))
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }
    }
}
