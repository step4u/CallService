using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;

namespace Com.Huen.Libs
{
    class CompareString : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, true);
        }
    }
}
