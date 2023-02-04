using System;
using System.Diagnostics;

namespace WinISOEditor.Entities
{
    public class Ps : System.ComponentModel.Component, IDisposable
    {
        public int Id { get; set; }
        public string Argument { get; set; }
        public string binaryExecutable { get; set; } = "cmd.exe";
        bool RedirectStdIn { get; set; } = true;
        bool RedirectStdOut { get; set; } = true;
        bool UseShellExecute { get; set; } = false;

        public Ps(int id, string argument, string binaryExecutable, bool redirectStdIn, bool redirectStdOut, bool useShellExecute)
        {
            Id = id;
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
            this.binaryExecutable = binaryExecutable ?? throw new ArgumentNullException(nameof(binaryExecutable));
            RedirectStdIn = redirectStdIn;
            RedirectStdOut = redirectStdOut;
            UseShellExecute = useShellExecute;
        }

        public static void Create(string argument, int id = 1)
        {
            int Id = id;
            string Argument = argument;

            if (Global.PID == 1)
            {
                Process ps = new Process();
                ProcessStartInfo psi = ps.StartInfo;
                psi.FileName = "CMD.EXE";
                psi.Arguments = "color 5f & cls";
                psi.UseShellExecute = false;
                psi.WorkingDirectory = "c:\\pe__data";
                //psi.WindowStyle = ProcessWindowStyle.Minimized;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                ps.StartInfo.RedirectStandardInput = true;
                ps.StartInfo.RedirectStandardOutput = true;

                ps.Start();
                ps.StandardInput.Write(Argument);
                //Global.ps = ps;
                Global.PID = ps.Id;
            }
            else
            {
                //Process ps = Ps.GetProcessById(Global.PID;
                //Process ps = Global.ps;
                //ProcessStartInfo psi = Ps.StartInfo;
                //ps.StandardInput.Write("Arguments " + argument);
                //ps.StandardInput.Write("WorkingDirectory " + psi.WorkingDirectory);
            }
        }
    }
}
