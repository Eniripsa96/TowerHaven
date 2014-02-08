using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for EnemyEditor.xaml
    /// </summary>
    public partial class EnemyEditor : Window, IAddable
    {
        /// <summary>
        /// Enemy being edited
        /// </summary>
        Enemy enemy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enemyName">name of enemy</param>
        public EnemyEditor(string enemyName)
        {
            InitializeComponent();

            EnemyManager enemy = new EnemyManager(this);
            enemy.Draw(enemyName, 0);

            this.enemy = EnemyData.GetEnemy(enemyName);

            healthBox.Text = this.enemy.Health.ToString();
            speedBox.Text = this.enemy.speed.ToString();
            damageBox.Text = this.enemy.damage.ToString();
            rewardBox.Text = this.enemy.Reward.ToString();
            flyingComboBox.SelectedIndex = this.enemy.flying ? 0 : 1;
        }

        /// <summary>
        /// Adds the canvas to the window
        /// </summary>
        /// <param name="canvas"></param>
        public void Add(Canvas canvas)
        {
            canvas.Margin = new Thickness(5, 5, 0, 0);
            ContentRoot.Children.Add(canvas);
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
        /// Updates the enemy data
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                enemy.Health = int.Parse(healthBox.Text);
                enemy.speed = int.Parse(speedBox.Text);
                enemy.damage = int.Parse(damageBox.Text);
                enemy.Reward = int.Parse(rewardBox.Text);
            }
            catch (Exception) 
            {
                MessageBox.Show("Invalid field(s).");
                return;
            }
            if (enemy.Health < 1)
            {
                MessageBox.Show("Health has to be a positive integer.");
                return;
            }
            enemy.flying = flyingComboBox.SelectedIndex == 0 ? true : false;
            EnemyData.UpdateEnemy(enemy);
            Close();
        }

        /// <summary>
        /// Opens the immunity menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void immunityButton_Click(object sender, RoutedEventArgs e)
        {
            ImmunitySelector selector = new ImmunitySelector(enemy);
            selector.Owner = this;
            Hide();
            selector.ShowDialog();
            Show();
        }
    }
}
