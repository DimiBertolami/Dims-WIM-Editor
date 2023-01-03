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
        public string args { get; set; } = "color 7e & echo hello";
        public bool RedirectStandardInput { get; private set; } = false;

        //public bool bAdmin { get; set; }
        public Process readStdOut(bool RedirectStdIn, string binaryExecutable = "cmd.exe")
        {
            this.binaryExecutable = binaryExecutable;
            Process ps = new Process();
            ps.StartInfo.FileName = binaryExecutable;
            ps.StartInfo.Arguments = args;
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.CreateNoWindow = false;
            ps.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            ps.StartInfo.RedirectStandardInput = true;
            ps.Start();


            Global.RedirectStandardInput = false;
            //Global.RedirectStandardOutput = false;
            //Global.RedirectStandardError = false;
            //ps.WaitForExit();
            //ps.StartInfo.RedirectStandardInput = RedirectStdIn;
            //ps.StartInfo.RedirectStandardOutput= true;
            //ps.StartInfo.RedirectStandardError= true;
            Global.PID= ps.Id;
            Global.ps = ps;
            return Global.ps;
        }

        public Process CreateProcess(string args, bool RedirectStandardInput, string binaryExecutable = "cmd.exe")
        {
            if (Global.PID == 0)
            {
                //Process process = new ReadStdOut().readStdOut(false);
                //MessageBox.Show("arguments: " + Global.Args);
                Process process = new Process();
                ProcessStartInfo psi = process.StartInfo;
                psi.FileName = this.binaryExecutable;
                psi.Arguments = args;
                psi.UseShellExecute = false;
                psi.WorkingDirectory = "c:\\";
                psi.CreateNoWindow = false;
                psi.WindowStyle = ProcessWindowStyle.Maximized;

                process.StartInfo.RedirectStandardInput = true;
                process.Start();
                //psi.RedirectStandardInput = true;
                process.StandardInput.WriteLine(Global.Args);
                process.StartInfo.RedirectStandardInput = false;

                //MessageBox.Show("pid: " + process.Id);
                Global.PID = process.Id;
                Global.ps = process;
                //Global.RedirectStandardInput = RedirectStdIn;
                //Global.RedirectStandardOutput = false;
                //Global.RedirectStandardError = false;
                return process;
            }
            else
            {
                Process process = Global.ps;
                //ProcessStartInfo psi = process.StartInfo;
                process.StartInfo.RedirectStandardInput = true;
                process.StandardInput.WriteLine(args);
                //process.StartInfo.RedirectStandardInput = false;
                return process;
            }
        }

        internal void CreateCommandOnPid(int pID, string args, bool RedirectStandardInput = false)
        {
            Global.PID = PID;
            if (PID == 0)
            {
                this.CreateProcess(" /k color 7c & title Wim-Editor & echo off & pushd c:\\AMDimPe\\", RedirectStandardInput);
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
            psi.RedirectStandardInput = RedirectStandardInput;
        }
    }
}