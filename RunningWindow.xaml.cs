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
using System.Timers;

namespace Flagellant
{
    /// <summary>
    /// Interaction logic for RunningWindow.xaml
    /// </summary>
    public partial class RunningWindow : Window
    {
        private AppLogic app;
        private Timer timer = new Timer();
        private static TextBox textBoxLog = new TextBox
        {
            Margin = new Thickness(0, 10, 0, 0),
            Width = 300,
            Height = 100,
            FontSize = 10,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            TextWrapping = TextWrapping.Wrap
        };

        public static void Log(string info)
        {
            textBoxLog.Dispatcher.Invoke(() => textBoxLog.AppendText(info));
        }

        public RunningWindow(AppLogic logic)
        {
            InitializeComponent();
            panelMain.Children.Add(textBoxLog);
            app = logic;
            app.SetDisplay(this);
            panelMain.Children.Add(app.CreateUI());
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            app.Tick();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            app = null;
            timer.Stop();
            timer = null;
            Close();
        }
    }
}
