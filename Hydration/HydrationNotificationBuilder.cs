using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Flagellant.Hydration
{
    public class HydrationNotificationBuilder : INotifierBuilderWData
    {
        private TextBox uiRepresentation;

        public WrapPanel ProvideUI()
        {
            WrapPanel main = new WrapPanel();
            TextBlock textBlock = new TextBlock
            {
                Text = "Today, so far I drank that many glasses (250 ml) of water: ",
                Width = double.NaN,
                Height = 30,
                FontSize = 12,
                Margin = new Thickness(5)
            };
            
            TextBox input = new TextBox
            {
                Width = 30,
                Height = 30,
                MaxLength = 2,
                FontSize = 12,
                Margin = new Thickness(5)
            };
            main.Children.Add(textBlock);
            main.Children.Add(input);
            uiRepresentation = input;
            return main;
        }

        public string ValidationMessage()
        {
            int inputNumber = 0;
            if (int.TryParse(uiRepresentation.Text, out inputNumber))
            {
                return string.Empty;
            }
            else
            {
                return "The drank glasses number cannot be parsed";
            }
        }

        public string CheckboxTitle()
        {
            return "Hydration";
        }

        public void VisitateBuilder(AppBuilder builder)
        {
            var notifier = new HydrationMonitor(int.Parse(uiRepresentation.Text));
            builder.AddRuntimeUI(notifier);
            builder.AddNotifier(notifier);
        }
    }
}
