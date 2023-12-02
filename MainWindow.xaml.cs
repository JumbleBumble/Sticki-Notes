using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StickyNotesJ
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();        
            if (appDataFolderPath != null)
            {
                filePath = Path.Combine(appDataFolderPath, "StickiNotes", "windowData.json");
            }
            CheckSave();
        }

        public List<Tuple<string, string>> WindowList { get; set; } = new List<Tuple<string, string>>();

        private readonly string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string? filePath = null;

        private void CheckSave()
        {

            if (filePath != null && File.Exists(filePath))
            {
                WindowList = LoadFromFile<List<Tuple<string, string>>>(filePath);
                if (WindowList != null)
                {
                    foreach (var window in WindowList)
                    {
                        NoteList.Items.Add(window.Item1);
                    }
                }            
            }
        }

        static T LoadFromFile<T>(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }


        public int SelectedIndex = -1;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = -1;
            NoteWindow newWindow = new();
            newWindow.Show();
            newWindow.Closing += (sender, e) => WindowClose(sender);
        }

        private void WindowClose(object sender)
        {
            if (sender != null && sender is NoteWindow closedWindow)
            {
                if (closedWindow.TextDescription != "")
                {
                    if (closedWindow.TextTitle == "")
                    {
                        closedWindow.TextTitle = closedWindow.TextDescription[..Math.Min(10, closedWindow.TextDescription.Length)];
                    }
                    var WindowTuple = Tuple.Create(closedWindow.TextTitle, closedWindow.TextDescription);
                    if (SelectedIndex == -1)
                    {
                        WindowList.Add(WindowTuple);
                        NoteList.Items.Add(closedWindow.TextTitle);
                    }
                    else if (NoteList.Items[SelectedIndex] != null && WindowList[SelectedIndex] != null)
                    {
                        NoteList.Items[SelectedIndex] = closedWindow.TextTitle;
                        WindowList[SelectedIndex] = WindowTuple;
                    }                 

                    if (File.Exists(filePath))
                    {
                        SaveToFile(filePath, WindowList);
                    }
                }                   
            }
        }

        private void NoteList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedIndex != -1)
                {
                    Tuple<string, string> SelectedWindow = WindowList[SelectedIndex];
                    if (SelectedWindow != null)
                    {
                        NoteWindow newWindow = new()
                        {
                            TextTitle = SelectedWindow.Item1,
                            TextDescription = SelectedWindow.Item2
                        };
                        newWindow.Show();
                        newWindow.Closing += (sender, e) => WindowClose(sender);
                    }
                }              
            } catch (ArgumentOutOfRangeException)
            {
                //MessageBox.Show("Note list is empty.", "Cannot open Note", MessageBoxButton.OK); // Not really needed
                return;
            }      
        }

        private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb != null && lb.SelectedItem != null)
            {
                SelectedIndex = lb.SelectedIndex;            
            }
        }

        static void SaveToFile<T>(string filePath, T data)
        {
            if (filePath != null && File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                string jsonData = JsonConvert.SerializeObject(data);

                File.WriteAllText(filePath, jsonData);
            }
        }

        private void RemoveNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedIndex != -1)
                {
                    WindowList.Remove(WindowList[SelectedIndex]);
                    NoteList.Items.Remove(NoteList.SelectedItem ?? NoteList.Items[^1]);
                    if (File.Exists(filePath))
                    {
                        SaveToFile(filePath, WindowList);
                    }
                    SelectedIndex = -1;
                }
            }
            catch (ArgumentOutOfRangeException)
            {          
                return;
            }
        }
    }
}
