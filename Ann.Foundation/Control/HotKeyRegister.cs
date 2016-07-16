// https://github.com/mok-aster/GlobalHotKey.NET を参考に実装

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Ann.Foundation.Control
{
    public class HotKeyRegister : IDisposable
    {
        public event EventHandler HotKeyPressed;

        public Key Key { get; }
        public ModifierKeys KeyModifier { get; }

        private readonly int _Id;
        private bool _isRegistered;
        private readonly IntPtr _Handle;

        // ReSharper disable once InconsistentNaming
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public HotKeyRegister(ModifierKeys modifierKeys, Key key, Window window)
        {
            Key = key;
            KeyModifier = modifierKeys;

            _Id = ++ _count;

            _Handle = new WindowInteropHelper(window).Handle;
        }

        public void Dispose()
        {
            Unregister();
        }

        private static int _count;

        public bool Register()
        {
            if (_isRegistered)
                Unregister();

            var vk = KeyInterop.VirtualKeyFromKey(Key);
            _isRegistered = RegisterHotKey(_Handle, _Id, KeyModifier, vk);

            if (_isRegistered == false)
                return false;

            if (_isRegistered)
                ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;

            return _isRegistered;
        }

        public void Unregister()
        {
            if (_isRegistered)
                ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;

            UnregisterHotKey(_Handle, _Id);

            _isRegistered = false;
        }

        private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
        {
            if (handled)
                return;

            if (msg.message != WM_HOTKEY)
                return;

            if ((int)msg.wParam != _Id)
                return;

            HotKeyPressed?.Invoke(this, EventArgs.Empty);

            handled = true;
        }
    }
}