using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Flagellant.Workflow
{
    public class UIHandler
    {
        private const string buttonDownText = "The break will be available in: ";
        private Button managedButton;
        private TextBlock managedText;
        private TimeSpan timeLeft;
        private ISubject subject;
        private readonly Statistics statistics;

        public event Action BreakRequested;

        private void StartBreak()
        {
            timeLeft = new TimeSpan(0, 45, 0);
            subject.Tick += TimeTick;
            managedButton.IsEnabled = false;
        }

        private void EnableBreak()
        {
            managedButton.IsEnabled = true;
            managedText.Text = string.Empty;
            subject.Tick -= TimeTick;
        }

        private void TimeTick()
        {
            timeLeft.Subtract(TimeSpan.FromSeconds(1));
            if (timeLeft.CompareTo(TimeSpan.Zero) == -1)
            {
                EnableBreak();
            }
        }

        public WrapPanel GetRuntimePanel()
        {
            const int elementsHeight = 25;
            const int fontsSize = 12;
            WrapPanel panel = new WrapPanel();
            Button statsButton = new Button
            {
                Content = "Show statistics",
                FontSize = fontsSize,
                Height = elementsHeight
            };
            statsButton.Click += (o, e) =>
            {
                var stats = statistics.GetStatistics();
                StatisticsWindow w = new StatisticsWindow(stats);
                w.ShowDialog();
            };

            managedButton = new Button();
            managedButton.Width = double.NaN;
            managedButton.Height = elementsHeight;
            managedButton.FontSize = fontsSize;
            managedButton.Content = "Start the break";
            managedButton.Click += (o, e) => { BreakRequested(); StartBreak(); };

            managedText = new TextBlock();
            managedText.Height = elementsHeight;
            managedText.FontSize = fontsSize;
            managedText.Text = "Break is available";
            panel.Children.Add(managedButton);
            panel.Children.Add(managedText);
            return panel;
        }

        public UIHandler(ISubject subject, Statistics stats)
        {
            this.subject = subject;
            this.statistics = stats;
        }
    }
}
