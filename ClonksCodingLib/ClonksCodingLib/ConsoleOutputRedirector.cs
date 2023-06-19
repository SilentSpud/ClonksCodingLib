using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CCL
{
    /// <summary>
    /// If you override the <see cref="System.Console.Out"/> property with a new instance of the <see cref="ConsoleOutputRedirector"/>
    /// you can subscribe to its <see cref="ConsoleOutputRedirector.OnLineAdded"/> event to be notified when a new line got written to the console using <see cref="System.Console.WriteLine(string)"/>.
    /// <br/><br/>
    /// You also have the possibility to view the history of the console using <see cref="ConsoleOutputRedirector.GetLines()"/>.
    /// </summary>
    public class ConsoleOutputRedirector : TextWriter
    {

        #region Variables
        private List<string> lines;
        private TextWriter original;
        #endregion

        #region Events
        public delegate void LineAddedDelegate(string value);
        public event LineAddedDelegate OnLineAdded;
        #endregion

        #region Constructor
        public ConsoleOutputRedirector(TextWriter pOriginal)
        {
            lines = new List<string>();
            original = pOriginal;
        }
        #endregion

        #region Overrides
        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
        public override void WriteLine(string value)
        {
            lines.Add(value);
            original.WriteLine(value);
            OnLineAdded?.Invoke(value);
        }
        #endregion

        #region Functions
        public string[] GetLines()
        {
            return lines.ToArray();
        }
        #endregion

    }
}
