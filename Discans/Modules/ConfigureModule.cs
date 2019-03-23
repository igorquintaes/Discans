using Discans.Attributes;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discord.Commands;
using System.Threading.Tasks;

namespace Discans.Modules
{
    public class ConfigureModule : ModuleBase<SocketCommandContext>
    {
        private readonly ChannelService channelService;
        private readonly AppDbContext dbContext;

        public ConfigureModule(ChannelService channelService, AppDbContext dbContext)
        {
            this.channelService = channelService;
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
    }
}
