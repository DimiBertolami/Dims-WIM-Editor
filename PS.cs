using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace DimsISOTweaker
{
    internal class PS
    {
        public Process process { get; set; } = new Process();
        public ProcessStartInfo psi { get; set; } = new ProcessStartInfo();
        public int PID { get; set; } = 0;
        public string cmd { get; set; } = "cmd.exe";
        public string argument { get; set; } = " /c color 9e";
        public PS(string argument) 
        {
            this.argument = argument;
        }

        public static Process StartCommand(params string[] commands) => StartCommand(commands, false);
        public static Process StartCommand(IEnumerable<string> commands, bool inBackground, bool runAsAdministrator = true)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.WorkingDirectory = @"C:\Mount";
            p.StartInfo.Arguments = " / c echo hello 1233";
            p.Start();
            p.WaitForExit();

            //if (commands.Any()) p.StartInfo.Arguments = @"/C " + string.Join("&&", commands);
            //if (runAsAdministrator)
            //    p.StartInfo.Verb = "runas";
            //if (inBackground)
            //{
            //    p.StartInfo.CreateNoWindow = true;
            //    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //}
            //p.Start();
            return p;
        }

        public int Create(string argument)
        {
            process = new Process();
            psi = new ProcessStartInfo();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = argument;
            process.StartInfo.UseShellExecute= false;
            process.StartInfo.RedirectStandardInput= true;
            process.StartInfo.WorkingDirectory = "c:\\Mount";
            process.Start();
            //MessageBox.Show("ProcessId = " + process.Id + "! You remember that!");
            PID = process.Id;
            return process.Id;
            //StreamWriter streamWriter = process.StandardInput;
            //string inputText = "";
            //int numLines = 0;

            //psi.FileName = "cmd.exe";
            //psi.RedirectStandardOutput = false;
            //psi.RedirectStandardError = false;
            //psi.RedirectStandardInput = false;
            //psi.UseShellExecute = false;
            //psi.WorkingDirectory = "c:\\Mount";
            //process.StartInfo = psi;

        }
        private int Start()
        {
            var oProcess = new Process();
            var oStartInfo = new ProcessStartInfo("cmd.exe", " /k color 9e");
            oStartInfo.UseShellExecute = false; ;
            oStartInfo.RedirectStandardOutput = true;
            oStartInfo.RedirectStandardInput = true;
            oStartInfo.CreateNoWindow = false;
            oProcess.StartInfo = oStartInfo;
            oProcess.Start();
            PID = oProcess.Id;
            return PID;
        }
        private void WriteToPID(int PID)
        {
            Process x = Process.GetProcessById(PID);
            x.StandardInput.WriteLine("test 123");
        }
        public static void WriteToProcessById(int Id, string line)
        {
            if(Id != 0)
            {
                Process x = Process.GetProcessById(Id); //.StandardInput.WriteLine(line);
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(line + " " + x.StartInfo.RedirectStandardInput);
                //Console.WriteLine("is STDInput redirected?" + x.StartInfo.RedirectStandardInput);

                //Process process = new Process();
                //ProcessStartInfo psi=new ProcessStartInfo();
                //psi.RedirectStandardInput = true;
                //psi.WorkingDirectory = "c:\\Mount\\";

                //process.StartInfo.FileName = "cmd.exe";
                //process.StartInfo.Arguments = line;
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.RedirectStandardInput = true;
                //MessageBox.Show("Id: " + Id);
                //var psi = new ProcessStartInfo();
                //process.StartInfo = psi;
                //psi.RedirectStandardInput = true;
                //process.StandardInput.WriteLine(line);

                //process.StartInfo.UseShellExecute= false;

                //psi.RedirectStandardInput = true;
                //process.StartInfo = psi;
                //process.OutputDataReceived
                //process.StandardInput.WriteLine(line);
            }
        }

    }
}
