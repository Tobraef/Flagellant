using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Flagellant.ProcessHandling;
using Flagellant.Prompt;

namespace Flagellant.Workflow
{
    public class WorkflowNotifier : IPrompter, IRuntimeResponder
    {
        private ISubject subject;
        private FlowHandler flowHandler;
        private ProcessManager processManager;
        private UIHandler uiManager;
        private readonly Statistics statisticsHandler;

        private void PromptUserBreak()
        {
            subject.Tick -= BreakTick;
            subject.Tick += WorkingTick;
            processManager.PopWorkingWindow();
            NotifyUser(new EndOfBreakPrompt());
        }

        private void StartBreak()
        {
            subject.Tick -= WorkingTick;
            subject.Tick += BreakTick;
            flowHandler.BreakRequested();
        }

        private void PromptUserFlow(Severity severity)
        {
            IPrompt toPop;
            if (severity == Severity.Bad)
            {
                toPop =  new BadPrompt();
            }
            else if (severity == Severity.Critical)
            {
                processManager.KillCurrentWindow();
                toPop = new CriticalPrompt();
            }
            else
            {
                processManager.KillCurrentWindow();
                toPop = new ChaosPrompt();
            }
            processManager.PopWorkingWindow();
            NotifyUser(toPop);
        }

        private void WorkingTick()
        {
            var working = processManager.UserIsWorking();
            if (working.HasValue)
            {
                if (working.Value)
                {
                    flowHandler.WorkingTick();
                    statisticsHandler.WorkTick(processManager.CurrentProcessName);
                }
                else
                {
                    flowHandler.SlackingTick();
                    statisticsHandler.SlackTick(processManager.CurrentProcessName);
                }
            }
            // else prompt user to input the name of the process? or break?
        }

        private void BreakTick()
        {
            flowHandler.BreakTick();
        }

        public event Action<IPrompt> NotifyUser;

        public void SetSubject(ISubject subject)
        {
            this.subject = subject;
            uiManager = new UIHandler(subject, statisticsHandler);
            uiManager.BreakRequested += StartBreak;
            subject.Tick += WorkingTick;
        }

        public bool IsMute
        {
            set;
            private get;
        }

        public WrapPanel GetRuntimePanel()
        {
            return uiManager.GetRuntimePanel();
        }

        public WorkflowNotifier(ProcessManager manager)
        {
            flowHandler = new FlowHandler();
            flowHandler.PromptUserRequired += PromptUserFlow;
            flowHandler.BreakFinished += PromptUserBreak;
            processManager = manager;
            statisticsHandler = new Statistics();
        }
    }
}
