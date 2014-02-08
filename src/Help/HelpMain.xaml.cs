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

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for HelpMain.xaml
    /// </summary>
    public partial class HelpMain : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HelpMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// OnClosed override
        /// shows the main menu before closing
        /// </summary>
        /// <param name="e">not used</param>
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();
            base.OnClosed(e);
        }
    }
}
