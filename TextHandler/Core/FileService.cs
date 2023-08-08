using SautinSoft.Document;
using System;
using System.IO;

namespace TextHandler.Core
{
    public interface IFileService
    {
        string Open(string filename);
        void Save(string filePath, string postProccText);
    }
    public class FileService : IFileService
    {
        public string Open(string filePath)
        {
            string allText = "";
            try
            {
                DocumentCore dc = DocumentCore.Load(filePath);
                allText = dc.Content.ToString();
            }
            catch (Exception ex)
            {
                allText = filePath + "\n\n" + ex;
            }
            return allText;
        }

        public void Save(string filePath, string postProccText)
        {
            DocumentCore dc = new DocumentCore();
            dc.Content.End.Insert(postProccText);
            dc.Save(filePath);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            
        }
    }
}
