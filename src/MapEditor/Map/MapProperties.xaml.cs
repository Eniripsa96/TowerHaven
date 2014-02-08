using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for MapProperties.xaml
    /// </summary>
    public partial class MapProperties : Window
    {
        /// <summary>
        /// List of towers included in the map
        /// </summary>
        private List<string> towers;

        /// <summary>
        /// Map name
        /// </summary>
        private string name;

        /// <summary>
        /// Map starting money
        /// </summary>
        private int money;

        /// <summary>
        /// Map starting health
        /// </summary>
        private int health;

        /// <summary>
        /// Map name property
        /// </summary>
        public string MapName
        {
            get { return name; }
        }

        /// <summary>
        /// Map money property
        /// </summary>
        public int Money
        {
            get { return money; }
        }

        /// <summary>
        /// Map health property
        /// </summary>
        public int Health
        {
            get { return health; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="towers">included towers</param>
        /// <param name="name">map name</param>
        /// <param name="health">starting health</param>
        /// <param name="money">starting money</param>
        public MapProperties(List<string> towers, string name, int health, int money)
        {
            InitializeComponent();
            this.towers = towers;
            nameBox.Text = name;
            healthBox.Text = health.ToString();
            moneyBox.Text = money.ToString();
            this.name = name;
            this.health = health;
            this.money = money;
        }

        /// <summary>
        /// Updates the map properties
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                money = int.Parse(moneyBox.Text);
                health = int.Parse(healthBox.Text);
                name = nameBox.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Health and money must be non-negative integers");
                return;
            }
            if (health < 1)
            {
                MessageBox.Show("Health must be positive.");
                return;
            }
            if (money < 0)
            {
                MessageBox.Show("Money must be non-negative");
                return;
            }
            if (MapData.MapExists(name))
            {
                MessageBox.Show("That map name is already taken.");
                return;
            }
            Close();
        }

        /// <summary>
        /// Opens the tower selection menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void towerButton_Click(object sender, RoutedEventArgs e)
        {
            if (TowerData.GetTowerNames().Length == 0)
            {
                MessageBox.Show("There are no towers to include.");
                return;
            }
            IncludeTowerSelection selector = new IncludeTowerSelection(towers);
            selector.Owner = this;
            selector.ShowDialog();
        }

        /// <summary>
        /// Opens th map details editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            MapDetails details = new MapDetails();
            details.Owner = this;
            details.Display();
        }
    }
}
