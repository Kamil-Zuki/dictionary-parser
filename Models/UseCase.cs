namespace dictionary_parser.Models
{
    public class UseCase
    {

        public string? Content { get; set; }

        public virtual ICollection<Definition> Definition { get; set; }


    }
}
