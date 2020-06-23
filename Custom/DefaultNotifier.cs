using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Custom
{
    public class DefaultNotifier : IPrompter, IRuntimeResponder
    {
        private RuntimeUIHandler runtimeUIHandler;
        private List<NotifState> notifs;

        public void SetSubject(ISubject subject)
        {
            subject.Tick += TimeTick;
        }

        private void TimeTick()
        {
            for (int i = 0; i < notifs.Count; ++i)
            {
                var n = notifs[i];
                if (n.TicksLeft-- <= 0)
                {
                    n.TicksLeft = n.NextTicks;
                    if (n.TicksLeft == 0)
                    {
                        runtimeUIHandler.RemoveNotifRow(n);
                        notifs.RemoveAt(i);
                        i--;
                    }
                    NotifyUser(new DefaultNotification(n.Text));
                }
            }
        }

        public bool IsMute
        {
            set;
            private get;
        }

        public event Action<IPrompt> NotifyUser;

        public System.Windows.Controls.WrapPanel GetRuntimePanel()
        {
            return runtimeUIHandler.GetRuntimeUI();
        }

        public DefaultNotifier(List<NotifState> notifs)
        {
            this.notifs = notifs;
            this.runtimeUIHandler = new RuntimeUIHandler(notifs);
        }
    }
}
