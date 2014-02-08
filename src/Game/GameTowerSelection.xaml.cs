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

using WPFEditor;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for GameTowerSelection.xaml
    /// </summary>
    public partial class GameTowerSelection : Window
    {
        /// <summary>
        /// List of towers to select from
        /// </summary>
        private List<Tower> towers;

        /// <summary>
        /// Cursor sprite
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Level data
        /// </summary>
        private Level level;

        /// <summary>
        /// Horizontal coordinate of selected tile
        /// </summary>
        private int x;

        /// <summary>
        /// Vertical coordinate of selected tile
        /// </summary>
        private int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">level data</param>
        /// <param name="x">horizontal coordinate of selected tile</param>
        /// <param name="y">vertical coordinate of selected tile</param>
        public GameTowerSelection(Level level, int x, int y)
        {
            InitializeComponent();

            // Set fields
            this.level = level;
            this.x = x;
            this.y = y;

            // Draw the cursor
            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 6, 8);
            nameGrid.Children.Add(cursor);

            // Find buildable towers that are affordable
            towers = level.GetBuildableTowers();
            int index = 0;
            foreach (Tower t in towers)
            {
                AddLabel(25, index, t.name, nameGrid);
                AddLabel(6, index, t.buildCost.ToString(), costGrid);
                AddLabel(6, index, t.range.ToString(), rangeGrid);
                AddLabel(6, index, t.damage.ToString(), damageGrid);
                AddLabel(6, index, t.status, statusGrid);
                index += 16;
            }
        }

        /// <summary>
        /// Adds a label to the display
        /// </summary>
        /// <param name="x">Left Margin</param>
        /// <param name="y">Top Margin</param>
        /// <param name="content">text</param>
        /// <param name="container">grid to add the label to</param>
        private void AddLabel(int x, int y, string content, Grid container)
        {
            Label label = ControlManager.CreateLabel(content, x, y);
            label.MouseLeftButtonUp += Label_Click;
            container.Children.Add(label);
        }

        /// <summary>
        /// Move the cursor to the selected tower
        /// </summary>
        /// <param name="sender">The tower label clicked on</param>
        /// <param name="e">not used</param>
        private void Label_Click(object sender, RoutedEventArgs e)
        {
            cursor.Margin = new Thickness(8, (sender as Label).Margin.Top + 6, 0, 0);
        }

        /// <summary>
        /// Close the menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Build the selected tower
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buildButton_Click(object sender, RoutedEventArgs e)
        {
            Tower t = towers[(int)cursor.Margin.Top / 16];
            level.BuyTower(t, x, y);
            if (Owner is Game)
                (Owner as Game).DrawTower(t.name, x, y);
            else
                (Owner as EndlessMode).DrawTower(t.name, x, y);
            Close();
        }
    }
}
