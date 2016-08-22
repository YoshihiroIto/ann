using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Ann.Core;

namespace Ann.MainWindow
{
    public class MessagesConverter : IMultiValueConverter  
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var messages = (StatusBarItemViewModel.Message[])values[0];
            var app = (App)values[1];

            return string.Join(string.Empty, messages.Select(m =>
            {
                var s = app.GetString(m.String);

                return m.Options != null ? string.Format(s, m.Options) : s;
            }));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}