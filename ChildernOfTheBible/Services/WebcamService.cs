using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace ChildernOfTheBible.Services
{
    public class WebcamService : IDisposable
    {
        private VideoCaptureDevice? _device;
        public event Action<string>? BarcodeDetected;
        public event Action<System.Windows.Media.ImageSource>? FrameReady;

        private bool _isRunning = false;

        public List<string> GetAvailableCameras()
        {
            var cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return cameras.Cast<FilterInfo>().Select(c => c.Name).ToList();
        }

        public void Start(int cameraIndex = 0)
        {
            if (_isRunning) return;

            var cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (cameras.Count == 0) return;

            _device = new VideoCaptureDevice(cameras[cameraIndex].MonikerString);
            _device.NewFrame += OnNewFrame;
            _device.Start();
            _isRunning = true;
        }

        public void Stop()
        {
            if (_device != null && _device.IsRunning)
            {
                _device.SignalToStop();
                _device.WaitForStop();
                _device.NewFrame -= OnNewFrame;
            }
            _isRunning = false;
        }

        private DateTime _lastDetected = DateTime.MinValue;

        private void OnNewFrame(object sender, NewFrameEventArgs e)
        {
            var frame = (Bitmap)e.Frame.Clone();

            // Send frame to UI for preview
            var imageSource = BitmapToImageSource(frame);
            FrameReady?.Invoke(imageSource);

            // Throttle decoding to every 500ms to avoid CPU overload
            if ((DateTime.Now - _lastDetected).TotalMilliseconds < 500)
            {
                frame.Dispose();
                return;
            }

            // Decode barcode from frame
            var reader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            var result = reader.Decode(frame);
            frame.Dispose();

            if (result != null)
            {
                _lastDetected = DateTime.Now;
                BarcodeDetected?.Invoke(result.Text);
            }
        }

        private static System.Windows.Media.ImageSource BitmapToImageSource(Bitmap bmp)
        {
            using var ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            var bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        public void Dispose() => Stop();
    }
}