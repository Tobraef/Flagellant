using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Workflow
{
    public class FlowHandler
    {
        private const int triggerValue = 60 * 5;
        private const int admireValue = 1;
        private const int punishValue = -1;
        private const int workToBreakRatio = 5;

        private Severity currentSeverity;
        private int workTicksSinceBreak;

        private int ticksWorking;
        private int ticksSlacking;

        public event Action BreakFinished;
        public event Action<Severity> PromptUserRequired;

        private void GoodBehaviour()
        {
            currentSeverity--;
            if (currentSeverity < Severity.Great)
            {
                currentSeverity = Severity.Great;
            }
        }

        private void BadBehaviour()
        {
            currentSeverity++;
            if (currentSeverity >= Severity.Bad)
            {
                PromptUserRequired(currentSeverity);
            }
        }

        public void WorkingTick()
        {
            workTicksSinceBreak++;
            ticksWorking++;
            if (ticksWorking == triggerValue)
            {
                ticksWorking -= ticksSlacking;
                ticksSlacking = 0;
                if (ticksWorking >= triggerValue)
                {
                    GoodBehaviour();
                    ticksWorking = 0;
                }
            }
        }

        public void SlackingTick()
        {
            ticksSlacking += 5;
            if (ticksSlacking == triggerValue)
            {
                ticksSlacking -= ticksWorking;
                ticksWorking = 0;
                if (ticksSlacking >= triggerValue)
                {
                    BadBehaviour();
                    ticksSlacking = 0;
                }
            }
        }

        public void BreakTick()
        {
            workTicksSinceBreak--;
            if (workTicksSinceBreak <= 0)
            {
                BreakFinished();
            }
        }

        public void BreakRequested()
        {
            workTicksSinceBreak /= workToBreakRatio;
        }

        public FlowHandler()
        {
            currentSeverity = Severity.Ok;
        }
    }
}
