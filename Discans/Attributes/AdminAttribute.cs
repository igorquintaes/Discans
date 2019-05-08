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
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) =>
            context.Channel is IDMChannel ||
            (context.Message.Author as IGuildUser).GuildPermissions.Administrator
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError(
                    ((LocaledResourceManager<AdminAttributeResource>)services.GetService(typeof(LocaledResourceManager<AdminAttributeResource>)))
                    .GetString(nameof(AdminAttributeResource.YouNeedToBeAnAdmin))));
    }
}
