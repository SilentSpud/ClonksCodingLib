using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CCL
{
    /// <summary>Windows Presentation Foundation (WPF) stuff.</summary>
    public static class WPFStuff
    {

        // TODO: Check if comment is right.
        /// <summary>
        /// Class that allows a WPF <see cref="Window"/> to be on top of every opened Window. Even games that run in fullscreen mode.
        /// </summary>
        public class WindowSinker
        {
            #region DLLImports
            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

            [DllImport("user32.dll")]
            private static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

            [DllImport("user32.dll")]
            private static extern IntPtr BeginDeferWindowPos(int nNumWindows);

            [DllImport("user32.dll")]
            private static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
            #endregion

            #region Variables and Properties
            // Variables
            private static readonly IntPtr HWND_BOTTOM = new IntPtr(0);

            private const UInt32 SWP_NOSIZE = 0x0001;
            private const UInt32 SWP_NOMOVE = 0x0002;
            private const UInt32 SWP_NOACTIVATE = 0x0010;
            private const UInt32 SWP_NOZORDER = 0x0004;

            private const int WM_ACTIVATEAPP = 0x001C;
            private const int WM_ACTIVATE = 0x0006;
            private const int WM_SETFOCUS = 0x0007;
            private const int WM_WINDOWPOSCHANGING = 0x0046;

            private Window targetWindow;

            // Properties
            /// <summary>
            /// Gets the target window handle of the specified window in the constructor of the <see cref="WindowSinker"/>.
            /// </summary>
            public IntPtr TargetWindowHandle
            {
                get {
                    if (targetWindow == null)
                        return IntPtr.Zero;

                    return new WindowInteropHelper(targetWindow).Handle;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Creates a new <see cref="WindowSinker"/>.
            /// </summary>
            /// <param name="window">The target <see cref="Window"/> to sink.</param>
            /// <exception cref="ArgumentNullException">Throws when the "window" parameter is null.</exception>
            public WindowSinker(Window window)
            {
                if (window == null)
                    throw new ArgumentNullException("window");

                targetWindow = window;
            }
            #endregion

            #region Events
            private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                IntPtr Handle = (new WindowInteropHelper(targetWindow)).Handle;

                if (Handle == IntPtr.Zero)
                    return;

                HwndSource Source = HwndSource.FromHwnd(Handle);
                Source.RemoveHook(new HwndSourceHook(WndProc));
            }
            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                IntPtr Hwnd = new WindowInteropHelper(targetWindow).Handle;
                SetWindowPos(Hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

                IntPtr Handle = (new WindowInteropHelper(targetWindow)).Handle;

                HwndSource Source = HwndSource.FromHwnd(Handle);
                Source.AddHook(new HwndSourceHook(WndProc));
            }
            #endregion

            #region Methods
            public void Sink()
            {
                targetWindow.Loaded += OnLoaded;
                targetWindow.Closing += OnClosing;
            }
            public void Unsink()
            {
                targetWindow.Loaded -= OnLoaded;
                targetWindow.Closing -= OnClosing;
            }
            #endregion

            #region Functions
            private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (msg == WM_SETFOCUS)
                {
                    hWnd = new WindowInteropHelper(targetWindow).Handle;
                    SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                    handled = true;
                }
                return IntPtr.Zero;
            }
            #endregion
        }

        /// <summary>
        /// Class that allows you to create a animation for a <see cref="Brush"/> instead of the <see cref="Color"/> (<see cref="ColorAnimation"/>).
        /// </summary>
        public class BrushAnimation : AnimationTimeline
        {

            public override Type TargetPropertyType
            {
                get {
                    return typeof(Brush);
                }
            }

            public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
            {
                return GetCurrentValue(defaultOriginValue as Brush, defaultDestinationValue as Brush, animationClock);
            }
            public object GetCurrentValue(Brush defaultOriginValue, Brush defaultDestinationValue, AnimationClock animationClock)
            {
                if (!animationClock.CurrentProgress.HasValue)
                    return Brushes.Transparent;

                // Use the standard values if From and To are not set 
                // (it is the value of the given property)
                defaultOriginValue = From ?? defaultOriginValue;
                defaultDestinationValue = To ?? defaultDestinationValue;

                if (animationClock.CurrentProgress.Value == 0)
                    return defaultOriginValue;
                if (animationClock.CurrentProgress.Value == 1)
                    return defaultDestinationValue;

                return new VisualBrush(new Border() {
                    Width = 1,
                    Height = 1,
                    Background = defaultOriginValue,
                    Child = new Border() {
                        Background = defaultDestinationValue,
                        Opacity = animationClock.CurrentProgress.Value,
                    }
                });
            }

            protected override Freezable CreateInstanceCore()
            {
                return new BrushAnimation();
            }

            // We must define From and To, AnimationTimeline does not have this properties
            public Brush From
            {
                get { return (Brush)GetValue(FromProperty); }
                set { SetValue(FromProperty, value); }
            }
            public Brush To
            {
                get { return (Brush)GetValue(ToProperty); }
                set { SetValue(ToProperty, value); }
            }

            public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Brush), typeof(BrushAnimation));
            public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Brush), typeof(BrushAnimation));

        }

        /// <summary>
        /// Caches the image into memory which prevents the image file from being locked.
        /// </summary>
        /// <param name="uri">The uri that leads to the file path.</param>
        /// <param name="freeze">If the image should be frozen or not.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns the <see cref="BitmapImage"/> if successfully.</returns>
        public static AResult<BitmapImage> LoadBitmapImageToMemory(Uri uri, bool freeze = false)
        {
            try {
                BitmapImage bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.UriSource = uri;
                bmi.CacheOption = BitmapCacheOption.OnLoad;
                bmi.EndInit();
                if (freeze) bmi.Freeze();

                return new AResult<BitmapImage>(null, bmi);
            }
            catch (Exception ex) {
                return new AResult<BitmapImage>(ex, null);
            }
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> to a WPF <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="bitmap">The <see cref="System.Drawing.Bitmap"/> to convert.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns the <see cref="BitmapImage"/> if successfully.</returns>
        public static AResult<BitmapImage> BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            try {
                MemoryStream memory = new MemoryStream();
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return new AResult<BitmapImage>(null, bitmapimage);
            }
            catch (Exception ex) {
                return new AResult<BitmapImage>(ex, null);
            }
        }

        /// <summary>
        /// Creates a resized <see cref="BitmapFrame"/> from the given <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="source">Source image.</param>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        /// <param name="offset">The optional offset of the X and Y coordinate.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns the <see cref="BitmapFrame"/> if successfully.</returns>
        public static AResult<BitmapFrame> CreateResizedImage(ImageSource source, int width, int height, int offset = 0)
        {
            try
            {
                Rect rect = new Rect(offset, offset, width * 2, height);

                DrawingGroup group = new DrawingGroup();
                RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
                group.Children.Add(new ImageDrawing(source, rect));

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    drawingContext.DrawDrawing(group);

                RenderTargetBitmap resizedImage = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
                resizedImage.Render(drawingVisual);

                return new AResult<BitmapFrame>(null, BitmapFrame.Create(resizedImage));
            }
            catch (Exception ex)
            {
                return new AResult<BitmapFrame>(ex, null);
            }
        }

        // TODO: Check function and add comment. Looks kinda sus.
        public static string FormatTwitterTextForWPF(string text)
        {
            string str = text;

            if (string.IsNullOrEmpty(str))
                return str;

            // Format
            if (str.Contains("<"))  str = str.Replace("&lt;", "<");
            if (str.Contains(">"))  str = str.Replace("&gt;", ">");
            if (str.Contains("&"))  str = str.Replace("&amp;", "&");
            if (str.Contains("\"")) str = str.Replace("&quot;", "\"");
            if (str.Contains("'"))  str = str.Replace("&apos;", "'");

            return str;
        }

    }
}
