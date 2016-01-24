using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class HotelClasses
    {
    }

    public class floors
    {
        public string floornum { get; set; }
    }

    public class chkinroom
    {
        public string roomnum { get; set; }
        public int language = -1;
        public string[] allowed = new string[3];
        public string[] forbidden = new string[3];
    }

    public class chroom
    {
        public string roomnum { get; set; }
        public string nextroomnum { get; set; }
    }

    public class mcallroom
    {
        public string roomnum = string.Empty;
        public string week = string.Empty;
        public int hour;
        public int min;
        public int repeat_week = 0;
    }

    public class calllog
    {
        public string roomnum = string.Empty;
        public string sdate = string.Empty;
        public string edate = string.Empty;
    }

    public class ReturnResult
    {
        public string roomnum { get; set; }
        public string nextroom { get; set; }
        public string mtimer = string.Empty;
        public int language = -1;
        public bool result;
    }
}
