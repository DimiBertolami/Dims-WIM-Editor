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

        public void CreateProcess(string binaryExecutable, string args)
        {
            MessageBox.Show(binaryExecutable + " " + args, " * ", MessageBoxButton.OK, MessageBoxImage.Information);
            Process process = new Process();
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = binaryExecutable,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                };
            }

            process.Start();
            {
                var line = string.Empty;
                while (!process.StandardOutput.EndOfStream)
                {
                    line += process.StandardOutput.ReadLine() + "\n";
                }
                MessageBox.Show( line );
            }
        }
    }
}
