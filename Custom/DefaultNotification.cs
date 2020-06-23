using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagellant.Prompt;

namespace Flagellant.Custom
{
    class DefaultNotification : MessageBoxPrompt
    {
        private string text;

        public override string Text
        {
            get { return text; }
        }

        public DefaultNotification(string text)
        {
            this.text = text;
        }
    }
}
