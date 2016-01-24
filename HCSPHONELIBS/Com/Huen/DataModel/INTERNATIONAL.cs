using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Com.Huen.Sql;
using Com.Huen.Libs;

namespace Com.Huen.DataModel
{
    public class INTERNATIONAL
    {
        private long _idx = 0;
        private int _areacode = 0;
        private int _firmidx = 0;
        private string _nation_num = string.Empty;
        private string _nation_local_num = string.Empty;
        private string _natione = string.Empty;
        private string _nationk = string.Empty;
        private string _lm = string.Empty;

        public long idx
        {
            get
            {
                return _idx;
            }
            set
            {
                _idx = value;
                OnPropertyChanged("idx");
            }
        }

        public int areacode
        {
            get
            {
                return _areacode;
            }
            set
            {
                _areacode = value;
                OnPropertyChanged("areacode");
            }
        }

        public int firmidx
        {
            get
            {
                return _firmidx;
            }
            set
            {
                _firmidx = value;
                OnPropertyChanged("firmidx");
            }
        }

        public string nation_num
        {
            get
            {
                return _nation_num;
            }
            set
            {
                _nation_num = value;
                OnPropertyChanged("nation_num");
            }
        }

        public string nation_local_num
        {
            get
            {
                return _nation_local_num;
            }
            set
            {
                _nation_local_num = value;
                OnPropertyChanged("nation_local_num");
            }
        }

        public string natione
        {
            get
            {
                return _natione;
            }
            set
            {
                _natione = value;
                OnPropertyChanged("natione");
            }
        }

        public string nationk
        {
            get
            {
                return _nationk;
            }
            set
            {
                _nationk = value;
                OnPropertyChanged("nationk");
            }
        }

        public string lm
        {
            get
            {
                return _lm;
            }
            set
            {
                _lm = value;
                OnPropertyChanged("lm");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class INTERNATIONALS
    {
        private ObservableCollection<INTERNATIONAL> _list = null;
        public INTERNATIONALS()
        {
            INIT();
        }

        private void INIT()
        {
            DataTable dt = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select idx, areacode, nation_num, nation_local_num, natione, nationk, lm from INTERNATIONAL order by nationk asc");

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

            _list = new ObservableCollection<INTERNATIONAL>(
                (from _row in dt.AsEnumerable()
                 select new INTERNATIONAL()
                 {
                     idx = long.Parse(_row["idx"].ToString())
                     ,
                     areacode = int.Parse(_row["areacode"].ToString())
                     ,
                     nation_num = _row["nation_num"].ToString()
                     ,
                     nation_local_num = _row["nation_local_num"].ToString()
                     ,
                     natione = _row["natione"].ToString()
                     ,
                     nationk = _row["nationk"].ToString()
                     ,
                     lm = _row["lm"].ToString()
                 }
                ).ToList<INTERNATIONAL>()
            );
        }

        public ObservableCollection<INTERNATIONAL> GETLIST()
        {
            return _list;
        }

        public void ADD(INTERNATIONAL item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into INTERNATIONAL");
            sb.Append("( areacode, nation_num, nation_local_num, natione, nationk, lm )");
            sb.Append(" values ");
            sb.AppendFormat("( {0}, '{1}', '{2}', '{3}', '{4}', '{5}' )", item.areacode, item.nation_num, item.nation_local_num, item.natione, item.nationk, item.lm);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    db.BeginTran();
                    int count = db.GetEffectedCount();
                    db.Commit();
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

        public void MODIFY(INTERNATIONAL item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update INTERNATIONAL set");
            sb.AppendFormat(" areacode={0}, nation_num='{1}', nation_local_num='{2}', natione='{3}', nationk='{4}', lm='{5}' where idx={6}", item.areacode, item.nation_num, item.nation_local_num, item.natione, item.nationk, item.lm, item.idx);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    db.BeginTran();
                    int count = db.GetEffectedCount();
                    db.Commit();
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

        public void REMOVE(INTERNATIONAL item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from INTERNATIONAL where idx={0}", item.idx);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    db.BeginTran();
                    int count = db.GetEffectedCount();
                    db.Commit();
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
