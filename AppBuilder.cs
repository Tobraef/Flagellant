using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flagellant.ProcessHandling;
using Flagellant.Workflow;
using Flagellant.Prompt;
using Flagellant.Custom;

namespace Flagellant
{
    public class AppLogic
    {
        private Subject subject;
        private List<IPrompter> prompters;
        private List<IRuntimeResponder> responders;

        public void SetDisplay(System.Windows.Window window)
        {
            foreach (var prompter in prompters)
            {
                prompter.NotifyUser += p => window.Dispatcher.Invoke(() => 
                {
                    ProcessTools.PopThis(window);
                    p.DisplayOn(window);
                });
            }
        }

        public Panel CreateUI()
        {
            StackPanel mainPanel = new StackPanel();
            foreach (var resp in responders)
            {
                Border border = new Border();
                border.Child = resp.GetRuntimePanel();
                mainPanel.Children.Add(border);
            }
            return mainPanel;
        }

        public void Tick()
        {
            subject.TriggerTick();
        }

        internal AppLogic(Subject s, List<IPrompter> ps, List<IRuntimeResponder> responders)
        {
            subject = s;
            prompters = ps;
            this.responders = responders;
        }
    }

    public class AppBuilder
    {
        private List<IPrompter> prompters = new List<IPrompter>();
        private List<IRuntimeResponder> responders = new List<IRuntimeResponder>();
        private Subject subject;

        public void AddWorkFlowPrompter(List<MainWindow.ProcessInfo> good, List<MainWindow.ProcessInfo> bad, string browser)
        {
            ProcessManager manager = new ProcessManager();
            manager.SetDesktopProcesses(good.Where(i => i.Process != null).Select(i => i.Process).ToList(),
                bad.Where(i => i.Process != null).Select(i => i.Process).ToList());
            manager.SetWebProcesses(good.Where(i => !string.IsNullOrEmpty(i.Url)).Select(i => i.Url).ToList(),
                bad.Where(i => !string.IsNullOrEmpty(i.Url)).Select(i => i.Url).ToList(), browser);

            var notifier = new WorkflowNotifier(manager);
            prompters.Add(notifier);
            responders.Add(notifier);
        }

        public void AddHealthPrompter()
        {
            prompters.Add(null);
        }

        public void AddStatistics()
        {
            prompters.Add(null);
        }

        public void AddCustomNotifs(List<NotifState> notifs)
        {
            var model = new DefaultNotifier(notifs);
            prompters.Add(model);
            responders.Add(model);
        }

        public void AddNotifier(IPrompter prompter)
        {
            prompters.Add(prompter);
        }

        public void AddRuntimeUI(IRuntimeResponder responder)
        {
            responders.Add(responder);
        }

        public AppLogic Build(bool mute)
        {
            subject = new Subject();
            prompters.ForEach(p => { p.SetSubject(subject); p.IsMute = mute; });
            return new AppLogic(subject, prompters, responders);
        }
    }
}
