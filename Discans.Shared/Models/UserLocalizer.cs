namespace Discans.Shared.Models
{
    public class UserLocalizer : Localizer
    {
        public UserLocalizer(ulong userId, string language)
            : base(language) => 
                UserId = userId;

        public ulong UserId { get; set; }
    }
}
