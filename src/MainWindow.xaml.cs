using Microsoft.Win32;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using TowerHaven.Properties;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            new System.Threading.Thread(() => Beep.Run()).Start();
            try
            {
                InitializeDirectories();

                // Set up file type association
                RegistryKey root = Registry.ClassesRoot;
                RegistryKey check = root.OpenSubKey("TowerHaven");
                if (check != null)
                    if (!check.OpenSubKey("shell").OpenSubKey("Open").OpenSubKey("Command").GetValue(string.Empty).Equals("\"" + Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName + "\" OpenMap %1"))
                        check = null;
                if (check == null)
                {
                    ConfirmFileSetup confirmation = new ConfirmFileSetup();
                    confirmation.ShowDialog();
                    if (confirmation.Confirm)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(Environment.CurrentDirectory + "//" + AppDomain.CurrentDomain.FriendlyName);
                        startInfo.Verb = "runas";
                        startInfo.Arguments = "setup";
                        Process.Start(startInfo);
                        MessageBox.Show("The setup has finished. Double clicking maps will\nnow play them.");
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// OnClosed override
        /// Forces all parts to close (due to rare instances where the program would run after closing)
        /// </summary>
        /// <param name="e">not used</param>
        protected override void OnClosed(EventArgs e)
        {
            Properties.Settings.Default.Save();
            base.OnClosed(e);
            Environment.Exit(0);
        }

        /// <summary>
        /// Opens the level editor when the button is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void editorButton_Click(object sender, RoutedEventArgs e)
        {
            WPFEditor.MainWindow window = null;
            try
            {
                window = new WPFEditor.MainWindow();
                Hide();
                window.ShowDialog();
                Show();
            }
            catch (Exception ex)
            {
                InitializeDirectories();
                if (window != null)
                    window.Close();
                MessageBox.Show("Files were illegally modified while in the editor.\n" + ex.Message + "\n" + ex.StackTrace);
                Show();
            }
        }

        /// <summary>
        /// Begins the default levels
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Campaign";
            InitializeDirectories();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
                try
                {
                    File.Delete(file);
                }
                catch (Exception) { }
            SaveCampaignMap("1 - Beginning", Properties.Resources._1___Beginning);
            SaveCampaignMap("2 - Conflict", Properties.Resources._2___Conflict);
            SaveCampaignMap("Endless Mode", Properties.Resources.Endless_Mode);
            LevelSelection levelSelection = new LevelSelection("Campaign");
            levelSelection.Owner = this;
            Hide();
            levelSelection.ShowDialog();
        }

        /// <summary>
        /// Opens the level selection when the button is clicked
        /// </summary>
        /// <param name="sender">not use</param>
        /// <param name="e">not used</param>
        private void customButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LevelSelection selection = new LevelSelection("Maps");
                if (selection.Ready)
                {
                    selection.Owner = this;
                    Hide();
                    selection.ShowDialog();
                }
            }
            catch (Exception)
            {
                InitializeDirectories();
                MessageBox.Show("The folder was deleted. It was recreated, but no maps exist.");
            }
        }
        
        /// <summary>
        /// Saves the campaign map to the appropriate folder
        /// </summary>
        /// <param name="name">map name</param>
        /// <param name="map">map data</param>
        private void SaveCampaignMap(string name, byte[] map)
        {
            try
            {
                using (FileStream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Campaign\\" + name + ".thld", FileMode.Create))
                {
                    stream.Write(map, 0, map.Length);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Opens the help menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpMain help = new HelpMain();
            help.Owner = this;
            Hide();
            help.ShowDialog();
        }

        /// <summary>
        /// Creates the folders necessary for the game
        /// </summary>
        private void InitializeDirectories()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\";
            Directory.CreateDirectory(directory + "Maps");
            Directory.CreateDirectory(directory + "temp");
            Directory.CreateDirectory(directory + "Campaign");
            Properties.Resources.Marker.Save(directory + "Marker.png");
        }

        /// <summary>
        /// Opens the options menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void optionsButton_Click(object sender, RoutedEventArgs e)
        {
            Options optionMenu = new Options();
            optionMenu.Owner = this;
            Hide();
            optionMenu.ShowDialog();
            Show();
        }
    }
}
