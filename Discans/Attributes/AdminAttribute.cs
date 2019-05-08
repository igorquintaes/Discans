using Discans.Resources;
using Discans.Resources.Attributes;
using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AdminAttribute : PreconditionAttribute
    {
        private static LocaledResourceManager resourceManager;

        public AdminAttribute() => 
            resourceManager = resourceManager 
                ?? new LocaledResourceManager(nameof(AdminAttributeResource), 
                                              typeof(AdminAttributeResource).Assembly);

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) =>
            (context.Message.Author as IGuildUser).GuildPermissions.Administrator
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError(resourceManager.GetString(
                    nameof(AdminAttributeResource.YouNeedToBeAnAdmin))));
    }
}
