using System;
using TextHandler.Core;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TextHandler.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private readonly CloseApplicationCommand closeApplicationCommand = new CloseApplicationCommand();
        public RelayCommand CloseApplicationCommand { get; }

        #region [Работа с файлами]
        public ObservableCollection<TextFile> TextFiles { get; set; }
        private readonly TextProcess textProcess = new TextProcess();
        private TextFile selectedTextFile = new TextFile();
        public TextFile SelectedTextFile { get => selectedTextFile; set { selectedTextFile = value; OnPropertyChanged(nameof(SelectedTextFile)); } }
        #endregion

        #region [Диалоговые окна]
        private readonly IFileService fileService = new FileService();
        private readonly IDialogService dialogService = new DialogService();

        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand { get { return openFileCommand ?? (openFileCommand = new RelayCommand(obj => { TryOpenFileDialog();})); } }

        private RelayCommand saveFileCommand;
        public RelayCommand SaveFileCommand { get { return saveFileCommand ?? (saveFileCommand = new RelayCommand(obj => { TrySaveFiles(); })); } }
        #endregion

        #region [ProgressBar]
        private readonly BackgroundWorker worker;
        private int progressBarValue = 0;
        public int ProgressBarValue { get => progressBarValue; set { progressBarValue = value; OnPropertyChanged(nameof(ProgressBarValue)); } }

        private string progressBarTitle = "";
        public string ProgressBarTitle { get => progressBarTitle; set { progressBarTitle = value; OnPropertyChanged(nameof(ProgressBarTitle)); } }

        private RelayCommand startWorkCommand;
        public RelayCommand StartWorkCommand { get { return this.startWorkCommand; } }
        #endregion

        public MainViewModel()
        {
            TextFiles = new ObservableCollection<TextFile>();            
            CloseApplicationCommand = new RelayCommand(closeApplicationCommand.Execute, closeApplicationCommand.CanExecute);
            
            startWorkCommand = new RelayCommand(o => worker.RunWorkerAsync(), o => !worker.IsBusy);
            worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
        }

        #region [Методы для ProgressBar]
        private async void DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> textInProcess = new List<string>();
            string subName = "";
            var progress = new Progress<int>(value => { ProgressBarValue = value; ProgressBarTitle = $"{subName}   {value}%"; });
            for (int i = 0; i < TextFiles.Count; i++)
            {
                string tempText = TextFiles[i].OldText;
                List<string> tokens = new List<string>();

                ProgressBarValue = 0;
                subName = $"Tokenization";
                await Task.Run(() => tokens.AddRange(textProcess.GetTokens(tempText, progress)));

                ProgressBarValue = 0;
                subName = $"Delete words less {TextFiles[i].MinLengthWord} chars";
                await Task.Run(() => tempText = textProcess.DeleteWord(tempText, TextFiles[i].MinLengthWord, tokens, progress));

                ProgressBarValue = 0;
                subName = $"Delete chars: {TextFiles[i].CharsForDel}";
                await Task.Run(() => tempText = textProcess.DeleteChars(tempText, TextFiles[i].CharsForDel.ToCharArray(), progress));

                TextFiles[i].NewText = tempText;
            }
            ProgressBarTitle = "Complete!";
            ProgressBarValue = 100;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e) { this.ProgressBarValue = e.ProgressPercentage; }
        #endregion

        #region [Методы для диалоговых окон]
        private async void TryOpenFileDialog()
        {
            if (dialogService.OpenFileDialog() == true)
            {
                try
                {
                    List<TextFile> _textFiles = new List<TextFile>();
                    await Task.Run(() => _textFiles = fileService.Open(dialogService.TextFiles));
                    TextFiles.Clear();
                    foreach (var t in _textFiles)
                    {
                        TextFiles.Add(t);
                    }
                    selectedTextFile = TextFiles[0];
                }
                catch (Exception ex)
                {
                    dialogService.ShowMessage("Failed to open file! " + ex.Message);
                }
            }
        }

        private async void TrySaveFiles()
        {            
            if (dialogService.SaveFileDialog(TextFiles.ToList()) == true)
            {
                try
                {
                    for (int i = 0; i < TextFiles.Count; i++)
                    {
                        await Task.Run(() => fileService.Save(dialogService.TextFiles));
                    }
                    if (TextFiles.Count > 1)
                        dialogService.ShowMessage("Файлы сохранены");
                    else
                        dialogService.ShowMessage("Файл сохранен");
                }
                catch (Exception ex)
                {
                    dialogService.ShowMessage("Failed to save file! " + ex.Message);
                }
            }            
        }
        #endregion
    }
}
