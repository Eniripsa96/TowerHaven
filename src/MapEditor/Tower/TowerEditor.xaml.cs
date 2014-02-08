using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for TowerEditor.xaml
    /// </summary>
    public partial class TowerEditor : Window, IAddable
    {
        /// <summary>
        /// Tower being edited
        /// </summary>
        Tower tower;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="towerName">tower name</param>
        public TowerEditor(string towerName)
        {
            InitializeComponent();

            // Draw the tower
            TowerManager tower = new TowerManager(this);
            tower.Draw(towerName, 0);

            // Set fields
            this.tower = TowerData.GetTower(towerName);
            this.tower.name = towerName;

            // Set labels' text
            costBox.Text = this.tower.buildCost.ToString();
            valueBox.Text = this.tower.sellValue.ToString();
            damageBox.Text = this.tower.damage.ToString();
            rangeBox.Text = this.tower.range.ToString();
            delayBox.Text = this.tower.delay.ToString();
            aoeRadiusBox.Text = this.tower.aoeRadius.ToString();
            aoeDamageBox.Text = this.tower.aoeDamage.ToString();
            aoeStatusComboBox.SelectedIndex = this.tower.aoeStatus ? 0 : 1;
            moneyPerAttackBox.Text = this.tower.moneyPerAttack.ToString();
            moneyPerKillBox.Text = this.tower.moneyPerKill.ToString();
            moneyPerTickBox.Text = this.tower.moneyPerTick.ToString();
            moneyPerWaveBox.Text = this.tower.moneyPerWave.ToString();

            // Setup the status combo box
            statusComboBox.Items.Add("None");
            statusComboBox.SelectedIndex = 0;
            string[] statusNames = StatusData.GetStatusNames();
            foreach (string s in statusNames)
            {
                statusComboBox.Items.Add(s);
                if (this.tower.status.Equals(s))
                    statusComboBox.SelectedItem = s;
            }

            // Setup the upgrade combo box
            upgradeComboBox.Items.Add("<None>");
            upgradeComboBox.SelectedIndex = 0;
            foreach (string s in TowerData.GetTowerNames())
                if (!s.Equals(this.tower.name))
                {
                    upgradeComboBox.Items.Add(s);
                    if (this.tower.baseTower.Equals(s))
                        upgradeComboBox.SelectedItem = s;
                }
        }

        /// <summary>
        /// Adds a canvas to the window
        /// </summary>
        /// <param name="canvas">canvas to add</param>
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
        /// Applies the tower data
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tower.buildCost = int.Parse(costBox.Text);
                tower.damage = int.Parse(damageBox.Text);
                tower.delay = int.Parse(delayBox.Text);
                tower.range = int.Parse(rangeBox.Text);
                tower.sellValue = int.Parse(valueBox.Text);
                tower.aoeRadius = int.Parse(aoeRadiusBox.Text);
                tower.aoeDamage = int.Parse(aoeDamageBox.Text);
                tower.aoeStatus = aoeStatusComboBox.SelectedIndex == 0 ? true : false;
                tower.status = statusComboBox.SelectedItem.ToString();
                tower.baseTower = upgradeComboBox.SelectedItem.ToString();
                tower.moneyPerAttack = int.Parse(moneyPerAttackBox.Text);
                tower.moneyPerKill = int.Parse(moneyPerKillBox.Text);
                tower.moneyPerTick = int.Parse(moneyPerTickBox.Text);
                tower.moneyPerWave = int.Parse(moneyPerWaveBox.Text);
            }
            catch (Exception) 
            {
                MessageBox.Show("Invalid field(s).");
                return;
            }
            if (tower.range < 1)
            {
                MessageBox.Show("Range has to be a positive integer.");
                return;
            }
            TowerData.UpdateTower(tower);
            Close();
        }
    }
}
