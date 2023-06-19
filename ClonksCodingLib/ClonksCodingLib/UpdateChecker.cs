using System;
using System.Net;

using Newtonsoft.Json;

namespace CCL
{
    /// <summary>
    /// Simple update checker.
    /// <para>Works great with Dropbox.</para>
    /// </summary>
    public class UpdateChecker : IDisposable
    {

        #region Variables and Properties
        // Variables
        private bool disposedValue;
        private WebClient webClient;
        private bool silentCheck;

        private string _currentVersion;
        private string _versionInfoDownloadURL;
        private string _debugVersionInfoDownloadURL;

        // Properties
        /// <summary>
        /// Gets the version this <see cref="UpdateChecker"/> got initialized with.
        /// <para>Used to compare the version of your application with the version retrieved from the <see cref="VersionInfoObject"/>.</para>
        /// </summary>
        public string CurrentVersion
        {
            get {
                if (disposedValue)
                    return null;

                return _currentVersion;
            }
            private set { _currentVersion = value; }
        }
        /// <summary>The url to the <see cref="VersionInfoObject"/> as a json string.</summary>
        public string VersionInfoDownloadURL
        {
            get {
                if (disposedValue)
                    return null;

                return _versionInfoDownloadURL;
            }
            private set { _versionInfoDownloadURL = value; }
        }
        /// <summary>The url to the <see cref="VersionInfoObject"/> as a json string for development purposes.</summary>
        public string DebugVersionInfoDownloadURL
        {
            get {
                if (disposedValue)
                    return null;

                return _debugVersionInfoDownloadURL;
            }
            private set { _debugVersionInfoDownloadURL = value; }
        }
        #endregion

        #region Classes
        /// <summary>
        /// The <see cref="VersionInfoObject"/> that will be downloaded and retrieved containing all the update info.
        /// </summary>
        public class VersionInfoObject
        {
            #region Variables
            [JsonIgnore] private bool _newVersionAvailable;
            [JsonIgnore] private bool _silent;

            [JsonIgnore] private string _rawJsonString;

            private string _currentVersion;
            private string _directDownloadLink, _downloadPage;

            private object _customData;

            // Properties
            /// <summary>Gets if a new version is available.</summary>
            [JsonIgnore] public bool NewVersionAvailable
            {
                get { return _newVersionAvailable; }
                internal set { _newVersionAvailable = value; }
            }
            /// <summary>Gets if this is supposed to be a silent check.</summary>
            [JsonIgnore] public bool SilentCheck
            {
                get { return _silent; }
                internal set { _silent = value; }
            }

            /// <summary>Gets the raw json string of this <see cref="VersionInfoObject"/>.</summary>
            public string RawJsonString
            {
                get { return _rawJsonString; }
                internal set { _rawJsonString = value; }
            }

            /// <summary>Gets the current version that is publicly available.</summary>
            public string CurrentVersion
            {
                get { return _currentVersion; }
                set { _currentVersion = value; }
            }

            /// <summary>Gets the direct download link to the new version if available.</summary>
            public string DirectDownloadLink
            {
                get { return _directDownloadLink; }
                set { _directDownloadLink = value; }
            }
            /// <summary>Gets the download link page for your application if available.</summary>
            public string DownloadPage
            {
                get { return _downloadPage; }
                set { _downloadPage = value; }
            }

            /// <summary>Gets the custom data that was assigned to the <see cref="VersionInfoObject"/>.</summary>
            public object CustomData
            {
                get { return _customData; }
                set { _customData = value; }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new <see cref="VersionInfoObject"/>.
            /// </summary>
            /// <param name="customData">The custom data that you want to assign to this <see cref="VersionInfoObject"/>.</param>
            public VersionInfoObject(object customData)
            {
                CurrentVersion = "0.0";
                DirectDownloadLink = "URL";
                DownloadPage = "URL";
                CustomData = customData;
            }

            /// <summary>Initializes a new <see cref="VersionInfoObject"/>.</summary>
            public VersionInfoObject()
            {
                CurrentVersion = "0.0";
                DirectDownloadLink = "URL";
                DownloadPage = "URL";
                CustomData = null;
            }
            #endregion

            /// <summary>
            /// Returns this <see cref="VersionInfoObject"/> class as a json string so you can copy it and fill out all fields for you desire.
            /// </summary>
            /// <param name="customData">The custom data that you want to assign to the <see cref="VersionInfoObject"/>.</param>
            /// <returns>The <see cref="VersionInfoObject"/> class as a json string.</returns>
            public static string GetJSONString(object customData)
            {
                return JsonConvert.SerializeObject(new VersionInfoObject(customData), Formatting.Indented);
            }
        }
        #endregion

        #region Events

        /// <summary>Delegate for the <see cref="UpdateCheckCompleted"/> event.</summary>
        /// <param name="result">The result of the async update check.</param>
        public delegate void UpdateCheckCompletedDelegate(VersionInfoObject result);
        /// <summary>Delegate for the <see cref="UpdateCheckFailed"/> event.</summary>
        /// <param name="e">The <see cref="Exception"/> that occured while trying to check for updates.</param>
        public delegate void UpdateCheckFailedDelegate(Exception e);

        /// <summary>Gets invoked when the async update check completed.</summary>
        public event UpdateCheckCompletedDelegate UpdateCheckCompleted;
        /// <summary>Gets invoked when the update check failed.</summary>
        public event UpdateCheckFailedDelegate UpdateCheckFailed;

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (!e.Cancelled)
                    {
                        string jsonObjectString = e.Result;

                        // Check if received string is valid.
                        if (string.IsNullOrWhiteSpace(jsonObjectString))
                        {
                            UpdateCheckFailed?.Invoke(new Exception("Received an invalid json string."));
                            return;
                        }

                        // Try to convert received string to json object
                        VersionInfoObject result = JsonConvert.DeserializeObject<VersionInfoObject>(jsonObjectString);
                        result.RawJsonString = jsonObjectString;
                        result.SilentCheck = silentCheck;

                        // Check if update is available
                        if (string.Compare(CurrentVersion, result.CurrentVersion) <= -1) // Update available
                        {
                            result.NewVersionAvailable = true;
                        }
                        else // No update available
                        {
                            result.NewVersionAvailable = false;
                        }

                        // Notify subscribers
                        UpdateCheckCompleted?.Invoke(result);
                    }
                    else
                    {
                        UpdateCheckFailed?.Invoke(new Exception("Update check was cancelled."));
                    }
                }
                else
                {
                    UpdateCheckFailed?.Invoke(e.Error);
                }
            }
            catch (Exception ex)
            {
                UpdateCheckFailed?.Invoke(ex);
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="UpdateChecker"/>.
        /// </summary>
        /// <param name="currentVersion">The current version of your application.</param>
        /// <param name="versionInfoDownloadURL">The url to the <see cref="VersionInfoObject"/> as a json string.</param>
        /// <param name="debugVersionInfoDownloadURL">The url to the <see cref="VersionInfoObject"/> as a json string for development purposes.</param>
        public UpdateChecker(string currentVersion, string versionInfoDownloadURL, string debugVersionInfoDownloadURL)
        {
            CurrentVersion = currentVersion;
            VersionInfoDownloadURL = versionInfoDownloadURL;
            DebugVersionInfoDownloadURL = debugVersionInfoDownloadURL;

            webClient = new WebClient();
            webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
        }
        #endregion

        #region Disposing
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (webClient != null)
                    {
                        webClient.Dispose();
                    }
                }

                webClient = null;
                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Checks for updates asynchronously.
        /// <para>Does not lock up your application.</para>
        /// </summary>
        /// <param name="silent">Sets if this update check should be a silent one.</param>
        /// <param name="debugMode">Sets if the <see cref="DebugVersionInfoDownloadURL"/> should be used instead of the <see cref="VersionInfoDownloadURL"/>.</param>
        public void CheckForUpdatesAsync(bool silent, bool debugMode = false)
        {
            if (disposedValue)
                return;

            try
            {
                silentCheck = silent;

                if (debugMode)
                {
                    webClient.DownloadStringAsync(new Uri(DebugVersionInfoDownloadURL));
                }
                else
                {
                    webClient.DownloadStringAsync(new Uri(VersionInfoDownloadURL));
                }
            }
            catch (Exception ex)
            {
                UpdateCheckFailed?.Invoke(ex);
            }
        }

        /// <summary>
        /// Checks for updates.
        /// <para>Can and will lock up your application if called from the main thread.</para>
        /// </summary>
        /// <param name="silent">Sets if this update check should be a silent one.</param>
        /// <param name="debugMode">Sets if the <see cref="DebugVersionInfoDownloadURL"/> should be used instead of the <see cref="VersionInfoDownloadURL"/>.</param>
        /// <returns>The <see cref="VersionInfoObject"/> received with the update information.</returns>
        public VersionInfoObject CheckForUpdates(bool silent, bool debugMode = false)
        {
            if (disposedValue)
                return null;

            try
            {
                string jsonObjectString = debugMode ? webClient.DownloadString(DebugVersionInfoDownloadURL) : webClient.DownloadString(VersionInfoDownloadURL);

                // Check if received string is valid.
                if (string.IsNullOrWhiteSpace(jsonObjectString))
                {
                    UpdateCheckFailed?.Invoke(new Exception("Received an invalid json string."));
                    return null;
                }

                // Try to convert received string to json object
                VersionInfoObject result = JsonConvert.DeserializeObject<VersionInfoObject>(jsonObjectString);
                result.RawJsonString = jsonObjectString;
                result.SilentCheck = silent;

                // Check if update is available
                if (string.Compare(CurrentVersion, result.CurrentVersion) <= -1) // Update available
                {
                    result.NewVersionAvailable = true;
                }
                else // No update available
                {
                    result.NewVersionAvailable = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                UpdateCheckFailed?.Invoke(ex);
            }
            return null;
        }

    }
}
