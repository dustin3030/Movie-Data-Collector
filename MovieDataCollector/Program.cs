using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieDataCollector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        /// <summary>
        /// Public method to parse text
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string GeneralParser(string InputString, string start, string end)
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }

            try
            {
                int startPosition = InputString.IndexOf(start) + start.Length;
                int endPosition = InputString.IndexOf(end, startPosition);

                if (startPosition == -1 || endPosition == -1) { return ""; }

                if (startPosition >= endPosition) { return ""; }

                if (InputString.Length - startPosition > endPosition - startPosition)
                {
                    return InputString.Substring(startPosition, endPosition - startPosition);
                }
                else { return ""; }
            }
            catch
            {
                return "";
            }
        }
    }
}
