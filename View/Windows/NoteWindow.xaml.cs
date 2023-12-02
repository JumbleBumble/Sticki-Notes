using StickyNotesJ.Utilities;
using StickyNotesJ.View.Windows;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StickyNotesJ
{
    public partial class NoteWindow : Window
    {
        public NoteWindow()
        {
            InitializeComponent();
        }

        public string TextTitle { get { return Title.OriginalText; } set { Title.OriginalText = value; } }

        public string TextDescription { get { return Description.OriginalText; } set { Description.OriginalText = value; } }

        private static bool Pinned { get; set; } = false;
        private void PinItem_Click(object sender, RoutedEventArgs e)
        {
            Pinned = !Pinned;
            if (Pinned)
            {
                BitmapImage pinnedImage = new(new Uri("/Resources/push-pin.png", UriKind.Relative));

                PinImage.Source = pinnedImage;
            } else
            {
                BitmapImage pinnedImage = new(new Uri("/Resources/pin.png", UriKind.Relative));

                PinImage.Source = pinnedImage;
            }
            this.Topmost = Pinned;
        }

        private string Password { get; set; } = string.Empty;

        private void DialogControl(int EncryptType)
        {
            PassDialog passDialog = new();
            passDialog.Closing += (sender, e) => WindowClose(sender, EncryptType);
            passDialog.ShowDialog();         
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {    
            if (sender != null)
            {
                DialogControl(0);
            }
        }


        private void DecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                DialogControl(1);
            }
        }


        private void WindowClose(object sender, int Etype)
        {          
            if (sender is PassDialog closedDialog)
            {
                Password = closedDialog.Password;
            }
            if (Password != string.Empty)
            {
                if (Etype == 0)
                    TextDescription = StringSecurity.EncryptString(TextDescription, Password);
                else
                    TextDescription = StringSecurity.DecryptString(TextDescription, Password);
            }
        }
    }
}
