using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Ann.MainWindow
{
    /// <summary>
    /// ExecutableUnitPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class ExecutableUnitPanel
    {
        public ExecutableUnitPanel()
        {
            InitializeComponent();
        }

        private void PopupBox_Closed(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("未実装");
        }

        private void PopupBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("未実装");
        }
    }
}
