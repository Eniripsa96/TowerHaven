using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for LevelSelection.xaml
    /// </summary>
    public partial class LevelSelection : Window
    {
        /// <summary>
        /// Cursor image
        /// </summary>
        private Canvas cursor;

        /// <summary>
        /// Names of maps
        /// </summary>
        private string[] mapNames;

        /// <summary>
        /// Whether or not looking through campaign maps
        /// </summary>
        private Boolean campaign;

        /// <summary>
        /// Returns if the menu is ready or not
        /// </summary>
        public Boolean Ready
        {
            get { return mapNames.Length > 0; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LevelSelection(string folder)
        {
            InitializeComponent();

            if (folder.Equals("Campaign"))
                campaign = true;

            // Draw the cursor
            cursor = ControlManager.CreateCanvas(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Marker", 8, 4, 8);
            grid1.Children.Add(cursor);

            // Get map names
            mapNames = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\" + folder);
            if (mapNames.Length == 0)
            {
                MessageBox.Show("There are no levels to play.");
                return;
            }
            for (int i = 0; i < mapNames.Length; ++i)
                mapNames[i] = mapNames[i].Substring(mapNames[i].LastIndexOf("\\") + 1).Replace(".thld", "");
            
            // Draw the label for each map
            int index = 0;
            foreach (string s in mapNames)
            {
                AddLabel(s, index);
                index++;
            }
        }

        /// <summary>
        /// Adds a label to the window
        /// </summary>
        /// <param name="text">label text</param>
        /// <param name="index">list index</param>
        private void AddLabel(string text, int index)
        {
            Label label = ControlManager.CreateLabel(text, 25, index * 20 - 5);
            label.MouseLeftButtonDown += LevelName_Click;
            grid1.Children.Add(label);
        }

        /// <summary>
        /// Level selected
        /// </summary>
        /// <param name="sender">level clicked</param>
        /// <param name="e">not used</param>
        private void LevelName_Click(object sender, RoutedEventArgs e)
        {
            Label levelName = sender as Label;
            cursor.Margin = new Thickness(8, levelName.Margin.Top + 9, 0, 0);
        }

        /// <summary>
        /// Starts the selected level
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)cursor.Margin.Top / 20;

            if (campaign && index == 2)
            {
                EndlessMode endlessMode = new EndlessMode();
                endlessMode.Owner = this;
                Hide();
                try
                {
                    endlessMode.ShowDialog();
                }
                catch (Exception)
                {
                    endlessMode.Close();
                    MessageBox.Show("Game data has been modified during play.\nAn error has occurred due to this.\nThe level was aborted.");
                }
                Show();
                return;
            }

            Boolean goodMap = false;
            BasicLevel level = new BasicLevel();

            // Load the map
            if (campaign)
                goodMap = level.LoadCampaign(mapNames[index]);
            else
                goodMap = level.Load(mapNames[index]);

            // If successful, play the map
            if (goodMap)
            {
                Game game = new Game(level);
                game.Owner = this;
                Hide();
                try
                {
                    game.ShowDialog();
                }
                catch (Exception)
                {
                    game.Close();
                    MessageBox.Show("Game data has been modified during play.\nAn error has occurred due to this.\nThe level was aborted.");
                }
                Show();
                return;
            }

            // Remove the map due to not being valid
            grid1.Children.RemoveAt(index + 1);
            for (int i = index + 1; i < grid1.Children.Count; ++i)
            {
                Label l = grid1.Children[i] as Label;
                l.Margin = new Thickness(l.Margin.Left, l.Margin.Top - 20, 0, 0);
            }
            if (grid1.Children.Count == 1)
            {
                MessageBox.Show("There are no more maps available");
                Close();
            }
            else if (index == grid1.Children.Count - 1)
            {
                cursor.Margin = new Thickness(8, 20 * index - 16, 0, 0);
            }
        }

        /// <summary>
        /// Shows the details of a map
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)cursor.Margin.Top / 20;
            BasicLevel level = new BasicLevel();

            bool goodMap;

            // Load the map
            if (campaign)
                goodMap = level.LoadCampaign(mapNames[index]);
            else
                goodMap = level.Load(mapNames[index]);

            if (goodMap)
            {
                MapDetails detailMenu = new MapDetails(level);
                detailMenu.Owner = this;
                Hide();
                detailMenu.ShowDialog();
                Show();
            }
        }

        /// <summary>
        /// OnClosed override
        /// Shows the main menu before closing
        /// </summary>
        /// <param name="e">not used</param>
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();
            base.OnClosed(e);
        }
    }
}
