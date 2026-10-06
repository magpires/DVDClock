namespace DVDClock
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                {
                    secondArgument = args[1];
                }

                if (firstArgument == "/c")
                {
                    Application.Run(new SettingsForm());
                }
                else if (firstArgument == "/p")
                {
                    if (secondArgument == null)
                    {
                        MessageBox.Show("No window handle provided.", "DVDClock", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    IntPtr previewWndHandle = new IntPtr(long.Parse(secondArgument));
                    Application.Run(new ScreensaverForm(previewWndHandle));
                }
                else if (firstArgument == "/s")
                {
                    ShowScreensaver();
                }
                else
                {
                    Application.Run(new SettingsForm());
                }
            }
            else
            {
                Application.Run(new SettingsForm());
            }
        }

        static void ShowScreensaver()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Application.Run(new ScreensaverForm(bounds));
        }
    }
}