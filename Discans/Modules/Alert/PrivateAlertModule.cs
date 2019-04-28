using Discans.Extensions;
using Discans.Shared.Database;
using Discans.Shared.DiscordServices;
using Discans.Shared.Services;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discans.Modules.Alert
{
    public class PrivateAlertModule : ModuleBase<SocketCommandContext>
    {
        private readonly MangaService mangaService;
        private readonly PrivateAlertService privateAlertService;
        private readonly CrawlerService crawlerService;
        private readonly AppDbContext dbContext;

        public PrivateAlertModule(
            MangaService mangaService,
            PrivateAlertService privateAlertService,
            CrawlerService crawlerService,
            AppDbContext dbContext)
        {
            this.mangaService = mangaService;
            this.privateAlertService = privateAlertService;
            this.crawlerService = crawlerService;
            this.dbContext = dbContext;
        }

        [Command("private-alert")]
        [RequireContext(ContextType.DM, ErrorMessage = "Só posso usar esse comando em privado >_>'")]
        public async Task UserAlert(string link)
        {
            var (isLinkValid, mangaCrawlerService) = await crawlerService.LoadPageAsync(link);
            if (!isLinkValid)
            {
                await ReplyAsync("Link inválido! T_T");
                return;
            }

            var mangaId = mangaCrawlerService.GetMangaId();
            var mangaName = mangaCrawlerService.GetMangaName();
            var lastRelease = mangaCrawlerService.GetLastChapter();
            var manga = await mangaService.GetOrCreateIfNew(mangaId, lastRelease, mangaName, mangaCrawlerService.MangaSite);

            await privateAlertService.Create(
                userId: Context.User.Id,
                manga: manga);

            await dbContext.SaveChangesAsync();

            var resposta = new StringBuilder();
            resposta.AppendLine("Muito bem! Você receberá o alerta quando sair um capítulo novo!");
            resposta.AppendLine($"Você pode ver a lista de todos os seus alertas o comando `{Consts.BotCommand}private-alert-list`");
            resposta.AppendLine("");

            resposta.AppendLine($@"Esse cadastro envolve as seguintes informações:
```ini
Nome do mangá: [{mangaName}]
Último capítulo lançado: [{lastRelease}]
Fonte de consulta: [{mangaCrawlerService.MangaSite.ToString()}]
```");

            await ReplyAsync(resposta.ToString());
        }

        [Command("private-alert-remove")]
        [RequireContext(ContextType.DM, ErrorMessage = "Só posso usar esse comando em privado >_>'")]
        public async Task UserAlertRemove(string link)
        {
            var (isLinkValid, mangaCrawlerService) = await crawlerService.LoadPageAsync(link);
            if (!isLinkValid)
            {
                await ReplyAsync("Link inválido! T_T");
                return;
            }

            var mangaSiteId = mangaCrawlerService.GetMangaId();
            await privateAlertService.Remove(Context.User.Id, mangaSiteId, mangaCrawlerService.MangaSite);
            await dbContext.SaveChangesAsync();
            await ReplyAsync("Os usuários foram desvinculados do projeto mencionado.");
        }
               
        [Command("private-alert-list")]
        [RequireContext(ContextType.DM, ErrorMessage = "Só posso usar esse comando em privado >_>'")]
        public async Task AlertList()
        {
            var alerts = await privateAlertService.GetAlerts(Context.User.Id);

            if (!alerts.Any())
            {
                await ReplyAsync("Não tenho nenhum alerta para você :/");
                return;
            }

            var alertMessages = alerts.Select(x => 
$@"```ini
Mangá: [{x.Manga.Name}]
Último lançamento: [{x.Manga.LastRelease}]
Fonte de consulta: [{x.Manga.MangaSite.ToString()}]
```");

            await ReplyAsync($@"Alertas configurados diretamente para você:");
            var alertChuncks = alertMessages.ToList().ChunkList(10);

            foreach (var alertChunck in alertChuncks)
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }
    }
}
