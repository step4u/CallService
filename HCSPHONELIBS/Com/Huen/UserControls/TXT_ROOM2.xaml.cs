using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Windows.Threading;

using Com.Huen.Sockets;
using Com.Huen.Libs;

namespace Com.Huen.UserControls
{
    /// <summary>
    /// Interaction logic for ICO_ROOM.xaml
    /// </summary>
    public partial class TXT_ROOM2 : UserControl, INotifyPropertyChanged
    {
        #region Dependency Property

        // Dependency Property
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked", typeof(Boolean), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsCheckValueChanged)));
        public static readonly DependencyProperty CheckSymbolWidthProperty = DependencyProperty.RegisterAttached("CheckSymbolWidth", typeof(Double), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckSymbolWidthChanged)));
        public static readonly DependencyProperty CheckSymbolHeightProperty = DependencyProperty.RegisterAttached("CheckSymbolHeight", typeof(Double), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckSymbolHeightChanged)));
        public static readonly DependencyProperty ROOMProperty = DependencyProperty.RegisterAttached("ROOM", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnROOMChanged)));
        public static readonly DependencyProperty LANGUAGESProperty = DependencyProperty.RegisterAttached("LANGUAGES", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLANGUAGESChanged)));
        public static readonly DependencyProperty MORNINGCALLProperty = DependencyProperty.RegisterAttached("MORNINGCALL", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMORNINGCALLChanged)));
        public static readonly DependencyProperty STATESProperty = DependencyProperty.RegisterAttached("STATES", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESChanged)));
        public static readonly DependencyProperty STATESCleanProperty = DependencyProperty.RegisterAttached("STATESClean", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESCleanChanged)));
        public static readonly DependencyProperty STATESLaundaryProperty = DependencyProperty.RegisterAttached("STATESLaundary", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESLaundaryChanged)));
        public static readonly DependencyProperty STATESParcelProperty = DependencyProperty.RegisterAttached("STATESParcel", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESParcelChanged)));
        public static readonly DependencyProperty STATESMorningcallProperty = DependencyProperty.RegisterAttached("STATESMorningcall", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESMorningcallChanged)));
        public static readonly DependencyProperty STATESDnDProperty = DependencyProperty.RegisterAttached("STATESDnD", typeof(String), typeof(TXT_ROOM2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESDnDChanged)));

        // .NET Property wrapper
        public Boolean IsChecked
        {
            get
            {
                return (Boolean)GetValue(IsCheckedProperty);
            }
            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }

        private static void OnIsCheckValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public Double CheckSymbolWidth
        {
            get
            {
                return (Double)GetValue(CheckSymbolWidthProperty);
            }
            set
            {
                SetValue(CheckSymbolWidthProperty, value);
            }
        }

        private static void OnCheckSymbolWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        public Double CheckSymbolHeight
        {
            get
            {
                return (Double)GetValue(CheckSymbolHeightProperty);
            }
            set
            {
                SetValue(CheckSymbolHeightProperty, value);
            }
        }

        private static void OnCheckSymbolHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public string ROOM
        {
            get
            {
                return (String)GetValue(ROOMProperty);
            }
            set
            {
                SetValue(ROOMProperty, value);
            }
        }
        private static void OnROOMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;
            item.room_num.Text = e.NewValue.ToString();
        }

        public string LANGUAGES
        {
            get
            {
                return (String)GetValue(LANGUAGESProperty);
            }
            set
            {
                SetValue(LANGUAGESProperty, value);
            }
        }
        private static void OnLANGUAGESChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;
            item.room_language.Text = e.NewValue.ToString();
        }

        public string MORNINGCALL
        {
            get
            {
                return (String)GetValue(MORNINGCALLProperty);
            }
            set
            {
                SetValue(MORNINGCALLProperty, value);
            }
        }
        private static void OnMORNINGCALLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;
            item.room_morningcall.Text = e.NewValue.ToString();
        }

        public string STATES
        {
            get
            {
                return (String)GetValue(STATESProperty);
            }
            set
            {
                SetValue(STATESProperty, value);
            }
        }
        private static void OnSTATESChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;

            switch (item.STATES)
            {
                case "0":
                    item.room_checkout.Visibility = Visibility.Visible;
                    item.room_checkin.Visibility = Visibility.Hidden;

                    item.room_req_clean.Visibility = Visibility.Hidden;
                    item.room_end_clean.Visibility = Visibility.Hidden;
                    item.room_confirm_clean.Visibility = Visibility.Hidden;
                    item.room_req_laundary.Visibility = Visibility.Hidden;
                    item.room_end_laundary.Visibility = Visibility.Hidden;
                    item.room_req_parcel.Visibility = Visibility.Hidden;
                    break;
                case "1":
                    item.room_checkout.Visibility = Visibility.Hidden;
                    item.room_checkin.Visibility = Visibility.Visible;
                    break;
            }
        }

        public string STATESClean
        {
            get
            {
                return (String)GetValue(STATESCleanProperty);
            }
            set
            {
                SetValue(STATESCleanProperty, value);
            }
        }

        private static void OnSTATESCleanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;

            switch (item.STATESClean)
            {
                case "0":
                    item.room_req_clean.Visibility = Visibility.Hidden;
                    item.room_end_clean.Visibility = Visibility.Hidden;
                    item.room_confirm_clean.Visibility = Visibility.Hidden;
                    break;
                case "1":
                    item.room_req_clean.Visibility = Visibility.Visible;
                    item.room_end_clean.Visibility = Visibility.Hidden;
                    item.room_confirm_clean.Visibility = Visibility.Hidden;
                    break;
                case "2":
                    item.room_req_clean.Visibility = Visibility.Hidden;
                    item.room_end_clean.Visibility = Visibility.Visible;
                    item.room_confirm_clean.Visibility = Visibility.Hidden;
                    break;
                case "3":
                    item.room_req_clean.Visibility = Visibility.Hidden;
                    item.room_end_clean.Visibility = Visibility.Hidden;
                    item.room_confirm_clean.Visibility = Visibility.Visible;
                    break;
                default:
                    item.room_req_clean.Visibility = Visibility.Hidden;
                    item.room_end_clean.Visibility = Visibility.Hidden;
                    item.room_confirm_clean.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public string STATESLaundary
        {
            get
            {
                return (String)GetValue(STATESLaundaryProperty);
            }
            set
            {
                SetValue(STATESLaundaryProperty, value);
            }
        }

        private static void OnSTATESLaundaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;

            switch (item.STATESLaundary)
            {
                case "0":
                    item.room_req_laundary.Visibility = Visibility.Hidden;
                    item.room_end_laundary.Visibility = Visibility.Hidden;
                    break;
                case "1":
                    item.room_req_laundary.Visibility = Visibility.Visible;
                    item.room_end_laundary.Visibility = Visibility.Hidden;
                    break;
                default:
                    item.room_req_laundary.Visibility = Visibility.Hidden;
                    item.room_end_laundary.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public string STATESParcel
        {
            get
            {
                return (String)GetValue(STATESParcelProperty);
            }
            set
            {
                SetValue(STATESParcelProperty, value);
            }
        }
        private static void OnSTATESParcelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;

            switch (item.STATESParcel)
            {
                case "0":
                    item.room_req_parcel.Visibility = Visibility.Hidden;
                    break;
                case "1":
                    item.room_req_parcel.Visibility = Visibility.Visible;
                    break;
                default:
                    item.room_req_parcel.Visibility = Visibility.Hidden;
                    break;
            }

        }

        public string STATESMorningcall
        {
            get
            {
                return (String)GetValue(STATESMorningcallProperty);
            }
            set
            {
                SetValue(STATESMorningcallProperty, value);
            }
        }
        private static void OnSTATESMorningcallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;
        }

        public string STATESDnD
        {
            get
            {
                return (String)GetValue(STATESDnDProperty);
            }
            set
            {
                SetValue(STATESDnDProperty, value);
            }
        }
        private static void OnSTATESDnDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (TXT_ROOM2)d;
            switch (item.STATESDnD)
            {
                case "1":
                    item.room_num.Foreground = Brushes.Red;
                    break;
                case "0":
                default:
                    item.room_num.Foreground = util.Str2Brush("#FFC7C0A2");
                    break;
            }
        }

        #endregion Dependency Property

        #region Properties

        private _pms_data_type o_pms_data_type;

        public _pms_data_type PMSDATA
        {
            get
            {
                return o_pms_data_type;
            }
            set
            {
                o_pms_data_type = value;
            }
        }
        #endregion Properties

        private System.Timers.Timer _timer;

        public TXT_ROOM2()
        {
            InitializeComponent();
            this.Loaded += TXT_ROOM_Loaded;
        }

        void TXT_ROOM_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Interval = 1000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
            _timer.Start();            
        }

        private int _count = 0;
        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            if (_count > 1)
            {
                if (now.Minute > 0)
                    _count = 0;

                return;
            }

            if (now.Hour == 24 && now.Minute == 0)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    this.room_req_clean.Visibility = Visibility.Hidden;
                    this.room_end_clean.Visibility = Visibility.Hidden;
                    this.room_confirm_clean.Visibility = Visibility.Hidden;

                }));
                _count++;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
