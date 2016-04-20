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
    public class Floor
    {
        public string txtFloor { get; set; }
        //public ObservableCollection<TXT_ROOM> list { get; set; }
        public ObservableCollection<RoomItem> list { get; set; }
    }

    public class RoomItem : INotifyPropertyChanged
    {
        private string _roomnum = string.Empty;
        private string _languages = string.Empty;
        private int _hour = -1;
        private int _minutes = -1;
        private bool _ischecked = false;
        private string _states = string.Empty;
        private string _states_clean = string.Empty;
        private string _states_laundary = string.Empty;
        private string _states_parcel = string.Empty;
        private string _states_morningcall = string.Empty;
        private string _states_dnd = string.Empty;
        private double _width = 0;
        private double _height = 0;
        private double _cwidth = 0;
        private double _cheight = 0;
        private Thickness _margin;
        private HorizontalAlignment _halignment;
        private VerticalAlignment _valignment;

        public string RoomNum
        {
            get
            {
                return _roomnum;
            }
            set
            {
                _roomnum = value;
                OnPropertyChanged("RoomNum");
            }
        }

        public string Languages
        {
            get
            {
                return _languages;
            }
            set
            {
                _languages = value;
                OnPropertyChanged("Languages");
            }
        }

        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                OnPropertyChanged("Hour");
            }
        }

        public int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                _minutes = value;
                OnPropertyChanged("Minutes");
            }
        }

        public bool IsChecked
        {
            get
            {
                return _ischecked;
            }
            set
            {
                _ischecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public string States
        {
            get
            {
                return _states;
            }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        public string States_Clean
        {
            get
            {
                return _states_clean;
            }
            set
            {
                _states_clean = value;
                OnPropertyChanged("States_Clean");
            }
        }

        public string States_Laundary
        {
            get
            {
                return _states_laundary;
            }
            set
            {
                _states_laundary = value;
                OnPropertyChanged("States_Laundary");
            }
        }

        public string States_Parcel
        {
            get
            {
                return _states_parcel;
            }
            set
            {
                _states_parcel = value;
                OnPropertyChanged("States_Parcel");
            }
        }

        public string States_Morningcall
        {
            get
            {
                return _states_morningcall;
            }
            set
            {
                _states_morningcall = value;
                OnPropertyChanged("States_Morningcall");
            }
        }

        public string States_DnD
        {
            get
            {
                return _states_dnd;
            }
            set
            {
                _states_dnd = value;
                OnPropertyChanged("States_DnD");
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        public double CWidth
        {
            get
            {
                return _cwidth;
            }
            set
            {
                _cwidth = value;
                OnPropertyChanged("CWidth");
            }
        }

        public double CHeight
        {
            get
            {
                return _cheight;
            }
            set
            {
                _cheight = value;
                OnPropertyChanged("CHeight");
            }
        }

        public Thickness Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                _margin = value;
                OnPropertyChanged("Margin");
            }
        }

        public HorizontalAlignment HAlignment
        {
            get
            {
                return _halignment;
            }
            set
            {
                _halignment = value;
                OnPropertyChanged("HAlignment");
            }
        }

        public VerticalAlignment VAlignment
        {
            get
            {
                return _valignment;
            }
            set
            {
                _valignment = value;
                OnPropertyChanged("VAlignment");
            }
        }

        public _cgi_pms_data_type PMSDATA { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Floors : ObservableCollection<Floor>
    {
        public Floors()
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
                DataTable dt1 = null;
                using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
                {
                    DataTable _input = util.CreateDT2SP();
                    _input.Rows.Add("@I_ROOM", row[0].ToString());

                    try
                    {
                        dt1 = db.GetDataTableSP("GET_ROOM_BY_FLOOR", _input);
                    }
                    catch (FirebirdSql.Data.FirebirdClient.FbException ex)
                    {
                        util.WriteLog(string.Format("DB Error ({0}) : {1}", this.GetType(), ex.Message));
                    }
                }

                ObservableCollection<RoomItem> _list = new ObservableCollection<RoomItem>();
                foreach (DataRow _row in dt1.Rows)
                {
                    int err_count = 0;
                    RoomItem txtroom = new RoomItem() {
                        RoomNum = _row[0].ToString(),
                        States = string.IsNullOrEmpty(_row[1].ToString()) == true || _row[1].ToString().Equals("0") ? "0" : "1",
                        States_Clean = _row[4].ToString(),
                        States_Laundary = _row[5].ToString(),
                        States_Parcel = _row[6].ToString(),
                        IsChecked = false,
                        Height = 130,
                        Width = 130,
                        CWidth = 70,
                        CHeight = 70,
                        Margin = new Thickness(10, 10, 10, 10),
                        HAlignment = System.Windows.HorizontalAlignment.Left,
                        VAlignment = System.Windows.VerticalAlignment.Top
                    };

                    _cgi_pms_data_type pms_data_type;
                    using (HotelHelper hh = new HotelHelper(util.PBXIP))
                    {
                        pms_data_type = hh.GetPolicy(_row[2].ToString());
                        if (pms_data_type.status == STRUCTS.ERR_SOCKET_TIMEOUT)
                        {
                            _list.Add(txtroom);
                            continue;
                        }
                    }
                    txtroom.Hour = pms_data_type.hour;
                    txtroom.Minutes = pms_data_type.minutes;
                    if (txtroom.States.Equals("0"))
                    {
                        pms_data_type.language = 0;
                    }
                    txtroom.Languages = pms_data_type.language.ToString();
                    txtroom.PMSDATA = pms_data_type;

                    _list.Add(txtroom);
                }

                var lastitem = _list.Where(x => x.PMSDATA.status == STRUCTS.ERR_SOCKET_TIMEOUT);
                if (lastitem.Count() > 1)
                {
                    break;
                }
                this.Add(new Floor() { txtFloor = string.Format("{0}F", row[0].ToString()), list = _list });
            }
        }
    }
}
