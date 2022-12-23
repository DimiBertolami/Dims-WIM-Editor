using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DimsISOTweaker;

namespace CmdApp
{
    public class CmdService : IDisposable
    {
        private Process _cmdProcess;
        private StreamWriter _streamWriter;
        private AutoResetEvent _outputWaitHandle;
        private string _cmdOutput;
        public int ProcessID { get; private set; } = 0;
        public CmdService(int ProcessID, string cmdPath)
        {
            if(ProcessID != 0)
            {
                //process was already running
                this.getProcessById(ProcessID).StandardInput.WriteLine(cmdPath);
            } else
            {
                //create process
                _cmdProcess = new Process();
                _outputWaitHandle = new AutoResetEvent(false);
                _cmdOutput = String.Empty;

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = cmdPath;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.CreateNoWindow = true;

                _cmdProcess.OutputDataReceived += _cmdProcess_OutputDataReceived;

                _cmdProcess.StartInfo = psi;
                _cmdProcess.Start();

                _streamWriter = _cmdProcess.StandardInput;
                _cmdProcess.BeginOutputReadLine();
            }
        }
        protected Process getProcessById(int processId)
        {
            return ReadStdOut.GetProcessById(processId);
        }
        public string ExecuteCommand(string command)
        {
            _cmdOutput = String.Empty;

            _streamWriter.WriteLine(command);
            _streamWriter.WriteLine("echo end");
            _outputWaitHandle.WaitOne();
            return _cmdOutput;
        }

        private void _cmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data == "end")
                _outputWaitHandle.Set();
            else
                _cmdOutput += e.Data + Environment.NewLine;
        }

        public void Dispose()
        {
            _cmdProcess.Close();
            _cmdProcess.Dispose();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }
    }
}
