using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Com.Huen.Sockets
{
    public class RelayService
    {
        private Timer timer;
        private const int timerInterval = 60000;
        private string strconnection = "Data Source={0}; Initial Catalog=INF_FDESK; Persist Security Info=True; User ID=inf_ctre; Password=ctree0211@";

        private string dbserver = string.Empty;
        private string pbxip = string.Empty;
        private int pbxport = 21007;

        private PMSServer pmsserver;
        private HotelHelper2 h2;

        public string DBServer
        {
            get { return dbserver; }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    dbserver = string.Format(strconnection, "61.74.156.122,3366"); 
                }
                else
                {
                    dbserver = string.Format(strconnection, value);
                }
            }
        }

        public string PBXip
        {
            get { return pbxip; }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    pbxip = "14.63.171.190";
                }
                else
                {
                    pbxip = value;
                }
            }
        }

        public int PBXport
        {
            get { return pbxport; }
            set { this.pbxport = value; }
        }

        public RelayService() : this(string.Empty, string.Empty, 21007)
        {
        }

        public RelayService(string _pbxip) : this(string.Empty, _pbxip, 21007)
        {
        }

        public RelayService(string _dbserver, string _pbxip, int _pbxport)
        {
            this.ReadIni();

            this.DBServer = _dbserver;
            this.PBXip = _pbxip;
            this.PBXport = _pbxport;

            h2 = new HotelHelper2(PBXip, PBXport);

            //CheckRoonetsDB();
            InitTimer();

            pmsserver = new PMSServer();
            pmsserver.ReqPMSSetEvent += Pmsserver_ReqPMSSetEvent;
        }

        private void Pmsserver_ReqPMSSetEvent(PMSServer sender, _pms_data_type pmsdata)
        {
            string t2_code = string.Empty;
            string t2_emer = "0";
            string t2_dnd = "0";
            string t2_mur = "0";

            switch (pmsdata.cmd)
            {
                case STRUCTS.PMS_SET_MORNING_CALL_REQ:
                    // Debug.WriteLine("Pmsserver_ReqPMSSetEvent: " + pmsdata.cmd + " // " + pmsdata.status);
                    t2_code = "C";
                    break;
                case STRUCTS.PMS_CLEAR_MORNING_CALL_REQ:
                    t2_code = "D";
                    break;
                default:
                    // Debug.WriteLine("Pmsserver_ReqPMSSetEvent: " + pmsdata.function_key + " // " + pmsdata.function_key_cmd);

                    if (pmsdata.function_key.Equals(fk_cleanroom))
                    {
                        switch (pmsdata.function_key_cmd)
                        {
                            case 1:
                                t2_mur = "1";
                                break;
                            case 2:
                            default:
                                t2_mur = "0";
                                break;
                        }
                    }
                    else if (pmsdata.function_key.Equals(fk_dnd))
                    {
                        switch (pmsdata.function_key_cmd)
                        {
                            case 1:
                                t2_dnd = "1";
                                break;
                            case 2:
                            default:
                                t2_dnd = "0";
                                break;
                        }
                    }
                    else if (pmsdata.function_key.Equals(fk_laundary))
                    {
                        switch (pmsdata.function_key_cmd)
                        {
                            case 1:
                                t2_code = "G";
                                break;
                            case 2:
                            default:
                                t2_code = "H";
                                break;
                        }
                    }
                    else if (pmsdata.function_key.Equals(fk_roomservice))
                    {
                        switch (pmsdata.function_key_cmd)
                        {
                            case 1:
                                t2_code = "E";
                                break;
                            case 2:
                            default:
                                t2_code = "F";
                                break;
                        }
                    }
                    else if (pmsdata.function_key.Equals(fk_cleaningroom_complete))
                    {
                        switch (pmsdata.function_key_cmd)
                        {
                            case 1:
                            case 2:
                            default:
                                t2_code = "0";
                                break;
                        }
                    }
                    else if (pmsdata.function_key.Equals(fk_cleaningroom_inspection))
                    {
                        switch (pmsdata.function_key_cmd)
                        {
                            case 1:
                            case 2:
                            default:
                                t2_code = "9";
                                break;
                        }
                    }
                    break;
            }

            using (MSDBHelper db = new MSDBHelper(DBServer))
            {
                string ext = string.Format("{0:D4}", int.Parse(pmsdata.extension));
                string sql = string.Format("insert into INF_CT02 ( T2_ROOM, T2_CODE, T2_EMER, T2_DND, T2_MUR, T2_DATE ) values ( '{0}', '{1}', '{2}', '{3}', '{4}', getdate() )", sitecode + ext, t2_code, t2_emer, t2_dnd, t2_mur);
                db.Sql = sql;
                try
                {
                    db.Open();
                    db.BeginTran();
                    int count = db.GetEffectedCount();
                    db.Commit();
                }
                catch (SqlException e)
                {
                    util.WriteLog(e.Message);
                    db.Rollback();
                }
            }
        }

        private string sitecode;
        private string fk_cleanroom;
        private string fk_dnd;
        private string fk_laundary;
        private string fk_roomservice;
        private string fk_cleaningroom_complete;
        private string fk_cleaningroom_inspection;

        private void ReadIni()
        {
            Ini ini = new Ini(".\\relay2pms.ini");
            sitecode = ini.IniReadValue("SITE", "code");
            fk_cleanroom = ini.IniReadValue("functionkeys", "fk_cleanroom");
            fk_dnd = ini.IniReadValue("functionkeys", "fk_dnd");
            fk_laundary = ini.IniReadValue("functionkeys", "fk_laundary");
            fk_roomservice = ini.IniReadValue("functionkeys", "fk_roomservice");
            fk_cleaningroom_complete = ini.IniReadValue("functionkeys", "fk_cleaningroom_complete");
            fk_cleaningroom_inspection = ini.IniReadValue("functionkeys", "fk_cleaningroom_inspection");
        }

        private void InitTimer()
        {
            timer = new Timer();
            timer.Interval = timerInterval;
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckRoonetsDB();
        }

        private void CheckRoonetsDB()
        {
            var watch = Stopwatch.StartNew();

            DataTable dt = null;

            // Check IN/OUT
            using (MSDBHelper db = new MSDBHelper(DBServer))
            {
                try
                {
                    db.Sql = "select T1_ID, T1_SITE, T1_ROOM, T1_CODE, T1_PERIOD, T1_PROOM, T1_DATE, T1_READ, T1_READ2, T1_TXT2 from INF_CT01 order by T1_SITE asc, T1_ROOM asc, T1_DATE asc";
                    db.Open();
                    dt = db.GetDataTable();
                }
                catch (SqlException e)
                {
                    util.WriteLog(e.Message);
                }
            }

            List<TCT01> tempdata = new List<TCT01> (from DataRow row in dt.AsEnumerable()
                           select new TCT01
                           {
                               T_ID = row[0].ToString(),
                               T_SITE = row[1].ToString(),
                               T_ROOM = row[2].ToString(),
                               T_CODE = row[3].ToString(),
                               T_PERIOD = row[4].ToString(),
                               T_PROOM = row[5].ToString(),
                               T_DATE = (DateTime)row[6],
                               T_READ = int.Parse(row[7].ToString()),
                               T_READ2 = int.Parse(row[8].ToString()),
                               T_TXT2 = row[9].ToString()
                           }).ToList<TCT01>();

            var tmpdatum = tempdata.Where(x => x.T_READ == 0 || x.T_READ2 == 0);
            foreach (TCT01 item in tmpdatum)
            {
                string roomnumberext = string.IsNullOrEmpty(item.T_ROOM) == true ? string.Empty : int.Parse(item.T_ROOM).ToString();

                if (string.IsNullOrEmpty(roomnumberext)) continue;

                _pms_data_type original_data = new _pms_data_type();
                original_data = h2.GetPolicy(roomnumberext);

                bool result = false;
                int count = 0;

                if (item.T_CODE.Equals("3"))
                {
                    result = h2.SetSystem("0", roomnumberext, item.T_PERIOD, item.T_TXT2);
                    if (item.T_PERIOD.Equals("0"))
                    {
                        result = h2.SetSystem("1", roomnumberext, item.T_PERIOD, item.T_TXT2);
                    }
                    else
                    {
                        result = h2.SetSystem("2", roomnumberext, item.T_PERIOD, item.T_TXT2);
                    }
                }
                else
                {
                    result = h2.SetSystem(item.T_CODE, roomnumberext, item.T_PERIOD, item.T_TXT2);
                }


                if (!result) continue;

                if (item.T_CODE.Equals("O"))
                {
                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Sql = string.Format("update INF_CT01 set T1_READ=1, T1_READ2=1 where T1_READ2={0} and T1_SITE='{1}' and T1_ROOM='{2}' and T1_DATE=cast('{3}' as datetime)", item.T_READ2, item.T_SITE, item.T_ROOM, item.T_DATE.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            db.Open();
                            db.BeginTran();
                            count = db.GetEffectedCount();
                            db.Commit();

                            item.T_READ = 1;
                            item.T_READ2 = 1;
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            result = h2.RestoreSystem(original_data);
                            continue;
                        }
                    }
                }
                else
                {
                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(item.T_TXT2))
                            {
                                db.Sql = string.Format("update INF_CT01 set T1_READ=1 where T1_SITE='{0}' and T1_ROOM='{1}' and T1_DATE=cast('{2}' as datetime)", item.T_SITE, item.T_ROOM, item.T_DATE.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }
                            else
                            {
                                db.Sql = string.Format("update INF_CT01 set T1_READ=1, T1_READ2=1 where T1_SITE='{0}' and T1_ROOM='{1}' and T1_DATE=cast('{2}' as datetime)", item.T_SITE, item.T_ROOM, item.T_DATE.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }

                            db.Open();
                            db.BeginTran();
                            count = db.GetEffectedCount();
                            db.Commit();

                            item.T_READ = 1;
                            item.T_READ2 = 1;
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            result = h2.RestoreSystem(original_data);
                            continue;
                        }
                    }
                }
            }

            // Check House Keeping
            dt = null;
            using (MSDBHelper db = new MSDBHelper(DBServer))
            {
                try
                {
                    db.Sql = "select T3_ID, T3_SITE, T3_ROOM, T3_CODE, T3_READ, T3_TXT1, T3_DATE from INF_CT03 where T3_READ=0;";
                    db.Open();
                    dt = db.GetDataTable();
                }
                catch (SqlException e)
                {
                    util.WriteLog(e.Message);
                }
            }

            List<TCT03> tempdata2 = new List<TCT03>(from DataRow row in dt.AsEnumerable()
                    select new TCT03
                    {
                        T_ID = row[0].ToString(),
                        T_SITE = row[1].ToString(),
                        T_ROOM = row[2].ToString(),
                        T_CODE = row[3].ToString(),
                        T_READ = int.Parse(row[4].ToString()),
                        T_TXT = row[5].ToString(),
                        T_DATE = (DateTime)row[6],
                    }).ToList<TCT03>();

            try
            {
                var tempdatum2 = tempdata2.Where(x => x.T_READ == 0);
                foreach (TCT03 item in tempdatum2)
                {
                    string roomnumberext = string.IsNullOrEmpty(item.T_ROOM) == true ? string.Empty : int.Parse(item.T_ROOM).ToString();

                    _pms_data_type original_data = new _pms_data_type();
                    original_data = h2.GetPolicy(roomnumberext);

                    bool result = false;
                    int count = 0;

                    result = h2.SetHouseKeep(item.T_CODE, roomnumberext, item.T_TXT);

                    if (!result) continue;

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Sql = string.Format("update INF_CT03 set T3_READ=1 where T3_SITE='{0}' and T3_ROOM='{1}' and T3_DATE=cast('{2}' as datetime)", item.T_SITE, item.T_ROOM, item.T_DATE.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            db.Open();
                            db.BeginTran();
                            count = db.GetEffectedCount();
                            db.Commit();
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            result = h2.RestoreSystem(original_data);
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> CheckRoonetsDB was done in " + elapsedMs + " mil.");
        }

        class TCT01
        {
            public string T_ID;
            public string T_SITE;
            public string T_ROOM;
            public string T_CODE;
            public string T_PERIOD;
            public string T_PROOM;
            public DateTime T_DATE;
            public int T_READ;
            public int T_READ2;
            public string T_TXT2;
        }

        class TCT03
        {
            public string T_ID;
            public string T_SITE;
            public string T_ROOM;
            public string T_CODE;
            public int T_READ;
            public string T_TXT;
            public DateTime T_DATE;
        }
    }
}