namespace Discans.Shared.Models
{
    public abstract class Localizer
    {
        public Localizer(string language) => 
            Language = language;
        
        public string Language { get; protected set; }

        public void UpdateLanguage(string language) =>
           Language = language;
    }
}
