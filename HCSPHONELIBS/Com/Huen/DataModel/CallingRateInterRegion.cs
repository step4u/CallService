using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class CallingRateInterRegion : INotifyPropertyChanged
    {
        private string _kind = string.Empty;
        private string _nationcode = string.Empty;
        private string _areacode = string.Empty;
        private string _nameko = string.Empty;
        private string _nameen = string.Empty;
        private string _lm = string.Empty;

        public string Kind
        {
            get
            {
                return _kind;
            }
            set
            {
                _kind = value;
                OnPropertyChanged("Kind");
            }
        }
        public string NationCode
        {
            get
            {
                return _nationcode;
            }
            set
            {
                _nationcode = value;
                OnPropertyChanged("NationCode");
            }
        }
        public string AreaCode
        {
            get
            {
                return _areacode;
            }
            set
            {
                _areacode = value;
                OnPropertyChanged("AreaCode");
            }
        }
        public string NameKo
        {
            get
            {
                return _nameko;
            }
            set
            {
                _nameko = value;
                OnPropertyChanged("NameKo");
            }
        }
        public string NameEn
        {
            get
            {
                return _nameen;
            }
            set
            {
                _nameen = value;
                OnPropertyChanged("NameEn");
            }
        }
        public string LM
        {
            get
            {
                return _lm;
            }
            set
            {
                _lm = value;
                OnPropertyChanged("LM");
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
