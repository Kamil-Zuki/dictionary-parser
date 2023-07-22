using dictionary_parser.BLL.Interfaces;
using dictionary_parser.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace dictionary_parser.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/cambridge-dictionary")]
    public class CambridgeDictionaryController : ControllerBase
    {
        private readonly IHtmlDataExtraction _htmlDataExtraction;
        public CambridgeDictionaryController(IHtmlDataExtraction htmlDataExtraction)
        {
            _htmlDataExtraction = htmlDataExtraction;
        }


        [HttpGet("v1/term-data")]
        public async Task<ActionResult<WordStructure>> ExtractWordData([FromQuery(Name = "term")] string term, bool? isWord = true, bool? isPhrasalVerb = true, bool? isIdiom = true)
        {
            try
            {
                string decodedTerm = WebUtility.UrlDecode(term);

                return Ok(await _htmlDataExtraction.GetTerms(decodedTerm, isWord, isPhrasalVerb, isIdiom));
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);

                return Problem(ex.Message + " Line: " + trace.GetFrame(0).GetFileLineNumber());
            }
        }
    }

}