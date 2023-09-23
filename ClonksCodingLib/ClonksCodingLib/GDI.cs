using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace CCL
{
    /// <summary>
    /// Graphics stuff.
    /// </summary>
    public static class GDI
    {

        /// <summary>
        /// Takes a screenshot of the screen and saves the file.
        /// </summary>
        /// <param name="saveTo">The path where the screenshot will be saved to.</param>
        /// <param name="offsetX">The left/right offset of the area the screenshot will be created from.</param>
        /// <param name="offsetY">The top/down offset of the area the screenshot will be created from.</param>
        /// <param name="width">The width of the screenshot.</param>
        /// <param name="height">The height of the screenshot.</param>
        /// <param name="imageFormat">The <see cref="ImageFormat"/> the screenshot should be saved as.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns true if the screenshot was taken and saved successfully.</returns>
        public static AResult<bool> TakeScreenshot(string saveTo, int offsetX, int offsetY, int width, int height, ImageFormat imageFormat)
        {
            try
            {
                // Take screenshot
                using (Bitmap b = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.CopyFromScreen(offsetX, offsetY, 0, 0, b.Size);
                    }

                    b.Save(saveTo, imageFormat);
                }

                // Check if screenshot was saved
                if (File.Exists(saveTo))
                    return new AResult<bool>(null, true);
                else
                    return new AResult<bool>(null, false);
            }
            catch (Exception ex)
            {
                return new AResult<bool>(ex, false);
            }
        }
        /// <summary>
        /// Takes a screenshot of the screen and saves the file.
        /// </summary>
        /// <param name="saveTo">The path where the screenshot will be saved to.</param>
        /// <param name="width">The width of the screenshot.</param>
        /// <param name="height">The height of the screenshot.</param>
        /// <param name="imageFormat">The <see cref="ImageFormat"/> the screenshot should be saved as.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns true if the screenshot was taken and saved successfully.</returns>
        public static AResult<bool> TakeScreenshot(string saveTo, int width, int height, ImageFormat imageFormat)
        {
            try
            {
                // Take screenshot
                using (Bitmap b = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, b.Size);
                    }

                    b.Save(saveTo, imageFormat);
                }

                // Check if screenshot was saved
                if (File.Exists(saveTo))
                    return new AResult<bool>(null, true);
                else
                    return new AResult<bool>(null, false);
            }
            catch (Exception ex)
            {
                return new AResult<bool>(ex, false);
            }
        }

        /// <summary>
        /// Takes a screenshot of the screen and return data.
        /// </summary>
        /// <param name="offsetX">The left/right offset of the area the screenshot will be created from.</param>
        /// <param name="offsetY">The top/down offset of the area the screenshot will be created from.</param>
        /// <param name="width">The width of the screenshot.</param>
        /// <param name="height">The height of the screenshot.</param>
        /// <param name="imageFormat">The <see cref="ImageFormat"/> the screenshot should be saved as.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns a <see cref="MemoryStream"/> object if the screenshot was taken.</returns>
        public static AResult<MemoryStream> TakeScreenshot(int offsetX, int offsetY, int width, int height, ImageFormat imageFormat)
        {
            try
            {
                // Take screenshot
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap b = new Bitmap(width, height))
                    {
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.CopyFromScreen(offsetX, offsetY, 0, 0, b.Size);
                        }

                        b.Save(ms, imageFormat);
                    }

                    return new AResult<MemoryStream>(null, ms);
                }
            }
            catch (Exception ex)
            {
                return new AResult<MemoryStream>(ex, null);
            }
        }
        /// <summary>
        /// Takes a screenshot of the screen and return data.
        /// </summary>
        /// <param name="width">The width of the screenshot.</param>
        /// <param name="height">The height of the screenshot.</param>
        /// <param name="imageFormat">The <see cref="ImageFormat"/> the screenshot should be saved as.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns a <see cref="MemoryStream"/> object if the screenshot was taken.</returns>
        public static AResult<MemoryStream> TakeScreenshot(int width, int height, ImageFormat imageFormat)
        {
            try
            {
                // Take screenshot
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap b = new Bitmap(width, height))
                    {
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.CopyFromScreen(0, 0, 0, 0, b.Size);
                        }

                        b.Save(ms, imageFormat);
                    }

                    return new AResult<MemoryStream>(null, ms);
                }
            }
            catch (Exception ex)
            {
                return new AResult<MemoryStream>(ex, null);
            }
        }

    }
}
