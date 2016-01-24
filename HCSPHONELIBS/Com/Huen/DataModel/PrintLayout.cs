﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Com.Huen.DataModel
{
    public class PrintLayout
    {
        public static readonly PrintLayout A4 = new PrintLayout("29.7cm", "42cm", "3.18cm", "2.54cm");
        //public static readonly PrintLayout A4 = new PrintLayout("29.7cm", "42cm", "1.5cm", "1.5cm");
        public static readonly PrintLayout A4Landscape = new PrintLayout("42cm", "29.7cm", "2.54cm", "3.18cm");
        public static readonly PrintLayout A4Narrow = new PrintLayout("29.7cm", "42cm", "1.27cm", "1.27cm");
        public static readonly PrintLayout A4Moderate = new PrintLayout("29.7cm", "42cm", "1.91cm", "2.54cm");

        private Size _Size;
        private Thickness _Margin;

        public PrintLayout(string w, string h, string leftright, string topbottom)
            : this(w, h, leftright, topbottom, leftright, topbottom)
        {
        }

        public PrintLayout(string w, string h, string left, string top, string right, string bottom)
        {
            var converter = new LengthConverter();
            var width = (double)converter.ConvertFromInvariantString(w);
            var height = (double)converter.ConvertFromInvariantString(h);
            var marginLeft = (double)converter.ConvertFromInvariantString(left);
            var marginTop = (double)converter.ConvertFromInvariantString(top);
            var marginRight = (double)converter.ConvertFromInvariantString(right);
            var marginBottom = (double)converter.ConvertFromInvariantString(bottom);
            this._Size = new Size(width, height);
            this._Margin = new Thickness(marginLeft, marginTop, marginRight, marginBottom);

        }


        public Thickness Margin
        {
            get { return _Margin; }
            set { _Margin = value; }
        }

        public Size Size
        {
            get { return _Size; }
        }

        public double ColumnWidth
        {
            get
            {
                var column = 0.0;
                column = this.Size.Width - Margin.Left - Margin.Right;
                return column;
            }
        }
    }
}
