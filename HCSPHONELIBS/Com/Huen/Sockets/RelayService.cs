using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System;

namespace Com.Huen.Sockets
{
    public class RelayService
    {
        private Timer timer;
        private const int timerInterval = 60000;
        private string strconnection = "Data Source={0}; Initial Catalog=INF_FDESK; Persist Security Info=True; User ID=inf_ctre; Password=ctree0211@";

        private string dbserver = string.Empty;
        private string pbxip = string.Empty;

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


        public RelayService() : this(string.Empty, string.Empty)
        {
        }

        public RelayService(string _pbxip) : this(string.Empty, _pbxip)
        {
        }

        public RelayService(string _dbserver, string _pbxip)
        {
            this.DBServer = _dbserver;
            this.PBXip = _pbxip;

            CheckRoonetsDB();
            InitTimer();
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
            DataTable dt = null;

            // Check House Check IN/OUT
            using (MSDBHelper db = new MSDBHelper(DBServer))
            {
                try
                {
                    db.Open();
                    db.Sql = "select T1_ID, T1_SITE, T1_ROOM, T1_CODE, T1_PERIOD, T1_PROOM from INF_CT01 where T1_READ=0;";
                    dt = db.GetDataTable();
                }
                catch (SqlException e)
                {
                    util.WriteLog(e.Message);
                }
            }

            try
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    _pms_data_type original_data = new _pms_data_type();
                    using (HotelHelper hh = new HotelHelper())
                    {
                        original_data = hh.GetPolicy(row[2].ToString());
                    }

                    bool result = false;
                    int count = 0;

                    using (HotelHelper hh = new HotelHelper())
                    {
                        result = hh.SetSystem(row[3].ToString(), row[2].ToString(), row[4].ToString(), string.Empty);
                    }

                    if (!result) continue;

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Open();
                            db.BeginTran();
                            db.Sql = string.Format("update INF_CT01 set T1_READ=1 where T1_ID={0}", row[0].ToString());
                            count = db.GetEffectedCount();
                            db.Commit();
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            using (HotelHelper hh = new HotelHelper())
                            {
                                result = hh.RestoreSystem(original_data);
                            }
                            continue;
                        }
                    }

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Open();
                            db.Sql = string.Format("select T1_ID, T1_SITE, T1_ROOM, T1_TXT2 from INF_CT01 where T1_READ2=0 and T1_ID={0};", row[0].ToString());
                            dt = db.GetDataTable();
                        }
                        catch (SqlException e)
                        {
                            continue;
                        }
                    }

                    if (dt.Rows.Count < 1) continue;

                    using (HotelHelper hh = new HotelHelper())
                    {
                        original_data = hh.GetPolicy(row[2].ToString());
                        result = hh.SetSystem(string.Empty, row[2].ToString(), string.Empty, dt.Rows[0][3].ToString());
                    }

                    if (!result) continue;

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Open();
                            db.BeginTran();
                            db.Sql = string.Format("update INF_CT01 set T1_READ=1 where T1_READ2=0 and T1_ID={0}", row[0].ToString());
                            count = db.GetEffectedCount();
                            db.Commit();
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            using (HotelHelper hh = new HotelHelper())
                            {
                                result = hh.RestoreSystem(original_data);
                            }
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message);
            }



            // Check House Keeping
            dt = null;
            using (MSDBHelper db = new MSDBHelper(DBServer))
            {
                try
                {
                    db.Open();
                    // db.Sql = "select T2_ID, left(T2_ROOM, 5) as T2_SITE, substring(T2_ROOM, 6, len(T2_ROOM)-5) as T2_ROOM from INF_CT02 where T1_READ=0;";
                    db.Sql = "select T3_ID, T3_SITE, T3_ROOM, T3_CODE, T3_READ, T3_TXT1 from INF_CT03 where T3_READ=0;";
                    dt = db.GetDataTable();
                }
                catch (SqlException e)
                {
                    util.WriteLog(e.Message);
                }
            }

            try
            {
                foreach (DataRow row in dt.AsEnumerable())
                {
                    _pms_data_type original_data = new _pms_data_type();
                    using (HotelHelper hh = new HotelHelper())
                    {
                        original_data = hh.GetPolicy(row[2].ToString());
                    }

                    bool result = false;
                    int count = 0;

                    using (HotelHelper hh = new HotelHelper())
                    {
                        result = hh.SetSystem(row[3].ToString(), row[2].ToString(), row[4].ToString(), string.Empty);
                    }

                    if (!result) continue;

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Open();
                            db.BeginTran();
                            db.Sql = string.Format("update INF_CT01 set T1_READ=1 where T1_ID={0}", row[0].ToString());
                            count = db.GetEffectedCount();
                            db.Commit();
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            using (HotelHelper hh = new HotelHelper())
                            {
                                result = hh.RestoreSystem(original_data);
                            }
                            continue;
                        }
                    }

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Open();
                            db.Sql = string.Format("select T1_ID, T1_SITE, T1_ROOM, T1_TXT2 from INF_CT01 where T1_READ2=0 and T1_ID={0};", row[0].ToString());
                            dt = db.GetDataTable();
                        }
                        catch (SqlException e)
                        {
                            continue;
                        }
                    }

                    if (dt.Rows.Count < 1) continue;

                    using (HotelHelper hh = new HotelHelper())
                    {
                        original_data = hh.GetPolicy(row[2].ToString());
                        result = hh.SetSystem(string.Empty, row[2].ToString(), string.Empty, dt.Rows[0][3].ToString());
                    }

                    if (!result) continue;

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        try
                        {
                            db.Open();
                            db.BeginTran();
                            db.Sql = string.Format("update INF_CT01 set T1_READ=1 where T1_READ2=0 and T1_ID={0}", row[0].ToString());
                            count = db.GetEffectedCount();
                            db.Commit();
                        }
                        catch (SqlException e)
                        {
                            db.Rollback();
                            using (HotelHelper hh = new HotelHelper())
                            {
                                result = hh.RestoreSystem(original_data);
                            }
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message);
            }
        }
    }
}