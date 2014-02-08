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
    /// Interaction logic for MapDetails.xaml
    /// </summary>
    public partial class MapDetails : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MapDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Displays the window, setting the text of the labels first
        /// </summary>
        public void Display()
        {
            WPFEditor.MainWindow w = Owner.Owner as WPFEditor.MainWindow;
            creatorBox.Text = w.Creator;
            dateBox.Text = w.Date;
            descriptionBox.Text = w.Description;
            ShowDialog();
        }

        /// <summary>
        /// Closes the menu, saving details before closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            WPFEditor.MainWindow w = Owner.Owner as WPFEditor.MainWindow;
            w.Creator = creatorBox.Text;
            w.Date = dateBox.Text;
            w.Description = descriptionBox.Text.Replace(",", "").Replace("{", "").Replace("}","").Replace(":", "");
            Close();
        }
    }
}
