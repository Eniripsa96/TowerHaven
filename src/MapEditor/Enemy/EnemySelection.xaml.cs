using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for EnemySelection.xaml
    /// </summary>
    public partial class EnemySelection : Window, IAddable
    {
        /// <summary>
        /// Enemy name
        /// </summary>
        private string enemy;

        /// <summary>
        /// Cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Returns if the menu is ready to use
        /// </summary>
        public static Boolean Ready
        {
            get { return EnemyData.GetEnemyNames().Length > 0; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EnemySelection()
        {
            InitializeComponent();
            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);
            string[] enemyNames = EnemyData.GetEnemyNames();
            int index = 0;
            EnemyManager tower = new EnemyManager(this);

            // Add enemy labels
            foreach (string s in enemyNames)
            {
                tower.Draw(s, index);
                Label label = ControlManager.CreateLabel(s, 50, 20 * index - 5);
                label.MouseLeftButtonDown += EnemyName_Click;
                grid1.Children.Add(label);
                index++;
            }
        }

        /// <summary>
        /// Enemy label selected
        /// </summary>
        /// <param name="sender">label clicked on</param>
        /// <param name="e">not used</param>
        private void EnemyName_Click(object sender, RoutedEventArgs e)
        {
            Label tileName = sender as Label;
            cursor.Margin = new Thickness(8, tileName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// Enemy sprite selected
        /// </summary>
        /// <param name="sender">sprite clicked on</param>
        /// <param name="e">not used</param>
        private void Enemy_Click(object sender, RoutedEventArgs e)
        {
            Canvas tile = sender as Canvas;
            cursor.Margin = new Thickness(8, tile.Margin.Top + 4, 0, 0);
        }

        /// <summary>
        /// Adds the canvas to the window
        /// </summary>
        /// <param name="canvas">canvas to add</param>
        public void Add(Canvas canvas)
        {
            grid1.Children.Add(canvas);
            canvas.MouseLeftButtonDown += Enemy_Click;
        }

        /// <summary>
        /// Edits the currently selected enemy
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            enemy = EnemyData.GetEnemyNames()[(int)cursor.Margin.Top / 20];
            EnemyEditor editor = new EnemyEditor(enemy);
            editor.Owner = this;
            Hide();
            editor.ShowDialog();
            Show();
        }
    }
}
