using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class CallingRateException : INotifyPropertyChanged
    {
        private string _telnum = string.Empty;
        private string _memo = string.Empty;

        public string TelNum
        {
            get
            {
                return _telnum;
            }
            set
            {
                _telnum = value;
                OnPropertyChanged("TelNum");
            }
        }
        public string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = value;
                OnPropertyChanged("Memo");
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
