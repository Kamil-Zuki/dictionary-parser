using dictionary_parser.BLL.Interfaces;
using dictionary_parser.Models;
using dictionary_parser.Models.Enums;
using HtmlAgilityPack;

namespace dictionary_parser.BLL.Services
{
    public class HtmlDataExtraction : IHtmlDataExtraction
    {
        private async Task<List<TermData>> FindTerm(string word, enumSpeechElement speechElement)
        {
            List<TermData> termData = new();
            await Task.Run(() =>
            {
                string campDictionaryPrefix = @"https://dictionary.cambridge.org/";

                string urlPrefix = @"https://dictionary.cambridge.org/dictionary/english/";
                string fullUrl = urlPrefix + word;

                WordStructure baseWord = new();

                HtmlWeb web = new HtmlWeb();
                HtmlDocument? htmlDoc = new(); 

                htmlDoc = web.Load(fullUrl);// Загрузка всей страницы Html

                HtmlNodeCollection? wordBlocks = speechElement switch
                {
                    enumSpeechElement.Word => htmlDoc.DocumentNode.SelectNodes("//div[@class='pr entry-body__el']"),//word
                    enumSpeechElement.PhrasalVerb => htmlDoc.DocumentNode.SelectNodes("//div[@class='pv-block']"),//phrasal verb
                    enumSpeechElement.Idiom => htmlDoc.DocumentNode.SelectNodes("//div[@class='idiom-block']"),//idiom
                    _ => null,
                };
                if (wordBlocks == null)
                    return null;

                for (int i = 0; i < wordBlocks.Count; i++)//Проходка по блокам слов
                {
                    var block = wordBlocks[i].OuterHtml;

                    HtmlDocument wordBlock = new HtmlDocument();
                    wordBlock.LoadHtml(block);

                    HtmlNode? wordNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='hw dhw']");//Нода слова
                    string term = wordNode == null ? "" : wordNode.InnerText;
                    //if (wordNode != null)
                    //{
                    //    term = wordNode.InnerText;//Слово
                    //}
                    if (speechElement == enumSpeechElement.PhrasalVerb || speechElement == enumSpeechElement.Idiom)
                    {
                        HtmlNode? phrasalVerbNode = wordBlock.DocumentNode.SelectSingleNode($"//div[@class='di-title']");
                        if (phrasalVerbNode != null)
                        {
                            term = phrasalVerbNode.InnerText;
                        }
                    }

                    string partOfspeech = "";
                    HtmlNode? partOfspeechNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='pos dpos']");//Нода части речи
                    if (partOfspeechNode != null)
                    {
                        partOfspeech = partOfspeechNode.InnerText;//Часть речи
                    }

                    HtmlNode? formalityNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='usage dusage']");//Нода формальности

                    string? formality = formalityNode == null ? "" : formalityNode.InnerText;
                    //if (formalityNode != null)
                    //{
                    //    formality = formalityNode.InnerText;//Формальность
                    //}
                    int j = 1, k = 2, l = 3;

                    #region Формы глагола
                    VebForms vebForms = new();

                    HtmlNode? presentParticipleNode = wordBlock.DocumentNode.SelectSingleNode($"(//b[@class='inf dinf'])[{j}]");//Причастие настоящего времени
                    string presentParticiple = "";

                    if (presentParticipleNode != null)
                    {
                        presentParticiple = presentParticipleNode.InnerText;
                        vebForms.PresentParticiple = presentParticiple;
                    }

                    HtmlNode? pastTenseNode = wordBlock.DocumentNode.SelectSingleNode($"(//b[@class='inf dinf'])[{k}]");
                    string pastTense = "";

                    if (pastTenseNode != null)
                    {
                        pastTense = pastTenseNode.InnerText;//Прошедшее время
                        vebForms.PastTense = pastTense;
                    }

                    HtmlNode? pastParticipleNode = wordBlock.DocumentNode.SelectSingleNode($"(//b[@class='inf dinf'])[{l}]");
                    string pastParticiple = "";

                    if (pastParticipleNode != null)
                    {
                        pastParticiple = pastParticipleNode.InnerText;//Причастие прошедшего времени
                        vebForms.PastParticiple = pastParticiple;
                    }

                    #endregion

                    #region UK
                    UK uk = new UK();
                    HtmlNode? UKTranscriptionNode = wordBlock.DocumentNode.SelectSingleNode($"(//span[@class='ipa dipa lpr-2 lpl-1'])[{j}]");


                    if (UKTranscriptionNode != null)
                    {
                        //Check on US or UK

                        HtmlNode? UKAudioNode = wordBlock.DocumentNode.SelectSingleNode($"(//source[@type='audio/mpeg'])[{j}]");
                        string? UKaudio = "";
                        string? check = "";
                        if (UKAudioNode != null)
                        {
                            UKaudio = UKAudioNode.OuterHtml;
                            UKaudio = UKaudio.Split("src=").Last().Replace(">", "").Replace("\\", "").Replace("\"", "").Remove(0, 1);

                            UKaudio = campDictionaryPrefix + UKaudio;
                            check = UKaudio.Split("https://dictionary.cambridge.org/media/english/uk_pron/").First();
                        }

                        if (check == "")
                        {
                            uk.Transcription = UKTranscriptionNode.InnerText;
                            uk.Audio = UKaudio;
                        }
                        else
                        {
                            uk.Transcription = null;
                            uk.Audio = null;
                        }

                    }

                    #endregion UK

                    #region US


                    US us = new();
                    HtmlNode? USTranscriptionNode = wordBlock.DocumentNode.SelectSingleNode($"(//span[@class='ipa dipa lpr-2 lpl-1'])[{j}]");

                    if (USTranscriptionNode != null)
                    {
                        HtmlNode? USAudioNode = wordBlock.DocumentNode.SelectSingleNode($"(//source[@type='audio/mpeg'])[{j}]");
                        string? USaudio = "";
                        string? check = "";
                        if (USAudioNode != null)
                        {
                            USaudio = USAudioNode.OuterHtml;
                            USaudio = USaudio.Split("src=").Last().Replace(">", "").Replace("\\", "").Replace("\"", "").Remove(0, 1);
                            USaudio = campDictionaryPrefix + USaudio;
                            check = USaudio.Split("https://dictionary.cambridge.org/media/english/us_pron/").First();
                        }


                        if (check == "")
                        {
                            us.Transcription = USTranscriptionNode.InnerText;
                            us.Audio = USaudio;
                        }
                        else
                        {
                            HtmlNode? USTranscriptionNode2 = wordBlock.DocumentNode.SelectSingleNode($"(//span[@class='ipa dipa lpr-2 lpl-1'])[{k}]");


                            if (USTranscriptionNode2 != null)
                            {
                                HtmlNode? USAudioNode2 = wordBlock.DocumentNode.SelectSingleNode($"(//source[@type='audio/mpeg'])[{k}]");
                                string? USaudio2 = "";
                                string check2 = "";

                                if (USAudioNode2 != null)
                                {
                                    USaudio2 = USAudioNode2.OuterHtml;

                                    USaudio2 = USaudio2.Split("src=").Last().Replace(">", "").Replace("\\", "").Replace("\"", "").Remove(0, 1);
                                    USaudio2 = campDictionaryPrefix + USaudio2;

                                    check2 = USaudio.Split("https://dictionary.cambridge.org/media/english/us_pron/").First();
                                }

                                us.Transcription = USTranscriptionNode2.InnerText;

                                if (check2 != "")
                                {
                                    us.Audio = USaudio2;
                                }
                                else
                                {
                                    us.Audio = null;
                                }
                            }

                        }
                    }
                    #endregion US

                    HtmlNodeCollection definitions = wordBlock.DocumentNode.SelectNodes("//div[@class='pr dsense ']");

                    if (definitions == null)
                    {
                        definitions = wordBlock.DocumentNode.SelectNodes("//div[@class='pr dsense dsense-noh']");
                    }


                    List<UseCase> defs = new();
                    if (definitions != null)
                    {
                        foreach (var definition in definitions)
                        {

                            var defBlock = definition.OuterHtml;

                            HtmlDocument debHtmlDoc = new HtmlDocument();
                            debHtmlDoc.LoadHtml(defBlock);


                            HtmlNode? useCaseNode = debHtmlDoc.DocumentNode.SelectSingleNode($"//h3[@class='dsense_h']");

                            UseCase useCase = new();
                            if (useCaseNode != null)
                            {
                                useCase.Content = useCaseNode.InnerText
                                    .Split('(').Last()
                                    .Split(')').First();
                            }

                            HtmlNodeCollection? ddef_blocks = debHtmlDoc.DocumentNode.SelectNodes($"//div[@class='def-block ddef_block ']");

                            List<Definition> defs_blocks = new();
                            foreach (var ddef_block in ddef_blocks)
                            {
                                var determBlock = ddef_block.OuterHtml;

                                HtmlDocument determHtmlDoc = new HtmlDocument();
                                determHtmlDoc.LoadHtml(determBlock);

                                HtmlNode? lvlNode = determHtmlDoc.DocumentNode.SelectSingleNode($"//span[starts-with(@class, 'epp-xref dxref')]/text()");

                                Definition def = new();

                                if (lvlNode != null)
                                {
                                    def.Lvl = lvlNode.InnerText;
                                }

                                HtmlNode? determinationNode = determHtmlDoc.DocumentNode.SelectSingleNode($"//div[@class='def ddef_d db']");
                                if (determinationNode != null)
                                {
                                    def.Content = determinationNode.InnerText;
                                }

                                List<string> examples = new();
                                HtmlNodeCollection? exampleNode = determHtmlDoc.DocumentNode.SelectNodes($"//span[@class='eg deg']");
                                if (exampleNode != null)
                                {
                                    foreach (var example in exampleNode)
                                    {
                                        examples.Add(example.InnerText);
                                    }
                                }

                                def.Examples = examples;

                                defs_blocks.Add(def);
                            }

                            useCase.Definition = defs_blocks;

                            defs.Add(useCase);
                        }
                    }


                    TermData terms = new TermData()
                    {
                        Word = term,
                        PartOfspeech = partOfspeech,
                        Formality = formality,
                        VebForms = vebForms,
                        UK = uk,
                        US = us,
                        UseCases = defs
                    };

                    termData.Add(terms);
                }

                return termData;

            });

            return termData;

        }

        private static async Task<List<string>> SearchWordSuggestings(string wrongWord)
        {
            string wrongWordUrl = @"https://dictionary.cambridge.org/spellcheck/english/?q=" + wrongWord;
            List<string> newWords = new List<string>();
            await Task.Run(() =>
            {
                HtmlWeb web = new HtmlWeb();

                var words = web.Load(wrongWordUrl).DocumentNode.SelectNodes($"//ul[@class='hul-u']")
                    .Select(x => x.InnerText).ToList()[0]
                    .Split("\n").ToList();


                foreach (var word in words)
                {
                    if (word.Trim() != string.Empty)
                    {
                        newWords.Add(word.Trim());
                    }
                }

            });

            return newWords;
        }

        public async Task<WordStructure> GetTerms(string term, bool? isWord, bool? isPhrasalVerb, bool? isIdiom)
        {
            term = term.Replace(" ", "-");
            WordStructure wordStructure = new WordStructure();
            List<TermData> words = new List<TermData>();
            List<TermData> phrasalVerbs = new List<TermData>();
            List<TermData> idioms = new List<TermData>();
            if (isWord == true)
                words = await FindTerm(term, enumSpeechElement.Word);
            if (isPhrasalVerb == true)
                phrasalVerbs = await FindTerm(term, enumSpeechElement.PhrasalVerb);
            if (isIdiom == true)
                idioms = await FindTerm(term, enumSpeechElement.Idiom);


            List<TermData> termData = new();
            if (words != null) termData.AddRange(words);
            if (phrasalVerbs != null) termData.AddRange(phrasalVerbs);
            if (idioms != null) termData.AddRange(idioms);


            if (termData.Count == 0)
            {
                var foundWordSuggestions = await SearchWordSuggestings(term);
                wordStructure.TermData = null;
                wordStructure.TermSuggestions = foundWordSuggestions;
                return wordStructure;
            }
            else
            {
                wordStructure.TermData = termData;
                wordStructure.TermSuggestions = null;
                return wordStructure;
            }

        }
    }
}
