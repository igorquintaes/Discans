using Discans.DiscordServices;
using Discans.Extensions;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discans.Modules.Alert
{
    public class UserAlertModule : ModuleBase<SocketCommandContext>
    {
        private readonly MangaService mangaService;
        private readonly UserAlertService userAlertService;
        private readonly MangaUpdatesCrawlerService crawlerService;
        private readonly AppDbContext dbContext;

        public UserAlertModule(
            MangaService mangaService,
            UserAlertService userAlertService,
            MangaUpdatesCrawlerService crawlerService,
            AppDbContext context)
        {
            this.mangaService = mangaService;
            this.userAlertService = userAlertService;
            this.crawlerService = crawlerService;
            this.dbContext = context;
        }

        [Command("user-alert")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task UserAlert(string link, params IGuildUser[] users)
        {
            if (!(Context.Message.Author as IGuildUser).Guild.Roles.Any(x => x.Permissions.Administrator)
                && users.Any(x => x.Id != Context.Message.Author.Id))
            {
                await ReplyAsync("Só o administrador pode alertar outras pessoas. " +
                    "Se quiser, ainda é possível criar um alerta apenas para você :)");
                return;
            }

            if (!users.Any())
            {
                await ReplyAsync("Não achei o nome de ninguém! T_T");
                return;
            }

            var isLinkValid = await crawlerService.LoadPageAsync(link);
            if (!isLinkValid)
            {
                await ReplyAsync("Link inválido! T_T");
                return;
            }

            users = users.GroupBy(x => x.Id).Select(x => x.First()).ToArray();
            var mangaId = crawlerService.GetMangaId();
            var mangaName = crawlerService.GetMangaName();
            var lastRelease = crawlerService.LastRelease();
            var manga = await mangaService.GetOrCreateIfNew(mangaId, lastRelease, mangaName);

            await userAlertService.Create(
                userIds: users.Select(x => x.Id),
                serverId: Context.Guild.Id,
                manga: manga);

            await dbContext.SaveChangesAsync();

            var resposta = new StringBuilder();
            resposta.AppendLine("Muito bem! As pessoas aí receberão o alerta quando sair um capítulo novo!");
            resposta.AppendLine($"Você pode ver a lista de todos os alerta deste servidor com o comando `{Consts.BotCommand}server-alert-list-all`");
            resposta.AppendLine("");

            var usersReplyMessage = string.Join(", ", users.Select(x => x.Nickname ?? x.Username));
            resposta.AppendLine($@"Esse cadastro envolve as seguintes informações:
```ini
Nome do mangá: [{mangaName}]
Quem será alertado: [{usersReplyMessage}]
Último capítulo lançado: [{lastRelease}]
```");

            await ReplyAsync(resposta.ToString());
        }
        
        [Command("user-alert-remove"), Priority(2)]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task UserAlertRemove(params IGuildUser[] users)
        {
            if (!(Context.Message.Author as IGuildUser).Guild.Roles.Any(x => x.Permissions.Administrator)
                && users.Any(x => x.Id != Context.Message.Author.Id))
            {
                await ReplyAsync("Só o administrador pode alertar outras pessoas. " +
                    "Se quiser, ainda é possível criar um alerta apenas para você :)");
                return;
            }

            if (!users.Any())
            {
                await ReplyAsync("Não achei o nome de ninguém! T_T");
                return;
            }

            await userAlertService.Remove(Context.Guild.Id, users.Select(x => x.Id));
            await dbContext.SaveChangesAsync();
            await ReplyAsync("Os usuários foram desvinculados de todos os projetos deste servidor.");
        }

        [Command("user-alert-remove"), Priority(1)]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task UserAlertRemove(string link, params IGuildUser[] users)
        {
            if (!(Context.Message.Author as IGuildUser).Guild.Roles.Any(x => x.Permissions.Administrator)
                && users.Any(x => x.Id != Context.Message.Author.Id))
            {
                await ReplyAsync("Só o administrador pode alertar outras pessoas. " +
                    "Se quiser, ainda é possível criar um alerta apenas para você :)");
                return;
            }

            if (!users.Any())
            {
                await ReplyAsync("Não achei o nome de ninguém! T_T");
                return;
            }

            var isLinkValid = await crawlerService.LoadPageAsync(link);
            if (!isLinkValid)
            {
                await ReplyAsync("Link inválido! T_T");
                return;
            }

            var mangaId = crawlerService.GetMangaId();
            await userAlertService.Remove(Context.Guild.Id, users.Select(x => x.Id), mangaId);
            await dbContext.SaveChangesAsync();
            await ReplyAsync("Os usuários foram desvinculados do projeto mencionado.");
        }
               
        [Command("user-alert-list")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Só posso usar esse comando em um servidor >_>'")]
        public async Task TrackList(IGuildUser user)
        {
            var alerts = await userAlertService.GerUserServerAlert(Context.Guild.Id, user.Id);

            if (!alerts.Any())
            {
                await ReplyAsync("Não tem nenhum alerta no servidor para esse mano aí :/");
                return;
            }

            var alertMessages = alerts.Select(x => {
                var userText = Context.Guild.GetUser(x.UserId).Username;
                var text =
$@"```ini
Mangá: [{x.Manga.Name}]
Último lançamento: [{x.Manga.LastRelease}]
```";
                return text;
            });

            await ReplyAsync($@"Alertas configurados diretamente para o usuário {Context.Guild.GetUser(user.Id).Username}:");
            var alertChuncks = alertMessages.ToList().ChunkList(10);

            foreach (var alertChunck in alertChuncks)
                await ReplyAsync(string.Join(Environment.NewLine, alertChunck));
        }
    }
}
