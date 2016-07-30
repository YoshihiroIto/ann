using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ann.Foundation.Control
{
    public static class WindowsHelper
    {
        public static bool IsOnTrayMouseCursor => GetTrayRectangle().Contains(Cursor.Position);

        // https://stackoverflow.com/questions/6926281/c-sharp-toggle-window-by-clicking-notifyicon-taskbar-icon 

        // ReSharper disable InconsistentNaming
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // ReSharper restore InconsistentNaming

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className,
            IntPtr windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private static IntPtr GetTrayHandle()
        {
            var taskBarHandle = FindWindow("Shell_TrayWnd", null);

            return taskBarHandle.Equals(IntPtr.Zero)
                ? IntPtr.Zero
                : FindWindowEx(taskBarHandle, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero);
        }

        private static Rectangle GetTrayRectangle()
        {
            RECT rect;
            GetWindowRect(GetTrayHandle(), out rect);

            return
                new Rectangle(new Point(rect.left, rect.top),
                    new Size(rect.right - rect.left + 1, rect.bottom - rect.top + 1));
        }
    }
}