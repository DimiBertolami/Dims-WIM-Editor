using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DimsISOTweaker
{
    internal class Global
    {
        public static int PID { get; set; } = 0;
        public static Process ps { get; set; }
        public static string Args { get; set; } = string.Empty;
        public static string EchoStat { get; set; } = "echo off";
        public static WebClient webClient { get; set; }
        public static bool RedirectStandardInput { get; set; } = true;
        public static bool RedirectStandardOutput { get; set; } = false;
        public static bool RedirectStandardError { get; set; } = false;
    }
}
