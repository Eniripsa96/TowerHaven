using System;
using System.Windows;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Options()
        {
            InitializeComponent();
            Properties.Settings settings = Properties.Settings.Default;
            gridLineComboBox.SelectedIndex = settings.GridLines ? 0 : 1;
            towerBeepComboBox.SelectedIndex = settings.TowerBeep ? 0 : 1;
        }

        /// <summary>
        /// Updates options
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings settings = Properties.Settings.Default;
            settings.GridLines = gridLineComboBox.SelectedIndex == 0;
            settings.TowerBeep = towerBeepComboBox.SelectedIndex == 0;
            Close();
        }
    }
}
