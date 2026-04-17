// Services/BarcodeService.cs
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace ChildernOfTheBible.Services
{
    public class BarcodeService
    {
        // Generate a Code-128 barcode as a WPF BitmapImage
        public BitmapImage GenerateBarcode(string content, int width = 300, int height = 80)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 4
                }
            };

            using var bmp = writer.Write(content);
            return BitmapToBitmapImage(bmp);
        }

        // Decode a barcode from a Bitmap (webcam frames or loaded images)
        public string? DecodeBarcode(Bitmap frame)
        {
            var reader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            var result = reader.Decode(frame);
            return result?.Text;
        }

        private static BitmapImage BitmapToBitmapImage(Bitmap bmp)
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            bi.Freeze(); // thread-safe for WPF binding
            return bi;
        }
    }
}