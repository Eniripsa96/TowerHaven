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
using Microsoft.Win32;

namespace WPFEditor
{
    /// <summary>
    /// Interaction logic for NewMapFromImage.xaml
    /// </summary>
    public partial class NewMapFromImage : Window
    {
        bool valid;

        public bool Valid
        {
            get { return valid; }
        }

        public NewMapFromImage()
        {
            InitializeComponent();
            valid = false;
        }

        private void selectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG Files (.png)|*.png";
            if (dialog.ShowDialog() == true)
            {
                imagePath.Text = dialog.FileName;
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            valid = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
