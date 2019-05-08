using Discans.Shared.Database;
using System.Collections.Generic;
using System.Linq;

namespace Discans.Shared.Services
{
    public class LanguageService
    {
        private readonly AppDbContext context;

        public static readonly IReadOnlyCollection<string> AllowedLanguages = new[] {
            "en-US",
            "pt-BR"
        };

        public LanguageService(AppDbContext dbContext) => this.context = dbContext;

        public string GetUserLanguage(ulong userId) =>
            UserLocalizerService.Languages.FirstOrDefault(x => x.Key == userId).Value
                ?? "en-US";

        public string GetServerLanguage(ulong serverId) =>
            ServerLocalizerService.Languages.FirstOrDefault(x => x.Key == serverId).Value
                ?? "en-US";
    }
}
