using System;
using System.Windows;
using System.Windows.Media.Imaging;
using DIP;
using Microsoft.Win32;

namespace BmpReader.UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            try {
                var dial = new OpenFileDialog();
                if ((bool)dial.ShowDialog()) {
                    var img = new Bmp4Image(dial.FileName);
                    image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        img.PixelData.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromWidthAndHeight(img.Width, img.Height)
                    );
                    textBox.Clear();
                    textBox.AppendText($"\nFile size: {img.FileSize}\n");
                    textBox.AppendText($"Bit count: {img.BitCount}\n");
                    textBox.AppendText($"Compression: {img.Compression}\n");
                    textBox.AppendText($"Header size: {img.HeaderSize}\n");
                    textBox.AppendText($"Height: {img.Height}\n");
                    textBox.AppendText($"Width: {img.Width}\n");
                    textBox.AppendText($"Color palette:\n");
                    foreach (var c in img.ColorPalette) {
                        textBox.AppendText($"- {c.ToString()}\n");
                    }
                }
            } catch(Exception ex) {
                textBox.AppendText($"Error: {ex.Message}");
            }          
        }
    }
}
