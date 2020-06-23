using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Flagellant.Prompt;

namespace Flagellant.Hydration
{
    public class HydrationMonitor : IPrompter, IRuntimeResponder
    {
        private const int glassOfWater = 250;
        private int userHydrationML;
        private const int ticksToOneML = 10;
        private int currentTickToML = 0;

        public void SetSubject(ISubject subject)
        {
            subject.Tick += HandleTimeTick;
        }

        private void HandleTimeTick()
        {
            currentTickToML++;
            if (currentTickToML == ticksToOneML)
            {
                userHydrationML--;
                if (userHydrationML >= 0)
                {
                    NotifyUserShouldDrink();
                }
            }
        }

        public event Action<IPrompt> NotifyUser;

        private void NotifyUserShouldDrink()
        {
            NotifyUser(new HydrationPrompt("You should have a glass of water now. Please press OK only if you have drank one. One glass of water is calculated as 250 ml"));
            userHydrationML += glassOfWater;
        }

        public bool IsMute
        {
            private get;
            set;
        }

        private TimeSpan CalculateApproximateDuration()
        {
            return TimeSpan.FromSeconds(userHydrationML * ticksToOneML);
        }

        private string MakeHydrationMessage()
        {
            if (userHydrationML < 200)
            {
                return "you should have a glass of water soon, up to one hour.";
            }
            if (userHydrationML < 500)
            {
                return "your hydration level is okay.";
            }
            if (userHydrationML < 750)
            {
                return "your hydration is almost too good, don't drink for some time.";
            }
            else
            {
                return "you should avoid drinking for now.";
            }
        }

        private void UserDrankWater()
        {
            userHydrationML += glassOfWater;
            NotifyUser(new HydrationPrompt("Nice, so far " + MakeHydrationMessage()));
        }

        private void ShowCurrentStats()
        {
            NotifyUser(new HydrationPrompt("So far " + MakeHydrationMessage() + " Your hydration level will be approximetly sufficient for "
                + CalculateApproximateDuration().ToString()));
        }

        public WrapPanel GetRuntimePanel()
        {
            WrapPanel panel = new WrapPanel();
            const int height = 30;
            Button statistics = new Button();
            Thickness thickness = new Thickness(5);
            statistics.Width = double.NaN;
            statistics.Height = height;
            statistics.FontSize = 12;
            statistics.Margin = thickness;
            statistics.Content = "Current hydration";
            statistics.Click += (o, e) => ShowCurrentStats();

            TextBlock block = new TextBlock
            {
                Width = double.NaN,
                Height = height,
                Margin = thickness,
                Text = "Press, if you drank a glass",
                FontSize = 12
            };

            Button drankWater = new Button();
            drankWater.Width = double.NaN;
            drankWater.Height = height;
            drankWater.FontSize = 12;
            drankWater.Margin = thickness;
            drankWater.Content = "Cheers!";
            drankWater.Click += (o, e) => UserDrankWater();
            panel.Children.Add(statistics);
            panel.Children.Add(block);
            panel.Children.Add(drankWater);
            return panel;
        }

        public HydrationMonitor(int drankGlasses)
        {
            var mlRequiredSoFar = (int)System.DateTime.Now.TimeOfDay.TotalSeconds / ticksToOneML;
            userHydrationML = (250 * drankGlasses) - mlRequiredSoFar;
        }
    }
}
