using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for TowerSelection.xaml
    /// </summary>
    public partial class TowerSelection : Window, IAddable
    {
        /// <summary>
        /// Selected tower
        /// </summary>
        private string tower;

        /// <summary>
        /// Cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Returns if the menu is ready to use
        /// </summary>
        public static Boolean Ready
        {
            get { return TowerData.GetTowerNames().Length > 0; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TowerSelection()
        {
            InitializeComponent();

            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);
            string[] towerNames = TowerData.GetTowerNames();
            int index = 0;
            TowerManager tower = new TowerManager(this);

            // Add tower labels
            foreach (string s in towerNames)
            {
                tower.Draw(s, index);
                Label label = ControlManager.CreateLabel(s, 50, 20 * index - 5);
                label.MouseLeftButtonDown += TowerName_Click;
                grid1.Children.Add(label);
                index++;
            }
        }

        /// <summary>
        /// Selected tower name
        /// </summary>
        /// <param name="sender">tower label clicked</param>
        /// <param name="e">not used</param>
        private void TowerName_Click(object sender, RoutedEventArgs e)
        {
            Label tileName = sender as Label;
            cursor.Margin = new Thickness(8, tileName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// Selected tower sprite
        /// </summary>
        /// <param name="sender">sprite clicked</param>
        /// <param name="e">not used</param>
        private void Tower_Click(object sender, RoutedEventArgs e)
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
            canvas.MouseLeftButtonDown += Tower_Click;
        }

        /// <summary>
        /// Edits the selected tower
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            tower = TowerData.GetTowerNames()[(int)cursor.Margin.Top / 20];
            TowerEditor editor = new TowerEditor(tower);
            editor.Owner = this;
            editor.ShowDialog();
        }
    }
}
