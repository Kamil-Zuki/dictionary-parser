using HtmlAgilityPack;
using dictionary_parser.Models;

namespace dictionary_parser.BLL.Services
{
    public class HtmlDataExtractionOld
    {
        public static async Task<WordStructure> CallWordStructure(string word)
        {
            string campDictionaryPrefix = @"https://dictionary.cambridge.org/";

            string urlPrefix = @"https://dictionary.cambridge.org/dictionary/english/";
            //string fullUrl = urlPrefix + word;
            string fullUrl = urlPrefix + word;

            WordStructure baseWord = new();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument? htmlDoc = new();
            await Task.Run(() =>
            {
                htmlDoc = web.Load(fullUrl);// Загрузка всей страницы Html
            });

            var wordBlocks = htmlDoc.DocumentNode.SelectNodes("//div[@class='pr entry-body__el']");//Поиск всех блоков слов
            var phrasalVerbWordBlocks = htmlDoc.DocumentNode.SelectNodes("//div[@class='pv-block']");
            //var idiomsBlocks = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='pos dpos']");
            //if(idiomsBlocks.InnerText == "idiom")
            //{
            //    Console.WriteLine("Good");
            //}


            if (phrasalVerbWordBlocks != null && wordBlocks == null) return await SearchPhrasalVerb(phrasalVerbWordBlocks, campDictionaryPrefix);

            if (wordBlocks == null)
            {
                var foundWordSuggestions = await SearchWordSuggestings(word);
                baseWord.TermData = null;
                baseWord.TermSuggestions = foundWordSuggestions;
                return baseWord;
            }

            List<TermData> wordTails = new List<TermData>();
            await Task.Run(() =>
            {
                for (int i = 0; i < wordBlocks.Count; i++)//Проходка по блокам слов
                {
                    var block = wordBlocks[i].OuterHtml;

                    HtmlDocument wordBlock = new HtmlDocument();
                    wordBlock.LoadHtml(block);

                    string word = "";
                    HtmlNode? wordNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='hw dhw']");//Нода слова
                    if (wordNode != null)
                    {
                        word = wordNode.InnerText;//Слово
                    }

                    string partOfspeech = "";
                    HtmlNode? partOfspeechNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='pos dpos']");//Нода части речи
                    if (partOfspeechNode != null)
                    {
                        partOfspeech = partOfspeechNode.InnerText;//Часть речи
                    }

                    HtmlNode? formalityNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='usage dusage']");//Нода формальности

                    string? formality = "";
                    if (formalityNode != null)
                    {
                        formality = formalityNode.InnerText;//Формальность
                    }
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


                    TermData wordTail = new TermData()
                    {
                        Word = word,
                        PartOfspeech = partOfspeech,
                        Formality = formality,
                        VebForms = vebForms,
                        UK = uk,
                        US = us,
                        UseCases = defs
                    };

                    wordTails.Add(wordTail);
                }
                baseWord.TermData = wordTails;
                baseWord.TermSuggestions = null;
            });


            return baseWord;

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

        private static async Task<WordStructure> SearchPhrasalVerb(HtmlNodeCollection? wordBlocks, string campDictionaryPrefix)
        {
            WordStructure baseWord = new(); 
            List<TermData> wordTails = new List<TermData>();
            await Task.Run(() =>
            {
                for (int i = 0; i < wordBlocks.Count; i++)//Проходка по блокам слов
                {
                    var block = wordBlocks[i].OuterHtml;

                    HtmlDocument wordBlock = new HtmlDocument();
                    wordBlock.LoadHtml(block);

                    string word = "";
                    HtmlNode? wordNode = wordBlock.DocumentNode.SelectSingleNode($"//div[@class='di-title']");//Нода слова
                    if (wordNode != null)
                    {
                        word = wordNode.FirstChild.FirstChild.InnerHtml;//Слово
                    }

                    string partOfspeech = "";
                    HtmlNode? partOfspeechNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='pos dpos']");//Нода части речи
                    if (partOfspeechNode != null)
                    {
                        partOfspeech = partOfspeechNode.InnerText;//Часть речи
                    }

                    HtmlNode? formalityNode = wordBlock.DocumentNode.SelectSingleNode($"//span[@class='usage dusage']");//Нода формальности

                    string? formality = "";
                    if (formalityNode != null)
                    {
                        formality = formalityNode.InnerText;//Формальность
                    }
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


                    TermData wordTail = new TermData()
                    {
                        Word = word,
                        PartOfspeech = partOfspeech,
                        Formality = formality,
                        VebForms = vebForms,
                        UK = uk,
                        US = us,
                        UseCases = defs
                    };

                    wordTails.Add(wordTail);
                }
                baseWord.TermData = wordTails;
                baseWord.TermSuggestions = null;
            });


            return baseWord;
        }

        //private static async Task<BaseWord> IdiomSearcher()
        //{

        //}
    }
}
