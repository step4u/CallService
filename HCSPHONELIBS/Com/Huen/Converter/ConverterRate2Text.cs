using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class ConverterRate2Text : IMultiValueConverter
    {
        public object Convert(object[] value,
                Type targetType,
                object parameter,
                System.Globalization.CultureInfo culture)
        {
            double __value0 = double.Parse(value[0].ToString());
            int __value1 = int.Parse(value[1].ToString());

            string __out = string.Empty;

            if (__value1 < 60)
            {
                __out = string.Format("{0}원 / {1}초", __value0, __value1);
            }
            else
            {
                TimeSpan __ts = new TimeSpan(0, 0, 0, __value1);
                double __tmp = __ts.TotalMinutes % 1;
                if (__tmp > 0)
                {
                    __out = string.Format("{0}원 / {1}분{2}초", __value0, __ts.Minutes, __ts.Seconds);
                }
                else
                {
                    __out = string.Format("{0}원 / {1}분", __value0, __ts.Minutes);
                }
            }
            
            return __out;
        }

        //public object ConvertBack(object value,
        //                            Type[] targetType,
        //                            object parameter,
        //                            System.Globalization.CultureInfo culture)
        //{
        //    return new NotSupportedException("ConvertBack is not supported");
        //}

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
