using System.Data;
using System.Diagnostics;
using System.Net;

namespace DimsISOTweaker
{
    internal class Global
    {
        public static int PID { get; set; } = 0;
        public static Process ps { get; set; }
        public static int Handle { get; set; } = 0;
        public static string Args { get; set; } = string.Empty;
        public static string EchoStat { get; set; } = "echo off";
        public static ProcessWindowStyle WindowStyle { get; set; } = ProcessWindowStyle.Maximized;
        public static bool CreateNoWindow { get; set; } = false;
        public static bool StayOnTop { get; set; } = true;
        public static int PositionX { get; set; } = 0;
        public static int PositionY { get; set; } = 0;
        public static WebClient webClient { get; set; }
        public static bool RedirectStandardInput { get; set; } = true;
        public static bool RedirectStandardOutput { get; set; } = false;
        public static bool RedirectStandardError { get; set; } = false;
    }
}
