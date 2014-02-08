using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    /// Interaction logic for TileSelection.xaml
    /// </summary>
    public partial class TileSelection : Window, IAddable
    {
        /// <summary>
        /// Tile data files directory
        /// </summary>
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Tiles";

        /// <summary>
        /// Cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Returns if the menu is ready to use
        /// </summary>
        public static Boolean Ready
        {
            get { return TileData.GetTileNames().Length > 0; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="currentTile">currently selected tile</param>
        public TileSelection(string currentTile)
        {
            InitializeComponent();
            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);
            string[] tiles = TileData.GetTileNames();
            int index = 0;
            TileManager sprite = new TileManager(this);

            // Add tile labels
            foreach (string s in tiles)
            {
                if (s.Equals(currentTile))
                    cursor.Margin = new Thickness(8, 4 + 16 * index, 0, 0);
                sprite.Draw(s, -6, index);
                Label label = ControlManager.CreateLabel(s, 50, 16 * index - 5);
                label.MouseLeftButtonDown += TileName_Clicked;
                grid1.Children.Add(label);
                index++;
            }
        }

        /// <summary>
        /// Tile sprite selected
        /// </summary>
        /// <param name="sender">sprite selected</param>
        /// <param name="e">not used</param>
        private void Tile_Clicked(object sender, RoutedEventArgs e)
        {
            Canvas tile = sender as Canvas;
            cursor.Margin = new Thickness(8, tile.Margin.Top + 4, 0, 0);
        }

        /// <summary>
        /// Tile name selected
        /// </summary>
        /// <param name="sender">label selected</param>
        /// <param name="e">not used</param>
        private void TileName_Clicked(object sender, RoutedEventArgs e)
        {
            Label tileName = sender as Label;
            cursor.Margin = new Thickness(8, tileName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// Adds canvas to the window
        /// </summary>
        /// <param name="canvas">canvas to add</param>
        public void Add(Canvas canvas)
        {
            canvas.Margin = new Thickness(28, canvas.Margin.Top, 0, 0);
            grid1.Children.Add(canvas);
            canvas.MouseLeftButtonDown += Tile_Clicked;
        }

        /// <summary>
        /// Edits the currently selected tile
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            string tile = TileData.GetTileNames()[(int)cursor.Margin.Top / 16];
            TileEditor editor = new TileEditor(tile);
            editor.Owner = this;
            editor.ShowDialog();
        }
    }
}
