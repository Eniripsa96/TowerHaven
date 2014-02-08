using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using WPFEditor;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for GameTileMenu.xaml
    /// </summary>
    public partial class GameTileMenu : Window
    {
        private readonly string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\";
        /// <summary>
        /// Sprite directory
        /// </summary>
        private readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\temp\\";

        /// <summary>
        /// Level object
        /// </summary>
        private Level level;

        /// <summary>
        /// Horizontal coordinate of the tile
        /// </summary>
        private int x;

        /// <summary>
        /// Vertical coordinate of the tile
        /// </summary>
        private int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">level data</param>
        /// <param name="x">horizontal position</param>
        /// <param name="y">vertical position</param>
        public GameTileMenu(Level level, int x, int y) 
            : this(level, x, y, true) 
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">level data</param>
        /// <param name="x">horizontal position</param>
        /// <param name="y">vertical position</param>
        /// <param name="canBuild">whether or not this tile can be built on presently</param>
        public GameTileMenu(Level level, int x, int y, Boolean canBuild)
        {
            InitializeComponent();

            // Set fields
            this.level = level;
            this.x = x;
            this.y = y;

            // Apply data to labels
            tileName.Content = level.GetTileName(x, y);
            typeLabel.Content = level.GetTileType(x, y);

            // Draw the tile or tower
            string path;
            if (level.IsTower(x, y))
                path = directory + "Tower" + level.GetTileName(x, y);
            else
                path = directory + "Tile" + level.GetTileName(x, y);
            Canvas tile = ControlManager.CreateCanvas(path, 4, 4, 16);
            ContentRoot.Children.Add(tile);

            // Add buttons

            // If it is a tower, add the "Sell" button
            // If the tower can be upgraded, add the "Upgrade" button
            if (level.IsTower(x, y))
            {
                Button option = CreateOptionButton("Sell Tower", 8, 60, 8);
                option.Click += Sell_Click;
                ContentRoot.Children.Add(option);

                // Check if the tower can be upgraded
                // Needs enough money and a tower to upgrade into
                foreach (Tower t in level.TowerList)
                {
                    if (t.baseTower.Equals(level.GetTileName(x, y)) && level.Money >= t.buildCost)
                    {
                        Button upgrade = CreateOptionButton("Upgrade", 8, 95, 8);
                        upgrade.Click += Upgrade_Click;
                        Height = Height + 35;
                        ContentRoot.Children.Add(upgrade);
                        break;
                    }
                }
            }

            // If it is a tile and it is buildable and enough money is owned to build at least one tower,
            // add the "Build" button
            else if (level.CanBuild && level.IsBuildable(x, y) && canBuild)
            {
                Button option = CreateOptionButton("Build Tower", 8, 60, 8);
                option.Click += Build_Click;
                ContentRoot.Children.Add(option);
            }
            else
                Height = Height - 35;
        }

        /// <summary>
        /// Creates a button for the options for the menu
        /// </summary>
        /// <param name="text">content</param>
        /// <param name="x">left margin</param>
        /// <param name="y">top margin</param>
        /// <param name="z">right margin</param>
        /// <returns>button</returns>
        private Button CreateOptionButton(string text, int x, int y, int z)
        {
            Button option = ControlManager.CreateButton(text, x, y, z);
            return option;
        }

        /// <summary>
        /// Sells the tower when the button is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            level.SellTower(x, y);
            if (Owner is Game)
                (Owner as Game).DeleteTower(x, y);
            else
                (Owner as EndlessMode).DeleteTower(x, y);
            Close();
        }

        /// <summary>
        /// Opens the build menu when the button is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Build_Click(object sender, RoutedEventArgs e)
        {
            GameTowerSelection towerSelection = new GameTowerSelection(level, x, y);
            towerSelection.Owner = this.Owner;
            Hide();
            towerSelection.ShowDialog();
            Show();
            Close();
        }

        /// <summary>
        /// Closes the menu when the button is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void X_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Shows the confirmation for an upgrade
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Upgrade_Click(object sender, RoutedEventArgs e)
        {
            Upgrade upgrade = new Upgrade(level.GetTower(level.GetTileName(x, y)), level, x, y);
            upgrade.Owner = this.Owner;
            Close();
            upgrade.ShowDialog();
        }
    }
}
