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

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for ConfirmFileSetup.xaml
    /// </summary>
    public partial class ConfirmFileSetup : Window
    {
        /// <summary>
        /// Whether or not the user clicked "Yes"
        /// </summary>
        private bool confirm;

        /// <summary>
        /// Confirmation property
        /// </summary>
        public bool Confirm
        {
            get { return confirm; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfirmFileSetup()
        {
            InitializeComponent();
            confirm = false;
        }

        /// <summary>
        /// Register the player hitting the yes button and then close the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            confirm = true;
            Close();
        }

        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
