using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;

namespace Ann.Foundation.Control.Behavior
{
    public sealed class WindowTaskTrayIconBehavior : Behavior<Window>
    {
        private NotifyIcon _notifyIcon = new NotifyIcon
        {
            Visible = false
        };

        protected override void OnAttached()
        {
            base.OnAttached();

            _notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    ShowAssociatedObject();

                else if (e.Button == MouseButtons.Right)
                    ShowContextMenu();
            };

            _notifyIcon.Visible = true;

            AssociatedObject.Closed += AssociatedObject_Closed;
            System.Windows.Application.Current.Deactivated += Application_Deactivated;
        }

        protected override void OnDetaching()
        {
            System.Windows.Application.Current.Deactivated -= Application_Deactivated;
            AssociatedObject.Closed -= AssociatedObject_Closed;

            base.OnDetaching();
        }

        private void ShowAssociatedObject()
        {
            AssociatedObject.Visibility = Visibility.Visible;

            AssociatedObject.Activate();
            AssociatedObject.Focus();
        }

        private void ShowContextMenu()
        {
            if (ContextMenu == null)
                return;

            ContextMenu.DataContext = AssociatedObject.DataContext;
            ContextMenu.IsOpen = true;

            var hwndSource = (HwndSource) PresentationSource.FromVisual(ContextMenu);
            if (hwndSource != null)
                SetForegroundWindow(hwndSource.Handle);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void AssociatedObject_Closed(object sender, EventArgs e)
        {
            _notifyIcon?.Dispose();
            _notifyIcon = null;
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
            if (ContextMenu == null)
                return;

            ContextMenu.IsOpen = false;
        }

        #region ToolTipText

        public string ToolTipText
        {
            get { return (string) GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register(
                nameof(ToolTipText),
                typeof(string),
                typeof(WindowTaskTrayIconBehavior),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = OnToolTipTextChanged,
                    DefaultValue = default(string),
                    BindsTwoWayByDefault = true
                }
                );

        private static void OnToolTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (WindowTaskTrayIconBehavior) d;

            self._notifyIcon.Text = self.ToolTipText;
        }

        #endregion

        #region IconSource

        public ImageSource IconSource
        {
            get { return (ImageSource) GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(
                nameof(IconSource),
                typeof(ImageSource),
                typeof(WindowTaskTrayIconBehavior),
                new FrameworkPropertyMetadata
                {
                    PropertyChangedCallback = OnIconSourceChanged,
                    DefaultValue = default(ImageSource),
                    BindsTwoWayByDefault = true
                }
                );

        private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (WindowTaskTrayIconBehavior) d;

            if (self._notifyIcon == null)
                return;

            if (WpfHelper.IsDesignMode)
                return;

            var resourceStream = System.Windows.Application.GetResourceStream(new Uri(self.IconSource.ToString()));
            if (resourceStream == null)
                return;

            self._notifyIcon.Icon = new Icon(resourceStream.Stream);
        }

        #endregion

        #region ContextMenu

        public System.Windows.Controls.ContextMenu ContextMenu
        {
            get { return (System.Windows.Controls.ContextMenu) GetValue(ContextMenuProperty); }
            set { SetValue(ContextMenuProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register(
                nameof(ContextMenu),
                typeof(System.Windows.Controls.ContextMenu),
                typeof(WindowTaskTrayIconBehavior),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = default(System.Windows.Controls.ContextMenu),
                    BindsTwoWayByDefault = true
                }
                );

        #endregion
    }
}