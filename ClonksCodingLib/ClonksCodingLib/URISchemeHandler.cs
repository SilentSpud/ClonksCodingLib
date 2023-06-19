using System;
using Microsoft.Win32;

namespace CCL
{
    /// <summary>
    /// Class to register or unregister URI Schemes.
    /// </summary>
    public static class URISchemeHandler
    {

        public static AResult<bool> Register(string protocolName, string executablePath)
        {
            try {
                RegistryKey regKey = Registry.ClassesRoot.CreateSubKey(protocolName);

                regKey.CreateSubKey("DefaultIcon").SetValue(null, string.Format("{0}{1},1{0}", (char)34, executablePath));

                regKey.SetValue(null, string.Format("URL:{0} Protocol", protocolName));
                regKey.SetValue("URL Protocol", "");

                regKey = regKey.CreateSubKey(@"shell\open\command");
                regKey.SetValue(null, string.Format("{0}{1}{0} {0}%1{0}", (char)34, executablePath));

                return new AResult<bool>(null, true);
            }
            catch (Exception ex) {
                return new AResult<bool>(ex, false);
            }
        }
        public static AResult<bool> Unregister(string protocolName)
        {
            try {
                Registry.ClassesRoot.DeleteSubKeyTree(protocolName, false);
                return new AResult<bool>(null, true);
            }
            catch (Exception ex) {
                return new AResult<bool>(ex, false);
            }
        }

    }
}
