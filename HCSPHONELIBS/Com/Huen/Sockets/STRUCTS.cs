using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace Com.Huen.Sockets
{
    public class STRUCTS
    {
        public const int PMS_SET_MORNING_CALL_REQ = 291;
        public const int PMS_SET_MORNING_CALL_RES = 292;
        public const int PMS_GET_MORNING_CALL_REQ = 293;
        public const int PMS_GET_MORNING_CALL_RES = 294;
        public const int PMS_CLEAR_MORNING_CALL_REQ = 295;
        public const int PMS_CLEAR_MORNING_CALL_RES = 296;
        public const int PMS_SET_OUTGOING_POLICY_REQ = 297;
        public const int PMS_SET_OUTGOING_POLICY_RES = 298;
        public const int PMS_GET_OUTGOING_POLICY_REQ = 299;
        public const int PMS_GET_OUTGOING_POLICY_RES = 300;
        public const int PMS_SET_LANGUAGE_REQ = 711;
        public const int PMS_SET_LANGUAGE_RES = 712;
        public const int PMS_GET_LANGUAGE_REQ = 713;
        public const int PMS_GET_LANGUAGE_RES = 714;
        public const int PMS_SET_ALL_REQ = 715;
        public const int PMS_SET_ALL_RES = 716;
        public const int PMS_GET_ALL_REQ = 717;
        public const int PMS_GET_ALL_RES = 718;

        public const int PMS_SET_POST_PARCEL_REQ = 719;
        public const int PMS_SET_POST_PARCEL_RES = 720;
        public const int PMS_GET_POST_PARCEL_REQ = 721;
        public const int PMS_GET_POST_PARCEL_RES = 722;

        public const int PMS_CLEAR_FUNCTION_KEY_REQ = 731;
        public const int PMS_CLEAR_FUNCTION_KEY_RES = 732;

        public const byte EXT_MAX_SIZE = 4;
        public const byte EXT_ALLOW_CHAR_WIDTH = 29;

        public const int ERR_SOCKET_TIMEOUT = 1001;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 120)]
    public struct _pms_data_type
    {
        public int cmd;
        // 0:성공, 1:실패
        public int status;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRUCTS.EXT_MAX_SIZE + 4)]
        public string extension;

        // 모닝콜 설정
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string week;
        public int hour;
        public int minutes;
        public int try_interval;
        public int repeat_times;
        public int repeat_week;
        public int ring_duration;

        // 언어 설정 변수
        public int language;

        // 외부발신 설정 변수
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRUCTS.EXT_ALLOW_CHAR_WIDTH + 3)]
        public string forbiddenPrefix;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRUCTS.EXT_ALLOW_CHAR_WIDTH + 3)]
        public string allowedPrefix;

        // 우편물 관리 설정 부분
        public int post_parcel;
    }
}
