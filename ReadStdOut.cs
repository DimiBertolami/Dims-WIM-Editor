using System.Diagnostics;

namespace DimsISOTweaker
{
    public class ReadStdOut : Process
    {
        public string binaryExecutable { get; set; } = "cmd.exe";
        bool RedirectStdIn { get; set; } = true;
        public int PID { get; set; } = 0;
        public string args { get; set; } = "color 56 & echo hello";
        public bool RedirectStandardInput { get; private set; }


        public Process CreateProcess(bool RedirectStandardInput = true, string binaryExecutable = "cmd.exe")
        {
            string Args = string.Empty;
            Process process = new Process();
            ProcessStartInfo psi = process.StartInfo;
            psi.FileName = binaryExecutable;
            if (process.StartInfo.RedirectStandardInput == true)
            {
                Args = "color 56 & @echo off & pushd c:\\PE__Data";
            }
            else
            {
                Args = "color 9e & @echo off & pushd c:\\PE__Data";
            }
            psi.Arguments = Args;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = "c:\\pe__data";
            psi.CreateNoWindow = false;
            psi.WindowStyle = Global.WindowStyle;
            process.StartInfo.RedirectStandardInput = RedirectStandardInput;
            process.Start();
            Global.PID = process.Id;
            Global.ps = process;
            return process;
        }
    }
}