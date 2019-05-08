using Discans.Attributes;
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

        public ConfigureModule(ChannelService channelService, UserLocalizerService userLocalizerService, ServerLocalizerService serverLocalizerService, AppDbContext dbContext)
        {
            this.channelService = channelService;
            this.userLocalizerService = userLocalizerService;
            this.serverLocalizerService = serverLocalizerService;
            this.dbContext = dbContext;
        }

        [Command("channel"), Admin]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task Channel()
        {
            await channelService.SaveOrUpdate(Context.Guild.Id, Context.Channel.Id);
            await dbContext.SaveChangesAsync();
            await ReplyAsync("Ok! Mandarei as atualizações pra cá! <3");
        }

        [Command("language"), Admin]
        [RequireContext(ContextType.Guild | ContextType.DM, ErrorMessage = "Não posso usar esse comando em grupos de usuários >_>'")]
        public async Task Language(string language)
        {
            if (!LanguageService.AllowedLanguages.Contains(language))
            {
                await ReplyAsync("Não temos suporte a esse idioma :(");
                return;
            }

            if (Context.Channel is IGuildChannel)
                await serverLocalizerService.CreateOrUpdate(Context.Guild.Id, language);
            else
                await userLocalizerService.CreateOrUpdate(Context.User.Id, language);

            await dbContext.SaveChangesAsync();
            await ReplyAsync("Seu idioma foi atualizado! <3");
        }
    }
}
