using System;
using System.Windows;

namespace SaintNotch
{
    public partial class App : Application
    {
        public static string InitialState { get; set; } = "S0";
        public static string? CaptureDir { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            System.IO.File.WriteAllText(@"C:\Users\yudis\.gemini\antigravity\brain\9d4b6f9f-e4d7-4c53-888c-4ca277e7d6d2\startup.log", "Args: " + string.Join(" | ", e.Args));
            if (e.Args.Length > 0)
            {
                for (int i = 0; i < e.Args.Length; i++)
                {
                    if (e.Args[i].Equals("--state", StringComparison.OrdinalIgnoreCase) && i + 1 < e.Args.Length)
                    {
                        InitialState = e.Args[i + 1].ToUpperInvariant();
                    }
                    else if (e.Args[i].Equals("--capture", StringComparison.OrdinalIgnoreCase) && i + 1 < e.Args.Length)
                    {
                        CaptureDir = e.Args[i + 1];
                    }
                }
            }
        }
    }
}
