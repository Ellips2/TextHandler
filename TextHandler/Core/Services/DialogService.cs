using System;
using Microsoft.Win32;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TextHandler.Core
{
    public interface IDialogService
    {
        void ShowMessage(string message);   // показ сообщения
        List<TextFile> TextFiles{ get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog(List<TextFile> files);  // сохранение файла
    }

    public class DialogService : IDialogService
    {
        public List<TextFile> TextFiles { get; set; }

        public bool OpenFileDialog()
        {
            TextFiles = new List<TextFile>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files|*.*|Word|*.docx|Word 97-2003|*.doc|PDF files|*.pdf|Text files|*.txt";
            Nullable<bool> dialogOk = openFileDialog.ShowDialog();

            if (dialogOk == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    TextFile tf = new TextFile();
                    tf.Path = openFileDialog.FileNames[i];
                    TextFiles.Add(tf);
                }
                return true;
            }
            return false;
        }        

        public bool SaveFileDialog(List<TextFile> files)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files|*.txt|Word|*.docx|Word 97-2003|*.doc|PDF files|*.pdf|All files|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string newPath = saveFileDialog.FileName;
                    newPath = newPath.Insert(newPath.IndexOf(Path.GetExtension(newPath)), "(" + i + ") ");
                    files[i].Path = newPath;
                }
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
