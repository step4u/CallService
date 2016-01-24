using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class Int2Language : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            string _outStr = string.Empty;
            int _value = int.Parse(value.ToString());

            switch (_value)
            {
                case 1:
                    _outStr = "EN";
                    break;
                case 2:
                    _outStr = "KR";
                    break;
                default:
                    _outStr = string.Empty;
                    break;
            }

            return _outStr;
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException("ConvertBack is not supported");
        }

    }
}
