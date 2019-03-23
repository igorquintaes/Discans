using System.Threading.Tasks;
using Discord.Commands;

namespace Discans.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        public Task Info()
            => ReplyAsync(
                $@"
Olá! Eu sou a {Context.Client.CurrentUser.Username}! 
Fui criada para te ajudar com alertas de novos capítulos em inglês.

Você pode conferir todos os comandos disponíveis, como usar e o meu próprio código-fonte no GitHub:
https://github.com/igorquintaes/Discans");
    }
}