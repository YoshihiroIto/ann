using System;
using System.Globalization;
using System.Windows.Data;

namespace Ann.Core
{
    public class StringTagToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length != 2)
                return null;

            if (values[0] is StringTags == false)
                return null;

            if (values[1] is App == false)
                return null;

            var tag = (StringTags) values[0];
            var app = (App) values[1];

            return app.GetString(tag);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}