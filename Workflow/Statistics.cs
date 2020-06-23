using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Workflow
{
    public class HourModel
    {
        public int Hour
        {
            get;
            private set;
        }

        public int Work
        {
            get;
            set;
        }

        public int Slack
        {
            get;
            set;
        }

        public int Break
        {
            get;
            set;
        }

        public Dictionary<string, int> ProcessTicks
        {
            get;
            private set;
        }

        public HourModel(int hour)
        {
            Hour = hour;
            ProcessTicks = new Dictionary<string, int>();
        }
    }

    public class Statistics
    {
        private readonly List<HourModel> hours;
        private HourModel currentHour;
        private int hourTicksLeft;

        private void BeginCounting()
        {
            var currentTime = System.DateTime.Now;
            hourTicksLeft = 3600 - currentTime.Minute * 60 - currentTime.Second;
            currentHour = new HourModel(currentTime.Hour);
            currentHour.Break = 3600 - hourTicksLeft;
        }

        private void HandleHour()
        {
            if (--hourTicksLeft <= 0)
            {
                hours.Add(currentHour);
                currentHour = new HourModel(hours.Last().Hour + 1);
            }
        }

        public void WorkTick(string processName)
        {
            if (!currentHour.ProcessTicks.ContainsKey(processName))
            {
                currentHour.ProcessTicks.Add(processName, 1);
            }
            else
            {
                currentHour.ProcessTicks[processName]++;
            }
            currentHour.Work++;
            HandleHour();
        }

        public void SlackTick(string processName)
        {
            if (!currentHour.ProcessTicks.ContainsKey(processName))
            {
                currentHour.ProcessTicks.Add(processName, 1);
            }
            else
            {
                currentHour.ProcessTicks[processName]++;
            }
            currentHour.Slack++;
            HandleHour();
        }

        public void BreakFinished()
        {
            var currentTime = System.DateTime.Now;
            var soFarLeft = 3600 - currentTime.Minute * 60 - currentTime.Second;
            var hourDifference = currentTime.Hour - currentHour.Hour;
            if (hourDifference == 0)
            {
                int breakDuration = hourTicksLeft - soFarLeft;
                currentHour.Break += breakDuration;
            }
            else
            {
                currentHour.Break += hourTicksLeft;
                hours.Add(currentHour);
                for (int i = 1; i < hourDifference; ++i)
                {
                    hours.Add(new HourModel(currentHour.Hour + i){ Break = 3600 });
                }
                currentHour = new HourModel(currentHour.Hour + hourDifference){ Break = 3600 - soFarLeft };
            }
            hourTicksLeft = soFarLeft;
        }

        public List<HourModel> GetStatistics()
        {
            return hours.ToList();
        }

        public Statistics()
        {
            BeginCounting();
            hours = new List<HourModel>();
        }
    }
}
