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
        // Device to PMS
        public const int PMS_REGISTER_REQ = 2000;
        public const int PMS_REGISTER_RES = 2001;
        public const int PMS_UNREGISTER_REQ = 2002;
        public const int PMS_UNREGISTER_RES = 2003;

        public const int PMS_SET_MORNING_CALL_REQ = 2004;
        public const int PMS_SET_MORNING_CALL_RES = 2005;

        public const int PMS_SET_LANGUAGE_REQ = 2014;
        public const int PMS_SET_LANGUAGE_RES = 2015;

        public const int PMS_REPORT_FUNCTION_KEY_REQ = 2100;
        public const int PMS_REPORT_FUNCTION_KEY_RES = 2101;

        public const int PMS_REPORT_MAKEUP_STATUS_REQ = 2102;
        public const int PMS_REPORT_MAKEUP_STATUS_RES = 2103;

        public const int PMS_STATUS_SUCCESS = 0;
        public const int PMS_STATUS_FAIL = 1;
        public const int PMS_STATUS_NOT_FOUND = 2;

        public const int PMS_FUNKEY_CLEAR = 0;
        public const int PMS_FUNKEY_REQUEST = 1;
        public const int PMS_FUNKEY_CANCEL = 2;

        public const int PMS_MAKEUP_ROOM_COMPLETE = 0;
        public const int PMS_MAKEUP_ROOM_CANCEL = 1;
        public const int PMS_MAKEUP_ROOM_CONFIRM_COMPLETE = 2;
        public const int PMS_MAKEUP_ROOM_CONFIRM_CANCEL = 3;

        // CGI
        public const int CGI_PMS_SET_MORNING_CALL_REQ = 291;
        public const int CGI_PMS_SET_MORNING_CALL_RES = 292;
        public const int CGI_PMS_GET_MORNING_CALL_REQ = 293;
        public const int CGI_PMS_GET_MORNING_CALL_RES = 294;
        public const int CGI_PMS_CLEAR_MORNING_CALL_REQ = 295;
        public const int CGI_PMS_CLEAR_MORNING_CALL_RES = 296;
        public const int CGI_PMS_SET_OUTGOING_POLICY_REQ = 297;
        public const int CGI_PMS_SET_OUTGOING_POLICY_RES = 298;
        public const int CGI_PMS_GET_OUTGOING_POLICY_REQ = 299;
        public const int CGI_PMS_GET_OUTGOING_POLICY_RES = 300;
        public const int CGI_PMS_SET_LANGUAGE_REQ = 711;
        public const int CGI_PMS_SET_LANGUAGE_RES = 712;
        public const int CGI_PMS_GET_LANGUAGE_REQ = 713;
        public const int CGI_PMS_GET_LANGUAGE_RES = 714;
        public const int CGI_PMS_SET_ALL_REQ = 715;
        public const int CGI_PMS_SET_ALL_RES = 716;
        public const int CGI_PMS_GET_ALL_REQ = 717;
        public const int CGI_PMS_GET_ALL_RES = 718;
        public const int CGI_PMS_SET_POST_PARCEL_REQ = 719;
        public const int CGI_PMS_SET_POST_PARCEL_RES = 720;
        public const int CGI_PMS_GET_POST_PARCEL_REQ = 721;
        public const int CGI_PMS_GET_POST_PARCEL_RES = 722;
        public const int CGI_PMS_CLEAR_FUNCTION_KEY_REQ = 731;
        public const int CGI_PMS_CLEAR_FUNCTION_KEY_RES = 732;
        public const int CGI_PMS_SET_CHECKOUT_TIME_REQ = 733;
        public const int CGI_PMS_SET_CHECKOUT_TIME_RES = 734;

        public const int CGI_IPPHONE_REBOOT_REQ = 729;
        public const int CGI_IPPHONE_REBOOT_RES = 730;

        public const byte EXT_MAX_SIZE = 4;
        public const byte EXT_ALLOW_CHAR_WIDTH = 29;

        public const int ERR_SOCKET_TIMEOUT = 1001;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 36)]
    public struct _pms_reg_t
    {
        public int cmd;
        public int status;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string ip;
        public int port;
        public int expires;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 172)]
    public struct _pms_data_type
    {
        public int cmd;
        // 0:성공, 1:실패
        public int status;
        public IntPtr pList;
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

        // check out 예정 알람 부분
        public int checkout_month;
        public int checkout_day;
        public int checkout_hour;
        public int checkout_minitues;
        public int checkout_before_min;
        public int checkout_try_interval;
        public int checkout_repeat_times;
        public int checkout_ring_duration;

        // 호텔 function-key 부분
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRUCTS.EXT_MAX_SIZE + 4)]
        public string function_key;
        public int function_key_cmd;

        public int makeup_room_status;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 164)]
    public struct _cgi_pms_data_type
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

        // check out 예정 알람 부분
        public int checkout_month;
        public int checkout_day;
        public int checkout_hour;
        public int checkout_minitues;
        public int checkout_before_min;
        public int checkout_try_interval;
        public int checkout_repeat_times;
        public int checkout_ring_duration;

        // 호텔 function-key 부분
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRUCTS.EXT_MAX_SIZE + 4)]
        public string function_key;
        public int function_key_cmd;
    }
}
