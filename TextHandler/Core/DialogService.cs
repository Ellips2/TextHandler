using System;
using Microsoft.Win32;
using System.Windows;
using System.Collections.Generic;

namespace TextHandler.Core
{
    public interface IDialogService
    {
        void ShowMessage(string message);   // показ сообщения
        string FilePath { get; set; }   // путь к выбранному файлу

        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();  // сохранение файла
    }

    public class DialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files|*.*|Word|*.docx|Word 97-2003|*.doc|PDF files|*.pdf|Text files|*.txt";
            Nullable<bool> dialogOk = openFileDialog.ShowDialog();

            if (dialogOk == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }        

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All files|*.*|Word|*.docx|Word 97-2003|*.doc|PDF files|*.pdf|Text files|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
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
