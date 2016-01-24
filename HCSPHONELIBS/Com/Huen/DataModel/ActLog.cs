using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Net.Sockets;

using Com.Huen.Libs;
using Com.Huen.Sql;
using Com.Huen.UserControls;
using Com.Huen.Sockets;

namespace Com.Huen.DataModel
{
    public class ActLog
    {
        public string TELNUM { get; set; }
        public string DETAIL { get; set; }
    }

    public class ActLogs : ObservableCollection<ActLog>
    {
        public ActLogs()
        {
            DataTable dt = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select room, memo, regdate from tbl_log where datediff(day, cast(regdate as date), current_date)<=1 order by regdate desc");

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTable();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException ex)
                {
                    util.WriteLog(string.Format("DB Error ({0}) : {1}", this.GetType(), ex.Message));
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                this.Add(new ActLog() { TELNUM = row[0].ToString(), DETAIL = row[1].ToString() });
            }
        }
    }
}
