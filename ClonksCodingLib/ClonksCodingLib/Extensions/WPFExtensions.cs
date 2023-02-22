using System.Windows.Media;

namespace CCL {
    public static class WPFExtensions {

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="HexColorString">The string representation of a HEXadecimal color string.</param>
        /// <returns>A new <see cref="SolidColorBrush"/> from the HEXadecimal color string.</returns>
        public static SolidColorBrush ToBrush(this string HexColorString)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(HexColorString));
        }

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="HexColorString">The string representation of a HEXadecimal color string.</param>
        /// <returns>A new <see cref="Color"/> from the HEXadecimal color string.</returns>
        public static Color ToColor(this string HexColorString)
        {
            return (Color)(new ColorConverter().ConvertFrom(HexColorString));
        }

    }
}
