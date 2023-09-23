using System;
using System.Text;

namespace CCL
{
    public static class StringArrayExtensions
    {

        /// <summary>
        /// Converts a string array to a single string.
        /// </summary>
        /// <param name="array">The string array that should be converted.</param>
        /// <returns>Null if the <paramref name="array"/> is null or doesn't contain any elements. Otherwise returns the converted string.</returns>
        public static string ConvertStringArrayToString(this string[] array)
        {
            if (array == null)
                return null;
            if (array.Length == 0)
                return null;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (string value in array)
                stringBuilder.AppendLine(value);

            return stringBuilder.ToString();
        }

    }
}
