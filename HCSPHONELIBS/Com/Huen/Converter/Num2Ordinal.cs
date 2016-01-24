using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class Num2Ordinal : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            string oStr = string.Empty;

            if (value == null || string.IsNullOrEmpty(value.ToString())) return oStr;

            string strFloor = value.ToString();
            int val = -1;

            try
            {
                val = int.Parse(strFloor);
            }
            catch
            {
                var tmp = strFloor.Split(' ');
                if (tmp.Length > 0)
                {
                    string _tmp = tmp[0].ToString();
                    _tmp = _tmp.Substring(0, _tmp.Length - 2);
                    val = int.Parse(_tmp);
                }
            }

            if (val >= 11 && val < 20)
            {
                oStr = string.Format("{0}th Floor", val);
            }
            else
            {
                char lastletter = strFloor.LastOrDefault();
                switch (lastletter)
                {
                    case '1':
                        oStr = string.Format("{0}st Floor", val);
                        break;
                    case '2':
                        oStr = string.Format("{0}nd Floor", val);
                        break;
                    case '3':
                        oStr = string.Format("{0}rd Floor", val);
                        break;
                    default:
                        oStr = string.Format("{0}th Floor", val);
                        break;
                }
            }

            return oStr;
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
