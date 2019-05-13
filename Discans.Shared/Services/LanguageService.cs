using System.Collections.Generic;
using System.Linq;

namespace Discans.Shared.Services
{
    public class LanguageService
    {
        public static readonly IReadOnlyCollection<string> AllowedLanguages = new[] 
        {
            "en-US",
            "pt-BR"
        };
        
        public string GetUserLanguage(ulong userId) =>
            UserLocalizerService.Languages.FirstOrDefault(x => x.Key == userId).Value
                ?? AllowedLanguages.ElementAt(0);

        public string GetServerLanguage(ulong serverId) =>
            ServerLocalizerService.Languages.FirstOrDefault(x => x.Key == serverId).Value
                ?? AllowedLanguages.ElementAt(0);
    }
}
