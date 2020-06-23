using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flagellant.Prompt
{
    public abstract class MessageBoxPrompt : IPrompt
    {
        private Action<MessageBoxResult> callback;

        public abstract string Text
        {
            get;
        }

        public virtual MessageBoxImage Image { get { return MessageBoxImage.None; } }

        public virtual string Caption { get { return "Attention"; } }

        public virtual MessageBoxButton Buttons { get { return MessageBoxButton.OK; } }

        public virtual string VoiceFilePath { get { return string.Empty; } }

        public void DisplayOn(Window window)
        {
            var result = MessageBox.Show(window, Text, Caption, Buttons, Image);
            if (callback != null)
            {
                callback(result);
            }
        }

        public MessageBoxPrompt(Action<MessageBoxResult> callbackListener = null)
        {
            callback = callbackListener;
        }
    }
}
