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
        private string newText;
        public string NewText { get {return newText; } set { newText = value; OnPropertyChanged("NewText"); } }
        
        private string postText;
        public string PostText { get {return postText; } set {postText = value; OnPropertyChanged("PostText"); } }
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
                        if (dialogService.SaveFileDialog() == true)
                        {
                            fileService.Save(dialogService.FilePath, NewText);
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
            string subName = "";
            var progress = new Progress<int>(value => { ProgressBarValue = value; ProgressBarTitle = $"{subName}   {value}%"; });
            string textInProcess = newText;
            List<string> tokens = new List<string>();

            ProgressBarValue = 0;
            subName = $"Tokenization";
            await Task.Run(() => tokens.AddRange(textProcess.GetTokens(textInProcess, progress)));

            ProgressBarValue = 0;
            subName = $"Delete words less {minLength} chars";
            await Task.Run(()=> textInProcess = textProcess.DeleteWord(textInProcess, minLength, tokens, progress));

            ProgressBarValue = 0;
            subName = $"Delete chars: {charsForDel}";
            await Task.Run(()=> textInProcess = textProcess.DeleteChars(textInProcess, charsForDel.ToCharArray(), progress));

            ProgressBarTitle = "Complete!";
            PostText = textInProcess;
            ProgressBarValue = 100;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBarValue = e.ProgressPercentage;
        }

        private async void TryOpenFileDialog()
        {
            if (dialogService.OpenFileDialog() == true)
            {
                try
                {
                    await Task.Run(() => NewText = fileService.Open(dialogService.FilePath));
                }
                catch (Exception ex)
                {
                    dialogService.ShowMessage("Failed to open file! " + ex.Message);
                }
            }
        }
    }
}
