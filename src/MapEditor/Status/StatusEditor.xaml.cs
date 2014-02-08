using System;
using System.Collections.Generic;
using System.Windows;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for StatusEditor.xaml
    /// </summary>
    public partial class StatusEditor : Window
    {
        /// <summary>
        /// Status being edited
        /// </summary>
        Status status;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusName">name of status to be edited</param>
        public StatusEditor(string statusName)
        {
            status = StatusData.GetStatus(statusName);
            InitializeComponent();
            nameBox.Text = statusName;
            slowDurationBox.Text = status.slowDuration.ToString();
            slowMultiplierBox.Text = status.slowMultiplier.ToString();
            slowBonusBox.Text = status.slowBonus.ToString();
            damageDurationBox.Text = status.extraDamageDuration.ToString();
            damageMultiplierBox.Text = status.extraDamageMultiplier.ToString();
            damageBonusBox.Text = status.extraDamageBonus.ToString();
            dotDurationBox.Text = status.dotDuration.ToString();
            dotFrameBox.Text = status.dotFrameDamage.ToString();
            dotMoveBox.Text = status.dotMoveDamage.ToString();
            stunDurationBox.Text = status.stunDuration.ToString();
            fearChanceBox.Text = status.fearChance.ToString();
            fearDurationBox.Text = status.fearDuration.ToString();
        }

        /// <summary>
        /// Updates the status
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (string s in StatusData.GetStatusNames())
                    if (s.Equals(nameBox.Text) && !s.Equals(status.name))
                    {
                        MessageBox.Show("That name is already in use.");
                        return;
                    }
                if (nameBox.Text.Equals("None"))
                {
                    MessageBox.Show("The status name cannot be \"None\".");
                    return;
                }
                if (nameBox.Text.Contains("}") || nameBox.Text.Contains("{") || nameBox.Text.Contains(";") || nameBox.Text.Contains("[") || nameBox.Text.Contains("]") || nameBox.Text.Contains(",") || nameBox.Text.Contains(":"))
                {
                    MessageBox.Show("You must not use the following characters: { } : ; , [ ]");
                    return;
                }
                status.name = nameBox.Text;
                status.slowDuration = int.Parse(slowDurationBox.Text);
                status.slowMultiplier = int.Parse(slowMultiplierBox.Text);
                status.slowBonus = int.Parse(slowBonusBox.Text);
                status.extraDamageDuration = int.Parse(damageDurationBox.Text);
                status.extraDamageMultiplier = int.Parse(damageMultiplierBox.Text);
                status.extraDamageBonus = int.Parse(damageBonusBox.Text);
                status.dotDuration = int.Parse(dotDurationBox.Text);
                status.dotFrameDamage = int.Parse(dotFrameBox.Text);
                status.dotMoveDamage = int.Parse(dotMoveBox.Text);
                status.stunDuration = int.Parse(stunDurationBox.Text);
                status.fearChance = int.Parse(fearChanceBox.Text);
                status.fearDuration = int.Parse(fearDurationBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid field(s).");
                return;
            }
            StatusData.UpdateStatus(status);
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
    }
}
