using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using JSON;
using TowerHaven;
using System.Windows.Media.Imaging;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IAddable
    {
        # region constants

        /// <summary>
        /// Directory for the game data
        /// </summary>
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\";

        # endregion constants

        # region fields

        /// <summary>
        /// Map tile name matrix
        /// </summary>
        public static string[,] map;

        /// <summary>
        /// Map Detail: creator
        /// </summary>
        private string creator;

        /// <summary>
        /// Map Detail: date
        /// </summary>
        private string date;

        /// <summary>
        /// Map detail: description
        /// </summary>
        private string description;

        /// <summary>
        /// Map name
        /// </summary>
        private string name;

        /// <summary>
        /// Included towers in the map
        /// </summary>
        private List<string> towers;

        /// <summary>
        /// Background tile of the map
        /// </summary>
        string backgroundTile;

        /// <summary>
        /// Currently selected tile
        /// </summary>
        string tile;

        /// <summary>
        /// Map height
        /// </summary>
        public static int height;

        /// <summary>
        /// Map width
        /// </summary>
        public static int width;

        /// <summary>
        /// Map starting money
        /// </summary>
        private int money;

        /// <summary>
        /// Map starting health
        /// </summary>
        private int health;

        /// <summary>
        /// Tile drawing object
        /// </summary>
        TileManager sprite;

        /// <summary>
        /// Drawing boolean
        /// </summary>
        Boolean mouseDown = false;

        # endregion fields

        # region properties

        /// <summary>
        /// creator property
        /// </summary>
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }

        /// <summary>
        /// date property
        /// </summary>
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        /// <summary>
        /// description property
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        # endregion properties

        # region constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            // Default WPF intializer
            InitializeComponent();

            towers = new List<string>();
            try
            {
                // Create directories if they do not exist
                Directory.CreateDirectory(directory + "Tiles");
                Directory.CreateDirectory(directory + "Towers");
                Directory.CreateDirectory(directory + "Enemies");
                Directory.CreateDirectory(directory + "MapsSource");
            }
            catch (Exception) { }

            // Copy default tiles if no tiles are currently available
            if (TileData.GetTileNames().Length == 0)
                TileData.LoadTiles();

            // Load editor data
            TileData.Load();
            TowerData.Load();
            EnemyData.Load();
            StatusData.Load();
            WaveData.Load();

            // Prepare map drawing
            sprite = new TileManager(this);
            backgroundTile = "Grass";
            tile = TileData.GetTileNames()[0];
            height = 25;
            width = 30;
            health = 1;

            // Setup tile palette updater
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += UpdateTilePalette;
            timer.Start();
            UpdateTilePalette(null, null);

            // Load a map if any
            Load();

            // Add a mouse listener (turn off drawing when mouse leaves the window)
            MouseLeave += Mouse_Released;

            // Set the size of the window
            SetSize();
        }

        # endregion constructor

        /// <summary>
        /// Updates the tile palette and makes sure the tile that is selected was not deleted
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void UpdateTilePalette(object sender, EventArgs e)
        {
            paletteGrid.Children.Clear();
            int index = 0;
            string[] names = TileData.GetTileNames();
            if (names.Length == 0)
            {
                TileData.LoadTiles();
                names = TileData.GetTileNames();
            }
            foreach (string name in names)
            {
                Canvas paletteCanvas = ControlManager.CreateCanvas(directory + "Tiles\\" + name, (index % 5) * 17, (index++ / 5) * 17, 16);
                if (name.Equals(tile))
                    selectedTileCanvas.Background = paletteCanvas.Background;
                paletteCanvas.ToolTip = name;
                paletteCanvas.MouseLeftButtonUp += PaletteTile_Click;
                paletteGrid.Children.Add(paletteCanvas);
            }

            foreach (string s in TileData.GetTileNames())
                if (s.Equals(tile))
                    return;
            tile.Equals(names[0]);
            selectedTileCanvas.Background = (paletteGrid.Children[0] as Canvas).Background;
        }

        # region IO

        /// <summary>
        /// Load the first map found, if any
        /// </summary>
        private void Load()
        {
            Load(MapData.GetMapNames()[0]);
        }

        /// <summary>
        /// Loads the map with the given name
        /// if the map doesn't exist, create a new map
        /// </summary>
        /// <param name="name">map name</param>
        public void Load(string name)
        {
            WaveData.Clear();
            this.towers.Clear();
            this.name = name;
            try
            {
                JSONParser parser;
                using (StreamReader read = new StreamReader(directory + "MapsSource\\" + name))
                {
                    parser = new JSONParser(read.ReadToEnd());
                }

                foreach (PairList<string, object> colletion in parser.Objects)
                    foreach (PairListNode<string, object> element in colletion)
                        if (element.Key.Equals("MapProperties"))
                        {
                            foreach (PairListNode<string, object> property in element.Value as PairList<string, object>)
                                if (property.Key.Equals("Money"))
                                    money = int.Parse(property.Value.ToString());
                                else if (property.Key.Equals("Health"))
                                    health = int.Parse(property.Value.ToString());
                                else if (property.Key.Equals("Width"))
                                    width = int.Parse(property.Value.ToString());
                                else if (property.Key.Equals("Height"))
                                    height = int.Parse(property.Value.ToString());
                                else if (property.Key.Equals("Details"))
                                {
                                    SimpleList<object> details = property.Value as SimpleList<object>;
                                    creator = details[0].ToString();
                                    date = details[1].ToString();
                                    description = details[2].ToString();
                                }
                                else if (property.Key.Equals("Towers"))
                                    foreach (string tower in property.Value as SimpleList<object>)
                                        towers.Add(tower);
                        }
                        else if (element.Key.Equals("MapTiles"))
                        {
                            map = new string[width, height];
                            int index = 0;
                            foreach (string s in element.Value as SimpleList<object>)
                            {
                                sprite.Draw(s, index % width, index / width);
                                map[index % width, index++ / width] = s;
                            }
                        }
                        else if (element.Key.Equals("Waves"))
                            foreach (PairList<string, object> wave in element.Value as SimpleList<object>)
                                WaveData.Load(wave);

            }

            // Create a new map when invalid map
            catch (Exception)
            {
                Reset();
            }
            SetSize();
        }

        /// <summary>
        /// Saves editor data
        /// </summary>
        private void SaveSource()
        {
            TileData.Save();
            TowerData.Save();
            EnemyData.Save();
            StatusData.Save();
            try
            {
                using (StreamWriter save = new StreamWriter(directory + "MapsSource\\" + name))
                {
                    save.Write("{MapProperties:{Money:" + money + ",Health:" + health + ",Width:" + width + ",Height:" + height + ",Details:[" + creator + "," + date + "," + description + "],Towers:[");
                    for (int i = 0; i < towers.Count; ++i)
                        save.Write(towers[i] + (i < towers.Count - 1 ? "," : "]},MapTiles:["));
                    if (towers.Count == 0)
                        save.Write("]},MapTiles:[");
                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                            save.Write(map[x, y] + (x < width - 1 || y < height - 1 ? "," : "],"));

                    save.Write(WaveData.Data + "}");
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves the map and a release version of the map
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSource();

            // Initializations
            List<string> names = new List<string>();
            string[] folders = new string[] { "Tiles" /* Backup */, "Towers", "Enemies" };
            List<string[]> data = new List<string[]>();
            foreach (string s in map)
                if (!names.Contains(s))
                    names.Add(s);
            data.Add(names.ToArray());
            data.Add(towers.ToArray());
            data.Add(WaveData.GetEnemyList());

            // Save images
            using (FileStream write = new FileStream(directory + "Maps\\" + name + ".thld", FileMode.Create))
                for (int i = 0; i < 3; ++i)
                {
                    // Write how many images there are in the collection
                    byte[] groupSize = BitConverter.GetBytes((short)data[i].Length);
                    write.Write(groupSize, 0, groupSize.Length);

                    // Save each image in the collection
                    foreach (string s in data[i])
                        try
                        {
                            using (FileStream file = new FileStream(directory + folders[i] + "\\" + s + ".png", FileMode.Open))
                            {
                                // Write the image name size
                                write.WriteByte(BitConverter.GetBytes(s.Length)[0]);

                                // Write the image name
                                for (int index = 0; index < s.Length; ++index)
                                    write.Write(BitConverter.GetBytes(s[index]), 0, 2);

                                // Write the image size
                                byte[] intBytes = BitConverter.GetBytes((int)file.Length);
                                write.Write(intBytes, 0, intBytes.Length);

                                // Save image data
                                byte[] imageSize = new byte[file.Length];
                                file.Read(imageSize, 0, (int)file.Length);
                                write.Write(imageSize, 0, (int)file.Length);
                            }
                        }
                        catch (Exception) { }
                }

            // Save the rest of the data
            using (StreamWriter write = new StreamWriter(directory + "Maps\\" + name + ".thld", true))
            {
                // Mark the end of image data
                write.WriteLine();
                write.WriteLine("[END OF SPRITE IMAGE DATA]");

                // Save the map
                write.Write("{MapProperties:{Money:" + money + ",Health:" + health + ",Width:" + width + ",Height:" + height + ",Details:[" + creator + "," + date + "," + description + "],Towers:[");
                for (int i = 0; i < towers.Count; ++i)
                    write.Write(towers[i] + (i < towers.Count - 1 ? "," : "]},MapTiles:["));
                if (towers.Count == 0)
                    write.Write("]},MapTiles:[");
                string tileName = "";
                int quantity = 0;
                for (int y = 0; y < height; ++y)
                    for (int x = 0; x < width; ++x)
                    {
                        if (tileName.Equals(""))
                        {
                            tileName = map[x, y];
                            quantity = 1;
                        }
                        else if (!tileName.Equals(map[x, y]))
                        {
                            write.Write(tileName + ":" + quantity + ",");
                            tileName = map[x, y];
                            quantity = 1;
                        }
                        else
                            quantity++;
                    }
                write.Write(tileName + ":" + quantity + "],");

                // Save the waves
                write.Write(WaveData.Data + ",");

                // Save tile types
                write.Write("Tiles:[");
                int index = 0;
                foreach (string s in data[0])
                {
                    write.Write(s + ":" + (TileData.GetType(s) == null ? "null" : TileData.GetType(s).ToString()));
                    if (++index < data[0].Length)
                        write.Write(',');
                }
                write.Write("],");

                // Save tower data
                index = 0;
                write.Write("Towers:[");
                foreach (string s in data[1])
                {
                    write.Write(TowerData.GetTowerData(s));
                    if (++index < data[1].Length)
                        write.Write(',');
                }
                write.Write("],");

                // Save enemy data
                index = 0;
                write.Write("Enemies:[");
                foreach (string s in data[2])
                {
                    write.Write(EnemyData.GetEnemyData(s));
                    if (++index < data[2].Length)
                        write.Write(',');
                }
                write.Write("],");

                // Save status data
                Boolean first = true;
                write.Write("Statuses:[");
                List<string> statuses = new List<string>();
                foreach (string s in data[1])
                {
                    string status = TowerData.GetTower(s).status;
                    if (!statuses.Contains(status) && !status.Equals("None"))
                    {
                        if (first)
                            first = false;
                        else
                            write.Write(',');
                        statuses.Add(status);
                        write.Write(StatusData.GetStatusData(status));
                    }
                }
                write.Write("]}");
            }

            MessageBox.Show("Map has been saved!");
        }

        /// <summary>
        /// Opens the load map menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            MapSelection selection = new MapSelection();
            selection.Owner = this;
            selection.ShowDialog();
        }

        # endregion IO

        # region buttons

        /// <summary>
        /// Creates a new map with user input
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            NewMap newMap = new NewMap(width, height);
            newMap.Owner = this;
            newMap.ShowDialog();
            if (newMap.MapWidth > 0)
            {
                width = newMap.MapWidth;
                height = newMap.MapHeight;
                backgroundTile = newMap.Tile;
                name = newMap.MapName;
                SetSize();
                Reset();
            }
        }

        /// <summary>
        /// Creates a new map from an image
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void newFromImageButton_Click(object sender, RoutedEventArgs e)
        {
            NewMapFromImage newMap = new NewMapFromImage();
            newMap.Owner = this;
            newMap.ShowDialog();

            if (newMap.Valid)
            {
                BitmapImage image = ControlManager.LoadBitmap(newMap.imagePath.Text.Replace(".png", ""));
                if ((int)image.Width % 16 != 0 || (int)image.Height % 16 != 0)
                {
                    MessageBox.Show("The image isn't the right size. must be a multiple of 16 in each dimension (ex. 320x400 for a 20x25 map).");
                    return;
                }
                else if ((int)image.Width / 16 > 75 || (int)image.Height / 16 > 50)
                {
                    MessageBox.Show("The image is too large. The maximum dimensions are 1200x800.");
                    return;
                }
                else
                {
                    string name = newMap.imagePath.Text;
                    name = name.Substring(name.LastIndexOf("\\") + 1).Replace(".png", "");
                    int xMultiplier = (int)image.Height / 16;

                    width = (int)image.Width / 16;
                    health = (int)image.Height / 16;
                    backgroundTile = "Grass";
                    this.name = name;
                    SetSize();
                    Reset();
                    ContentRoot.Children.RemoveRange(12, ContentRoot.Children.Count - 12);

                    Directory.CreateDirectory(directory + "Tiles/" + name);

                    for (int x = 0; x < (int)image.Width; x += 16)
                    {
                        for (int y = 0; y < (int)image.Height; y += 16)
                        {
                            CroppedBitmap cropped = new CroppedBitmap(image, new Int32Rect(x, y, 16, 16));
                            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                            pngEncoder.Frames.Add(BitmapFrame.Create(cropped));
                            using (FileStream stream = new FileStream(directory + "Tiles/" + name + "/" + (x / 16 * xMultiplier + y / 16) + ".png", FileMode.Create))
                            {
                                pngEncoder.Save(stream);
                            }
                            sprite.Draw(name + "/" + (x / 16 * xMultiplier + y / 16), x / 16, y / 16);
                            map[x / 16, y / 16] = name + "/" + (x / 16 * xMultiplier + y / 16);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens the map properties menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void propertyButton_Click(object sender, RoutedEventArgs e)
        {
            MapProperties properties = new MapProperties(towers, name, health, money);
            properties.Owner = this;
            properties.ShowDialog();
            name = properties.MapName;
            money = properties.Money;
            health = properties.Health;
        }

        /// <summary>
        /// Opens the tile menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void tileButton_Click(object sender, RoutedEventArgs e)
        {
            if (TileSelection.Ready)
            {
                TileSelection tileSelection = new TileSelection(tile);
                tileSelection.Owner = this;
                tileSelection.ShowDialog();
            }
        }

        /// <summary>
        /// Opens the tower menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void towerButton_Click(object sender, RoutedEventArgs e)
        {
            if (TowerSelection.Ready)
            {
                TowerSelection towerSelection = new TowerSelection();
                towerSelection.Owner = this;
                towerSelection.ShowDialog();
            }
            else
                MessageBox.Show("There are no towers. Add a .png 16x16 image\nto the towers folder to add it to the editor.");
        }

        /// <summary>
        /// Opens the enemy menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void enemyButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemySelection.Ready)
            {
                EnemySelection enemySelection = new EnemySelection();
                enemySelection.Owner = this;
                enemySelection.ShowDialog();
            }
            else
                MessageBox.Show("There are no enemies. Add a .png 16x16 image\nto the enemies folder to add it to the editor.");
        }

        /// <summary>
        /// Opens the folder containing the editor data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            string myDocspath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven";
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = myDocspath;
            prc.Start();
        }

        /// <summary>
        /// Opens the status menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void statusButton_Click(object sender, RoutedEventArgs e)
        {
            StatusSelection statusSelection = new StatusSelection();
            statusSelection.Owner = this;
            statusSelection.ShowDialog();
        }

        /// <summary>
        /// Opens the wave menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void waveButton_Click(object sender, RoutedEventArgs e)
        {
            WaveSelection waveSelection = new WaveSelection();
            waveSelection.Owner = this;
            waveSelection.ShowDialog();
        }

        # endregion buttons

        # region mouse events

        /// <summary>
        /// Mouse Pressed event
        /// Activates drawing and draws on the current tile
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Mouse_Pressed(object sender, RoutedEventArgs e)
        {
            mouseDown = true;
            Tile_Changed(sender, e);
        }

        /// <summary>
        /// Mose Released event
        /// Deactivates drawing
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Mouse_Released(object sender, RoutedEventArgs e)
        {
            mouseDown = false;
        }

        # endregion mouse events

        # region closing

        /// <summary>
        /// OnClosed override
        /// Saves editor data before closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            SaveSource();
            Owner.Show();
            base.OnClosed(e);
        }

        # endregion closing

        # region map

        /// <summary>
        /// Adds the canvas to the display
        /// </summary>
        /// <param name="control"></param>
        public void Add(Canvas control)
        {
            control.MouseLeftButtonDown += Mouse_Pressed;
            control.MouseUp += Mouse_Released;
            control.MouseEnter += Tile_Changed;
            ContentRoot.Children.Add(control);
        }

        /// <summary>
        /// Resets the map to the background tile
        /// </summary>
        private void Reset()
        {
            description = "No description.";
            creator = "Not specified";
            date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local).ToString(@"MM/dd/yyyy");
            WaveData.Clear();
            map = new string[width, height];
            ContentRoot.Children.RemoveRange(12, ContentRoot.Children.Count - 12);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    sprite.Draw(backgroundTile, x, y);
                    map[x, y] = backgroundTile;
                }
        }

        /// <summary>
        /// Changes the tile to the currently selected tile
        /// </summary>
        /// <param name="sender">tile being changed</param>
        /// <param name="e">not used</param>
        private void Tile_Changed(object sender, RoutedEventArgs e)
        {
            if (!mouseDown)
                return;
            Canvas canvas = sender as Canvas;
            ContentRoot.Children.Remove(canvas);
            int x = (int)(canvas.Margin.Left - 115) / 16;
            int y = (int)(canvas.Margin.Top / 16);
            map[x, y] = tile;
            sprite.Draw(tile, x, y);
        }

        /// <summary>
        /// Sets the size of the window and centers it
        /// </summary>
        private void SetSize()
        {
            this.Width = 121 + 16 * width;
            this.Height = 27 + 16 * height;
            if (this.Height < 357)
                this.Height = 357;
            if (TowerHaven.Properties.Settings.Default.GridLines)
            {
                this.Width += 1;
                this.Height += 1;
                ContentRoot.Margin = new Thickness(ContentRoot.Margin.Left + 1, ContentRoot.Margin.Top + 1, 0, 0);
            }
            this.Left = (int)(SystemParameters.PrimaryScreenWidth - Width) / 2;
            this.Top = (int)(SystemParameters.PrimaryScreenHeight - Height) / 2;
        }

        # endregion map

        /// <summary>
        /// Displays the help menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            HelpMain help = new HelpMain();
            help.Owner = this;
            help.ShowDialog();
        }

        /// <summary>
        /// Selects the tile as the target tile
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void PaletteTile_Click(object sender, RoutedEventArgs e)
        {
            tile = (sender as Canvas).ToolTip.ToString();
            selectedTileCanvas.Background = (sender as Canvas).Background;
        }
    }
}
