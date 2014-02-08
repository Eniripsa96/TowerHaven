using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for PositionSelection.xaml
    /// </summary>
    public partial class PositionSelection : Window, IAddable
    {
        /// <summary>
        /// horizontal coordinate
        /// </summary>
        int x;
        
        /// <summary>
        /// vertical coordinate
        /// </summary>
        int y;

        /// <summary>
        /// horizontal coordinate property
        /// </summary>
        public int X
        {
            get { return x; }
        }

        /// <summary>
        /// vertical coordinate property
        /// </summary>
        public int Y
        {
            get { return y; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">message text</param>
        public PositionSelection(string s)
        {
            InitializeComponent();
            messageLabel.Content = s;
            x = 0;
            y = 0;
            TileManager tiles = new TileManager(this);
            foreach (string tile in MainWindow.map)
            {
                tiles.Draw(tile, 0, 0);
                y++;
                if (y == MainWindow.height)
                {
                    y = 0;
                    x++;
                }
            }
            x = -1;
            y = -1;
        }

        /// <summary>
        /// Adds the canvas to the window
        /// </summary>
        /// <param name="canvas">canvas to add</param>
        public void Add(Canvas canvas)
        {
            canvas.Margin = new Thickness(x * 15, y * 15 + 25, 0, 0);
            canvas.MouseLeftButtonDown += Tile_Click;
            ContentRoot.Children.Add(canvas);
        }

        /// <summary>
        /// Map tile was selected
        /// </summary>
        /// <param name="sender">tile clicked</param>
        /// <param name="e">not used</param>
        public void Tile_Click(object sender, RoutedEventArgs e)
        {
            Canvas tile = sender as Canvas;
            x = (int)tile.Margin.Left / 15;
            y = (int)(tile.Margin.Top - 25) / 15;
            MessageBox.Show("Position set to: (" + x + ", " + y + ")");
            Close();
        }
    }
}
