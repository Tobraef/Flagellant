using System;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Net;
using Flagellant.Custom;

namespace Flagellant
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window child;
        private string browser;
        private Dictionary<CheckBox, INotifierBuilder> notifiers = new Dictionary<CheckBox,INotifierBuilder>();
        private Dictionary<CheckBox, INotifierBuilderWData> uiContainers = new Dictionary<CheckBox, INotifierBuilderWData>();
        private static TextBox s_logging;

        public static void Log(string text)
        {
            s_logging.Dispatcher.Invoke(() => s_logging.AppendText(text + "\n"));
        }

        public MainWindow()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            InitializeComponent();
            s_logging = textBoxLogger;
            FillWithProcesses();
            AppendBuilders();
            SetupUI();
        }

        private void AppendBuilders()
        {
            var customNotifier = new CustomNotificationBuilder();
            var hydrationMonitor = new Hydration.HydrationNotificationBuilder();
            var workouter = new Workout.WorkoutNotifyBuilder();

            AppendUIBuilder(workouter);
            AppendUIBuilder(customNotifier);
            AppendUIBuilder(hydrationMonitor);
        }

        private void AppendSimpleBuilder(INotifierBuilder b)
        {
            var box = new CheckBox();
            notifiers.Add(box, b);
        }

        private void AppendUIBuilder(INotifierBuilderWData b)
        {
            var box = new CheckBox();
            uiContainers.Add(box, b);
        }

        private void SetupUI()
        {
            foreach (var pair in notifiers)
            {
                var box = pair.Key;
                box.Content = "Trigger " + pair.Value.CheckboxTitle().ToLower();
                box.IsChecked = false;
                panelBoxes.Children.Add(box);
            }
            foreach (var pair in uiContainers)
            {
                var box = pair.Key;
                box.Content = "Trigger " + pair.Value.CheckboxTitle().ToLower();
                box.IsChecked = false;
                var ui = pair.Value.ProvideUI();
                ui.Children.Insert(0, new Border
                {
                    Child = new TextBlock
                        {
                            Width = ui.Width,
                            Height = 25,
                            FontSize = 12,
                            Text = pair.Value.CheckboxTitle()
                        },
                    Margin = new Thickness(5),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black)
                });
                ui.Children.OfType<Border>().First().Width += 5;
                ui.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                var border = new Border
                {
                    Padding = new Thickness(5),
                    Child = ui,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5)
                };
                box.Checked += (o, e) =>
                {
                    panelBuildersUI.Children.Add(border);
                };
                box.Unchecked += (o, e) =>
                {
                    panelBuildersUI.Children.Remove(border);
                };
                panelBoxes.Children.Add(box);
            }
        }

        public class ProcessInfo 
        {
            public Process Process
            {
                get;
                set;
            }

            public string Url
            {
                get;
                set;
            }

            public override string ToString()
            {
                return Process == null ? Url : Process.ProcessName + "/" + Process.MainWindowTitle;
            }
        }

        private IEnumerable<string> GetPopularSlackingWebsites()
        {
            yield return "jbzd.com.pl";
            yield return "Facebook.com";
            yield return "youtube.com";
            yield return "twitch.tv";
        }

        private void FillWithProcesses()
        {
            ProcessHandling.ProcessManager mgr = new ProcessHandling.ProcessManager();
            var openStuff = mgr.LoadDesktopProcesses();
            var webProcess = openStuff.FirstOrDefault(o => o.ProcessName.Equals("chrome") || o.ProcessName.Equals("firefox"));
            if (webProcess != null)
            {
                browser = webProcess.ProcessName;
                openStuff.Remove(webProcess);
            }
            openStuff.ForEach(p => textBoxMiddle.Items.Add(new ProcessInfo{ Process = p }));
            foreach (var w in GetPopularSlackingWebsites())
            {
                textBoxMiddle.Items.Add(new ProcessInfo { Url = w });
            }
        }

        private void buttonBeginWork_Click(object sender, RoutedEventArgs e)
        {
            AppBuilder builder = new AppBuilder();
            foreach (var pair in uiContainers.Where(kvp => kvp.Key.IsChecked.Value))
            {
                var validation = pair.Value.ValidationMessage();
                if (string.IsNullOrEmpty(validation))
                {
                    pair.Value.VisitateBuilder(builder);
                }
                else
                {
                    MessageBox.Show(validation);
                    return;
                }
            }
            foreach (var pair in notifiers.Where(kvp => kvp.Key.IsChecked.Value))
            {
                pair.Value.VisitateBuilder(builder);
            }
            if (checkBoxWork.IsChecked.Value) { builder.AddWorkFlowPrompter(textBoxRight.Items.Cast<ProcessInfo>().ToList(), textBoxLeft.Items.Cast<ProcessInfo>().ToList(), browser); }
            child = new RunningWindow(builder.Build(!checkBoxSound.IsChecked.Value));
            child.Show();
            this.Close();
        }

        private void MoveFromTo(ListBox from, ListBox to)
        {
            var item = from.SelectedItem;
            if (item == null) return;
            from.Items.Remove(item);
            to.Items.Add(item);
            from.SelectedIndex = from.Items.Count - 1;
        }

        private void moveLeftButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            MoveFromTo(textBoxMiddle, textBoxLeft);
        }

        private void moveRightButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            MoveFromTo(textBoxLeft, textBoxMiddle);
        }

        private void moveRightButtonRight_Click(object sender, RoutedEventArgs e)
        {
            MoveFromTo(textBoxRight, textBoxMiddle);
        }

        private void moveLeftButtonRight_Click(object sender, RoutedEventArgs e)
        {
            MoveFromTo(textBoxMiddle, textBoxRight);
        }

        private void buttonURLInput_Click(object sender, RoutedEventArgs e)
        {
            var input = textBoxURLInput.Text;
            if (ProcessHandling.BrowserHandler.PingWebsite(input))
            {
                textBoxMiddle.Items.Add(new ProcessInfo { Url = input });
                textBoxMiddle.SelectedIndex = textBoxMiddle.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("Couldn't connect to given URL.");
            } 
            textBoxURLInput.Text = string.Empty;
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 5000;
            t.Start();
            t.Elapsed += (o, x) =>
            {
                Log("Killin");
                ProcessHandling.ProcessTools.KillCurrent();
                t.Enabled = false;
                Log("Done");
            };
        }
    }
}
