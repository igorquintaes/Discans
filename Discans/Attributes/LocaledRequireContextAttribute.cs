using Discord;
using Discord.Commands;
using System;
using System.Resources;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LocaledRequireContextAttribute : RequireContextAttribute
    {
        public Type ResourceType { get; set; }

        private static ResourceManager resourceManager;

        public LocaledRequireContextAttribute(ContextType contexts, Type type) 
            : base(contexts)
        {
            resourceManager = resourceManager ?? new ResourceManager(type);
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            bool isValid = false;

            if ((Contexts & ContextType.Guild) != 0)
                isValid = context.Channel is IGuildChannel;
            if ((Contexts & ContextType.DM) != 0)
                isValid = isValid || context.Channel is IDMChannel;
            if ((Contexts & ContextType.Group) != 0)
                isValid = isValid || context.Channel is IGroupChannel;

            if (isValid)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError(resourceManager.GetString(ErrorMessage)));
        }
    }
}
