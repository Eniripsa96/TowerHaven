using System;
using System.Windows;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for NewMap.xaml
    /// </summary>
    public partial class NewMap : Window
    {
        /// <summary>
        /// Map width
        /// </summary>
        private int width;

        /// <summary>
        /// Map height
        /// </summary>
        private int height;

        /// <summary>
        /// Background tile
        /// </summary>
        private string tile;

        /// <summary>
        /// Map name
        /// </summary>
        private string name;

        /// <summary>
        /// Width property
        /// </summary>
        public int MapWidth
        {
            get { return width; }
        }

        /// <summary>
        /// Height property
        /// </summary>
        public int MapHeight
        {
            get { return height; }
        }

        /// <summary>
        /// Background tile property
        /// </summary>
        public string Tile
        {
            get { return tile; }
        }

        /// <summary>
        /// Name property
        /// </summary>
        public string MapName
        {
            get { return name; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">current width</param>
        /// <param name="height">current height</param>
        public NewMap(int width, int height)
        {
            InitializeComponent();
            nameBox.Text = GetName();
            widthBox.Text = width.ToString();
            heightBox.Text = height.ToString();
            string[] sprites = TileData.GetTileNames();
            foreach (string s in sprites)
                tileComboBox.Items.Add(s);
            tileComboBox.SelectedIndex = 0;
            this.width = -1;
        }

        /// <summary>
        /// Creates a new map with the provided name
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                width = int.Parse(widthBox.Text);
                height = int.Parse(heightBox.Text);
                tile = tileComboBox.Text;
                name = nameBox.Text;
            }
            catch (Exception)
            {
                width = -1;
                MessageBox.Show("Width and Height must be positive integers.");
                return;
            }
            if (width <= 0 || height <= 0)
            {
                width = -1;
                MessageBox.Show("Width and Height must be positive.");
                return;
            }
            if (!tileComboBox.Items.Contains(tile))
            {
                width = -1;
                MessageBox.Show("That is an invalid tile.");
                return;
            }
            if (MapData.MapExists(name))
            {
                width = -1;
                MessageBox.Show("That map name is already taken.");
                return;
            }
            if (width > 75)
            {
                width = -1;
                MessageBox.Show("Width must be at most 75.");
                return;
            }
            if (height > 50)
            {
                width = -1;
                MessageBox.Show("Height must be at most 50.");
                return;
            }
            if (width < 2)
            {
                width = -1;
                MessageBox.Show("Width must be at least 2");
                return;
            }
            if (height < 2)
            {
                width = -1;
                MessageBox.Show("Height must be at least 2");
                return;
            }
            if (width * height < 10)
            {
                width = -1;
                MessageBox.Show("The map area must be at least 10.");
                return;
            }
            Close();
        }

        /// <summary>
        /// Closes the menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Gets an available map name
        /// </summary>
        /// <returns></returns>
        private string GetName()
        {
            int index = 1;
            string name = "New Map 1";
            Boolean valid;
            do
            {
                valid = true;
                if (MapData.MapExists(name))
                {
                    valid = false;
                    name = name.Replace(index + "", ++index + "");
                }
            }
            while (!valid);
            return name;
        }
    }
}
