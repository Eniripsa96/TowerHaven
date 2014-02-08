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

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for MapSelection.xaml
    /// </summary>
    public partial class MapSelection : Window
    {
        /// <summary>
        /// Cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Returns if the menu is ready to use
        /// </summary>
        public static Boolean Ready
        {
            get { return MapData.GetMapNames().Length > 0; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MapSelection()
        {
            InitializeComponent();
            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);
            string[] mapNames = MapData.GetMapNames();
            int index = 0;

            // Add map labels
            foreach (string s in mapNames)
            {
                Label label = ControlManager.CreateLabel(s, 50, 20 * index - 5);
                label.MouseLeftButtonDown += MapName_Click;
                grid1.Children.Add(label);
                index++;
            }
        }

        /// <summary>
        /// Select the clicked map name
        /// </summary>
        /// <param name="sender">map label clicked on</param>
        /// <param name="e">not used</param>
        private void MapName_Click(object sender, RoutedEventArgs e)
        {
            Label tileName = sender as Label;
            cursor.Margin = new Thickness(8, tileName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// Loads the currently selected map
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            string mapName = MapData.GetMapNames()[(int)cursor.Margin.Top / 20];
            (Owner as MainWindow).Load(mapName);
            Close();
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
    }
}
