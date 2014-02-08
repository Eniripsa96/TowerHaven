using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for TileEditor.xaml
    /// </summary>
    public partial class TileEditor : Window, IAddable
    {
        /// <summary>
        /// Tile type labels
        /// </summary>
        private static readonly string[] types = { "Buildable", "Walkable", "Not Walkable" };

        /// <summary>
        /// Tile type values
        /// </summary>
        private static readonly Boolean?[] typeValues = { true, false, null };

        /// <summary>
        /// Tile being edited
        /// </summary>
        private static string tileName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tile">name of tile to edit</param>
        public TileEditor(string tile)
        {
            tileName = tile;
            InitializeComponent();
            bool? currentType = TileData.GetType(tile);
            int index = 0;
            foreach (string s in types)
            {
                typeComboBox.Items.Add(s);
                if (typeValues[index++] == currentType)
                    typeComboBox.SelectedIndex = index - 1;
            }
            TileManager sprite = new TileManager(this);
            sprite.Draw(tile, 0, 0);
        }

        /// <summary>
        /// Adds the canvas to the window
        /// </summary>
        /// <param name="canvas">canvas to add</param>
        public void Add(Canvas canvas)
        {
            canvas.Margin = new Thickness(5, 5, 0, 0);
            ContentRoot.Children.Add(canvas);
        }

        /// <summary>
        /// updates the tile
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            if (typeComboBox.SelectedIndex >= 0)
            {
                TileData.SetType(tileName, typeValues[typeComboBox.SelectedIndex]);
                Close();
            }
            else
                MessageBox.Show("Please select a value.");
        }
    }
}
