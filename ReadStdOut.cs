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


            //Global.RedirectStandardInput = false;
            Global.PID= ps.Id;
            Global.ps = ps;
            return Global.ps;
        }

        public Process CreateProcess(string args, bool RedirectStandardInput, string binaryExecutable = "cmd.exe")
        {
            //if (Global.PID == 0)
            //{
                Process process = new Process();
                ProcessStartInfo psi = process.StartInfo;
                psi.FileName = this.binaryExecutable;
                psi.Arguments = "color 3e & @echo off & pushd c:\\";
                psi.UseShellExecute = false;
                psi.WorkingDirectory = "c:\\";
                psi.CreateNoWindow = false;
                psi.WindowStyle = ProcessWindowStyle.Maximized;

                process.StartInfo.RedirectStandardInput = true;
                process.Start();
                process.StandardInput.WriteLine(psi.Arguments);


                Global.PID = process.Id;
                Global.ps = process;
                return process;
        }

    }
}