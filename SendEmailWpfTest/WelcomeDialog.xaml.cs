using System.Windows;
using System.Windows.Input;

namespace SendEmailWpfTest
{
    /// <summary>
    /// Interaction logic for WelcomeDialog.xaml
    /// </summary>
    public partial class WelcomeDialog : Window
    {
        public WelcomeDialog()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
