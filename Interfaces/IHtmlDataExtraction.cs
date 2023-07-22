using dictionary_parser.Models;

namespace dictionary_parser.BLL.Interfaces
{
    public interface IHtmlDataExtraction
    {
        Task<WordStructure> GetTerms(string term, bool? isWord, bool? isPhrasalVerb, bool? isIdiom);
    }
}
