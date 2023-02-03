using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        public string args { get; set; } = "color 56 & echo hello";
        public bool RedirectStandardInput { get; private set; }


        public Process CreateProcess(string args, bool RedirectStandardInput, string binaryExecutable = "cmd.exe")
        {
            string Args = string.Empty;
            Process process = new Process();
            ProcessStartInfo psi = process.StartInfo;
            psi.FileName = binaryExecutable;
            //psi.Arguments = "@echo off & pushd c:\\ & cls";
            //psi.FileName = this.binaryExecutable;
            if (process.StartInfo.RedirectStandardInput == true)
            {
                Args = "color 56 & @echo off & pushd c:\\";
            }
            else
            {
                Args = "color 9e & @echo off & pushd c:\\";
            }
            psi.Arguments = Args;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = "c:\\pe__data";
            psi.CreateNoWindow = false;
            Global.WindowStyle = Global.WindowStyle;
            psi.WindowStyle = Global.WindowStyle;

            process.StartInfo.RedirectStandardInput = RedirectStandardInput;
            process.Start();

            WebClient webClient = new WebClient();
            Global.webClient = webClient;
            Global.PID = process.Id;
            Global.ps = process;
            return process;
        }

    }
}