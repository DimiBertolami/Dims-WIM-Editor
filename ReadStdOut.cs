using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DimsISOTweaker
{
    public class ReadStdOut : Process
    {
        public string binaryExecutable { get; set; } = "cmd.exe";
        bool RedirectStdIn { get; set; } = true;
        public int PID { get; set; } = 0;
        public string args { get; set; } = " /k echo hello world!";
        public bool RedirectStandardInput { get; private set; }

        //public bool bAdmin { get; set; }
        public int readStdOut(string binaryExecutable = "cmd.exe", bool RedirectStdIn = true)
        {
            this.binaryExecutable = binaryExecutable;
            Process ps = new Process();
            ps.StartInfo.FileName = binaryExecutable;
            ps.StartInfo.Arguments = args;
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.RedirectStandardInput = true;
            ps.StartInfo.CreateNoWindow = false;
            ps.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            ps.Start();
            ps.WaitForExit();
            Global.PID= ps.Id;
            return Global.PID;
        }

        public int CreateProcess(string args, bool RedirectStandardInput = true, string binaryExecutable = "cmd.exe")
        {
            Console.WriteLine("Process Starting...");
            Process process = new Process();
            process.StartInfo.FileName = this.binaryExecutable;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = RedirectStandardInput;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle= ProcessWindowStyle.Maximized;
            process.Start();
            Global.PID = process.Id;
            process.StandardInput.WriteLine(binaryExecutable + "\n" + 
                args);
            process.WaitForExit();
            return Global.PID;
        }

        internal void CreateCommandOnPid(int pID, string args, bool RedirectStandardInput = true)
        {
            Global.PID = PID;
            if (PID == 0)
            {
                this.CreateProcess(" /k color 7c & title Wim-Editor & echo off & pushd c:\\AMDimPe\\", RedirectStandardInput = true);
            }
            else
            {
                var x = new ReadStdOut();
                x.CreateCommandOnPid(PID, "color 7c & title Wim-Editor & echo off & pushd c:\\AMDimPe\\", RedirectStandardInput = true);

            }
            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = "c:\\Mount";
            process.StartInfo = psi;
            process.Start();
            process.StandardInput.WriteLine("echo hello test!");
            psi.RedirectStandardInput = false;
        }
    }
}