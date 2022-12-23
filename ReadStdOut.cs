using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DimsISOTweaker
{
    public class ReadStdOut : Process
    {
        public string binaryExecutable { get; set; } = "cmd.exe";
        public string args { get; set; }
        public bool bAdmin { get; set; }
        public int PID { get; set; } = 0;

        public void getPID(int PID, string args)
        {
            if (PID == 0)
            {
                MessageBox.Show("test blabla"); 
                this.StartInfo.FileName = "cmd.exe"; //CreateProcess(binaryExecutable, " /c echo test 123 test");
                this.StartInfo.Arguments = " /k echo hello Dimi" +args;
                ExitCode.Equals(666);   //process is created exitcode 666
            } else
            {
                Process x = Process.GetProcessById(PID);
                x.StandardInput.WriteLine("test test test");
                ExitCode.Equals(667);   //process was already running exitcode 667
            }
        }
            //Console.WriteLine("Process Starting...");
            //Process process = new Process();
            //process.StartInfo.FileName = binaryExecutable;
            //process.StartInfo.Arguments = " /c echo test 123 test";
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.CreateNoWindow = false;
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            //process.Start();
            //process.WaitForExit();
            //{
            //    var line = string.Empty;
            //    while (!process.StandardOutput.EndOfStream)
            //    {
            //        line = process.StandardOutput.ReadToEnd();
            //    }
            //    MessageBox.Show( line );
            //}
        //}
        public void CreateProcess(string args) //, bool bAdmin
        {
            Console.WriteLine("Process Starting...");
            Process process = new Process();
            process.StartInfo.FileName = binaryExecutable;
            process.StartInfo.Arguments = " /c echo test 123 test";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle= ProcessWindowStyle.Maximized;
            process.Start();
            process.WaitForExit();
        }
        public void CreateCMD(List<string> cmds, string workingDirectory = "")
        {
            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            psi.RedirectStandardInput = false;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = "c:\\Mount";
            process.StartInfo= psi;
            process.Start();
            process.StandardInput.WriteLine("echo hello shitbag!");

            //cmdStartInfo.RedirectStandardOutput = true;
            //cmdStartInfo.RedirectStandardError = true;
            //cmdStartInfo.RedirectStandardInput = true;
            //cmdStartInfo.CreateNoWindow = false;
            //cmdStartInfo.UseShellExecute = true;
            //cmdStartInfo.FileName = "cmd.exe";
            //cmdStartInfo.Arguments = arg;
            //cmdStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            //cmdStartInfo.WorkingDirectory = "c:\\Mount";
            //var cmdProcess = Process.Start(cmdStartInfo);
        }
    }
}