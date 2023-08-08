using SautinSoft.Document;
using System;
using System.Collections.Generic;

namespace TextHandler.Core
{
    public interface IFileService
    {
        List<string> Open(List<string> filename);
        void Save(List<string> filePath, List<string> postProccText);
    }
    public class FileService : IFileService
    {
        public List<string> Open(List<string> filePath)
        {
            List<string> allText = new List<string>();
            for (int i = 0; i < filePath.Count; i++)
            {
                try
                {
                    DocumentCore dc = DocumentCore.Load(filePath[i]);
                    allText.Add(dc.Content.ToString());
                }
                catch (Exception ex)
                {
                    allText.Add(filePath[i] + "\n\n" + ex);
                }
            }            
            return allText;
        }

        public void Save(List<string> filePath, List<string> postProccText)
        {
            for (int i = 0; i < postProccText.Count; i++)
            {
                DocumentCore dc = new DocumentCore();
                dc.Content.End.Insert(postProccText[i]);
                dc.Save(filePath[i]);
                //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath[i]) { UseShellExecute = true });
            }
        }
    }
}
