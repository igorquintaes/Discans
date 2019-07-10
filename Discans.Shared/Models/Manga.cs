using System.Collections.Generic;

namespace Discans.Shared.Models
{
    public class Manga
    {
        protected Manga()
        {
            UserAlerts = new List<UserAlert>();
            ServerAlerts = new List<ServerAlert>();
            PrivateAlerts = new List<PrivateAlert>();
        }

        public Manga(string mangaSiteId, string name, string lastRelease, MangaSite mangaSite) : this()
        {
            MangaSiteId = mangaSiteId;
            Name = name;
            LastRelease = lastRelease;
            MangaSite = mangaSite;
        }

        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string LastRelease { get; protected set; }
        public MangaSite MangaSite { get; protected set; }
        public string MangaSiteId { get; protected set; }
        public virtual ICollection<UserAlert> UserAlerts { get; protected set; }
        public virtual ICollection<ServerAlert> ServerAlerts { get; protected set; }
        public virtual ICollection<PrivateAlert> PrivateAlerts { get; protected set; }
        
        public void UpdateLastRelase(string lastRelease) => 
            LastRelease = lastRelease;       
    }

    public enum MangaSite
    {
        MangaUpdates = 1,
        TuManga = 2,
        UnionMangas = 3
    }
}
