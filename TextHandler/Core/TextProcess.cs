using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace TextHandler.Core
{
    class TextProcess
    {
        private const int MAX_PROGRESS_BAR_VALUE = 100;
        public List<String> GetTokens(string text, IProgress<int> progress)
        {
            List<string> lines = new List<string>();
            List<string> tokens = new List<string>();

            using var modelIn = new java.io.FileInputStream(GetModel("opennlp-en-ud-ewt-tokens-1.0-1.9.3.bin"));
            var model = new opennlp.tools.tokenize.TokenizerModel(modelIn);
            var tokenizer = new opennlp.tools.tokenize.TokenizerME(model);
             
            text = text.Replace("+", string.Empty);//Удаляем Nested quantifier '+'
            lines.AddRange(text.Split('\n'));
            lines.RemoveAll((x)=> x == "\r"); //Удаляем мусор в тексте                                   

            for (int i = 0; i < lines.Count; i++)   //Обрабатываем текст построчно, чтобы отображать прогресс в ProgressBar
            {
                tokens.AddRange(tokenizer.tokenize(lines[i]).ToList());

                var percentComplete = (MAX_PROGRESS_BAR_VALUE * i + 1) / lines.Count;
                progress.Report(percentComplete);                
            }
            return tokens;
        }
        private string GetModel(string fileName)
        {
            var asmFolder = Environment.CurrentDirectory;
            int ind = asmFolder.LastIndexOf("bin");
            var filePath = asmFolder.Substring(0, ind) + fileName;
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            return filePath;
        }

        public string DeleteWord(string text, int minLength, List<string> tokens, IProgress<int> progress)
        {
            string finalText = text;

            for (int i = 0; i < tokens.Count; i++) 
            {
                //Отбираем все токены, являющиеся словами с недостаточной длинной и не являющиеся знаками препинания
                if (tokens[i].Length <= minLength && !char.IsPunctuation(tokens[i][0]) && tokens[i] != "C++") 
                {
                    Regex reg = new Regex($@"\b{tokens[i]}\b"); //Формируем текст без неподходящих слов
                    finalText = reg.Replace(finalText, "", 1);
                }

                var percentComplete = (MAX_PROGRESS_BAR_VALUE * i+1) / tokens.Count;
                progress.Report(percentComplete);
            }
            
            return finalText;
        }

        public string DeleteChars(string text, char[] chars, IProgress<int> progress)
        {
            for (int i = 0; i < chars.Length; i++) //Удаляем указанные пользователем символы
            {
                text = text.Replace(chars[i].ToString(), string.Empty);

                var percentComplete = (MAX_PROGRESS_BAR_VALUE * i+1) / chars.Length;
                progress.Report(percentComplete);
            }
            text = Regex.Replace(text, "[ ]+", " ");    //Удаляем лишние пробелы                         
            return text;
        }
    }
}
