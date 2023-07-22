using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dictionary_parser.Models
{
    public class WordStructure
    {
        public List<TermData>? TermData { get; set; }
        public List<string>? TermSuggestions { get; set; }
    }
}
