using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Tower sprite manager
    /// </summary>
    class TowerManager
    {
        /// <summary>
        /// Tower sprite directory
        /// </summary>
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Towers\\";
        
        /// <summary>
        /// Window to draw to
        /// </summary>
        IAddable window;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">window to draw to</param>
        public TowerManager(IAddable window)
        {
            this.window = window;
        }

        /// <summary>
        /// Draws the tower to the window
        /// </summary>
        /// <param name="name">tower name</param>
        /// <param name="y">list coordinate</param>
        public void Draw(string name, int y)
        {
            Canvas tower = ControlManager.CreateCanvas(directory + name, 25, y * 20, 16);
            window.Add(tower);
        }
    }
}
