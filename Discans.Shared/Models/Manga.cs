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

        public Manga(int id, string name, string lastRelease) : this()
        {
            Id = id;
            Name = name;
            LastRelease = lastRelease;
        }

        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string LastRelease { get; protected set; }
        public virtual ICollection<UserAlert> UserAlerts { get; protected set; }
        public virtual ICollection<ServerAlert> ServerAlerts { get; protected set; }
        public virtual ICollection<PrivateAlert> PrivateAlerts { get; protected set; }
        
        public void UpdateLastRelase(string lastRelease) => 
            LastRelease = lastRelease;
    }
}
