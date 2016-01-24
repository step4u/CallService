using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Com.Huen.DataModel
{
    public class Language : INotifyPropertyChanged
    {
        private int _langcode = -1;
        private string _langstr = string.Empty;

        public int LangCode
        {
            get
            {
                return _langcode;
            }
            set
            {
                _langcode = value;
                OnPropertyChanged("LangCode");
            }
        }

        public string LangStr
        {
            get
            {
                return _langstr;
            }
            set
            {
                _langstr = value;
                OnPropertyChanged("LangStr");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Languages : List<Language>
    {
        public Languages()
        {
            this.Add(new Language() { LangCode = 2, LangStr = "한국어" });
            this.Add(new Language() { LangCode = 1, LangStr = "English" });
        }
    }
}
