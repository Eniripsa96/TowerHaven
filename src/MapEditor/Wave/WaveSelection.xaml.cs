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
    public partial class WaveSelection : Window
    {
        /// <summary>
        /// cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Constructor
        /// </summary>
        public WaveSelection()
        {
            InitializeComponent();
            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);
            string[] waveNames = WaveData.GetWaveNames();
            int index = 0;
            foreach (string s in waveNames)
            {
                AddLabel(s, index);
                index++;
            }
            if (WaveData.GetWaveNames().Length == 0)
                newButton_Click(null, null);
        }

        /// <summary>
        /// Adds a label to the window
        /// </summary>
        /// <param name="text">content</param>
        /// <param name="index">list index</param>
        private void AddLabel(string text, int index)
        {
            Label label = ControlManager.CreateLabel(text, 25, index * 20 - 5);
            label.MouseLeftButtonDown += WaveName_Click;
            grid1.Children.Add(label);
        }

        /// <summary>
        /// Wave name selected
        /// </summary>
        /// <param name="sender">label clicked</param>
        /// <param name="e">not used</param>
        private void WaveName_Click(object sender, RoutedEventArgs e)
        {
            Label waveName = sender as Label;
            cursor.Margin = new Thickness(8, waveName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// edits the selected wave
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)cursor.Margin.Top / 20;
            WaveEditor editor = new WaveEditor(index);
            editor.Owner = this;
            editor.ShowDialog();
            (grid1.Children[index + 1] as Label).Content = WaveData.GetWaveNames()[index];
        }

        /// <summary>
        /// Creates a new wave
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            cursor.Margin = new Thickness(8, 20 * grid1.Children.Count - 16, 0, 0);
            WaveData.GetWave(-1);
            AddLabel("Wave " + grid1.Children.Count, grid1.Children.Count - 1);
            WaveEditor editor = new WaveEditor(grid1.Children.Count - 2);
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
        }

        /// <summary>
        /// Deletes selected wave
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (grid1.Children.Count < 3)
            {
                MessageBox.Show("You cannot delete the last wave!");
                return;
            }
            int index = (int)cursor.Margin.Top / 20;
            WaveData.Remove(index);
            if (index == WaveData.GetWaveNames().Length)
            {
                cursor.Margin = new Thickness(8, 20 * (index - 1) + 4, 0, 0);
            }
            grid1.Children.RemoveAt(grid1.Children.Count - 1);
        }
    }
}
