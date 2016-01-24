using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class CDRData
    {
        public int INDEX;
        public string OFFICE_NAME = string.Empty;
        public DateTime STARTDATE;
        public DateTime ENDDATE;
        public int CALLER_TYPE;
        public int CALLEE_TYPE;
        public string CALLER = string.Empty;
        public string CALLER_IPN_NUMBER = string.Empty;
        public string CALLER_GROUP_CODE = string.Empty;
        public string CALLER_GROUP_NAME = string.Empty;
        public string CALLER_HUMAN_NAME = string.Empty;
        public string CALLEE = string.Empty;
        public string CALLEE_IPN_NUMBER = string.Empty;
        public string CALLEE_GROUP_CODE = string.Empty;
        public string CALLEE_GROUP_NAME = string.Empty;
        public string CALLEE_HUMAN_NAME = string.Empty;
        public int RESULT;
        public int SEQ;
    }
}
