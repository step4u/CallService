using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using Com.Huen.Sql;
using Com.Huen.Libs;

namespace Com.Huen.DataModel
{
    public class Firm
    {
        public int idx = 0;
        public string title = string.Empty;
    }

    public class Firms
    {
        private List<Firm> _list = new List<Firm>();
        public Firms()
        {
            INIT();
        }

        private void INIT()
        {
            DataTable dt = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select idx, title from FIRMS order by title asc");

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTable();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    util.WriteLog(string.Format("{0} INIT ERR : {1}", this.GetType(), fex.Message));
                    return;
                }
            }

            _list = new List<Firm>(
                (from _row in dt.AsEnumerable()
                 select new Firm()
                 {
                     idx = int.Parse(_row["idx"].ToString())
                     ,
                     title = _row["title"].ToString()
                 }
                ).ToList<Firm>()
            );
        }

        public List<Firm> GETLIST()
        {
            return _list;
        }

        public void ADD(Firm item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into FIRMS");
            sb.Append("( title )");
            sb.Append(" values ");
            sb.AppendFormat("( {0} )", item.title);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    int count = db.GetEffectedCount();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    db.Rollback();
                    util.WriteLog(string.Format("{0} ADD ERR : {1}", this.GetType(), fex.Message));
                    return;
                }
            }

            INIT();
        }

        public void MODIFY(Firm item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update FIRMS set title={0} where idx={1}", item.title, item.idx);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    int count = db.GetEffectedCount();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    db.Rollback();
                    util.WriteLog(string.Format("{0} MODIFY ERR : {1}", this.GetType(), fex.Message));
                    return;
                }
            }

            INIT();
        }

        public void REMOVE(Firm item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from FIRMS where idx={0}", item.idx);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    int count = db.GetEffectedCount();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    db.Rollback();
                    util.WriteLog(string.Format("{0} REMOVE ERR : {1}", this.GetType(), fex.Message));
                    return;
                }
            }

            INIT();
        }
    }
}
