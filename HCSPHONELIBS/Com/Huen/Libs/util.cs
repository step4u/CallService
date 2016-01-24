using System;
using System.Linq;
using System.Text;

using System.Data;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Controls;

using System.Diagnostics;
using System.Windows.Documents;

using Com.Huen.Sockets;
using Com.Huen.DataModel;
using Com.Huen.Sql;

namespace Com.Huen.Libs
{
    public class util
    {

        public static UserInfo Userinfo = new UserInfo();
        public static User loginuserinfo = new User();

        public static int i_day = 25;
        public static DateTime i_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i_day);
        public static int WordLength = 40;
        //public static int dialogdelaytime = 1000;

        //private static string pbxip = "121.138.213.225";          //서울
        private static string pbxip = "210.126.111.55";             //대전
        private static string dbip = "127.0.0.1";

        public static string PBXIP
        {
            get
            {
                return pbxip;
            }
            set
            {
                pbxip = value;
            }
        }

        public static string DBIP
        {
            get
            {
                return dbip;
            }
            set
            {
                dbip = value;
            }
        }

        // 리소스 키 로더
        public static Object LoadProjectResource(string strResName, string imgtype)
        {
            Object rtnObj = new Object();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string strBaseName = assembly.GetName().Name + "." + "Properties.Resources";

            ResourceManager rm = new ResourceManager(strBaseName, assembly);

            if (IsBitmapImageType(imgtype))
                rtnObj = (Object)ToImageSource((System.Drawing.Bitmap)rm.GetObject(strResName), imgtype);
            else
                rtnObj = rm.GetObject(strResName);

            return rtnObj;
        }

        // 리소스 키 로더
        public static Object LoadProjectResource(string strResName, string ns, string imgtype)
        {
            Object rtnObj = new Object();
            Assembly assembly = Assembly.Load(ns);
            string strBaseName = assembly.GetName().Name + "." + "Properties.Resources";

            ResourceManager rm = new ResourceManager(strBaseName, assembly);

            if (IsBitmapImageType(imgtype))
            {
                rtnObj = (Object)ToImageSource((System.Drawing.Bitmap)rm.GetObject(strResName), imgtype);
            }
            else if (imgtype == "ico")
            {
                rtnObj = (Object)ToImageSource((System.Drawing.Icon)rm.GetObject(strResName), imgtype);
            }
            else
            {
                rtnObj = rm.GetObject(strResName);
            }

            return rtnObj;
        }

        // 이미지를 이미지 리소스로 변경
        private static BitmapImage ToImageSource(System.Drawing.Bitmap bitmap, string imgtype)
        {
            var memoryStream = new MemoryStream();

            switch (imgtype)
            {
                case "gif":
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case "bmp":
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case "png":
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case "jpg":
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "ico":
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Icon);
                    break;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        // 아이콘을 아이콘 리소스로 변경
        private static BitmapImage ToImageSource(System.Drawing.Icon icon, string imgtype)
        {
            var memoryStream = new MemoryStream();

            icon.Save(memoryStream);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        // 이미지 타입 오브젝트인지 확인
        private static bool IsBitmapImageType(string imgtype)
        {
            bool rtnBool = false;

            //string[] tokens = { "gif", "bmp", "png", "jpg", "tif", "ico" };
            string[] tokens = { "gif", "bmp", "png", "jpg", "tif" };
            int chk = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (imgtype.IndexOf(tokens[i]) > -1)
                {
                    chk = 1;
                    break;
                }
            }
            if (chk > 0) rtnBool = true;

            return rtnBool;
        }

        // String to IPaddress
        public static long ToInt(string addr)
        {
            return (long)(uint)System.Net.IPAddress.NetworkToHostOrder((int)System.Net.IPAddress.Parse(addr).Address);
        }

        // Object to Bytes
        public static byte[] getBytes(Object str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr prt = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, prt, true);
            Marshal.Copy(prt, arr, 0, size);
            Marshal.FreeHGlobal(prt);

            return arr;
        }

        // bytes to Object
        public static Object getObject(byte[] arr)
        {
            CommandMsg str = new CommandMsg();

            //int size = Marshal.SizeOf(str);
            int size = arr.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);

            str = (CommandMsg)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        // bytes to Object
        public static Object getObject(byte[] arr, Object obj)
        {
            int size = Marshal.SizeOf(obj);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);

            obj = Marshal.PtrToStructure(ptr, obj.GetType());
            Marshal.FreeHGlobal(ptr);

            return obj;
        }

        public static byte[] GetBytes(Object st)
        {
            if (BitConverter.IsLittleEndian)
            {
                System.Type t = st.GetType();
                FieldInfo[] fieldInfo = t.GetFields();

                foreach (FieldInfo fi in fieldInfo)
                {
                    if (fi.FieldType == typeof(System.Int16))
                    {
                        Int16 i = (Int16)fi.GetValue(st);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(st), BitConverter.ToInt16(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.Int32))
                    {
                        Int32 i = (Int32)fi.GetValue(st);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(st), BitConverter.ToInt32(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.Int64))
                    {
                        Int64 i = (Int64)fi.GetValue(st);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(st), BitConverter.ToInt64(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.UInt16))
                    {
                        UInt16 i = (UInt16)fi.GetValue(st);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(st), BitConverter.ToUInt16(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.UInt32))
                    {
                        UInt32 i = (UInt32)fi.GetValue(st);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(st), BitConverter.ToUInt32(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.UInt64))
                    {
                        UInt64 i = (UInt64)fi.GetValue(st);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(st), BitConverter.ToUInt64(br, 0));
                    }
                }
            }

            int size = Marshal.SizeOf(st);
            byte[] arr = new byte[size];
            IntPtr prt = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(st, prt, true);
            Marshal.Copy(prt, arr, 0, size);
            Marshal.FreeHGlobal(prt);

            return arr;
        }

        public static T GetObject<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            if (BitConverter.IsLittleEndian)
            {
                System.Type t = stuff.GetType();
                FieldInfo[] fieldInfo = t.GetFields();

                foreach (FieldInfo fi in fieldInfo)
                {
                    if (fi.FieldType == typeof(System.Int16))
                    {
                        Int16 i = (Int16)fi.GetValue(stuff);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(stuff), BitConverter.ToInt16(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.Int32))
                    {
                        Int32 i = (Int32)fi.GetValue(stuff);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(stuff), BitConverter.ToInt32(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.Int64))
                    {
                        Int64 i = (Int64)fi.GetValue(stuff);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(stuff), BitConverter.ToInt64(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.UInt16))
                    {
                        UInt16 i = (UInt16)fi.GetValue(stuff);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(stuff), BitConverter.ToUInt16(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.UInt32))
                    {
                        UInt32 i = (UInt32)fi.GetValue(stuff);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(stuff), BitConverter.ToUInt32(br, 0));
                    }
                    else if (fi.FieldType == typeof(System.UInt64))
                    {
                        UInt64 i = (UInt64)fi.GetValue(stuff);
                        byte[] b = BitConverter.GetBytes(i);
                        byte[] br = b.Reverse().ToArray();
                        fi.SetValueDirect(__makeref(stuff), BitConverter.ToUInt64(br, 0));
                    }
                }
            }

            return stuff;
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            bf.Serialize(memStream, obj);
            return memStream.ToArray();
        }

        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return (T)bf.Deserialize(memStream);
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return bf.Deserialize(memStream);
            }
        }

        public static string encStr(string srcStr)
        {
            //srcStr = srcStr.Replace(Environment.NewLine, "\r\n");
            srcStr = srcStr.Replace(Environment.NewLine, "&#13&#10");
            srcStr = srcStr.Replace("\"", "&#34;");
            srcStr = srcStr.Replace("'", "&#39;");

            return srcStr;
        }

        public static string decStr(string srcStr)
        {
            srcStr = srcStr.Replace("&#39;", "'");
            srcStr = srcStr.Replace("&#34;", "\"");
            srcStr = srcStr.Replace("&#13&#10", Environment.NewLine);

            return srcStr;
        }

        public static double DateDiff(string token, DateTime d1, DateTime d2)
        {
            TimeSpan ts = (TimeSpan)(d2 - d1);
            double output = 0.0d;

            switch (token)
            {
                case "dd":
                    output = ts.TotalDays;
                    break;
                case "hh":
                    output = ts.TotalHours;
                    break;
                case "mm":
                    output = ts.TotalMinutes;
                    break;
                case "ss":
                    output = ts.TotalSeconds;
                    break;
            }

            return output;
        }

        public static void WriteLog(string logMessage)
        {
            try
            {
                if (!Directory.Exists("./log"))
                    Directory.CreateDirectory("log");

                string strPath = string.Format("./log/{0}{1:00}{2:00}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                if (!File.Exists(strPath))
                    File.CreateText(strPath);

                StreamWriter __w = File.AppendText(strPath);
                __w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                __w.WriteLine("  :{0}", logMessage);
                __w.WriteLine("---------------------------------------------------");
                __w.Flush();
                __w.Close();

                foreach (var logfile in System.IO.Directory.EnumerateFiles("./log"))
                {
                    if (File.GetCreationTime(logfile) < DateTime.Now.AddMonths(-2))
                    {
                        if (File.Exists(logfile))
                        {
                            File.Delete(logfile);
                        }
                    }
                }
            }
            catch(IOException __ex)
            {
                //Debug.WriteLine(__ex.Message);

                Type __type = __ex.GetType();

                Debug.WriteLine(string.Format("{0} : {1}", __type.ToString(), __ex.Message));

                //if (__type == typeof(FileNotFoundException))
                //{
                //    Debug.WriteLine(string.Format("FileNotFoundException : {0}"), __ex.Message);
                //}
                //else if (__type == typeof(DirectoryNotFoundException))
                //{
                //    Debug.WriteLine(string.Format("FileNotFoundException : {0}"), __ex.Message);
                //}
            }
        }

        public static void Log2DB(string ext, string msg, string chk)
        {
            DataTable dt = util.CreateDT2SP();
            dt.Rows.Add("@I_ROOM", ext);
            dt.Rows.Add("@I_MEMO", msg);
            dt.Rows.Add("@I_CHK", chk);

            using (FirebirdDBHelper db = new FirebirdDBHelper(strDBConn))
            {
                try
                {
                    db.BeginTran();
                    db.ExcuteSP("INS_LOG", dt);
                    db.Commit();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    db.Rollback();
                }
            }
        }

        // 프로시저 실행을 위한 값 셋팅 테이블
        public static DataTable MakeDataTable2Proc()
        {
            DataTable dt = new DataTable();

            DataColumn paramname = new DataColumn();
            paramname.DataType = System.Type.GetType("System.String");
            paramname.ColumnName = "DataName";
            dt.Columns.Add(paramname);

            DataColumn datatype = new DataColumn();
            datatype.DataType = System.Type.GetType("System.Object");
            datatype.ColumnName = "DataType";
            dt.Columns.Add(datatype);

            DataColumn datasize = new DataColumn();
            datasize.DataType = System.Type.GetType("System.Int32");
            datasize.ColumnName = "DataSize";
            dt.Columns.Add(datasize);

            DataColumn datavalue = new DataColumn();
            datavalue.DataType = System.Type.GetType("System.String");
            datavalue.ColumnName = "DataValue";
            dt.Columns.Add(datavalue);

            DataColumn datadirection = new DataColumn();
            datadirection.DataType = System.Type.GetType("System.String");
            datadirection.ColumnName = "DataDirection";
            dt.Columns.Add(datadirection);

            return dt;
        }

        // 프로시저 실행을 위한 값 셋팅 테이블
        public static DataTable CreateDT2SP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("DataName", typeof(string));
            //dt.Columns.Add("DataType", typeof(object));
            //dt.Columns.Add("DataSize", typeof(int));
            dt.Columns.Add("DataValue", typeof(string));
            //dt.Columns.Add("DataDirection", typeof(string));

            return dt;
        }

        public static string _dbfile = @"D:\FBDB\KCTV_JEJU.FDB";
        public static string _ipaddr = "127.0.0.1";
        public static string strDBConn
        {
            get
            {
                Ini ini = new Ini(@".\cdr.ini");
                string _file = string.IsNullOrEmpty(ini.IniReadValue("DB", "FILE")) == true ? _dbfile : ini.IniReadValue("DB", "FILE");
                string _ip = string.IsNullOrEmpty(ini.IniReadValue("DB", "IP")) == true ? _ipaddr : ini.IniReadValue("DB", "IP");
                string str = LoadProjectResource("DBCONSTR_FBSQL", "COMMONRES", "").ToString();
                str = string.Format(str, _file, _ip);
                return str;
            }
        }

        public static string strDBConnTest
        {
            get
            {
                return LoadProjectResource("DBCONSTR_MSSQL_TEST", "COMMONRES", "").ToString();
            }
        }

        public static string GetWordBytes(string src, double toLength, string suffixStr)
        {
            double Length = 0;
            int i, j;
            string Value = src;

            for (i = 0, j = Value.Length; i < j; i++)
            {
                char C = Value[i];
                Length += (Char.GetUnicodeCategory(C).ToString() == "OtherLetter") ? 1.85 : 1;

                if (Length > toLength)
                {
                    break;
                }
            }

            String newString = "";

            if (Value.Length > i) newString = Value.Substring(0, i) + suffixStr;
            else newString = Value.Substring(0, i);

            return newString;
        }

        public static byte[] String2Binary(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static string Binary2String(byte[] arr)
        {
            string result = string.Empty;

            result = System.Text.Encoding.Default.GetString(arr);

            return result;
        }

        public static string GetSHA1(string text)
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = ue.GetBytes(text);

            SHA1Managed hashString = new SHA1Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public static string GetSHA256(string text)
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = ue.GetBytes(text);

            SHA256Managed hashString = new SHA256Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public static void ValidateInteger(object sender)
        {
            Exception X = new Exception();

            TextBox T = (TextBox)sender;

            T.Text = T.Text.Trim();

            try
            {
                int x = int.Parse(T.Text);

                //Customizing Condition (Only numbers larger than zero are permitted)
                if (x < 0)
                    throw X;
            }
            catch (Exception)
            {
                try
                {
                    if (T.SelectionStart < 1) T.SelectionStart = 1;

                    int CursorIndex = T.SelectionStart - 1;
                    T.Text = T.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    T.SelectionStart = CursorIndex;
                    T.SelectionLength = 0;
                }
                catch (Exception) { }
            }
        }

        public static void ValidateDouble(object sender)
        {
            Exception X = new Exception();

            TextBox T = (TextBox)sender;

            T.Text = T.Text.Trim();

            try
            {

                double x = double.Parse(T.Text);

                //Customizing Condition (Only numbers larger than or equal to zero are permitted)
                if (x < 0 || T.Text.Contains(','))
                    throw X;

            }
            catch (Exception)
            {
                try
                {
                    if (T.SelectionStart < 1) T.SelectionStart = 1;

                    int CursorIndex = T.SelectionStart - 1;
                    T.Text = T.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    T.SelectionStart = CursorIndex;
                    T.SelectionLength = 0;
                }
                catch (Exception) { }

            }
        }

        public static TableCell CreateTblCell(Thickness _border, Thickness _cellpadding, string _bordercolor, string _foregroundcolor, string _backgroundcolor, TextAlignment _txtalign, double _fontsize, string _txt, double lineheight)
        {
            Run _run = new Run(_txt);
            _run.BaselineAlignment = BaselineAlignment.Center;
            TableCell _tblcell = new TableCell(new Paragraph(_run));
            _tblcell.Foreground = Str2Brush(_foregroundcolor);
            _tblcell.Background = Str2Brush(_backgroundcolor);
            _tblcell.BorderBrush = Str2Brush(_bordercolor);
            _tblcell.BorderThickness = _border;
            _tblcell.Padding = _cellpadding;
            _tblcell.TextAlignment = _txtalign;
            _tblcell.FontSize = _fontsize;
            _tblcell.LineHeight = lineheight;

            return _tblcell;
        }

        public static SolidColorBrush Str2Brush(string _strcolor)
        {
            var color = (Color)ColorConverter.ConvertFromString(_strcolor);
            var brush = new SolidColorBrush(color);
            return brush;
        }

        public static string ConvtNum2StrLang(Object lang)
        {
            string result = string.Empty;
            int _lang = -1;

            if (lang.GetType() == typeof(String))
            {
                if (!string.IsNullOrEmpty(lang.ToString()))
                {
                    _lang = int.Parse(lang.ToString());
                }
            }
            else
            {
                _lang = (int)lang;
            }

            switch (_lang)
            {
                case 1:
                    result = "English";
                    break;
                case 2:
                    result = "Korean";
                    break;
                default:
                    result = "";
                    break;
            }

            return result;
        }
    }
}