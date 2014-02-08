using System;
using System.Windows;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for SpawnEditor.xaml
    /// </summary>
    public partial class SpawnEditor : Window
    {
        /// <summary>
        /// Spawn being edited
        /// </summary>
        private SpawnPoint spawn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">spawn to edit</param>
        public SpawnEditor(SpawnPoint s)
        {
            InitializeComponent();

            spawn = s;

            foreach (EnemyInstance e in s.enemies)
                enemyComboBox.Items.Add(e.name);
            if (enemyComboBox.Items.Count == 0)
                enemyComboBox.Items.Add("None");
            enemyComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Adds a new enemy
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnemyInstance enemy = new EnemyInstance(EnemyData.GetEnemyNames()[0], 0, 1);
                if (spawn.enemies.Count == 0)
                    enemyComboBox.Items.Remove("None");
                spawn.enemies.Add(enemy);
                enemyComboBox.Items.Add(enemy.name);
                WaveEnemyEditor editor = new WaveEnemyEditor(enemy);
                editor.Owner = this;
                editor.ShowDialog();
                enemyComboBox.Items[enemyComboBox.Items.Count - 1] = enemy.name;
                enemyComboBox.SelectedItem = enemy.name;
            }
            catch (Exception)
            {
                MessageBox.Show("There are no enemies to add.");
            }
        }

        /// <summary>
        /// Deletes the selected enemy
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (spawn.enemies.Count == 0)
            {
                MessageBox.Show("There are no enemies to delete.");
                return;
            }
            spawn.enemies.RemoveAt(enemyComboBox.SelectedIndex);
            enemyComboBox.Items.RemoveAt(enemyComboBox.SelectedIndex);
            if (spawn.enemies.Count == 0)
                enemyComboBox.Items.Add("None");
            enemyComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Edits the selected enemy
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (spawn.enemies.Count == 0)
            {
                MessageBox.Show("There are no enemies to edit.");
                return;
            }
            EnemyInstance enemy = spawn.enemies[enemyComboBox.SelectedIndex];
            WaveEnemyEditor editor = new WaveEnemyEditor(enemy);
            editor.Owner = this;
            editor.ShowDialog();
            enemyComboBox.Items[enemyComboBox.SelectedIndex] = enemy.name;
            enemyComboBox.SelectedItem = enemy.name;
        }

        /// <summary>
        /// Opens the position selection menu for the wave's starting position
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not use</param>
        private void startChooseButton_Click(object sender, RoutedEventArgs e)
        {
            PositionSelection selection = new PositionSelection("Select Starting Location");
            selection.Owner = this;
            selection.ShowDialog();
            if (selection.X >= 0)
                spawn.xStart = selection.X;
            if (selection.Y >= 0)
                spawn.yStart = selection.Y;
        }

        /// <summary>
        /// Opens the position selection menu for the wave's end position
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void endChooseButton_Click(object sender, RoutedEventArgs e)
        {
            PositionSelection selection = new PositionSelection("Select End Location");
            selection.Owner = this;
            selection.ShowDialog();
            if (selection.X >= 0)
                spawn.xEnd = selection.X;
            if (selection.Y >= 0)
                spawn.yEnd = selection.Y;
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
