using SautinSoft.Document;
using System;
using System.Collections.Generic;
using System.IO;

namespace TextHandler.Core
{
    public interface IFileService
    {
        List<TextFile> Open(List<TextFile> files);
        void Save(List<TextFile> files);
    }
    public class FileService : IFileService
    {
        public List<TextFile> Open(List<TextFile> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                try
                {
                    DocumentCore dc = DocumentCore.Load(files[i].Path);
                    files[i].OldText = dc.Content.ToString();
                    files[i].Name = Path.GetFileName(files[i].Path);
                }
                catch (Exception ex)
                {
                    files[i].OldText = files[i].Name + "\n\n" + ex;
                }
            }            
            return files;
        }

        public void Save(List<TextFile> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                DocumentCore dc = new DocumentCore();
                if (files[i].NewText != "")
                    dc.Content.End.Insert(files[i].NewText);
                else
                    dc.Content.End.Insert(files[i].OldText);
                dc.Save(files[i].Path);
                //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath[i]) { UseShellExecute = true });
            }
        }
    }
}
