using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Discans
{
    public class CommandHandling
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;

        public CommandHandling(IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.SetGameAsync($"{Consts.BotCommand}info", "https://github.com/igorquintaes/Discans");
            _discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!message.HasStringPrefix(Consts.BotCommand, ref argPos)) return;

            using (var scope = _provider.CreateScope())
            {
                var context = new SocketCommandContext(_discord, message);
                var result = await _commands.ExecuteAsync(context, argPos, scope.ServiceProvider);
                    
                if (result.Error.HasValue &&
                    result.Error.Value != CommandError.UnknownCommand)
                    await context.Channel.SendMessageAsync($"Pera, tô confusa...! {result.ErrorReason}");
            }
        }
    }
}