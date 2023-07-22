namespace dictionary_parser.Models
{
    public class TermData
    {

        public TermData()
        {

            UseCases = new HashSet<UseCase>();
        }

        public string? Word { get; set; }
        public string? PartOfspeech { get; set; }
        public string? Formality { get; set; }

        public virtual VebForms? VebForms { get; set; }

        public virtual UK? UK { get; set; }

        public virtual US? US { get; set; }

        public virtual ICollection<UseCase> UseCases { get; set; }

    }
}
