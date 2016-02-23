using System;
using System.Drawing;
using System.Threading.Tasks;
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

        private async void scaleButton_Click(object sender, RoutedEventArgs e) {
            try {
                scaleButton.IsEnabled = false;
                button.IsEnabled = false;
                var coef = Convert.ToDouble(coefTextBox.Text);
                await Task.Run(() => {
                    img = new DIP.Scaling(img, coef).Image;
                    Dispatcher.Invoke((Action)(() => {
                        image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            img.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromWidthAndHeight(img.Width, img.Height)
                        );
                        scaleButton.IsEnabled = true;
                        button.IsEnabled = true;
                    }));
                    img.Save("e:\\SAVED.bmp");
                });
            } catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}");
                scaleButton.IsEnabled = true;
                button.IsEnabled = true;
            }
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
