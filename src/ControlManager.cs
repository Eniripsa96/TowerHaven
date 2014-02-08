using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TowerHaven
{
    /// <summary>
    /// Manages the dynamic creation of controls
    /// </summary>
    class ControlManager
    {
        /// <summary>
        /// White SoldColorBrush to apply to controls
        /// </summary>
        private static readonly SolidColorBrush whiteBrush = new SolidColorBrush(Color.FromRgb((byte)255, (byte)255, (byte)255));

        /// <summary>
        /// Blue SolidColorBrush to apply to controls
        /// </summary>
        private static readonly SolidColorBrush blueBrush = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0, (byte)255));

        /// <summary>
        /// Creates a canvas with the given parameters
        /// The canvas is aligned to the top left and is square
        /// </summary>
        /// <param name="path">image path</param>
        /// <param name="x">left margin</param>
        /// <param name="y">top margin</param>
        /// <param name="size">size of one side</param>
        /// <returns>canvas</returns>
        public static Canvas CreateCanvas(string path, int x, int y, int size)
        {
            Canvas canvas = new Canvas();

            canvas.Margin = new Thickness(x, y, 0, 0);
            canvas.Width = size;
            canvas.Height = size;

            BitmapImage image = LoadBitmap(path);
            canvas.Background = new ImageBrush(image);

            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            canvas.VerticalAlignment = VerticalAlignment.Top;
            
            return canvas;
        }

        /// <summary>
        /// Loads a bitmap from a PNG image at the given path
        /// </summary>
        /// <param name="path">PNG image path</param>
        /// <returns>bitmap</returns>
        public static BitmapImage LoadBitmap(string path)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path + ".png");
            image.EndInit();
            return image;
        }

        /// <summary>
        /// Creates a label
        /// </summary>
        /// <param name="text">content</param>
        /// <param name="x">left margin</param>
        /// <param name="y">top margin</param>
        /// <returns></returns>
        public static Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label();

            label.Margin = new Thickness(x, y, 0, 0);
            
            label.Foreground = whiteBrush;

            label.FontFamily = new FontFamily("Times New Roman");
            label.FontSize = 14;

            label.Content = text;

            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;

            return label;
        }

        /// <summary>
        /// Creates a check box
        /// </summary>
        /// <param name="text">content</param>
        /// <param name="x">left margin</param>
        /// <param name="y">top margin</param>
        /// <returns></returns>
        public static CheckBox CreateCheckBox(string text, int x, int y)
        {
            CheckBox checkBox = new CheckBox();

            checkBox.Margin = new Thickness(x, y, 0, 0);

            checkBox.Foreground = whiteBrush;

            checkBox.FontFamily = new FontFamily("Times New Roman");
            checkBox.FontSize = 14;
            
            checkBox.Content = text;

            checkBox.HorizontalAlignment = HorizontalAlignment.Left;
            checkBox.VerticalAlignment = VerticalAlignment.Top;

            checkBox.IsChecked = false;
            
            return checkBox;
        }

        /// <summary>
        /// Creates a button
        /// </summary>
        /// <param name="text">content</param>
        /// <param name="x">left margin</param>
        /// <param name="y">top margin</param>
        /// <param name="z">right margin</param>
        /// <returns></returns>
        public static Button CreateButton(string text, int x, int y, int z)
        {
            Button button = new Button();

            button.Margin = new Thickness(x, y, z, 0);

            button.Background = blueBrush;
            button.Foreground = whiteBrush;
            button.BorderBrush = whiteBrush;

            button.FontFamily = new FontFamily("Times New Roman");
            button.FontSize = 14;

            button.Content = text;
            
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.VerticalAlignment = VerticalAlignment.Top;

            button.Focusable = false;
            
            return button;
        }

        /// <summary>
        /// Creates a palette item
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>label</returns>
        public static Label CreatePaletteItem(string name)
        {
            Label label = new Label();

            label.FontFamily = new FontFamily("Times New Roman");
            label.FontSize = 14;

            label.Content = name;

            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;

            return label;
        }
    }
}
