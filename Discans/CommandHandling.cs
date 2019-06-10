using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discans.Modules;
using Discans.Shared.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Discans
{
    public class CommandHandling
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        private readonly LanguageService languageService;
        private IServiceProvider provider;

        public CommandHandling(
            IServiceProvider provider, 
            DiscordSocketClient discord, 
            CommandService commands, 
            LanguageService languageService)
        {
            this.discord = discord;
            this.commands = commands;
            this.languageService = languageService;
            this.provider = provider;

            this.discord.SetGameAsync($"{Consts.BotCommand}{InfoModule.InfoCommand}", "https://github.com/igorquintaes/Discans");
            this.discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            this.provider = provider;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!message.HasStringPrefix(Consts.BotCommand, ref argPos)) return;

            using (var scope = provider.CreateScope())
            {
                var context = new SocketCommandContext(discord, message);
                var language = context.Channel is IGuildChannel 
                    ? languageService.GetServerLanguage(context.Guild.Id)
                    : languageService.GetUserLanguage(context.User.Id);

                Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
                var result = await commands.ExecuteAsync(context, argPos, scope.ServiceProvider);
                    
                if (result.Error.HasValue &&
                    result.Error.Value != CommandError.UnknownCommand)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}