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
    public class DOMESTICRATE : INotifyPropertyChanged
    {
        private string _prefix = string.Empty;
        private int _firmidx = 0;
        private string _type = string.Empty;
        private float _rate = 0.0f;
        private int _sec = 0;

        public string prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
                OnPropertyChanged("prefix");
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

        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged("type");
            }
        }

        public float rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
                OnPropertyChanged("rate");
            }
        }

        public int sec
        {
            get
            {
                return _sec;
            }
            set
            {
                _sec = value;
                OnPropertyChanged("sec");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DOMESTICRATES
    {
        private ObservableCollection<DOMESTICRATE> _list = null;
        public DOMESTICRATES()
        {
            INIT();
        }

        private void INIT()
        {
            DataTable dt = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select prefix, type, rate, sec from DOMESTIC_RATE order by prefix asc");

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

            _list = new ObservableCollection<DOMESTICRATE>(
                (from _row in dt.AsEnumerable()
                 select new DOMESTICRATE()
                 {
                     prefix = _row["prefix"].ToString()
                     ,
                     type = _row["type"].ToString()
                     ,
                     rate = float.Parse(_row["rate"].ToString())
                     ,
                     sec = int.Parse(_row["sec"].ToString())
                 }
                ).ToList<DOMESTICRATE>()
            );
        }

        public ObservableCollection<DOMESTICRATE> GETLIST()
        {
            return _list;
        }

        public void ADD(DOMESTICRATE item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into DOMESTIC_RATE");
            sb.Append(" ( prefix, type, rate, sec )");
            sb.Append(" values ");
            sb.AppendFormat("( '{0}', '{1}', {2}, {3} )", item.prefix, item.type, item.rate, item.sec);

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

            //INIT();
            _list.Add(item);
        }

        public void MODIFY(DOMESTICRATE item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update DOMESTIC_RATE set");
            sb.AppendFormat(" type='{0}', rate={1}, sec={2} where prefix='{3}'", item.type, item.rate, item.sec, item.prefix);

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

            //INIT();
        }

        public void REMOVE(DOMESTICRATE item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from DOMESTIC_RATE where prefix={0}", item.prefix);

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

            _list.Remove(item);
            //INIT();
        }
    }
}
