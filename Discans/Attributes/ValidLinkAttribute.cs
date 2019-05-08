using Discans.Shared.DiscordServices;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    public class ValidLinkAttribute : PreconditionAttribute
    {
        private int linkIndex;

        public ValidLinkAttribute() =>
            linkIndex = 2;

        public ValidLinkAttribute(int linkIndex) => 
            this.linkIndex = linkIndex;

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) => 
            await ((CrawlerService)services.GetService(typeof(CrawlerService)))
                .LoadPageAsync(context.Message.Content.Split(' ')[linkIndex])
                    ? PreconditionResult.FromSuccess()
                    : PreconditionResult.FromError("Link inválido! T_T");
    }
}
