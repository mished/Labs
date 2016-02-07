using System;
using System.Drawing;
using System.IO;

namespace DIP {

    public class Bmp4Image {

        private BinaryReader buffer;
        private Bitmap       pixelData;

        public  char[]       Type             { get; private set; }
        public  int          FileSize         { get; private set; }
        public  short        Reserved1        { get; private set; }
        public  short        Reserved2        { get; private set; }
        public  int          Offset           { get; private set; }
        public  int          HeaderSize       { get; private set; }
        public  int          Width            { get; private set; }
        public  int          Height           { get; private set; }
        public  short        Planes           { get; private set; }
        public  short        BitCount         { get; private set; }
        public  int          Compression      { get; private set; }
        public  int          SizeImage        { get; private set; }
        public  int          XPelsPerMeter    { get; private set; }
        public  int          YPelsPerMeter    { get; private set; }
        public  int          ClrUsed          { get; private set; }
        public  int          ClrImportant     { get; private set; }
        public  Color[]      ColorPalette     { get; private set; }

        public Bitmap PixelData => (Bitmap)pixelData.Clone();

        public Bmp4Image(Stream stream) {
            using (buffer = new BinaryReader(stream)) {
                ReadImage();
            }
        }

        public Bmp4Image(string fileName) {
            using (var s = new FileStream(fileName, FileMode.Open))
            using (buffer = new BinaryReader(s)) {
                ReadImage();
            }
        }

        private void ReadImage() {
            Type = buffer.ReadChars(2);
            if (new string(Type) != "BM")
                throw new FormatException("Invalid BMP file.");
            ReadHeaders();
            ReadColorPalette();
            ReadPixelData();
        }

        private void ReadHeaders() {
            FileSize      = buffer.ReadInt32();
            Reserved1     = buffer.ReadInt16();
            Reserved2     = buffer.ReadInt16();
            Offset        = buffer.ReadInt32();
            HeaderSize    = buffer.ReadInt32();
            Width         = buffer.ReadInt32();
            Height        = buffer.ReadInt32();
            Planes        = buffer.ReadInt16();
            BitCount      = buffer.ReadInt16();
            if (BitCount != 4)
                throw new FormatException("Only 4-bit bmp image supported.");
            Compression   = buffer.ReadInt32();
            SizeImage     = buffer.ReadInt32();
            XPelsPerMeter = buffer.ReadInt32();
            YPelsPerMeter = buffer.ReadInt32();
            ClrUsed       = buffer.ReadInt32();
            ClrImportant  = buffer.ReadInt32();
        }

        private void ReadColorPalette() {
            ColorPalette = new Color[(Offset - buffer.BaseStream.Position) / 4];

            for (var i = 0; buffer.BaseStream.Position != Offset; i++) {
                var b = buffer.ReadByte();
                var g = buffer.ReadByte();
                var r = buffer.ReadByte();
                buffer.ReadByte();
                ColorPalette[i] = Color.FromArgb(r, g, b);
            }
        }

        private void ReadPixelData() {
            pixelData = new Bitmap(Width, Height);
            var bytesWidth = Math.Ceiling((double)Width * 4 / 8);

            var padding = 4 - ((int)bytesWidth % 4);
            if (padding == 4) padding = 0;

            for (var i = Height - 1; i >= 0; i--) {
                var x = 0;
                for (var j = 0; j < bytesWidth; j++) {
                    var cur = buffer.ReadByte();
                    pixelData.SetPixel(x, i, ColorPalette[cur.H()]);
                    x += 1;
                    if (x >= Width) break;
                    pixelData.SetPixel(x, i, ColorPalette[cur.L()]);
                    x += 1;
                }
                buffer.ReadBytes(padding);
            }
        }
    }

    public static class BmpUtils {

        public static int H(this byte val) {
            return val >> 4;
        }

        public static int L(this byte val) {
            return val & 0x0F;
        }
    }
}
