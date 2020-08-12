using Discans.Attributes;
using Discans.Resources;
using Discans.Resources.Modules;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Modules
{
    public class ConfigureModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient discord;
        private readonly ChannelService channelService;
        private readonly UserLocalizerService userLocalizerService;
        private readonly ServerLocalizerService serverLocalizerService;
        private readonly AppDbContext dbContext;
        private readonly LocaledResourceManager<ConfigureModuleResource> resourceManager;
        private readonly IConfiguration configuration;
        public const string ChannelCommand = "channel";
        public const string LanguageCommand = "language";

        public ConfigureModule(
            ChannelService channelService,
            UserLocalizerService userLocalizerService,
            ServerLocalizerService serverLocalizerService,
            AppDbContext dbContext,
            LocaledResourceManager<ConfigureModuleResource> resourceManager,
            IConfiguration  configuration,
            DiscordSocketClient discord)
        {
            this.channelService = channelService;
            this.userLocalizerService = userLocalizerService;
            this.serverLocalizerService = serverLocalizerService;
            this.dbContext = dbContext;
            this.resourceManager = resourceManager;
            this.configuration = configuration;
            this.discord = discord;
        }

        [Command(ChannelCommand), Admin]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task Channel()
        {
            await channelService.SaveOrUpdate(Context.Guild.Id, Context.Channel.Id);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(
                nameof(ConfigureModuleResource.ChannelSuccess)));
        }

        [Command(LanguageCommand), Admin]
        [LocaledRequireContext(ContextType.Guild | ContextType.DM)]
        public async Task Language(string language)
        {
            if (!LanguageService.AllowedLanguages.Contains(language))
            {
                await ReplyAsync(string.Format(resourceManager.GetString(
                    nameof(ConfigureModuleResource.LanguageUnsupported)),
                    string.Join(Environment.NewLine, LanguageService.AllowedLanguages)));
                return;
            }

            if (Context.Channel is IGuildChannel)
                await serverLocalizerService.CreateOrUpdate(Context.Guild.Id, language);
            else
                await userLocalizerService.CreateOrUpdate(Context.User.Id, language);

            await dbContext.SaveChangesAsync();
            await ReplyAsync(resourceManager.GetString(
                nameof(ConfigureModuleResource.LanguageUpdated), CultureInfo.GetCultureInfo(language)));
        }

        [Command("message-all-servers")]
        [LocaledRequireContext(ContextType.Guild | ContextType.DM)]
        public async Task MessageAllServers(string message)
        {
            var userId = configuration.GetValue<ulong>("BOTOWNERID");
            if (Context.User.Id != userId)
                return;

            var channels = dbContext.ServerChannels.ToList();
            foreach(var channel in channels)
            {
                await (discord
                        .GetGuild(channel.Id)
                        .GetChannel(channel.ChannelId) as SocketTextChannel)
                        .SendMessageAsync(message);
            }
        }
    }
}
