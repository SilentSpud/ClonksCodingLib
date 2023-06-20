using System;
using System.Globalization;
using System.Threading;

namespace CCL
{
    public static class Culture
    {

        /// <summary>
        /// Sets the current threads culture, and UI culture to the given <paramref name="cultureInfo"/>.
        /// </summary>
        /// <param name="cultureInfo">The new <see cref="CultureInfo"/> for the current thread.</param>
        public static void SetThreadCulture(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
        /// <summary>
        /// Sets the current threads culture, and UI culture to the given <paramref name="cultureName"/>.
        /// </summary>
        /// <param name="cultureName">The name of the <see cref="CultureInfo"/> that will be set for the current thread.</param>
        public static void SetThreadCulture(string cultureName)
        {
            CultureInfo newCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }

    }
}
