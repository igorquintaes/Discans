namespace Discans.Shared.Models
{
    public class PrivateAlert
    {
        public int Id { get; protected set; }
        public ulong UserId { get; protected set; }
        public Manga Manga { get; protected set; }
    }
}
