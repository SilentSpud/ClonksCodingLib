using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace CCL {
    /// <summary>
    /// Windows Presentation Foundation (WPF) stuff.
    /// </summary>
    public static class WPFStuff {

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
                using (MemoryStream memory = new MemoryStream()) {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return new AResult<BitmapImage>(null, bitmapimage);
                }
            }
            catch (Exception ex) {
                return new AResult<BitmapImage>(ex, null);
            }
        }

    }
}
