using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discans.DiscordServices;
using Discans.Shared.Services;
using Discord.Commands;
using Discord.WebSocket;

namespace Discans.WebJob.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        private readonly MangaService mangaService;
        private readonly UserAlertService userAlertService;
        private readonly ChannelService channelService;
        private readonly ServerAlertService serverAlertService;
        private readonly MangaUpdatesCrawlerService crawlerService;
        private readonly IServiceProvider provider;

        public CommandHandlingService(
            IServiceProvider provider, 
            DiscordSocketClient discord,
            CommandService commands, 
            MangaService mangaService,
            UserAlertService userAlertService,
            ChannelService channelService,
            ServerAlertService serverAlertService,
            MangaUpdatesCrawlerService crawlerService)
        {
            this.discord = discord;
            this.commands = commands;
            this.mangaService = mangaService;
            this.userAlertService = userAlertService;
            this.channelService = channelService;
            this.serverAlertService = serverAlertService;
            this.crawlerService = crawlerService;
            this.provider = provider;

            this.discord.Connected += Update;
        }

        public async Task InitializeAsync(IServiceProvider provider) =>
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);

        private Task Update()
        {
            LastReleases().GetAwaiter().GetResult();
            return Task.FromResult(Program.CloseProgram = true);
        }

        private async Task LastReleases()
        {
            var mangas = await mangaService.GetAll();
            var lastReleases = crawlerService.GetLastReleases();
            var lastReleasesIds = lastReleases.Select(y => y.Id);

            foreach (var manga in mangas.Where(x => lastReleasesIds.Contains(x.Id)))
            {
                var lastRelease = lastReleases.First(x => x.Id == manga.Id).LastRelease;
                if (manga.LastRelease == lastRelease)
                    continue;

                await mangaService.UpdateLastRelease(manga.Id, lastRelease);

                foreach (var serverAlert in manga.ServerAlerts.GroupBy(x => x.ServerId))
                {
                    await SendServerMessage("@everyone", manga.Name, lastRelease, serverAlert.Key);
                }

                foreach (var userAlert in manga.UserAlerts.GroupBy(x => x.ServerId))
                {
                    var users = string.Join(", ", userAlert.Select(x => $"<@{x.UserId}>"));
                    await SendServerMessage(users, manga.Name, lastRelease, userAlert.Key);
                }

                foreach (var privateAlert in manga.PrivateAlerts)
                {
                    await SendPrivateMessage(privateAlert.UserId, manga.Name, manga.LastRelease);
                }
            }
        }

        private async Task SendPrivateMessage(ulong userId, string mangaName, string lastRelease)
        {
            try
            {
                var message = 
$@"Temos um novo capítulo em inglês de `{mangaName}!`
Capítulo lançado: {lastRelease}";

                var channel = await discord.GetUser(userId).GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(message);
            }
            catch (Exception e) when (e.Message.Contains("error 50007")  // Cannot send messages to this user
                                   || e.Message.Contains("error 10013")) // Unknown user
            {
                // We will not send messages under those conditions. 
                // It occurs when a user blocks Discans Bot, or when a user account is deleted.
            }
            catch(Exception e)
            {
                // todo: log
            }
        }

        private async Task SendServerMessage(string user, string mangaName, string lastRelease, ulong serverId)
        {
            var message = $@"{user}
Temos um novo capítulo em inglês de `{mangaName}!`
Capítulo lançado: {lastRelease}";

            if (discord.GetGuild(serverId) == null)
            {
                await userAlertService.Remove(serverId);
                await serverAlertService.Remove(serverId);
            }

            var dbChannel = await channelService.GetByServerId(serverId);                

            try
            {
                if (dbChannel != null)
                {
                    await (discord
                        .GetGuild(serverId)
                        .GetChannel(dbChannel.ChannelId) as SocketTextChannel)
                        .SendMessageAsync(message);
                }
                else
                {
                    message = $@"{user}
Temos um novo capítulo em inglês de `{mangaName}!`
Capítulo lançado: {lastRelease}
Como não houve uma configuração de canal para eu mandar os alertas, estou mandando por aqui '-'
O Administrador do grupo pode configurar o canal através do comando `channel`";

                    await discord
                        .GetGuild(serverId)
                        .DefaultChannel
                        .SendMessageAsync(message);
                }

                return;
            }
            catch
            { }

            foreach(var channel in discord.GetGuild(serverId).TextChannels)
            {
                try
                {
                    await channel.SendMessageAsync(message);
                    return;
                }
                catch
                { }
            }

            // todo: save notification to send again - discord server error maybe
        }
    }
}