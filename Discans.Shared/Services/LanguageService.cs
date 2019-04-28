using System.Linq;

namespace Discans.Shared.Services
{
    public class LanguageService
    {
        public string GetUserLanguage(ulong userId) =>
            UserLocalizerService.Languages.FirstOrDefault(x => x.Key == userId).Value
                ?? "en-US";

        public string GetServerLanguage(ulong serverId) =>
            ServerLocalizerService.Languages.FirstOrDefault(x => x.Key == serverId).Value
                ?? "en-US";
    }
}
