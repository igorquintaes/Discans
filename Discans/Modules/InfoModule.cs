using System.Threading.Tasks;
using Discans.Modules.Base;
using Discans.Resources;
using Discans.Resources.Modules;
using Discord.Commands;

namespace Discans.Modules
{
    public class InfoModule : ProjectModuleBase<InfoModuleResource>
    {
        public const string InfoCommand = "info";

        public InfoModule(LocaledResourceManager<InfoModuleResource> resourceManager) 
            : base(resourceManager)
        { }

        [Command(InfoCommand)]
        public async Task Info() => 
            await ReplyAsync(string.Format(ResourceManager.GetString(
                nameof(InfoModuleResource.InfoMessage)), AppUserName));
    }
}