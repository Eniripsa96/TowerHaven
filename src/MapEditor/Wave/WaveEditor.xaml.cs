using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for WaveEditor.xaml
    /// </summary>
    public partial class WaveEditor : Window
    {
        /// <summary>
        /// Wave being edited
        /// </summary>
        Wave wave;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">id of wave to edit</param>
        public WaveEditor(int index)
        {
            InitializeComponent();
            wave = WaveData.GetWave(index);
            rewardBox.Text = wave.reward.ToString();
            int spawnIndex = 0;
            foreach (SpawnPoint s in wave.spawns)
                spawnBox.Items.Add("Spawn " + ++spawnIndex);
            if (wave.spawns.Count == 0)
                spawnBox.Items.Add("None");
            spawnBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates the wave data
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wave.reward = int.Parse(rewardBox.Text);
                Close();
            }
            catch (Exception) 
            {
                MessageBox.Show("Invalid reward. Must be an integer.");
            }
        }

        /// <summary>
        /// Closes the menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Adds a new spawn point
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            SpawnPoint spawn = new SpawnPoint();
            if (wave.spawns.Count == 0)
                spawnBox.Items.Remove("None");
            spawnBox.Items.Add("Spawn" + (spawnBox.Items.Count + 1));
            wave.spawns.Add(spawn);
            SpawnEditor editor = new SpawnEditor(spawn);
            editor.Owner = this;
            editor.ShowDialog();
        }

        /// <summary>
        /// Deletes the selected enemy
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (wave.spawns.Count == 0)
            {
                MessageBox.Show("There are no spawns to delete.");
                return;
            }
            wave.spawns.RemoveAt(spawnBox.SelectedIndex);
            spawnBox.Items.RemoveAt(spawnBox.SelectedIndex);
            if (spawnBox.Items.Count == 0)
                spawnBox.Items.Add("None");
            spawnBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Edits the selected enemy
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (wave.spawns.Count == 0)
            {
                MessageBox.Show("There are no enemies to edit.");
                return;
            }
            SpawnPoint spawn = wave.spawns[spawnBox.SelectedIndex];
            SpawnEditor editor = new SpawnEditor(spawn);
            editor.Owner = this;
            editor.ShowDialog();
        }
    }
}
