using Discans.Resources;
using Discans.Resources.Attributes;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LocaledRequireContextAttribute : RequireContextAttribute
    {
        public Type ResourceType { get; set; }

        public LocaledRequireContextAttribute(ContextType contexts)
            : base(contexts)
        { }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var resourceManager = (LocaledResourceManager<LocaledRequireContextAttributeResource>)services
                .GetService(typeof(LocaledResourceManager<LocaledRequireContextAttributeResource>));

            bool isValid = false;
            var allowedContexts = resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.ErrorMessage));

            if ((Contexts & ContextType.Guild) != 0)
            {
                isValid = context.Channel is IGuildChannel;
                allowedContexts += $"{Environment.NewLine}{resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.Server))}";
            }

            if ((Contexts & ContextType.DM) != 0)
            {
                isValid = isValid || context.Channel is IDMChannel;
                allowedContexts += $"{Environment.NewLine}{resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.DM))}";
            }

            if ((Contexts & ContextType.Group) != 0)
            {
                isValid = isValid || context.Channel is IGroupChannel;
                allowedContexts += $"{Environment.NewLine}{resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.Group))}";
            }

            return isValid
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError(allowedContexts));
        }
    }
}
