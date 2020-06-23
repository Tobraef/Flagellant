using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Prompt
{
    public class HydrationPrompt : MessageBoxPrompt
    {
        private string toPost;

        public override string Text
        {
            get { return toPost; }
        }

        public HydrationPrompt(string text)
        {
            toPost = text;
        }
    }
}
