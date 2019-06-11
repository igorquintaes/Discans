using Discans.Resources;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Discans.Modules.Base
{
    public class ProjectModuleBase<Resource> : ModuleBase<SocketCommandContext>
    {
        public readonly LocaledResourceManager<Resource> ResourceManager;

        public virtual string CurrentUserName => Context.Client.CurrentUser.Username;

        public ProjectModuleBase(LocaledResourceManager<Resource> resourceManager) => 
            ResourceManager = resourceManager;

        public virtual new async Task ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) =>
            await base.ReplyAsync(message, isTTS, embed, options);
    }
}
