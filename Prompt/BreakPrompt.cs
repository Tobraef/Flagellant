using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagellant.ProcessHandling;
using System.Diagnostics;
using System.Windows;

namespace Flagellant.Prompt
{
    public class EndOfBreakPrompt : MessageBoxPrompt 
    {
        public override string Text { get { return "Okay my boy, the break is over, back to work"; } }
        
        public override MessageBoxImage Image { get { return MessageBoxImage.Information; } }
        
        public override string Caption { get { return "Slacking time is over mate"; } }
    }
}
