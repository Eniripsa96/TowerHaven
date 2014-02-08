using Microsoft.Win32;

using System;
using System.IO;
using System.Windows;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Checks for arguments on startup
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">list of arguments</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // File type setup
            if (e.Args.Length == 1 && e.Args[0].Equals("setup"))
            {
                try
                {
                    // Get a path for the icon in appdata
                    string iconPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\icon.ico";

                    // Save the icon
                    FileStream iconStream = new FileStream(iconPath, FileMode.Create);
                    TowerHaven.Properties.Resources.icon.Save(iconStream);
                    iconStream.Close();

                    // Create the application reference
                    RegistryKey root = Registry.ClassesRoot;
                    RegistryKey application = root.CreateSubKey("TowerHaven");
                    application.SetValue(string.Empty, "Tower Haven Level Data");
                    application.CreateSubKey("DefaultIcon")
                        .SetValue(string.Empty, iconPath);
                    application.CreateSubKey("shell")
                        .CreateSubKey("Open")
                        .CreateSubKey("Command")
                        .SetValue(string.Empty, "\"" + Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName + "\" OpenMap %1");

                    // Create the file type association
                    RegistryKey extension = root.CreateSubKey(".thld");
                    extension.SetValue(string.Empty, "TowerHaven");
                }
                catch (Exception) { /* In case someone runs the program with the argument without elevation or when it is already set up */ }
                Environment.Exit(0);
            }

            // Play level
            else if (e.Args.Length > 1 && e.Args[0].Equals("OpenMap"))
            {
                BasicLevel level = new BasicLevel();
                string path = e.Args[1];
                for (int i = 2; i < e.Args.Length; ++i)
                    path += " " + e.Args[i];
                level.LoadMap(path);
                Game game = new Game(level);
                game.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                game.ShowDialog();
            }
        }
    }
}
