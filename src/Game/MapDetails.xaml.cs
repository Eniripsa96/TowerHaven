using System.Windows;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for MapDetails.xaml
    /// </summary>
    public partial class MapDetails : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">level data</param>
        public MapDetails(BasicLevel level)
        {
            InitializeComponent();
            creatorLabel.Content = level.Creator;
            dateLabel.Content = level.Date;
            descriptionLabel.Text = level.Description;
        }

        /// <summary>
        /// Closes the menu when the button is pressed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
