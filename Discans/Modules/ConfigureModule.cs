using Discans.Attributes;
using Discans.Resources;
using Discans.Resources.Modules;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Modules
{
    public class ConfigureModule : ModuleBase<SocketCommandContext>
    {
        private readonly ChannelService channelService;
        private readonly UserLocalizerService userLocalizerService;
        private readonly ServerLocalizerService serverLocalizerService;
        private readonly AppDbContext dbContext;
        private static LocaledResourceManager resourceManager;

        public const string ChannelCommand = "channel";
        public const string LanguageCommand = "language";

        public ConfigureModule(
            ChannelService channelService,
            UserLocalizerService userLocalizerService,
            ServerLocalizerService serverLocalizerService,
            AppDbContext dbContext)
        {
            this.channelService = channelService;
            this.userLocalizerService = userLocalizerService;
            this.serverLocalizerService = serverLocalizerService;
            this.dbContext = dbContext;

            resourceManager = resourceManager
                ?? new LocaledResourceManager(nameof(ConfigureModuleResource),
                                              typeof(ConfigureModuleResource).Assembly);
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
                await ReplyAsync(resourceManager.GetString(
                    nameof(ConfigureModuleResource.LanguageUnsupported)));
                return;
            }

            if (Context.Channel is IGuildChannel)
                await serverLocalizerService.CreateOrUpdate(Context.Guild.Id, language);
            else
                await userLocalizerService.CreateOrUpdate(Context.User.Id, language);

            await dbContext.SaveChangesAsync(); 
            await ReplyAsync(resourceManager.GetString(
                nameof(ConfigureModuleResource.LanguageUpdated)));
        }
    }
}
