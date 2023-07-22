namespace dictionary_parser.Models
{
    public class Definition
    {
        public string? Content { get; set; }

        public string? Lvl { get; set; }
        public string? Translate { get; set; }
        public virtual ICollection<string>? Examples { get; set; }
    }
}
