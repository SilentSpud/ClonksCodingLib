using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace CCL {
    public static class Web {

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

    }
}
