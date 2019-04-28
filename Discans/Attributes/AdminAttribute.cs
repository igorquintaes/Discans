using Discans.Resources.Attributes;
using Discord;
using Discord.Commands;
using System;
using System.Resources;
using System.Threading.Tasks;

namespace Discans.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AdminAttribute : PreconditionAttribute
    {
        private readonly ResourceManager resourceManager;

        public AdminAttribute() => 
            resourceManager = new ResourceManager("AdminAttributeResource", typeof(AdminAttributeResource).Assembly);

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) =>
            (context.Message.Author as IGuildUser).GuildPermissions.Administrator
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError(resourceManager.GetString("YouNeedToBeAnAdmin")));
    }
}
