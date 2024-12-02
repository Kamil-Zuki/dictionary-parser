using dictionary_parser.BLL.Interfaces;
using dictionary_parser.Models;
using Microsoft.AspNetCore.Mvc;

namespace dictionary_parser.Controllers
{
    [ApiController]
    [Route("api/v1/cambridge-dictionary")]
    public class CambridgeDictionaryController : ControllerBase
    {
        private readonly IHtmlDataExtraction _htmlDataExtraction;
        public CambridgeDictionaryController(IHtmlDataExtraction htmlDataExtraction)
        {
            _htmlDataExtraction = htmlDataExtraction;
        }


        [HttpGet]
        public async Task<ActionResult<WordStructure>> ExtractWordData(string term, bool? isWord = true, bool? isPhrasalVerb = true, bool? isIdiom = true)
        {
            try
            {
                return Ok(await _htmlDataExtraction.GetTerms(term, isWord, isPhrasalVerb, isIdiom));
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                return Problem(ex.Message + " Line: " + trace.GetFrame(0).GetFileLineNumber());
            }
        }
    }

}