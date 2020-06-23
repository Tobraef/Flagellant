using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Flagellant.ProcessHandling
{
    public class ProcessManager
    {
        private BrowserHandler browserHandler;
        // a divide for desktop good and web good if you like 
        private readonly List<IProcess> workingProcesses = new List<IProcess>();
        private readonly List<IProcess> slackingProcesses = new List<IProcess>();
        private string currentProcess;

        public void SetDesktopProcesses(List<Process> good, List<Process> bad)
        {
            workingProcesses.AddRange(good.Select(p => new DesktopProcess
            {
                Name = p.ProcessName,
                PostFix = p.MainWindowTitle,
                ObservedProcess = p
            }));
            slackingProcesses.AddRange(bad.Select(p => new DesktopProcess
            {
                Name = p.ProcessName,
                PostFix = p.MainWindowTitle,
                ObservedProcess = p
            }));
        }

        public void SetWebProcesses(List<string> good, List<string> bad, string browser)
        {
            if (browser.Equals("chrome"))
            {
                browserHandler = new ChromeHandler();
            }
            else if (browser.Equals("firefox"))
            {
                browserHandler = new FirefoxHandler();
            }
            else
            {
                browserHandler = null;
            }
            workingProcesses.AddRange(good.Select(p => new DesktopProcess
            {
                Name = browserHandler.BrowserProcessName,
                PostFix = p
            }));
            slackingProcesses.AddRange(bad.Select(p => new DesktopProcess
            {
                Name = browserHandler.BrowserProcessName,
                PostFix = p
            }));
        }

        public bool? UserIsWorking()
        {
            var current = ProcessTools.GetCurrentWorkingProcess();
            if (current == null) { currentProcess = string.Empty; return null; }
            if (current.ProcessName.Equals(browserHandler.BrowserProcessName))
            {
                var url = browserHandler.ExtractUrl(current);
                currentProcess = url;
                if (workingProcesses.Any(p => p.PostFix.Equals(url)))
                {
                    return true;
                }
                else if (slackingProcesses.Any(p => p.PostFix.Equals(url)))
                {
                    return false;
                }
                else if (string.IsNullOrEmpty(url))
                {
                    return true;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                currentProcess = current.ProcessName;
                foreach (var p in workingProcesses)
                {
                    if (p.Name.Equals(current.ProcessName))
                    {
                        if (p.PostFix.Equals(current.MainWindowTitle))
                        {
                            return true;
                        }
                    }
                }
                foreach (var p in slackingProcesses)
                {
                    if (p.Name.Equals(current.ProcessName))
                    {
                        return false;
                    }
                }
                return null;
            }
        }

        public void KillCurrentWindow()
        {
            ProcessTools.KillCurrent();
        }

        public void PopWorkingWindow()
        {
            foreach (var p in workingProcesses.Where(p => p is DesktopProcess))
            {
                if (p.PopWindow()) break;
            }
        }

        public void KillAllSlackingProcesses()
        {
            slackingProcesses.ForEach(p => p.Kill());
        }

        public List<Process> LoadDesktopProcesses()
        {
            return Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero).ToList();
        }

        public string CurrentProcessName
        {
            get
            {
                return currentProcess;
            }
        }
    }
}
