namespace Discans.Shared.Models
{
    public class PrivateAlert
    {
        protected PrivateAlert()
        { }

        public PrivateAlert(ulong userId, Manga manga)
        {
            Manga = manga;
            UserId = userId;
        }

        public int Id { get; protected set; }
        public ulong UserId { get; protected set; }
        public Manga Manga { get; protected set; }
    }
}
