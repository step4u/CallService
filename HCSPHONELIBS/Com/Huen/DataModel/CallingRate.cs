using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class CallingRate : INotifyPropertyChanged
    {
        private string _type = string.Empty;
        private double _rate = 1.0d;
        private int _sec = 1;

        public string Prefix { get; set; }
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                _rate = value;
                OnPropertyChanged("Rate");
            }
        }
        public int Sec
        {
            get
            {
                return _sec;
            }
            set
            {
                _sec = value;
                OnPropertyChanged("Sec");
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
