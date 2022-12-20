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
        public string binaryExecutable { get; set; }
        public string args { get; set; }
        public bool bAdmin { get; set; }

        public void CreateProcess(string binaryExecutable, string args, bool bAdmin)
        {
            Process process = new Process();
            {
                StartInfo = new ProcessStartInfo{ };
                StartInfo.Verb = "runas";
            }
            process.StartInfo.FileName = binaryExecutable;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            {
                var line = string.Empty;
                while (!process.StandardOutput.EndOfStream)
                {
                    line = process.StandardOutput.ReadToEnd();
                }
                MessageBox.Show( line );
            }
        }
    }
}