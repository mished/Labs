using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Scaling.UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        Bitmap img;

        private void scaleButton_Click(object sender, RoutedEventArgs e) {
            img = new DIP.Scaling(img, Convert.ToDouble(coefTextBox.Text)).ScaleImage();
            //image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            //        img.GetHbitmap(),
            //        IntPtr.Zero,
            //        Int32Rect.Empty,
            //        BitmapSizeOptions.FromWidthAndHeight(img.Width, img.Height)
            //    );

            img.Save("e:\\SAVED.bmp");
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            var dial = new OpenFileDialog();
            if ((bool)dial.ShowDialog()) {
                img = new Bitmap(dial.FileName);
                image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    img.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(img.Width, img.Height)
                );
            }
        }
    }
}
