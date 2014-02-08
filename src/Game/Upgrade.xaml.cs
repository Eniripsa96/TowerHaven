using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using WPFEditor;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for Upgrade.xaml
    /// </summary>
    public partial class Upgrade : Window
    {
        /// <summary>
        /// Level data
        /// </summary>
        Level level;

        /// <summary>
        /// Horizontal position of tower being upgraded
        /// </summary>
        int x;
        
        /// <summary>
        /// Vertical position of tower being upgraded
        /// </summary>
        int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="child">tower being upgraded</param>
        /// <param name="level">level the tower is in</param>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        public Upgrade(Tower child, Level level, int x, int y)
        {
            InitializeComponent();

            this.level = level;
            this.x = x;
            this.y = y;

            List<Tower> upgrades = level.GetUpgrades(child);
            foreach (Tower t in upgrades)
                upgradeComboBox.Items.Add(t.name);
            upgradeComboBox.SelectedIndex = 0;
            UpdateLabels();

            childAoeLabel.Content = child.aoeRadius > 0 ? child.aoeRadius + " (" + child.aoeDamage + "%)" : "None";
            childDamageLabel.Content = child.damage;
            childDelayLabel.Content = child.delay;
            childRangeLabel.Content = child.range;
            childStatusLabel.Content = child.status;
        }

        /// <summary>
        /// Updates the labels of the target upgraded tower
        /// </summary>
        private void UpdateLabels()
        {
            Tower t = level.GetTower(upgradeComboBox.SelectedItem.ToString());

            parentAoeLabel.Content = t.aoeRadius > 0 ? t.aoeRadius + " (" + t.aoeDamage + "%)" : "None"; ;
            parentDamageLabel.Content = t.damage;
            parentDelayLabel.Content = t.delay;
            parentRangeLabel.Content = t.range;
            parentStatusLabel.Content = t.status;
            upgradeCostLabel.Content = t.buildCost;
        }

        /// <summary>
        /// Updates the information depending on what tower was selected
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void enemyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLabels();
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Upgrades the tower to the currently selected one
        /// </summary>
        /// <param name="sender">not uesd</param>
        /// <param name="e">not used</param>
        private void upgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (upgradeComboBox.SelectedIndex >= 0)
            {
                level.UpgradeTower(x, y, upgradeComboBox.SelectedItem.ToString());
                if (Owner is Game)
                {
                    (Owner as Game).DeleteTower(x, y);
                    (Owner as Game).DrawTower(upgradeComboBox.SelectedItem.ToString(), x, y);
                }
                else
                {
                    (Owner as EndlessMode).DeleteTower(x, y);
                    (Owner as EndlessMode).DrawTower(upgradeComboBox.SelectedItem.ToString(), x, y);
                }
                Close();
            }
            else
                MessageBox.Show("Choose a tower to upgrade to\nwithout typing in anything");
        }
    }
}
