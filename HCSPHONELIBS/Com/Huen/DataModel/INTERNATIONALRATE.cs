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
    public class INTERNATIONALRATE : INotifyPropertyChanged
    {
        private int _areacode = 0;
        private int _firmidx = 0;
        private double _lrate = 0.0f;
        private int _lsec = 0;
        private double _mrate = 0.0f;
        private int _msec = 0;

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

        public double lrate
        {
            get
            {
                return _lrate;
            }
            set
            {
                _lrate = value;
                OnPropertyChanged("lrate");
            }
        }

        public int lsec
        {
            get
            {
                return _lsec;
            }
            set
            {
                _lsec = value;
                OnPropertyChanged("lsec");
            }
        }

        public double mrate
        {
            get
            {
                return _mrate;
            }
            set
            {
                _mrate = value;
                OnPropertyChanged("mrate");
            }
        }

        public int msec
        {
            get
            {
                return _msec;
            }
            set
            {
                _msec = value;
                OnPropertyChanged("msec");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class INTERNATIONALRATES
    {
        private ObservableCollection<INTERNATIONALRATE> _list = null;
        public INTERNATIONALRATES()
        {
            INIT();
        }

        private void INIT()
        {
            DataTable dt = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select areacode, lrate, lsec, mrate, msec from INTERNATIONAL_RATE order by areacode asc");

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

            _list = new ObservableCollection<INTERNATIONALRATE>(
                (from _row in dt.AsEnumerable()
                 select new INTERNATIONALRATE()
                 {
                     areacode = int.Parse(_row["areacode"].ToString())
                     ,
                     lrate = double.Parse(_row["lrate"].ToString())
                     ,
                     lsec = int.Parse(_row["lsec"].ToString())
                     ,
                     mrate = double.Parse(_row["mrate"].ToString())
                     ,
                     msec = int.Parse(_row["msec"].ToString())
                 }
                ).ToList<INTERNATIONALRATE>()
            );
        }

        public ObservableCollection<INTERNATIONALRATE> GETLIST()
        {
            return _list;
        }

        public void ADD(INTERNATIONALRATE item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into INTERNATIONAL_RATE");
            sb.Append("( areacode, lrate, lsec, mrate, msec )");
            sb.Append(" values ");
            sb.AppendFormat("( {0}, {1}, {2}, {3}, {4} )", item.areacode, item.lrate, item.lsec, item.mrate, item.msec);

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

            _list.Add(item);

            //INIT();
        }

        public void MODIFY(INTERNATIONALRATE item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update INTERNATIONAL_RATE set");
            sb.AppendFormat(" lrate={0}, lsec={1}, mrate={2}, msec={3} where areacode={4}", item.lrate, item.lsec, item.mrate, item.msec, item.areacode);

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

            INTERNATIONALRATE _tmpobj = (INTERNATIONALRATE)_list.Where(x => x.areacode == item.areacode);
            _tmpobj.lrate = item.lrate;
            _tmpobj.lsec = item.lsec;
            _tmpobj.mrate = item.mrate;
            _tmpobj.msec = item.msec;

            //INIT();
        }

        public void REMOVE(INTERNATIONALRATE item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from INTERNATIONAL_RATE where areacode={0}", item.areacode);

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

            _list.Remove(item);

            //INIT();
        }
    }
}
