using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for StatusSelection.xaml
    /// </summary>
    public partial class StatusSelection : Window
    {
        /// <summary>
        /// Currently selected status
        /// </summary>
        private string status;
        
        /// <summary>
        /// Cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Constructor
        /// </summary>
        public StatusSelection()
        {
            InitializeComponent();

            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);
            
            string[] statusNames = StatusData.GetStatusNames();
            int index = 0;
            foreach (string s in statusNames)
            {
                AddLabel(s, index);
                index++;
            }

            if (StatusData.GetStatusNames().Length == 0)
                newButton_Click(null, null);
        }

        /// <summary>
        /// Adds a label to the window
        /// </summary>
        /// <param name="text">label text</param>
        /// <param name="index">list index</param>
        private void AddLabel(string text, int index)
        {
            Label label = ControlManager.CreateLabel(text, 25, 20 * index - 5);
            label.MouseLeftButtonDown += StatusName_Click;
            grid1.Children.Add(label);
        }

        /// <summary>
        /// Status label selected
        /// </summary>
        /// <param name="sender">label clicked on</param>
        /// <param name="e">not used</param>
        private void StatusName_Click(object sender, RoutedEventArgs e)
        {
            Label statusName = sender as Label;
            cursor.Margin = new Thickness(8, statusName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// Edits the currently selected status
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)cursor.Margin.Top / 20;
            status = StatusData.GetStatusNames()[index];
            StatusEditor editor = new StatusEditor(status);
            editor.Owner = this;
            editor.ShowDialog();
            (grid1.Children[index + 1] as Label).Content = StatusData.GetStatusNames()[index];
        }

        /// <summary>
        /// Creates a new status
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            // Move the cursor to the end
            cursor.Margin = new Thickness(8, 20 * grid1.Children.Count - 16, 0, 0);

            // Get an available name
            string name = "NewStatus1";
            int index = 1;
            Boolean valid;
            do
            {
                valid = true;
                foreach (string s in StatusData.GetStatusNames())
                    if (s.Equals(name))
                    {
                        name = name.Replace(index + "", ++index + "");
                        valid = false;
                    }
            }
            while (!valid);

            // Add the status to the window and to the level data
            StatusData.UpdateStatus(new Status(name, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0));
            StatusEditor editor = new StatusEditor(name);
            try
            {
                editor.Owner = this;
            }
            catch (Exception) 
            {
                editor.Left = (SystemParameters.PrimaryScreenWidth - editor.Width) / 2;
                editor.Top = (SystemParameters.PrimaryScreenHeight - editor.Height) / 2;
            }
            editor.ShowDialog();

            AddLabel(StatusData.GetStatusNames()[StatusData.GetStatusNames().Length - 1], grid1.Children.Count - 1);
        }
    }
}
