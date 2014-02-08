using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Enemy sprite manager
    /// </summary>
    class EnemyManager
    {
        /// <summary>
        /// Enemy sprite directory
        /// </summary>
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Enemies\\";
        
        /// <summary>
        /// Window to add the sprite canvases to
        /// </summary>
        IAddable window;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">target window</param>
        public EnemyManager(IAddable window)
        {
            this.window = window;
        }

        /// <summary>
        /// Draws the enemy sprite with the given name to the window
        /// </summary>
        /// <param name="name">enemy name</param>
        /// <param name="y">vertical list coordinate</param>
        public void Draw(string name, int y)
        {
            Canvas enemy = ControlManager.CreateCanvas(directory + name, 25, y * 20, 16);
            window.Add(enemy);
        }
    }
}
