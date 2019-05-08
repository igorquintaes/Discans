using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discans.Shared.DiscordServices;
using Discans.Shared;
using Discans.Shared.DiscordServices.CrawlerSites;

namespace Discans
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private IConfiguration _config;

        public async Task MainAsync()
        {
            _config = BuildConfig();
            var client = new DiscordSocketClient();
            var services = ConfigureServices(client);

            using (var context = services.GetRequiredService<AppDbContext>())
            {
                context.Database.Migrate();
                UserLocalizerService.Languages = await context.UserLocalizer.ToDictionaryAsync(x => x.UserId, x => x.Language);
                ServerLocalizerService.Languages = await context.ServerLocalizer.ToDictionaryAsync(x => x.ServerId, x => x.Language);
            }

            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandling>().InitializeAsync(services);
            await client.LoginAsync(TokenType.Bot, Helpers.EnvironmentVar(_config, "TOKEN"));
            await client.StartAsync();
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices(DiscordSocketClient client) => 
            new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandling>()
                .AddLogging()
                .AddSingleton<LogService>()
                .AddSingleton<LanguageService>()
                .AddSingleton(_config)
                .AddScoped<CrawlerService>()
                .AddScoped<MangaUpdatesCrawlerService>()
                .AddScoped<TuMangaCrawlerService>()
                .AddScoped<MangaService>()
                .AddScoped<PrivateAlertService>()
                .AddScoped<UserAlertService>()
                .AddScoped<ServerAlertService>()
                .AddScoped<ChannelService>()
                .AddScoped<UserLocalizerService>()
                .AddScoped<ServerLocalizerService>()
                .AddDbContext<AppDbContext>(options => options.UseMySql(Helpers.EnvironmentVar(_config, "CONN")))
                .BuildServiceProvider();

        private IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
    }
}