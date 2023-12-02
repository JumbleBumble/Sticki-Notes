using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace StickyNotesJ.userControl
{
    public partial class JTextbox : UserControl
    {
        public JTextbox()
        {
            DataContext = this;
            InitializeComponent();
        }

        private string placeholder = "";

        public String Placeholder
        {
            get { return placeholder; }
            set {
                txtPlaceholder.Text = value;
                placeholder = value;
                OnPropertyChanged();
            }
        }

        private string originalText="";

        public String OriginalText
        {
            get { return originalText; }
            set
            {           
                originalText = value;
                txtInput.Text = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            originalText = txtInput.Text;
            if (string.IsNullOrEmpty(txtInput.Text))
                txtPlaceholder.Visibility = Visibility.Visible;
            else
                txtPlaceholder.Visibility = Visibility.Hidden;
        }

        private void OnPropertyChanged( [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
