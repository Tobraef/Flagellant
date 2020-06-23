using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Flagellant.Workflow;

namespace Flagellant
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        private void InitializeData(List<HourModel> hours)
        {
            var datas = new ObservableCollection<ViewModel>();
            hours.ForEach(h => datas.Add(new ViewModel{ 
                Slack = h.Slack / 36,
                Work = h.Work / 36,
                Break = h.Break / 36,
                Hour = h.Hour.ToString(),
                ProcessToTicks = h.ProcessTicks
            }));
            datas.Add(new ViewModel
            {
                Work = 33,
                Break = 33,
                Slack = 34,
                Hour = "15",
                ProcessToTicks = new Dictionary<string, int> { {"a", 12 } }
            });
            datas.Add(new ViewModel
            {
                Work = 80,
                Break = 10,
                Slack = 10,
                Hour = "16",
                ProcessToTicks = new Dictionary<string, int> { { "a", 5 }, { "b", 100 } }
            });
            datas.Add(new ViewModel
            {
                Work = 30,
                Break = 10,
                Slack = 60,
                Hour = "17",
                ProcessToTicks = new Dictionary<string, int> { { "c", 12 } }
            });
            datas.Add(new ViewModel
            {
                Work = 50,
                Break = 20,
                Slack = 30,
                Hour = "18",
                ProcessToTicks = new Dictionary<string, int> { { "a", 20 }, { "c", 5 } }
            });
            datas.Add(new ViewModel
            {
                Work = datas.Sum(kvp => kvp.Work) / datas.Count,
                Break = datas.Sum(kvp => kvp.Break) / datas.Count,
                Slack = datas.Sum(kvp => kvp.Slack) / datas.Count,
                Hour = "Whole day",
                ProcessToTicks = new Dictionary<string,int>()
            });
            mainGrid.DataContext = datas;
            Dictionary<string, float> processSummary = new Dictionary<string, float>();
            int sum = 0;
            foreach (var data in datas)
            {
                foreach (var processData in data.ProcessToTicks)
                {
                    if (!processSummary.ContainsKey(processData.Key))
                    {
                        processSummary.Add(processData.Key, processData.Value);
                    }
                    else
                    {
                        processSummary[processData.Key] += processData.Value;
                    }
                    sum += processData.Value;
                }
            }
            ObservableCollection<KeyValuePair<string, float>> collction = new ObservableCollection<KeyValuePair<string, float>>();
            foreach (var kvp in processSummary)
            {
                collction.Add(new KeyValuePair<string, float>(kvp.Key, kvp.Value * 100 / sum));
            }
            panelProcessTime.DataContext = collction;
        }

        public StatisticsWindow(List<Workflow.HourModel> hours)
        {
            InitializeComponent();
            InitializeData(hours);
        }

        private class ViewModel
        {
            public int Work { get; set; }
            public int Slack { get; set; }
            public int Break { get; set; }
            public int Offset { get { return Work + Slack; } }
            public string Hour { get; set; }
            public Dictionary<string, int> ProcessToTicks { get; set; }
        }
    }
}
