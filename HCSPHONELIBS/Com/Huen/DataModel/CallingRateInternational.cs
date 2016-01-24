using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class CallingRateInternational : INotifyPropertyChanged
    {
        private double _lrate = 1.0d;
        private Int32 _lsec = 1;
        private double _mrate = 1.0d;
        private Int32 _msec = 1;

        public string Kind { get; set; }
        public double LRate
        {
            get
            {
                return _lrate;
            }
            set
            {
                _lrate = value;
                OnPropertyChanged("LRate");
            }
        }
        public Int32 LSec
        {
            get
            {
                return _lsec;
            }
            set
            {
                _lsec = value;
                OnPropertyChanged("LSec");
            }
        }
        public double MRate
        {
            get
            {
                return _mrate;
            }
            set
            {
                _mrate = value;
                OnPropertyChanged("MRate");
            }
        }
        public int MSec
        {
            get
            {
                return _msec;
            }
            set
            {
                _msec = value;
                OnPropertyChanged("MSec");
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
