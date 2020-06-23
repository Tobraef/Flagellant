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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Flagellant.ProcessHandling;

namespace Flagellant.Prompt
{
    public class BadPrompt : MessageBoxPrompt
    {
        public override string Text { get { return "Get back to work you lazy ass, you have now been working " +
                "too much recently, haven't you? BACK TO WORK OR I WILL MESS YOU UP!"; } }
        public override MessageBoxImage Image { get { return MessageBoxImage.Warning; } }
        
        public override string Caption { get { return "This is getting bad..."; } }
    }

    public class CriticalPrompt : MessageBoxPrompt
    {
        public override string Text { get { return "Okay, listen here you dipshit, GET BACK TO WORK OR WAIT, I'LL FORCE YOU TO WORK, FUCK YOUR PROGRESS, BACK TO WORK NOW"; } }
        
        public override MessageBoxImage Image { get { return MessageBoxImage.Stop; } }
        
        public override string Caption { get { return "This is getting bad..."; } }
    }

    public class ChaosPrompt : MessageBoxPrompt
    {
        public override string Text { get { return "THIS IS OVER NOW, YOU HAVE TRIGGERED ME TO THE EXTREME, ENJOY YOUR SUFFERING UNTILL YOU LEARN HOW TO WORK PROPERLY"; } }
       
        public override MessageBoxImage Image { get { return MessageBoxImage.Stop; } }

        public override string Caption { get { return "This is getting bad..."; } }
    }
}
