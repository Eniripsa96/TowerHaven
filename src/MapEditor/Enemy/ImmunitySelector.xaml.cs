using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for ImmunitySelector.xaml
    /// </summary>
    public partial class ImmunitySelector : Window
    {
        /// <summary>
        /// Enemy being edited
        /// </summary>
        Enemy enemy;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e">enemy to edit</param>
        public ImmunitySelector(Enemy e)
        {
            this.enemy = e;
            InitializeComponent();
            int index = 0;

            // Add each status to the window as a CheckBox
            foreach (string s in StatusData.GetStatusNames())
            {
                CheckBox checkBox = ControlManager.CreateCheckBox(s, 8, 20 * index);
                foreach (string immunity in e.ImmunitiesList)
                    if (immunity.Equals(s))
                        checkBox.IsChecked = true;
                grid1.Children.Add(checkBox);
                index++;
            }
        }

        /// <summary>
        /// Deselects all statuses
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control c in grid1.Children)
                if (c is CheckBox)
                    (c as CheckBox).IsChecked = false;
        }

        /// <summary>
        /// Updates the enemy immunities
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            string immunity = "";
            foreach (Control c in grid1.Children)
                if (c is CheckBox)
                {
                    CheckBox box = c as CheckBox;
                    if ((bool)box.IsChecked)
                        immunity += box.Content + ";";
                }
            if (immunity.Length > 0)
                immunity = immunity.Substring(0, immunity.Length - 1);
            else
                immunity = "None";
            enemy.immune = immunity;
            Close();
        }
    }
}
