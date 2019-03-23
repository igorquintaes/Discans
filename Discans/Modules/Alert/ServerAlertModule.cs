using Discans.Attributes;
using Discans.DiscordServices;
using Discans.Extensions;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discans.Modules.Alert
{
    public class ServerAlertModule : ModuleBase<SocketCommandContext>
    {
        private readonly MangaService mangaService;
        private readonly ServerAlertService serverAlertService;
        private readonly UserAlertService userAlertService;
        private readonly MangaUpdatesCrawlerService crawlerService;
        private readonly AppDbContext dbContext;

        public ServerAlertModule(
            MangaService mangaService,
            ServerAlertService serverAlertService,
            UserAlertService userAlertService,
            MangaUpdatesCrawlerService crawlerService,
            AppDbContext dbContext)
        {
            this.mangaService = mangaService;
            this.serverAlertService = serverAlertService;
            this.userAlertService = userAlertService;
            this.crawlerService = crawlerService;
            this.dbContext = dbContext;
        }

        [Command("server-alert"), Admin]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task ServerAlert(string link)
        {
            var isLinkValid = await crawlerService.LoadPageAsync(link);
            if (!isLinkValid)
            {
                await ReplyAsync("Link inválido! T_T");
                return;
            }

            var mangaId = crawlerService.GetMangaId();
            var mangaName = crawlerService.GetMangaName();
            var lastRelease = crawlerService.LastRelease();
            var manga = await mangaService.GetOrCreateIfNew(mangaId, lastRelease, mangaName);

            await serverAlertService.Create(
                serverId: Context.Guild.Id,
                manga: manga);

            await dbContext.SaveChangesAsync();
            var resposta = new StringBuilder();
            resposta.AppendLine("Beleza, tá anotado!");
            resposta.AppendLine("Assim que sair um capítulo novo eu aviso!");
            resposta.AppendLine("");

            resposta.AppendLine($@"Esse cadastro envolve as seguintes informações:
```ini
Nome do mangá: [{mangaName}]
Quem será alertado: [Todos do servidor]
Último capítulo lançado: [{lastRelease}]
```");

            await ReplyAsync(resposta.ToString());
        }

        [Command("server-alert-remove"), Admin]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task ServerAlertRemove(string link)
        {
            var isLinkValid = await crawlerService.LoadPageAsync(link);
            if (!isLinkValid)
            {
                await ReplyAsync("Link inválido! T_T");
                return;
            }

            var mangaId = crawlerService.GetMangaId();
            await serverAlertService.Remove(Context.Guild.Id, mangaId);
            await dbContext.SaveChangesAsync();
            await ReplyAsync("O servidor foi desvinculado do projeto.");
        }

        [Command("server-alert-remove-all TENHO CERTEZA"), Admin]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task ServerAlertRemoveAll()
        {
            await serverAlertService.Remove(Context.Guild.Id);
            await userAlertService.Remove(Context.Guild.Id);
            await dbContext.SaveChangesAsync();
            await ReplyAsync("Todos os projetos e usuários foram desvinculados deste servidor. Sentirei a sua falta T_T");
        }

        [Command("server-alert-remove-all"), Admin]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task ServerAlertRemoveAllUnconfirm()
        {
            await ReplyAsync("Mano, isso vai remover TODOS os alertas do seu servidor. Tem certeza? " +
                "Se você estiver certo disso, escreva `server-alert-remove-all TENHO CERTEZA`");
        }

        [Command("server-alert-list")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task ServerAlertList()
        {
            var alerts = await serverAlertService.Get(Context.Guild.Id);

            if (!alerts.Any())
            {
                await ReplyAsync("Não tem nenhum alerta para todos deste servidor :/");
                return;
            }

            var alertMessages = alerts.Select(x => {
                var text =
$@"```ini
Mangá: [{x.Manga.Name}]
Último lançamento: [{x.Manga.LastRelease}]
```";
                return text;
            });

            await ReplyAsync($@"Alertas configurados diretamente para o servidor {Context.Guild.Name}:");
            var alertChuncks = alertMessages.ToList().ChunkList(10);

            foreach (var alertChunck in alertChuncks)
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }

        [Command("server-alert-list-all")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task ServerAlertListAll()
        {
            var serverAlerts = await serverAlertService.Get(Context.Guild.Id);
            var userAlerts = await userAlertService.GetServerAlert(Context.Guild.Id);

            if (!serverAlerts.Any() && !userAlerts.Any())
            {
                await ReplyAsync("Não tem nenhum alerta para todos deste servidor :/");
                return;
            }

            var serverAlertMessages = serverAlerts.Select(x => {
                var text =
$@"```ini
Mangá: [{x.Manga.Name}]
Quem será marcado: [todos de '{Context.Guild.Name}']
Último lançamento: [{x.Manga.LastRelease}]
```";
                return text;
            });

            var userAlertMessages = userAlerts.GroupBy(x => x.Manga.Id).Select(x => {
                var text =
$@"```ini
Mangá: [{x.First().Manga.Name}]
Quem será marcado: [{string.Join(", ",  
                            x.Select(alert => Context.Guild.Users.FirstOrDefault(y => y.Id == alert.UserId)?.Username 
                            ?? "usuário que saiu do servidor"))}]
Último lançamento: [{x.First().Manga.LastRelease}]
```";
                return text;
            });
            
            await ReplyAsync($@"Abaixo a lista de topdos os alertas configurados:");
            var alertChuncks = serverAlertMessages.Concat(userAlertMessages).ToList().ChunkList(7);

            foreach (var alertChunck in alertChuncks)
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }
    }
}
