using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Flagellant.ProcessHandling
{
    public static class ProcessTools
    {
        public static class LowLevel
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("User32.dll")]
            public static extern bool SetForegroundWindow(IntPtr handle);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            public delegate bool EnumWindowsDelegate(IntPtr handle, IntPtr parameters);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

            [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowText(IntPtr windowHandle, StringBuilder stringBuilder, int nMaxCount);

            [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
            internal static extern int GetWindowTextLength(IntPtr hwnd);

            [DllImport("user32.dll")]
            public static extern bool IsWindowVisible(IntPtr hwnd);

            [DllImport("user32.dll")]
            public static extern bool IsWindowEnabled(IntPtr hWnd);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern void ShowWindow(IntPtr hWnd, int nCmdShow);
        }

        public static void KillCurrent()
        {
            var current = LowLevel.GetForegroundWindow();
            UInt32 WM_CLOSE = 0x0010;
            IntPtr _zero = IntPtr.Zero;
            IntPtr success = LowLevel.SendMessage(current, WM_CLOSE, _zero, _zero);
        }

        public static void PopProcess(Process p)
        {
            LowLevel.ShowWindow(p.MainWindowHandle, 9);
            LowLevel.SetForegroundWindow(p.MainWindowHandle);
        }

        public static void PopThis(System.Windows.Window window)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            LowLevel.ShowWindow(handle, 9);
            LowLevel.SetForegroundWindow(handle);
        }

        public static Process GetCurrentWorkingProcess()
        {
            try
            {
                return Process.GetProcesses().FirstOrDefault(p => p.MainWindowHandle == LowLevel.GetForegroundWindow());
            }
            catch (System.ComponentModel.Win32Exception e) { RunningWindow.Log(e.Message + " " + e.NativeErrorCode + " " + e.Source + " " + e.StackTrace); }
            return null;
        }
    }
}
