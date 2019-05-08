using System.Threading.Tasks;
using Discans.Resources;
using Discans.Resources.Modules;
using Discord.Commands;

namespace Discans.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private static LocaledResourceManager resourceManager;
        public const string InfoCommand = "info";

        public InfoModule() => 
            resourceManager = resourceManager
                ?? new LocaledResourceManager(typeof(InfoModuleResource).FullName,
                                              typeof(InfoModuleResource).Assembly);

        [Command(InfoCommand)]
        public Task Info() => 
            ReplyAsync(string.Format(resourceManager.GetString(
                nameof(InfoModuleResource.InfoMessage)),
                Context.Client.CurrentUser.Username));
    }
}