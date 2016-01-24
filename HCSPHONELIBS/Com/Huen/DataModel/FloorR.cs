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
    public class FloorR : INotifyPropertyChanged
    {
        private string _txtFloor = string.Empty;

        public string txtFloor
        {
            get
            {
                return _txtFloor;
            }
            set
            {
                _txtFloor = value;
                OnPropertyChanged(txtFloor);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FloorRs : ObservableCollection<FloorR>
    {
        public FloorRs()
        {
            DataTable dt = null;
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTableSP("GET_FLOOR");
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException ex)
                {
                    util.WriteLog(string.Format("DB Error ({0}) : {1}", this.GetType(), ex.Message));
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                this.Add(new FloorR() { txtFloor = row[0].ToString() });
            }
        }
    }

    public class RoomR
    {
        public string txtRoom { get; set; }
    }

    public class RoomRs : ObservableCollection<RoomR>
    {
        public RoomRs(string floor)
        {
            DataTable dt = null;
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                DataTable _input = util.CreateDT2SP();
                _input.Rows.Add("@I_ROOM", floor);

                try
                {
                    dt = db.GetDataTableSP("GET_ROOM_BY_FLOOR", _input);

                    foreach (DataRow row in dt.Rows)
                    {
                        this.Add(new RoomR() { txtRoom = row[0].ToString() });
                    }
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException ex)
                {
                    util.WriteLog(string.Format("DB Error ({0}) : {1}", this.GetType(), ex.Message));
                }
            }
        }
    }

    public class TelR
    {
        public string txtTel { get; set; }
    }

    public class TelRs : ObservableCollection<TelR>
    {
        public TelRs(string val)
        {
            DataTable dt = null;
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                DataTable _input = util.CreateDT2SP();
                _input.Rows.Add("@I_ROOM", val);

                try
                {
                    dt = db.GetDataTableSP("GET_TEL_BY_ROOM", _input);

                    foreach (DataRow row in dt.Rows)
                    {
                        this.Add(new TelR() { txtTel = row[0].ToString() });
                    }
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException ex)
                {
                    util.WriteLog(string.Format("DB Error ({0}) : {1}", this.GetType(), ex.Message));
                }
            }
        }
    }
}
