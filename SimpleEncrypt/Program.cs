namespace com.mercantilbanco.api.sample
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    internal static class Program
    {
        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return s.Length % 4 == 0 && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
