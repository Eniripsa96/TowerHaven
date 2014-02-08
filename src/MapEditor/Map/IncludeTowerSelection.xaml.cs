using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for IncludeTowerSelection.xaml
    /// </summary>
    public partial class IncludeTowerSelection : Window
    {
        /// <summary>
        /// Towers included in the map
        /// </summary>
        List<string> includedTowers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="includedTowers">towers included in the map</param>
        public IncludeTowerSelection(List<string> includedTowers)
        {
            this.includedTowers = includedTowers;
            InitializeComponent();
            int index = 0;
            
            // Add each tower to the window as a CheckBox
            foreach (string s in TowerData.GetTowerNames())
            {
                CheckBox checkBox = ControlManager.CreateCheckBox(s, 8, 20 * index);
                foreach (string tower in includedTowers)
                    if (tower.Equals(s))
                        checkBox.IsChecked = true;
                grid1.Children.Add(checkBox);
                index++;
            }
        }

        /// <summary>
        /// Deselects all towers
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
        /// Applies the tower inclusion changes
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            includedTowers.Clear();
            foreach (Control c in grid1.Children)
                if (c is CheckBox)
                {
                    CheckBox box = c as CheckBox;
                    if ((bool)box.IsChecked)
                        includedTowers.Add(box.Content.ToString());
                }
            Close();
        }
    }
}
