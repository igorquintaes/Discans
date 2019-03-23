using System;

namespace Discans.Shared.Models
{
    public class ServerAlert
    {
        protected ServerAlert()
        { }

        public ServerAlert(ulong serverId, Manga manga)
        {
            manga = manga ?? throw new ArgumentNullException(nameof(manga));
            ServerId = serverId;
        }

        public int Id { get; protected set; }
        public ulong ServerId { get; protected set; }
        public Manga Manga { get; protected set; }
    }
}
