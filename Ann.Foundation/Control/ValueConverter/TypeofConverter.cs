using System;
using System.Globalization;
using System.Windows.Data;

namespace Ann.Foundation.Control.ValueConverter
{
    public class TypeofConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}