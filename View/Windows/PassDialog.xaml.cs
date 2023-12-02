using System.Windows;

namespace StickyNotesJ.View.Windows
{
    public partial class PassDialog : Window
    {
        public PassDialog()
        {
            InitializeComponent();
        }

        public string Password { get; set; } = "password";

        private void EnterPassBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.OriginalText != "")
            {
                Password = PasswordInput.OriginalText;
                this.Close();
            }     
        }
    }
}
