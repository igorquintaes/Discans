using System;

namespace Discans.Shared.Models
{
    public class UserAlert
    {
        protected UserAlert()
        { }

        public UserAlert(ulong userId, ulong serverId, Manga manga)
        {
            manga = manga ?? throw new ArgumentNullException(nameof(manga));
            UserId = userId;
            ServerId = serverId;
        }
        public int Id { get; protected set; }
        public ulong ServerId { get; protected set; }
        public Manga Manga { get; protected set; }
        public ulong UserId { get; protected set; }
    }
}
