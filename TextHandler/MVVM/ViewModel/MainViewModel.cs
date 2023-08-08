using System;
using TextHandler.Core;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TextHandler.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand DiscoveryViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public DiscoveryViewModel DiscoveryVM { get; set; }

        private object _currentView;

        public RelayCommand CloseApplicationCommand { get; }
        private CloseApplicationCommand closeApplicationCommand = new CloseApplicationCommand();

        
        private List<string> oldTexts = new List<string>();
        
        private List<string> postTexts = new List<string>();

        private string beforeProcText;
        public string BeforeProcText { get { return beforeProcText; } set { beforeProcText = value; OnPropertyChanged(nameof(BeforeProcText)); } }

        private string afterProcText;
        public string AfterProcText { get {return afterProcText; } set { afterProcText = value; OnPropertyChanged(nameof(AfterProcText)); } }


        private IFileService fileService = new FileService();
        private IDialogService dialogService = new DialogService();

        public MainViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
        }

        // команда открытия файла
        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ?? (openFileCommand = new RelayCommand(obj =>
                {
                    TryOpenFileDialog();
                }));
            }
        }

        // команда сохранения файла
        private RelayCommand saveFileCommand;
        public RelayCommand SaveFileCommand
        {
            get
            {
                return saveFileCommand ?? (saveFileCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        List<string> textForSave = new List<string>();
                        if (postTexts.Count > 0)
                            textForSave.AddRange(postTexts);
                        else
                            textForSave.AddRange(oldTexts);

                        if (dialogService.SaveFileDialog(textForSave) == true)
                        {
                            for (int i = 0; i < textForSave.Count; i++)
                            {
                                fileService.Save(dialogService.FilePath, textForSave);
                            }
                            if (textForSave.Count > 1)
                                dialogService.ShowMessage("Файлы сохранены");                            
                            else
                                dialogService.ShowMessage("Файл сохранен");
                        }
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage("Failed to save file! " + ex.Message);
                    }
                }));
            }
        }

        //// команда добавления нового объекта
        //private RelayCommand addCommand;
        //public RelayCommand AddCommand
        //{
        //    get
        //    {
        //        return addCommand ??
        //          (addCommand = new RelayCommand(obj =>
        //          {
        //              Phone phone = new Phone();
        //              Phones.Insert(0, phone);
        //              SelectedPhone = phone;
        //          }));
        //    }
        //}

        //private RelayCommand removeCommand;
        //public RelayCommand RemoveCommand
        //{
        //    get
        //    {
        //        return removeCommand ??
        //          (removeCommand = new RelayCommand(obj =>
        //          {
        //              Phone phone = obj as Phone;
        //              if (phone != null)
        //              {
        //                  Phones.Remove(phone);
        //              }
        //          },
        //         (obj) => Phones.Count > 0));
        //    }
        //}
        //private RelayCommand doubleCommand;
        //public RelayCommand DoubleCommand
        //{
        //    get
        //    {
        //        return doubleCommand ??
        //          (doubleCommand = new RelayCommand(obj =>
        //          {
        //              Phone phone = obj as Phone;
        //              if (phone != null)
        //              {
        //                  Phone phoneCopy = new Phone
        //                  {
        //                      Company = phone.Company,
        //                      Price = phone.Price,
        //                      Title = phone.Title
        //                  };
        //                  Phones.Insert(0, phoneCopy);
        //              }
        //          }));
        //    }
        //}

        private int minLength = 0;
        public int MinLength { get => minLength; set { minLength = value; OnPropertyChanged(nameof(MinLength));} }

        private string charsForDel = "";
        public string CharsForDel { get => charsForDel; set { charsForDel = value; OnPropertyChanged(nameof(CharsForDel)); } }
        

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private readonly TextProcess textProcess = new TextProcess();

        private readonly BackgroundWorker worker;
        private int progressBarValue = 0;
        public int ProgressBarValue { get => progressBarValue; set { progressBarValue = value; OnPropertyChanged(nameof(ProgressBarValue)); } }

        private string progressBarTitle = "";
        public string ProgressBarTitle { get => progressBarTitle; set { progressBarTitle = value; OnPropertyChanged(nameof(ProgressBarTitle)); } }

        private RelayCommand startWorkCommand;
        public RelayCommand StartWorkCommand { get { return this.startWorkCommand; } }

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            DiscoveryVM = new DiscoveryViewModel();
            CurrentView = HomeVM;
            HomeViewCommand = new RelayCommand(o => { CurrentView = HomeVM; });
            DiscoveryViewCommand = new RelayCommand(o => { CurrentView = DiscoveryVM; });
            CloseApplicationCommand = new RelayCommand(closeApplicationCommand.Execute, closeApplicationCommand.CanExecute);
            
            startWorkCommand = new RelayCommand(o => worker.RunWorkerAsync(), o => !worker.IsBusy);
            worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
        }

        private async void DoWork(object sender, DoWorkEventArgs e)
        {
            postTexts = new List<string>();
            List<string> textInProcess = new List<string>();
            string subName = "";
            var progress = new Progress<int>(value => { ProgressBarValue = value; ProgressBarTitle = $"{subName}   {value}%"; });
            for (int i = 0; i < oldTexts.Count; i++)
            {
                string tempText = oldTexts[i];
                List<string> tokens = new List<string>();

                ProgressBarValue = 0;
                subName = $"Tokenization";
                await Task.Run(() => tokens.AddRange(textProcess.GetTokens(tempText, progress)));

                ProgressBarValue = 0;
                subName = $"Delete words less {minLength} chars";
                await Task.Run(() => tempText = textProcess.DeleteWord(tempText, minLength, tokens, progress));

                ProgressBarValue = 0;
                subName = $"Delete chars: {charsForDel}";
                await Task.Run(() => tempText = textProcess.DeleteChars(tempText, charsForDel.ToCharArray(), progress));

                textInProcess.Add(tempText);
            }
            postTexts.AddRange(textInProcess);
            AfterProcText = String.Join("\n\n"+ "=======================" +"\n\n", postTexts);
            ProgressBarTitle = "Complete!";
            ProgressBarValue = 100;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBarValue = e.ProgressPercentage;
        }

        private async void TryOpenFileDialog()
        {
            oldTexts = new List<string>();
            if (dialogService.OpenFileDialog() == true)
            {
                try
                {
                    await Task.Run(() => oldTexts.AddRange(fileService.Open(dialogService.FilePath)));
                    BeforeProcText = String.Join("\n\n" + "=======================" + "\n\n", oldTexts);
                }
                catch (Exception ex)
                {
                    dialogService.ShowMessage("Failed to open file! " + ex.Message);
                }
            }
        }
    }
}
