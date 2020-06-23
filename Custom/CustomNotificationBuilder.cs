using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Custom
{
    public class CustomNotificationBuilder : INotifierBuilderWData
    {
        private UIHandler uiHandler;

        public System.Windows.Controls.WrapPanel ProvideUI()
        {
            return uiHandler.GetPreparationUI();
        }

        public string CheckboxTitle()
        {
            return "Custom notifications";
        }

        public void VisitateBuilder(AppBuilder builder)
        {
            builder.AddCustomNotifs(uiHandler.GetUserNotifs());
        }

        public string ValidationMessage()
        {
            return uiHandler.ValidateInput();
        }

        public CustomNotificationBuilder()
        {
            uiHandler = new UIHandler();
        }
    }
}
