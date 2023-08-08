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
        List<string> FilePath { get; set; }   // путь к выбранному файлу

        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog(List<string> files);  // сохранение файла
    }

    public class DialogService : IDialogService
    {
        public List<string> FilePath { get; set; }

        public bool OpenFileDialog()
        {
            FilePath = new List<string>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files|*.*|Word|*.docx|Word 97-2003|*.doc|PDF files|*.pdf|Text files|*.txt";
            Nullable<bool> dialogOk = openFileDialog.ShowDialog();

            if (dialogOk == true)
            {
                FilePath.AddRange(openFileDialog.FileNames.ToList());
                return true;
            }
            return false;
        }        

        public bool SaveFileDialog(List<string> files)
        {
            FilePath = new List<string>();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All files|*.*|Word|*.docx|Word 97-2003|*.doc|PDF files|*.pdf|Text files|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string newPath = saveFileDialog.FileName;
                    newPath = newPath.Insert(newPath.IndexOf(Path.GetExtension(newPath)), "(" + i + ") ");
                    FilePath.Add(newPath);
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
