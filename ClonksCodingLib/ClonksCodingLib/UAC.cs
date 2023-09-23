using System;
using System.Security.Principal;

namespace CCL
{
    /// <summary>
    /// User Account Control related stuff.
    /// </summary>
    public static class UAC
    {

        /// <summary>
        /// Gets if this application is running with administrator privileges.
        /// </summary>
        /// <returns>True if this app is running with administrator privileges. Otherwise, false.</returns>
        public static bool IsAppRunningWithAdminPrivileges()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}
