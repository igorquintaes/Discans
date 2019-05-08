namespace Discans.Shared.Models
{
    public class Localizer
    {
        public Localizer(string language) => 
            Language = language;
        
        public string Language { get; protected set; }
    }
}
