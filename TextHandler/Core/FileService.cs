using SautinSoft.Document;
using System.Threading.Tasks;
using System;

namespace TextHandler.Core
{
    public interface IFileService
    {
        Task<string> Open(string filename);
        void Save(string filePath, string postProccText);
    }
    public class FileService : IFileService
    {
        public async Task<string> Open(string filePath)
        {
            string allText = "";            
            await Task.Run(() => 
            {
                try
                {
                    DocumentCore dc = DocumentCore.Load(filePath);
                    allText = dc.Content.ToString();
                }
                catch (Exception ex)
                {
                    allText = filePath + "\n\n" + ex;
                }                
            });
            return allText;
        }

        public async void Save(string filePath, string postProccText)
        {
            await Task.Run(() =>
            {
                DocumentCore dc = new DocumentCore();
                dc.Content.End.Insert(postProccText);
                dc.Save(filePath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            });
        }
    }
}
