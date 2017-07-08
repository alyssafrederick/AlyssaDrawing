using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace alyssadrawing
{ 

    public partial class MainWindow : Window
    {
        bool mousedown = false;
        Brush color = Brushes.White;
        float thickness = 1;
        Polyline polyLine;
        PointCollection points;
        bool delete = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousedown = true;
            points = new PointCollection();
            points.Add(e.GetPosition(canvas));
            polyLine = new Polyline();
            polyLine.MouseDown += (s, arg) =>
            {
                Polyline line = (Polyline)s;
                if (e.RightButton == MouseButtonState.Pressed && delete)
                {
                    canvas.Children.Remove(line);
                }
                else if(e.RightButton == MouseButtonState.Pressed)
                {
                    line.Stroke = color;
                }
            };
            polyLine.Stroke = color;
            polyLine.StrokeThickness = thickness;
            polyLine.Fill = Brushes.Transparent;
            polyLine.Points = points;
            canvas.Children.Add(polyLine);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown == true)
            {
                points.Add(e.GetPosition(canvas));
                polyLine.Points = points;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mousedown = false;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                delete = true;
            }
            if (e.Key == Key.X)
            {
                canvas.Children.Clear();
            }
            if (e.Key == Key.S)
            {
                saveImage();
            }

            //opening up an image does not work rip 2k16
            if (e.Key == Key.O)
            {
                openImage();
            }
        }


        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                delete = false;
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            color = new SolidColorBrush(ClrPcker_Background.SelectedColor.Value);
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            thickness++;
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (thickness > 1)
            {
                thickness--;
            }
        }

        #region Image Saving and Loading functionality

        /// <summary>
        /// Saves current image to disk
        /// </summary>
        private void saveImage()
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Image";
            dialog.DefaultExt = ".png";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;

                RenderTargetBitmap renderer = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96, 96, PixelFormats.Default);
                renderer.Render(canvas);

                BitmapEncoder bitMapEncoder = new PngBitmapEncoder();
                bitMapEncoder.Frames.Add(BitmapFrame.Create(renderer));

                using (var stream = new System.IO.FileStream(filename, System.IO.FileMode.Create))
                {
                    bitMapEncoder.Save(stream);
                }
            }
        }

        /// <summary>
        /// Loads an image from disk. Current image will be discarded.
        /// </summary>
        private void openImage()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".png";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                string fileName = dialog.FileName;

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(fileName));
                canvas.Background = imageBrush;
            }
        }

        #endregion Image Saving and Loading functionality

    }
}
