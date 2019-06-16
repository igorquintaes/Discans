using Discans.Attributes;
using Discans.Modules.Base;
using Discans.Resources;
using Discans.Resources.Modules;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discord;
using Discord.Commands;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Modules
{
    public class ConfigureModule : ProjectModuleBase<ConfigureModuleResource>
    {
        private readonly ChannelService channelService;
        private readonly UserLocalizerService userLocalizerService;
        private readonly ServerLocalizerService serverLocalizerService;
        private readonly AppDbContext dbContext;

        public const string ChannelCommand = "channel";
        public const string LanguageCommand = "language";

        public ConfigureModule(
            ChannelService channelService,
            UserLocalizerService userLocalizerService,
            ServerLocalizerService serverLocalizerService,
            AppDbContext dbContext,
            LocaledResourceManager<ConfigureModuleResource> resourceManager)
            : base (resourceManager)
        {
            this.channelService = channelService;
            this.userLocalizerService = userLocalizerService;
            this.serverLocalizerService = serverLocalizerService;
            this.dbContext = dbContext;
        }

        [Command(ChannelCommand), Admin]
        [LocaledRequireContext(ContextType.Guild)]
        public async Task SetChannel()
        {
            await channelService.SaveOrUpdate(GuildId, ChannelId);
            await dbContext.SaveChangesAsync();
            await ReplyAsync(ResourceManager.GetString(
                nameof(ConfigureModuleResource.ChannelSuccess)));
        }

        [Command(LanguageCommand), Admin]
        [LocaledRequireContext(ContextType.Guild | ContextType.DM)]
        public async Task Language(string language)
        {
            if (!LanguageService.AllowedLanguages.Contains(language))
            {
                await ReplyAsync(string.Format(ResourceManager.GetString(
                    nameof(ConfigureModuleResource.LanguageUnsupported)),
                    string.Join(Environment.NewLine, LanguageService.AllowedLanguages)));
                return;
            }

            if (Channel is IGuildChannel)
                await serverLocalizerService.CreateOrUpdate(GuildId, language);
            else
                await userLocalizerService.CreateOrUpdate(UserId, language);

            await dbContext.SaveChangesAsync(); 
            await ReplyAsync(ResourceManager.GetString(
                nameof(ConfigureModuleResource.LanguageUpdated), CultureInfo.GetCultureInfo(language)));
        }
    }
}
