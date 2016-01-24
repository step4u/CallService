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

namespace Com.Huen.UserControls
{
    /// <summary>
    /// Interaction logic for ICO_ROOM.xaml
    /// </summary>
    public partial class ICO_ROOM : UserControl, INotifyPropertyChanged
    {
        #region Dependency Property

        // Dependency Property
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked", typeof(Boolean), typeof(ICO_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsCheckValueChanged)));
        public static readonly DependencyProperty CheckSymbolWidthProperty = DependencyProperty.RegisterAttached("CheckSymbolWidth", typeof(Double), typeof(ICO_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckSymbolWidthChanged)));
        public static readonly DependencyProperty CheckSymbolHeightProperty = DependencyProperty.RegisterAttached("CheckSymbolHeight", typeof(Double), typeof(ICO_ROOM),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCheckSymbolHeightChanged)));

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
                if (value)
                {
                    checksign.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    checksign.Visibility = System.Windows.Visibility.Collapsed;
                }
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
                checksign.Width = value;
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
                checksign.Height = value;
            }
        }

        private static void OnCheckSymbolHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
        #endregion Dependency Property

        #region Properties

        private string o_room = string.Empty;
        private string o_telnum = string.Empty;
        private string o_mcall = string.Empty;
        private string o_language = string.Empty;
        private ROOMSTATES o_states = ROOMSTATES.CHECKOUT;
        public string ROOM
        {
            get
            {
                return o_room;
            }
            set
            {
                o_room = value;
            }
        }

        public string TELNUM
        {
            get
            {
                return o_telnum;
            }
            set
            {
                o_telnum = value;
            }
        }

        public string MORNINGCALL
        {
            get
            {
                return o_mcall;
            }
            set
            {
                o_mcall = value;
            }
        }

        public string LANGUAGE
        {
            get
            {
                return o_language;
            }
            set
            {
                o_language = value;
            }
        }

        public ROOMSTATES STATES
        {
            get
            {
                return o_states;
            }
            set
            {
                o_states = value;

                switch (o_states)
                {
                    case ROOMSTATES.CHECKIN:
                        ico_room.Fill = Brushes.CadetBlue;
                        ico_room.Stroke = Brushes.CadetBlue;
                        break;
                    case ROOMSTATES.CHECKOUT:
                        ico_room.Fill = Brushes.LightGray;
                        ico_room.Stroke = Brushes.LightGray;
                        break;
                }

                OnPropertyChanged("STATES");
            }
        }

        #endregion Properties

        public ICO_ROOM()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ROOMSTATES
    {
        CHECKIN,
        CHECKOUT
    }
}
