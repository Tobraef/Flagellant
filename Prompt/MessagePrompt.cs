using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Prompt
{
    class MessagePrompt : Prompt.MessageBoxPrompt
    {
        private string text;

        public override string Text
        {
            get { return text; ; }
        }

        public MessagePrompt(string text)
        {
            this.text = text;
        }
    }
}
