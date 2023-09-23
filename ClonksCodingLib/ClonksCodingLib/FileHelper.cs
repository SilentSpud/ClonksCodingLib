using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CCL
{
    public static class FileHelper
    {

        public struct AFileSize
        {
            #region Properties
            public long Size { get; private set; }
            public string SizeCharacter { get; private set; }
            #endregion

            #region Constructor
            public AFileSize(long size, string sizeCharacter)
            {
                Size = size;
                SizeCharacter = sizeCharacter;
            }
            #endregion

            public override string ToString()
            {
                return string.Format("{0} {1}", Size.ToString(), SizeCharacter.ToString());
            }
        }

        /// <summary>
        /// An array of all file sizes.
        /// Longs run out around EB.
        /// </summary>
        public static readonly string[] FileSizesStrArr = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        /// <summary>
        /// Gets the byte array of the given stream.
        /// </summary>
        /// <param name="input">The stream to get the byte array of.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns a <see cref="byte"/> array if successful.</returns>
        public static AResult<byte[]> GetByteArray(Stream input)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    input.CopyTo(ms);
                    return new AResult<byte[]>(null, ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                return new AResult<byte[]>(ex, null);
            }
        }

        /// <summary>
        /// Gets the file version of the given file.
        /// </summary>
        /// <param name="fileName">The file to get the file version of.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns a <see cref="string"/> if successful.</returns>
        public static AResult<string> GetFileVersion(string fileName)
        {
            try
            {
                string version = FileVersionInfo.GetVersionInfo(fileName).FileVersion;
                if (!string.IsNullOrEmpty(version))
                    return new AResult<string>(null, version.Replace(",", "."));

                return new AResult<string>(null, string.Empty);
            }
            catch (Exception ex)
            {
                return new AResult<string>(ex, null);
            }
        }

        /// <summary>
        /// Creates an MD5 Hash string from the given directory.
        /// </summary>
        /// <param name="folder">The directory the hash should be created from.</param>
        /// <param name="ignoredFiles">
        /// Specify files that should be ignored while creating the MD5 Hash.<br/>
        /// The file name will be <b>lowered</b> while checking, so you should add file names to this list that are <b>lowercase</b>.
        /// </param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns an MD5 Hash <see cref="string"/> if successful.</returns>
        public static AResult<string> GetMD5StringFromFolder(string folder, List<string> ignoredFiles = null)
        {
            try
            {
                List<string> files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly).OrderBy(p => p).ToList();
                using (MD5 md5 = MD5.Create())
                {

                    // Generate hash from all files in directory
                    for (int i = 0; i < files.Count; i++)
                    {
                        string file = files[i];

                        // There are files to be ignored
                        if (ignoredFiles != null)
                        {
                            if (ignoredFiles.Contains(Path.GetFileName(file).ToLower()))
                                continue;
                        }

                        // Hash path
                        string realtivePath = file.Substring(folder.Length + 1);
                        byte[] pathBytes = Encoding.UTF8.GetBytes(realtivePath.ToLower());
                        md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                        // Hash contents
                        byte[] contentBytes = File.ReadAllBytes(file);
                        if (contentBytes == null) return new AResult<string>(new ArgumentNullException("contentBytes was null."), null);

                        if (i == (files.Count - 1))
                            md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                        else
                            md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);

                    }

                    return new AResult<string>(null, BitConverter.ToString(md5.Hash).Replace("-", "").ToLower());
                }
            }
            catch (Exception ex)
            {
                return new AResult<string>(ex, null);
            }
        }

        /// <summary>
        /// Creates an MD5 Hash string from the given file.
        /// </summary>
        /// <param name="file">The file the hash should be created from.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns an MD5 Hash <see cref="string"/> if successful.</returns>
        public static AResult<string> GetMD5StringFromFile(string file)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if (contentBytes == null) return new AResult<string>(new ArgumentNullException("contentBytes was null."), null);

                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);

                    return new AResult<string>(null, BitConverter.ToString(md5.Hash).Replace("-", "").ToLower());
                }
            }
            catch (Exception ex)
            {
                return new AResult<string>(ex, null);
            }
        }

        /// <summary>
        /// Gets the exact file size string from the given byte count.
        /// </summary>
        /// <param name="byteCount">The byte count to get the exact file size of.</param>
        /// <returns>The exact file size.</returns>
        public static string GetExactFileSize(long byteCount)
        {
            if (byteCount == 0)
                return "0 B";

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return string.Format("{0} {1}", (Math.Sign(byteCount) * num).ToString(), FileSizesStrArr[place]);
        }

        /// <summary>
        /// Gets the exact file size from the given byte count.
        /// </summary>
        /// <param name="byteCount">The byte count to get the exact file size of.</param>
        /// <returns>An object containing the file size with its corresponding character.</returns>
        public static AFileSize GetExactFileSizeAdvanced(long byteCount)
        {
            if (byteCount == 0)
                return new AFileSize(0, "B");

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return new AFileSize((long)(Math.Sign(byteCount) * num), FileSizesStrArr[place]);
        }

        /// <summary>
        /// Opens the Windows File Explorer and selects the target File.
        /// </summary>
        /// <param name="filePath"></param>
        public static void OpenDirectoryAndSelectTargetFile(string filePath)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.Arguments = string.Format("/e, /select, \"{0}\"", filePath);
            Process.Start(info);
        }

    }
}
