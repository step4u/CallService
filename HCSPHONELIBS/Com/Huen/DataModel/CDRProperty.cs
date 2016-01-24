using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class CDRProperty
    {
        public ObservableCollection<CallingRate> CDRProperties = new ObservableCollection<CallingRate>();
        public ObservableCollection<CallingRateInternational> CDRInternational = new ObservableCollection<CallingRateInternational>();
        public ObservableCollection<CallingRateInterRegion> CDRInterRegion = new ObservableCollection<CallingRateInterRegion>();
        public ObservableCollection<CallingRateException> CDRException = new ObservableCollection<CallingRateException>();
        public bool etc_01 = false;
    }
}
