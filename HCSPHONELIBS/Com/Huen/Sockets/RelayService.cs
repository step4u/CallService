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
                    pbxip = "127.0.0.1";
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
            DataSet ds = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select T1_ID, T1_CODE, T1_PERIOD, T1_PROOM from INF_CT01 where where T1_READ=0;");
            sb.Append("select T1_ID, T1_TXT2 from INF_CT01 where where T1_READ2=0;");

            using (MSDBHelper db = new MSDBHelper(DBServer))
            {
                try
                {
                    db.Open();
                    db.Sql = sb.ToString();
                    ds = db.GetDataSet();
                }
                catch (SqlException e)
                {
                    util.WriteLog(e.Message);
                }
            }

            try
            {
                foreach (DataRow row in ds.Tables[0].AsEnumerable())
                {

                    switch (row[1].ToString())
                    {
                        case "0":
                            break;
                        case "1":
                            break;
                        case "2":
                            break;
                        case "3":
                            break;
                        case "4":
                            break;
                        case "5":
                            break;
                    }

                    using (MSDBHelper db = new MSDBHelper(DBServer))
                    {
                        db.Open();
                        db.Sql = string.Format("update INF_CT01 set T1_READ=1 where T1_ID={0}", row[0].ToString());
                    }
                }
            } catch (Exception e)
            {
                util.WriteLog(e.Message);
            }
        }
    }
}