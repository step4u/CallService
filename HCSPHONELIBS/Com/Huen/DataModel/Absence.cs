using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Data;

using Com.Huen.Libs;
using Com.Huen.Sql;

namespace Com.Huen.DataModel
{
    public class Absence
    {
        public string TELNUM { get; set; }
        public string DETAIL { get; set; }
        public string STATES { get; set; }
    }

    public class Absences : ObservableCollection<Absence>
    {
        public Absences()
        {
            DataTable dt = null;
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTableSP("GET_ABSENCECALL");
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException ex)
                {
                    util.WriteLog(string.Format("DB Error ({0}) : {1}", this.GetType(), ex.Message));
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                Absence item = new Absence() {
                    TELNUM = row[0].ToString(),
                    DETAIL = row[1].ToString(),
                    STATES = row[2].ToString()
                };

                this.Add(item);
            }
        }
    }
}
