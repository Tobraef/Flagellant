using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagellant.ProcessHandling;

namespace Flagellant
{
    public interface ISubject
    {
        event Action Tick;
        event Action BreakStart;
        event Action BreakEnd;

        void TriggerBreakStart();
        void TriggerBreakEnd();
    }

    public class Subject : ISubject
    {
        public event Action Tick;

        public event Action BreakStart;

        public event Action BreakEnd;

        public void TriggerBreakStart() { BreakStart(); }
        
        public void TriggerBreakEnd() { BreakEnd(); }

        public void TriggerTick() { Tick(); }
    }
}
