using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Diagnostics;
using System.Net;

namespace Flagellant.ProcessHandling
{
    public abstract class BrowserHandler
    {
        public abstract string BrowserProcessName
        {
            get;
        }

        public abstract string ExtractUrl(Process p);

        private static bool Ping(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                req.Method = "HEAD";
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    return resp.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        public static bool PingWebsite(string url)
        {
            if (url.Contains("http"))
            {
                return Ping(url);
            }
            else
            {
                string raw = url;
                url = raw.Insert(0, "https://");
                if (Ping(url)) { return true; }
                url = raw.Insert(0, "http://");
                return Ping(url);
            }
        }

        public static string TrimToMainUrl(string url)
        {
            string toRet = url;
            if (toRet.IndexOf("http") == 0)
            {
                toRet = toRet.Substring(toRet.IndexOf('/') + 1);
            }
            var lastIndex = toRet.IndexOf('/');
            if (lastIndex == -1)
                return url;
            return toRet.Substring(0, lastIndex);
        }
    }

    public class ChromeHandler : BrowserHandler
    {
        public override string BrowserProcessName
        {
            get { return "chrome"; }
        }

        public override string ExtractUrl(Process pr)
        {
            AutomationElement elm = AutomationElement.FromHandle(pr.MainWindowHandle);
            var e = elm.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            if (e == null)
            {
                return "youtube.com";
            }
            
            var patterns = e.GetSupportedPatterns();
            if (patterns.Length > 0)
            {
                foreach (var p in patterns)
                {
                    ValuePattern val = (ValuePattern)e.GetCurrentPattern(p);
                    return string.IsNullOrEmpty(val.Current.Value) ? string.Empty : BrowserHandler.TrimToMainUrl(val.Current.Value);
                }
            }
            return string.Empty;
        }
    }

    public class FirefoxHandler : BrowserHandler
    {
        public override string BrowserProcessName
        {
            get { return "firefox"; }
        }

        public override string ExtractUrl(Process p)
        {
            AutomationElement root = AutomationElement.FromHandle(p.MainWindowHandle);

            Condition toolBar = new AndCondition(
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),
            new PropertyCondition(AutomationElement.NameProperty, "Browser tabs"));
            var tool = root.FindFirst(TreeScope.Children, toolBar);

            var tool2 = TreeWalker.ControlViewWalker.GetNextSibling(tool);

            var children = tool2.FindAll(TreeScope.Children, Condition.TrueCondition);

            foreach (AutomationElement item in children)
            {
                foreach (AutomationElement i in item.FindAll(TreeScope.Children, Condition.TrueCondition))
                {
                    foreach (AutomationElement ii in i.FindAll(TreeScope.Children, Condition.TrueCondition))
                    {
                        if (ii.Current.LocalizedControlType == "document")
                        {
                            if (!ii.Current.BoundingRectangle.X.ToString().Contains("Infinity"))
                            {
                                ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                                var activeUrl = activeTab.Current.Value;
                                return activeUrl;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
