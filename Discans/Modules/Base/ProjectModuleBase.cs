using Discans.Resources;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discans.Modules.Base
{
    public class ProjectModuleBase<Resource> : ModuleBase<SocketCommandContext>
    {
        public readonly LocaledResourceManager<Resource> ResourceManager;

        public virtual string AppUserName => Context.Client.CurrentUser.Username;
        public virtual ulong GuildId => Context.Guild.Id;
        public virtual ulong ChannelId => Context.Channel.Id;
        public virtual ulong UserId => Context.User.Id;
        public virtual ISocketMessageChannel Channel => Context.Channel;

        public ProjectModuleBase(LocaledResourceManager<Resource> resourceManager) => 
            ResourceManager = resourceManager;

        public virtual async Task ReplyAsync(string message) =>
            await base.ReplyAsync(message, default, default, default);

        public virtual new async Task ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null) =>
            await base.ReplyAsync(message, isTTS, embed, options);
    }
}
