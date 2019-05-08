using Discans.Shared.DiscordServices;
using Discans.Resources;
using Discans.Resources.Attributes;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    public class ValidLinkAttribute : PreconditionAttribute
    {
        private readonly int linkIndex;
        private static LocaledResourceManager resourceManager;

        public ValidLinkAttribute() 
            : this(2)
        { }

        public ValidLinkAttribute(int linkIndex)
        {
            this.linkIndex = linkIndex;
            resourceManager = resourceManager
                              ?? new LocaledResourceManager(typeof(ValidLinkAttributeResource).FullName,
                                                            typeof(ValidLinkAttributeResource).Assembly);
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) => 
            await ((CrawlerService)services.GetService(typeof(CrawlerService)))
                .LoadPageAsync(context.Message.Content.Split(' ')[linkIndex])
                    ? PreconditionResult.FromSuccess()
                    : PreconditionResult.FromError(resourceManager.GetString(nameof(ValidLinkAttributeResource.InvalidLink)));
    }
}
