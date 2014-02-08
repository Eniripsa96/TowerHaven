using System;
using System.Windows;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for WaveEnemyEditor.xaml
    /// </summary>
    public partial class WaveEnemyEditor : Window
    {
        /// <summary>
        /// Enemy being edited
        /// </summary>
        EnemyInstance enemy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e">enemy to edit</param>
        public WaveEnemyEditor(EnemyInstance e)
        {
            enemy = e;
            InitializeComponent();
            foreach (string s in EnemyData.GetEnemyNames())
            {
                enemyComboBox.Items.Add(s);
                if (s.Equals(e.name))
                    enemyComboBox.SelectedItem = s;
            }
            quantityBox.Text = e.quantity.ToString();
            delayBox.Text = e.delay.ToString();
        }

        /// <summary>
        /// Updates enemy data
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                enemy.name = enemyComboBox.SelectedItem.ToString();
                enemy.quantity = int.Parse(quantityBox.Text);
                enemy.delay = int.Parse(delayBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid field(s).");
                return;
            }
            if (enemy.quantity < 0)
            {
                MessageBox.Show("Quantity must be positive.");
                return;
            }
            Close();
        }
    }
}
