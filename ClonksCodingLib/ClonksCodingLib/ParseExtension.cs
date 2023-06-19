using System.Globalization;

namespace CCL
{
    /// <summary>
    /// Parsing stuff.
    /// </summary>
    public class ParseExtension
    {

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="bool"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static bool Parse(string s, bool defaultValue = false)
        {
            bool result;
            bool parseResult = bool.TryParse(s, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="int"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static int Parse(string s, int defaultValue = 0)
        {
            int result;
            bool parseResult = int.TryParse(s, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="ushort"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static ushort Parse(string s, ushort defaultValue = 0)
        {
            ushort result;
            bool parseResult = ushort.TryParse(s, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="long"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static long Parse(string s, long defaultValue = 0)
        {
            long result;
            bool parseResult = long.TryParse(s, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="ulong"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static ulong Parse(string s, ulong defaultValue = 0)
        {
            ulong result;
            bool parseResult = ulong.TryParse(s, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="double"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static double Parse(string s, double defaultValue = 0)
        {
            double result;
            bool parseResult = double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Parses the given <see langword="string"/> to a <see langword="float"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static float Parse(string s, float defaultValue = 0f)
        {
            float result;
            bool parseResult = float.TryParse(s, out result);
            if (parseResult)
            {
                return result;
            }
            return defaultValue;
        }

    }
}
