using System.Threading.Tasks;
using Discans.Resources;
using Discans.Resources.Modules;
using Discord.Commands;

namespace Discans.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly LocaledResourceManager<InfoModuleResource> resourceManager;
        public const string InfoCommand = "info";

        public InfoModule(LocaledResourceManager<InfoModuleResource> resourceManager) =>
            this.resourceManager = resourceManager;

        [Command(InfoCommand)]
        public Task Info() => 
            ReplyAsync(string.Format(resourceManager.GetString(
                nameof(InfoModuleResource.InfoMessage)),
                Context.Client.CurrentUser.Username));
    }
}