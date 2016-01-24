using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

using Com.Huen.Libs;

namespace Com.Huen.Converter
{
    public class ChangeInt2Color : IValueConverter
    {
        public object Convert(object value,
        Type targetType,
        object parameter,
        System.Globalization.CultureInfo culture)
        {
            int val = (int)value;

            SolidColorBrush color = new SolidColorBrush();

            switch (val)
            {
                case 0:
                    color = new SolidColorBrush(Colors.White);
                    break;
                case 1:
                    color = new SolidColorBrush(Colors.Red);
                    break;
                case 2:
                    color = new SolidColorBrush(Colors.Green);
                    break;
                case 3:
                    color = new SolidColorBrush(Colors.White);
                    break;
            }

            return color;
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
