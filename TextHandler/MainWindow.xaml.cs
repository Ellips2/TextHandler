using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace TextHandler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {            
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }    
}
