using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Flagellant.Custom
{
    public class UIHandler
    {
        private Panel rowsUI;

        private void ConfigureRadioButton(RadioButton toPop, RadioButton toDown, 
            string name, IEnumerable<RadioButton> toShow, Panel mainPanel)
        {
            toPop.Width = double.NaN;
            toPop.Height = double.NaN;
            toPop.Content = name;
            toPop.Click += (o,e) =>
            {
                toPop.IsChecked = true;
                toDown.IsChecked = false;
                mainPanel.Children.Clear();
                foreach (var t in toShow) 
                { 
                    mainPanel.Children.Add(t);
                }
                if (toShow.Count() > 0)
                {
                    toShow.First().IsChecked = true;
                }
            };
        }

        public string ValidateInput()
        {
            foreach (var row in rowsUI.Children.OfType<TimerRow>())
            {
                string validation = row.ValidateInput();
                if (!string.IsNullOrEmpty(validation))
                {
                    return validation;
                }
            }
            return string.Empty;
        }

        private class TimerRow : WrapPanel
        {
            private RadioButton oneTime;
            private Panel timeChoicePanel;
            private TextBox timeBox;
            private TextBox titleBox;

            private string VerifyTime(string incorrectValue, string received, int max)
            {
                string toRet = string.Empty;
                int timePart = 0;
                if (int.TryParse(received, out timePart))
                {
                    if (timePart >= max)
                    {
                        toRet = "Incorrect " + incorrectValue + " input\n";
                    }
                }
                else
                {
                    toRet = "Incorrect " + incorrectValue +  " input\n";
                }
                return toRet;
            }

            public string ValidateInput()
            {
                string toRet = string.Empty;
                if (oneTime.IsChecked.Value && timeChoicePanel.Children.Cast<RadioButton>().All(s => !s.IsChecked.Value))
                {
                    return "No choice made for row!";
                }
                if (string.IsNullOrEmpty(titleBox.Text))
                {
                    toRet += "Title cannot be empty\n";
                }
                var timeInput = timeBox.Text.Split(':');
                if (timeInput.Count() != 3)
                {
                    return "Incorrect time input";
                }
                toRet += VerifyTime("hour", timeInput[0], 24);
                toRet += VerifyTime("minute", timeInput[1], 60);
                toRet += VerifyTime("second", timeInput[2], 60);
                if (string.IsNullOrEmpty(toRet) && !IsNotifyAfter() && oneTime.IsChecked.Value)
                {
                    TimeSpan time = new TimeSpan(int.Parse(timeInput[0]), int.Parse(timeInput[1]), int.Parse(timeInput[2]));
                    if (DateTime.Now.TimeOfDay > time)
                    {
                        toRet += "When specifid time option is chosen, cannot input time before current\n";
                    }
                }
                return toRet;
            }

            private bool IsNotifyAfter()
            {
                return oneTime.IsChecked.Value && timeChoicePanel.Children.OfType<RadioButton>().Last().IsChecked.Value;
            }

            public NotifState CreateNotifier()
            {
                string[] timeText = timeBox.Text.Split(':');
                TimeSpan time = new TimeSpan(int.Parse(timeText[0]), int.Parse(timeText[1]), int.Parse(timeText[2]));
                if (oneTime.IsChecked.Value)
                {
                    if (IsNotifyAfter())
                    {
                        return new NotifState
                        {
                            NextTicks = 0,
                            Text = titleBox.Text,
                            TicksLeft = (int)(time.TotalSeconds)
                        };
                    }
                    else
                    {
                        return new NotifState
                        {
                            NextTicks = 0,
                            Text = titleBox.Text,
                            TicksLeft = (int)((time - DateTime.Now.TimeOfDay).TotalSeconds)
                        };
                    }
                }
                else
                {
                    int ticks = (int)time.TotalSeconds;
                    return new NotifState
                    {
                        NextTicks = ticks,
                        Text = titleBox.Text,
                        TicksLeft = ticks
                    };
                }
            }

            public TimerRow(RadioButton oneTime, Panel choicePanel, TextBox timeBox, TextBox titleBox)
            {
                this.oneTime = oneTime;
                this.timeChoicePanel = choicePanel;
                this.timeBox = timeBox;
                this.titleBox = titleBox;
            }
        }

        private RadioButton ConfigureTimeButton(string name, IEnumerable<RadioButton> group)
        {
            RadioButton b = new RadioButton();
            b.Content = name;
            b.Width = double.NaN;
            b.Height = double.NaN;
            b.IsChecked = false;
            b.Click += (o, e) =>
            {
                foreach (var t in group)
                {
                    t.IsChecked = false;
                }
                b.IsChecked = true;
            };
            return b;
        }

        private TimerRow ConstructRow()
        {
            RadioButton oneTime = new RadioButton();
            RadioButton repeat = new RadioButton();

            List<RadioButton> disableGroup = new List<RadioButton>();
            RadioButton specificTimeButton = ConfigureTimeButton("At specific time", disableGroup);
            RadioButton everySomeTimeButton = ConfigureTimeButton("Notify every..", disableGroup);
            RadioButton notifyAfterButton = ConfigureTimeButton("Notify after", disableGroup);
            disableGroup.AddRange(new List<RadioButton>{ specificTimeButton, everySomeTimeButton, notifyAfterButton });

            TextBox timeBox = new TextBox();
            timeBox.Width = 70;
            timeBox.Height = 25;
            timeBox.FontSize = 12;
            timeBox.Text = "00:00:00";
            timeBox.MaxLength = 8;

            List<RadioButton> oneTimeButtons = new List<RadioButton> {
                specificTimeButton, notifyAfterButton
            };
            List<RadioButton> repeatButtons = new List<RadioButton> {};
    
            TextBox titleBox = new TextBox();
            titleBox.Margin = new Thickness(5);
            titleBox.Width = 200;
            titleBox.Height = 25;
            titleBox.FontSize = 12;
            titleBox.MaxLength = 75;

            StackPanel choicePanel = new StackPanel();
            choicePanel.Margin = new Thickness(5);
            choicePanel.Children.Add(oneTime);
            choicePanel.Children.Add(repeat);

            StackPanel timePanel = new StackPanel();
            timePanel.Margin = new Thickness(5);
            ConfigureRadioButton(oneTime, repeat, "One time", oneTimeButtons, timePanel);
            ConfigureRadioButton(repeat, oneTime, "Repeat", repeatButtons, timePanel);

            TimerRow row = new TimerRow(oneTime, timePanel, timeBox, titleBox);
            row.Children.Add(choicePanel);
            row.Children.Add(timePanel);
            row.Children.Add(timeBox);
            row.Children.Add(titleBox);
            return row;
        }

        private Button ConfigureRemoveButton(TimerRow toRemove, Panel from)
        {
            Button b = new Button();
            b.Content = "X";
            b.FontSize = 13;
            b.Width = double.NaN;
            b.Height = 20;
            b.Click += (o, e) =>
            {
                from.Children.Remove(toRemove);
            };
            return b;
        }

        public WrapPanel GetPreparationUI()
        {
            WrapPanel main = new WrapPanel();
            StackPanel rows = new StackPanel();

            Button addButton = new Button();
            addButton.Width = double.NaN;
            addButton.Margin = new Thickness(5);
            addButton.Height = 25;
            addButton.Content = "Add";
            addButton.Click += (o, e) =>
            {
                var row = ConstructRow();
                var button = ConfigureRemoveButton(row, rows);
                row.Children.Add(button);
                rows.Children.Add(row);
            };
            main.Children.Add(rows);
            main.Children.Add(addButton);
            rowsUI = rows;
            return main;
        }

        public List<NotifState> GetUserNotifs()
        {
            return rowsUI.Children.OfType<TimerRow>().Select(r => r.CreateNotifier()).ToList();
        }
    }

    public class RuntimeUIHandler
    {
        private List<NotifState> activeNotifications;
        private StackPanel rows;

        public WrapPanel GetRuntimeUI()
        {
            WrapPanel main = new WrapPanel();
            StackPanel rows = new StackPanel();
            foreach (var n in activeNotifications)
            {
                WrapPanel row = new WrapPanel();
                TextBlock text = new TextBlock();
                text.FontSize = 12;
                text.Width = double.NaN;
                text.Height = double.NaN;
                text.Text = n.Text;
                text.Margin = new Thickness(5);

                Button removeButton = new Button();
                removeButton.Content = "X";
                removeButton.Height = double.NaN;
                removeButton.Width = double.NaN;
                removeButton.Click += (o, e) =>
                {
                    activeNotifications.Remove(n);
                    rows.Children.Remove(row);
                };
                removeButton.Margin = new Thickness(5);
                row.Children.Add(text);
                row.Children.Add(removeButton);
                rows.Children.Add(row);
            }
            this.rows = rows;
            main.Children.Add(rows);
            return main;
        }

        public void RemoveNotifRow(NotifState notif)
        {
            rows.Dispatcher.Invoke(() => rows.Children.Remove(
                rows.Children.OfType<WrapPanel>()
                    .First(panel => panel.Children.OfType<TextBlock>().Single().Text.Equals(notif.Text))));
        }

        public RuntimeUIHandler(List<NotifState> notifs)
        {
            this.activeNotifications = notifs;
        }
    }
}
