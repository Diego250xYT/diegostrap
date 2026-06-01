using System;
using System.Windows.Forms;

namespace DiegoStrap
{
    internal static class EntryPoint
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0 && RobloxProtocolDispatcher.IsRobloxProtocolUri(args[0]))
            {
                try
                {
                    RobloxProtocolDispatcher.ForwardUri(args[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            Application.Run(new MainForm(args));
        }
    }
}