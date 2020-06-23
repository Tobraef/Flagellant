using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Flagellant.ProcessHandling
{
    public class DesktopProcess : IProcess
    {
        private Process observedProcess;

        public void Kill()
        {
            if (!observedProcess.HasExited)
            {
                observedProcess.Kill();
            }
        }

        public bool PopWindow()
        {
            if (observedProcess.HasExited)
            {
                var temp = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Equals(Name));
                if (temp == null)
                    return false;
                observedProcess = temp;
            }
            ProcessTools.LowLevel.SetForegroundWindow(observedProcess.MainWindowHandle);
            return true;
        }

        public Process ObservedProcess
        {
            set
            {
                observedProcess = value;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string PostFix
        {
            get;
            set;
        }

        public bool IsAlive
        {
            get { return !observedProcess.HasExited; }
        }
    }
}
