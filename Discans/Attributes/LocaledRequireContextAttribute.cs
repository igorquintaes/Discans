using Discans.Resources.Attributes;
using Discord;
using Discord.Commands;
using System;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LocaledRequireContextAttribute : RequireContextAttribute
    {
        public Type ResourceType { get; set; }

        private static ResourceManager resourceManager;

        public LocaledRequireContextAttribute(ContextType contexts)
            : base(contexts) => 
                resourceManager = resourceManager 
                                  ?? new ResourceManager(typeof(LocaledRequireContextAttributeResource));

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            bool isValid = false;
            var allowedContexts = resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.ErrorMessage), Thread.CurrentThread.CurrentCulture);

            if ((Contexts & ContextType.Guild) != 0)
            {
                isValid = context.Channel is IGuildChannel;
                allowedContexts += $"{Environment.NewLine}{resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.Server), Thread.CurrentThread.CurrentCulture)}";
            }

            if ((Contexts & ContextType.DM) != 0)
            {
                isValid = isValid || context.Channel is IDMChannel;
                allowedContexts += $"{Environment.NewLine}{resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.DM), Thread.CurrentThread.CurrentCulture)}";
            }

            if ((Contexts & ContextType.Group) != 0)
            {
                isValid = isValid || context.Channel is IGroupChannel;
                allowedContexts += $"{Environment.NewLine}{resourceManager.GetString(nameof(LocaledRequireContextAttributeResource.Group), Thread.CurrentThread.CurrentCulture)}";
            }

            return isValid
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError(allowedContexts));
        }
    }
}
