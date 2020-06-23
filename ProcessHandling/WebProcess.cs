using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Flagellant.ProcessHandling
{
    /// <summary>
    /// Holds browser info and URL. 
    /// </summary>
    public class WebProcess : IProcess
    {
        public void Kill()
        {
            var openChrome = Process.GetProcesses().First(p => p.ProcessName.Equals(Handler.BrowserProcessName) &&
                p.MainWindowHandle != IntPtr.Zero);
            if (Handler.ExtractUrl(openChrome).Equals(PostFix))
            {
                openChrome.Kill();
            }
        }

        public bool PopWindow()
        {
            return false;
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

        public Process ObservedProcess
        {
            set;
            get;
        }

        public BrowserHandler Handler
        {
            set;
            get;
        }

        public bool IsAlive
        {
            get 
            {
                return false; 
            }
        }
    }
}
