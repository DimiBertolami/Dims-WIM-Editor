using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DimsISOTweaker
{
    public class ReadStdOut : Process
    {
        public string binaryExecutable { get; set; }
        public Parameter args { get; set; }

        public ReadStdOut(string binaryExecutable, Parameter args)
        {
            this.binaryExecutable = binaryExecutable;
            this.args = args;
        }
    }
}
