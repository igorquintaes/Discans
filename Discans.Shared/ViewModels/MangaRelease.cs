using Discans.Shared.Models;

namespace Discans.Shared.ViewModels
{
    public class MangaRelease : Manga
    {
        public MangaRelease(string mangaSiteId, string name, string lastRelease, MangaSite mangaSite)
            : base(mangaSiteId, name, lastRelease, mangaSite)
        { }

        public MangaRelease(string mangaSiteId, string name, string lastRelease, MangaSite mangaSite, string scanName, string scanLink)
            : base(mangaSiteId, name, lastRelease, mangaSite)
        {
            ScanName = scanName;
            ScanLink = scanLink;
        }

        public string ScanName { get; protected set; }
        public string ScanLink { get; set; }
        public string MangaLink { get; set; }
    }
}
