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
    public partial class TXT_ROOM : UserControl, INotifyPropertyChanged
    {
        #region Dependency Property

        // Dependency Property
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked", typeof(Boolean), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsCheckValueChanged)));
        public static readonly DependencyProperty CheckSymbolWidthProperty = DependencyProperty.RegisterAttached("CheckSymbolWidth", typeof(Double), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckSymbolWidthChanged)));
        public static readonly DependencyProperty CheckSymbolHeightProperty = DependencyProperty.RegisterAttached("CheckSymbolHeight", typeof(Double), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckSymbolHeightChanged)));
        public static readonly DependencyProperty ROOMProperty = DependencyProperty.RegisterAttached("ROOM", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnROOMChanged)));
        public static readonly DependencyProperty LANGUAGESProperty = DependencyProperty.RegisterAttached("LANGUAGES", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLANGUAGESChanged)));
        public static readonly DependencyProperty MORNINGCALLProperty = DependencyProperty.RegisterAttached("MORNINGCALL", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMORNINGCALLChanged)));
        public static readonly DependencyProperty STATESProperty = DependencyProperty.RegisterAttached("STATES", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESChanged)));
        public static readonly DependencyProperty STATESCleanProperty = DependencyProperty.RegisterAttached("STATESClean", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESCleanChanged)));
        public static readonly DependencyProperty STATESLaundaryProperty = DependencyProperty.RegisterAttached("STATESLaundary", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESLaundaryChanged)));
        public static readonly DependencyProperty STATESParcelProperty = DependencyProperty.RegisterAttached("STATESParcel", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESParcelChanged)));
        public static readonly DependencyProperty STATESMorningcallProperty = DependencyProperty.RegisterAttached("STATESMorningcall", typeof(String), typeof(TXT_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSTATESMorningcallChanged)));

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
            var item = (TXT_ROOM)d;

            if ((Boolean)e.NewValue)
            {
                item.checksign.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                item.checksign.Visibility = System.Windows.Visibility.Collapsed;
            }
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
            var item = (TXT_ROOM)d;
            item.checksign.Width = (Double)e.NewValue;
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
            var item = (TXT_ROOM)d;
            item.checksign.Height = (Double)e.NewValue;
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
            var item = (TXT_ROOM)d;
            item.txtroomnum.Text = e.NewValue.ToString();
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
            var item = (TXT_ROOM)d;
            item.txtlanguage.Text = e.NewValue.ToString();
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
            var item = (TXT_ROOM)d;
            item.txtmorning.Text = e.NewValue.ToString();
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
            var item = (TXT_ROOM)d;
            ChangeFontStyle(item);
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
            var item = (TXT_ROOM)d;

            Brush color = null;

            switch (item.STATESClean)
            {
                case "0":
                    color = Brushes.White;
                    break;
                case "1":
                    color = Brushes.Red;
                    break;
                case "2":
                    color = Brushes.Yellow;
                    break;
                case "3":
                    color = Brushes.Blue;
                    break;
                default:
                    color = Brushes.White;
                    break;
            }

            item.states_clean.Fill = color;
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
            var item = (TXT_ROOM)d;

            Brush color = null;

            switch (item.STATESLaundary)
            {
                case "0":
                    color = Brushes.White;
                    break;
                case "1":
                    color = Brushes.Red;
                    break;
                default:
                    color = Brushes.Black;
                    break;
            }

            item.states_laundary.Fill = color;
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
            var item = (TXT_ROOM)d;

            Brush color = null;

            switch (item.STATESParcel)
            {
                case "0":
                    color = Brushes.White;
                    break;
                case "1":
                    color = Brushes.Red;
                    break;
                case "2":
                    color = Brushes.White;
                    break;
                default:
                    color = Brushes.Black;
                    break;
            }

            item.states_parcel.Fill = color;
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
            var item = (TXT_ROOM)d;
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

        public double ROOM_FONTSIZE
        {
            get
            {
                return txtroomnum.FontSize;
            }
            set
            {
                txtroomnum.FontSize = value;
            }
        }

        public double MCALL_FONTSIZE
        {
            get
            {
                return txtmorning.FontSize;
            }
            set
            {
                txtmorning.FontSize = value;
            }
        }

        public double LANG_FONTSIZE
        {
            get
            {
                return txtlanguage.FontSize;
            }
            set
            {
                txtlanguage.FontSize = value;
            }
        }

        #endregion Properties

        private System.Timers.Timer _timer;

        public TXT_ROOM()
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
                    if (states_clean.Fill != Brushes.White)
                        states_clean.Fill = Brushes.White;
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

        private static void ChangeFontStyle(TXT_ROOM item)
        {
            switch (item.STATES)
            {
                case "1":
                    GradientStopCollection gc = new GradientStopCollection();
                    gc.Add(new GradientStop(Color.FromRgb(255, 166, 0), 0));
                    gc.Add(new GradientStop(Color.FromRgb(249, 41, 0), 0.5));
                    gc.Add(new GradientStop(Color.FromRgb(255, 166, 0), 1));

                    LinearGradientBrush gradientBrush = new LinearGradientBrush(gc);
                    SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromRgb(249, 41, 0));

                    item.txtroomnum.Fill = gradientBrush;
                    item.txtroomnum.Stroke = solidColorBrush;

                    item.txtlanguage.Fill = gradientBrush;
                    item.txtlanguage.Stroke = solidColorBrush;

                    item.txtmorning.Fill = gradientBrush;
                    item.txtmorning.Stroke = solidColorBrush;
                    break;
                case "0":
                    item.txtroomnum.Fill = Brushes.White;
                    item.txtroomnum.Stroke = Brushes.Black;

                    item.txtlanguage.Fill = Brushes.White;
                    item.txtlanguage.Stroke = Brushes.Black;

                    item.txtmorning.Fill = Brushes.White;
                    item.txtmorning.Stroke = Brushes.Black;
                    break;
            }
        }

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            var item = (Ellipse)e.OriginalSource;
            var items = states_circle.Children;
            int idx = items.IndexOf(item);
        }
    }
}
