using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Tile sprite manager
    /// </summary>
    class TileManager
    {
        /// <summary>
        /// Tile sprite directory
        /// </summary>
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Tiles\\";
        
        /// <summary>
        /// Window to draw to
        /// </summary>
        IAddable window;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">window to draw to</param>
        public TileManager(IAddable window)
        {
            this.window = window;
        }

        /// <summary>
        /// Draws the tile to the window
        /// </summary>
        /// <param name="name">tile name</param>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        public void Draw(string name, int x, int y)
        {
            Canvas tile = ControlManager.CreateCanvas(directory + name, x * 16 + 116, y * 16, 16);
            window.Add(tile);
        }
    }
}
