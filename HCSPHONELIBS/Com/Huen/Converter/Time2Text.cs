using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class Time2Text : IMultiValueConverter
    {
        public object Convert(object[] value,
                Type targetType,
                object parameter,
                System.Globalization.CultureInfo culture)
        {
            int _value0 = int.Parse(value[0].ToString());
            int _value1 = int.Parse(value[1].ToString());

            string _out = string.Empty;

            if (_value0 == -1 && _value1 == -1)
            {
                _out = string.Empty;
            }
            else
            {
                _out = string.Format("{0:00}:{1:00}", _value0, _value1);
            }
            

            return _out;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
