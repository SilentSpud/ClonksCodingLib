using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace CCL
{
    /// <summary>Web related stuff.</summary>
    public static class Web
    {

        public static AResult<bool> CheckForInternetConnection(int timeoutMs = 10000, string urlOverride = null)
        {
            try {
                if (string.IsNullOrWhiteSpace(urlOverride)) {
                    switch (CultureInfo.CurrentUICulture.Name) {
                        case "fa-IR": // Iran
                            urlOverride = "http://www.aparat.com";
                            break;
                        case "zh-CN": // China
                            urlOverride = "http://www.baidu.com";
                            break;
                        default:
                            urlOverride = "http://www.gstatic.com/generate_204";
                            break;
                    }
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlOverride);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    return new AResult<bool>(null, true);
            }
            catch (Exception ex) {
                return new AResult<bool>(ex, false);
            }
        }

        public static AResult<string> GetYouTubeVideoIDFromURL(string url)
        {
            try {
                if (!string.IsNullOrWhiteSpace(url)) {
                    string id = url;

                    // Check URL format
                    if (id.Contains("youtube.com") && id.Contains("embed")) {   // Example: https://www.youtube.com/embed/M80K51DosFo?rel=0&autoplay=0&controls=1&fs=0&iv_load_policy=3
                        id = id.Split('/')[4];

                        if (id.Contains('?')) { // Contains parameters
                            return new AResult<string>(null, id.Split('?')[0]);
                        }
                        else {
                            return new AResult<string>(null, id);
                        }
                    }
                    if (id.Contains("youtube.com") && id.Contains("watch?v="))  // Example: https://www.youtube.com/watch?v=M80K51DosFo
                        return new AResult<string>(null, id.Split('=')[1]);
                    if (id.Contains("youtu.be"))                                // Example: https://youtu.be/M80K51DosFo
                        return new AResult<string>(null, id.Split('/')[3]);

                    return new AResult<string>(null, id);
                }

                return new AResult<string>(null, string.Empty);
            }
            catch (Exception ex) {
                return new AResult<string>(ex, null);
            }
        }
        public static string GetYouTubeThumbnailURLFromVideoID(string id)
        {
            return string.Format("https://img.youtube.com/vi/{0}/0.jpg", id);
        }

        /// <summary>
        /// Shows a messagebox that asks the user if he wants to navigate to the given webpage.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns>True if the user wanted to navigate to the page, otherwise false.</returns>
        public static AResult<bool> AskUserToGoToURL(Uri uri)
        {
            try
            {
                if (uri != null)
                {
                    switch (MessageBox.Show(string.Format("This link takes you to {0} ({1}). Do you want to go there?", uri.Host, uri.ToString()), "Open link?", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    {
                        case MessageBoxResult.Yes:
                            return new AResult<bool>(null, Process.Start(uri.ToString()) != null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AResult<bool>(ex, false);
            }
            return new AResult<bool>(null, false);
        }

        /// <summary>
        /// Gets the round trip time (Ping) from the specified hostname asynchronously.
        /// <para>Does not lock up your application.</para>
        /// </summary>
        /// <param name="hostName">The target hostname to get the ping from.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public static Task<PingReply> GetPingAsync(string hostName)
        {
            using (Ping pingSender = new Ping())
            {
                return pingSender.SendPingAsync(hostName);
            }
        }

        /// <summary>
        /// Gets the round trip time (Ping) from the specified hostname.
        /// <para>Can and will lock up your application if called from the main thread.</para>
        /// </summary>
        /// <param name="hostName">The target hostname to get the ping from.</param>
        /// <returns>A <see cref="AResult{T}"/> object that contains information if the operation failed or not. Returns the ping if successfully, or -1 if not.</returns>
        public static AResult<long> GetPing(string hostName)
        {
            try
            {
                using (Ping pingSender = new Ping())
                {
                    PingReply reply = pingSender.Send(hostName);
                    if (reply.Status == IPStatus.Success)
                    {
                        return new AResult<long>(null, reply.RoundtripTime);
                    }
                    else
                    {
                        return new AResult<long>(null, -1);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AResult<long>(ex, -1);
            }
        }

    }
}
