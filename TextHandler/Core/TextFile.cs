namespace TextHandler.Core
{
    public class TextFile : ObservableObject
    {
        private string name = "";
        public string Name { get => name; set { name = value; OnPropertyChanged(nameof(name)); } }

        private string path = "";
        public string Path { get => path; set { path = value; OnPropertyChanged(nameof(path)); } }

        private string oldText = "";
        public string OldText { get => oldText; set { oldText = value; OnPropertyChanged(nameof(oldText)); } }

        private string newText = "";
        public string NewText { get => newText; set { newText = value; OnPropertyChanged(nameof(newText)); } }

        private string charsForDel = "";
        public string CharsForDel { get => charsForDel; set { charsForDel = value; OnPropertyChanged(nameof(charsForDel));} }

        private int minLengthWord = 0;
        public int MinLengthWord { get => minLengthWord; set { minLengthWord = value; OnPropertyChanged(nameof(minLengthWord)); } }
    }
}
