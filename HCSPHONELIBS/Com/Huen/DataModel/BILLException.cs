using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.Sql;

namespace Com.Huen.DataModel
{
    public class BILLException : INotifyPropertyChanged
    {
        private string _innertel = string.Empty;
        private string _title = string.Empty;

        public string innertel
        {
            get
            {
                return _innertel;
            }
            set
            {
                _innertel = value;
                OnPropertyChanged("innertel");
            }
        }

        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged("title");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BILLExceptions
    {
        private ObservableCollection<BILLException> _list = null;
        public BILLExceptions()
        {
            INIT();
        }

        private void INIT()
        {
            DataTable dt = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("select innertel, title from EXCEPTIONS order by innertel asc");

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

            _list = new ObservableCollection<BILLException>(
                (from _row in dt.AsEnumerable()
                 select new BILLException()
                 {
                     innertel = _row[0].ToString()
                     , title = _row[1].ToString()
                 }
                ).ToList<BILLException>()
            );
        }

        public ObservableCollection<BILLException> GETLIST()
        {
            return _list;
        }

        public void ADD(BILLException item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into EXCEPTIONS");
            sb.Append("( innertel, title )");
            sb.Append(" values ");
            sb.AppendFormat("( '{0}', '{1}' )", item.innertel, item.title);

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

        public void MODIFY(BILLException item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update EXCEPTIONS set");
            sb.AppendFormat(" title='{0}' where innertel='{1}'", item.title, item.innertel);

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

        public void REMOVE(BILLException item)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from EXCEPTIONS where innertel={0}", item.innertel);

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
