using Discans.Shared;
using Discans.Shared.Database;
using Discans.Shared.DiscordServices;
using Discans.Shared.DiscordServices.CrawlerSites;
using Discans.Shared.Services;
using Discans.WebJob.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discans.WebJob
{
    class Program
    {
        public static bool CloseProgram = false;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private IConfiguration _config;

        public async Task MainAsync()
        {
            _config = BuildConfig();
            var client = new DiscordSocketClient();
            var services = ConfigureServices(client);

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            var context = services.GetRequiredService<AppDbContext>();
            await client.LoginAsync(TokenType.Bot, Helpers.EnvironmentVar(_config, "TOKEN"));
            await client.StartAsync();
            await WaitUntilTaskEnds();
        }

        private IServiceProvider ConfigureServices(DiscordSocketClient client) =>
            new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddLogging()
                .AddSingleton<LogService>()
                .AddSingleton(_config)
                .AddSingleton<MangaService>()
                .AddSingleton<UserAlertService>()
                .AddSingleton<PrivateAlertService>()
                .AddSingleton<ServerAlertService>()
                .AddSingleton<CrawlerService>()
                .AddSingleton<MangaUpdatesCrawlerService>()
                .AddSingleton<TuMangaCrawlerService>()
                .AddSingleton<ChannelService>()
                .AddDbContext<AppDbContext>(options => options.UseMySql(Helpers.EnvironmentVar(_config, "CONN")), ServiceLifetime.Singleton)
                .BuildServiceProvider();

        private IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

        private async Task WaitUntilTaskEnds() => 
            await Task.Run(() => {
                do
                {
                    Thread.Sleep(5000);
                } while (!CloseProgram);
            });
    }
}
