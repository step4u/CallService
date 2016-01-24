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
using System.Windows.Shapes;

using System.ServiceProcess;
using System.Collections.ObjectModel;
using System.IO;

using Com.Huen.Libs;
using Com.Huen.DataModel;
using Com.Huen.Sql;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for CdrAgent.xaml
    /// </summary>
    public partial class CdrAgent : Window
    {
        public ModifyRegistry _reg;
        
        private System.Windows.Forms.NotifyIcon ni;
        private ServiceController _servicecontroller;
        private bool _trueExit = false;
        public bool IsCdrAgent = true;

        public CdrAgent()
        {
            InitializeComponent();

            this.Loaded += CdrAgent_Loaded;
            this.Closing += CdrAgent_Closing;
            this.Closed += CdrAgent_Closed;

            InitializeProperties();
            InitializeWindow();

            if (IsCdrAgent)
            {
                TrayIconInitialize();
                tab4.Visibility = System.Windows.Visibility.Visible;
                this.Hide();
            }
        }

        public CdrAgent(bool iscdragent)
        {
            InitializeComponent();

            IsCdrAgent = iscdragent;

            this.Loaded += CdrAgent_Loaded;
            this.Closing += CdrAgent_Closing;
            this.Closed += CdrAgent_Closed;

            InitializeProperties();
            InitializeWindow();

            if (IsCdrAgent)
            {
                TrayIconInitialize();
                tab4.Visibility = System.Windows.Visibility.Visible;
                this.Hide();
            }
        }

        void CdrAgent_Closed(object sender, EventArgs e)
        {
            if (IsCdrAgent)
            {
                ni.Visible = false;
            }
        }

        void CdrAgent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_trueExit)
            {
                e.Cancel = true;
                this.Hide();
            }

            this.SaveProps();
        }

        void CdrAgent_Loaded(object sender, RoutedEventArgs e)
        {
            tabs.SelectionChanged += tabs_SelectionChanged;
        }

        void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.Source is TabControl)) return;

            TabControl _tabs = (TabControl)sender;

            int _selectedidx = _tabs.SelectedIndex;
            //TabItem _tab = (TabItem)_tabs.SelectedItem;

            switch (_selectedidx)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;
            }
        }

        public bool? _runstart = false;
        private void InitializeProperties()
        {
            Ini ini = new Ini(@".\cdr.ini");
            string _start = ini.IniReadValue("ETC", "RUNSTART");
            _runstart = string.IsNullOrEmpty(_start) == true ? false : bool.Parse(_start);
            props_etc_01.IsChecked = _runstart;
            
            /*
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into international_rate ( areacode, firms_idx, lrate, lsec, mrate, msec )");
            sb.Append(" values ");
            sb.Append("( {0}, 74, {1}, {2}, {3}, {4} )");
            string query = string.Empty;

            foreach (var item in _cdrprops.CDRInternational)
            {
                query = string.Format(sb.ToString(), item.Kind, item.LRate, item.LSec, item.MRate, item.MSec);
                using (FirebirdDBHelper db = new FirebirdDBHelper(query, util.strDBConn))
                {
                    int _count = db.GetEffectedCount();
                }
            }
            */

            /*
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into international");
            sb.Append("( areacode, nation_num, nation_local_num, natione, nationk, lm )");
            sb.Append(" values ");
            sb.Append("( {0}, '{1}', '{2}', '{3}', '{4}', '{5}' )");
            string query = string.Empty;

            foreach (var item in _cdrprops.CDRInterRegion)
            {
                query = string.Format(sb.ToString(), item.Kind, item.NationCode, item.AreaCode, item.NameEn, item.NameKo, item.LM);
                using (FirebirdDBHelper db = new FirebirdDBHelper(query, util.strDBConn))
                {
                    int _count = db.GetEffectedCount();
                }
            }
            */

            /*
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into DOMESTIC_RATE");
            sb.Append("( prefix, firms_idx, type, rate, sec )");
            sb.Append(" values ");
            sb.Append("( '{0}', 1, '{1}', {2}, {3} )");
            string query = string.Empty;

            foreach (var item in _cdrprops.CDRProperties)
            {
                string _type = string.Empty;
                if (item.Type == "이동")
                {
                    _type = "M";
                }
                else if (item.Type == "시내")
                {
                    _type = "I";
                }
                else if (item.Type == "시외")
                {
                    _type = "O";
                }

                query = string.Format(sb.ToString(), item.Prefix, _type, item.Rate, item.Sec);
                using (FirebirdDBHelper db = new FirebirdDBHelper(query, util.strDBConn))
                {
                    int _count = db.GetEffectedCount();
                }
            }
            */
        }

        public DOMESTICRATES domestics = null;
        public INTERNATIONALRATES internationalrates = null;
        public INTERNATIONALS internationals = null;
        public BILLExceptions billexceptions = null;

        public ObservableCollection<DOMESTICRATE> _domestics = null;
        public ObservableCollection<INTERNATIONALRATE> _internationalrates = null;
        public ObservableCollection<INTERNATIONAL> _internationals = null;
        public ObservableCollection<BILLException> _billexceptions = null;
        private void InitializeWindow()
        {
            domestics = new DOMESTICRATES();
            internationalrates = new INTERNATIONALRATES();
            internationals = new INTERNATIONALS();
            billexceptions = new BILLExceptions();

            listview_rate.ItemsSource = _domestics = domestics.GETLIST();
            lv_interrate.ItemsSource = _internationalrates = internationalrates.GETLIST();
            lv_nation.ItemsSource = _internationals = internationals.GETLIST();
            lv_exception.ItemsSource = _billexceptions = billexceptions.GETLIST();
        }

        private void TrayIconInitialize()
        {
            _servicecontroller = new ServiceController("Huen CDR Service");

            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = (System.Drawing.Icon)util.LoadProjectResource("icon", "COMMONRES", "");
            ni.Text = string.Format("{0} ({1})", "Huen CDR Agent", util.LoadProjectResource("HOTEL_CODE", "COMMONRES", "").ToString());

            System.Windows.Forms.ContextMenu contextmenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem miTitle = new System.Windows.Forms.MenuItem();
            miTitle.Index = 0;
            miTitle.Enabled = false;
            miTitle.Text = "Huen CDR Agent";

            System.Windows.Forms.MenuItem miSeperator0 = new System.Windows.Forms.MenuItem();
            miSeperator0.Index = 1;
            miSeperator0.Text = "-";

            System.Windows.Forms.MenuItem mi0 = new System.Windows.Forms.MenuItem();
            mi0.Index = 2;
            mi0.Text = "환경설정(&P)";
            mi0.Click += delegate(object sender, EventArgs args)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            System.Windows.Forms.MenuItem mi1 = new System.Windows.Forms.MenuItem();
            mi1.Index = 3;
            mi1.Text = "서비스 시작(&S)";
            mi1.Click += delegate(object sender, EventArgs args)
            {
                if (_servicecontroller.Status == ServiceControllerStatus.Stopped)
                {
                    _servicecontroller.Start();
                }
            };

            System.Windows.Forms.MenuItem mi2 = new System.Windows.Forms.MenuItem();
            mi2.Index = 4;
            mi2.Text = "서비스 멈춤(&S)";
            mi2.Enabled = false;
            mi2.Click += delegate(object sender, EventArgs args)
            {
                if (_servicecontroller.Status == ServiceControllerStatus.Running)
                {
                    _servicecontroller.Stop();
                }
            };

            System.Windows.Forms.MenuItem miSeperator1 = new System.Windows.Forms.MenuItem();
            miSeperator1.Index = 5;
            miSeperator1.Text = "-";

            System.Windows.Forms.MenuItem mi3 = new System.Windows.Forms.MenuItem();
            mi3.Index = 6;
            mi3.Text = "종료(&X)";
            mi3.Click += delegate(object sender, EventArgs args)
            {
                this._trueExit = true;
                this.Close();
            };

            contextmenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { miTitle, miSeperator0, mi0, mi1, mi2, miSeperator1, mi3 });

            this.ni.ContextMenu = contextmenu;
            this.ni.Visible = true;
            this.ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            this.ni.ContextMenu.Popup += ContextMenu_Popup;
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            _servicecontroller = new ServiceController("Huen CDR Service");

            System.Windows.Forms.ContextMenu __contextmenu = (System.Windows.Forms.ContextMenu)sender;
            var __menuitemcollection = __contextmenu.MenuItems;

            try
            {
                foreach (System.Windows.Forms.MenuItem _menuitem in __menuitemcollection)
                {
                    _menuitem.Enabled = true;
                }

                System.Windows.Forms.MenuItem __item;
                if (_servicecontroller.Status == ServiceControllerStatus.Stopped)
                {
                    __item = __menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 3);
                    __item.Enabled = true;
                    __item = __menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 4);
                    __item.Enabled = false;
                }
                else if (_servicecontroller.Status == ServiceControllerStatus.Running)
                {
                    __item = __menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 3);
                    __item.Enabled = false;
                    __item = __menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 4);
                    __item.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MenuItem __item;
                __item = __menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 3);
                __item.Enabled = false;
                __item = __menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 4);
                __item.Enabled = false;
            }
        }

        private void lv_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView lv = (ListView)e.Source;
            ContextMenu cm = lv.ContextMenu;

            foreach (MenuItem mi in cm.Items)
            {
                mi.IsEnabled = true;
            }

            int __locSelectedIndex = lv.SelectedIndex;

            for (int i = 0; i < cm.Items.Count; i++)
            {
                MenuItem mi = (MenuItem)cm.Items[i];

                if (__locSelectedIndex == -1)
                {
                    if (i == 1 || i == 2)
                        mi.IsEnabled = false;
                }
                else
                {
                    if (i == 1 || i == 2)
                        mi.IsEnabled = true;
                }
            }
        }

        #region 탭0 contextmenu s
        private void tab0_menu0_Click(object sender, RoutedEventArgs e)
        {
            //탭1 추가
            AddPrefix __pop = new AddPrefix(AddPrefix.AddPrefixStates.Add);
            __pop.Owner = this;
            __pop.Show();
        }

        private void tab0_menu1_Click(object sender, RoutedEventArgs e)
        {
            //탭1 수정
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            DOMESTICRATE __rowview = (DOMESTICRATE)__lv.SelectedItem;

            AddPrefix __pop = new AddPrefix(AddPrefix.AddPrefixStates.Modify);
            __pop.Owner = this;
            __pop._domesticrate = __rowview;
            __pop.Show();
        }

        private void tab0_menu2_Click(object sender, RoutedEventArgs e)
        {
            //탭1 삭제
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            DOMESTICRATE __rowview = (DOMESTICRATE)__lv.SelectedItem;

            domestics.REMOVE(__rowview);
        }

        private void tab1_menu0_Click(object sender, RoutedEventArgs e)
        {
            //탭2 추가
            AddPrefixInternational __pop = new AddPrefixInternational(AddPrefixStates.Add);
            __pop.Owner = this;
            __pop.Show();
        }

        private void tab1_menu1_Click(object sender, RoutedEventArgs e)
        {
            //탭2 수정
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            INTERNATIONALRATE __rowview = (INTERNATIONALRATE)__lv.SelectedItem;

            AddPrefixInternational __pop = new AddPrefixInternational(AddPrefixStates.Modify);
            __pop.Owner = this;
            __pop._obj = __rowview;
            __pop.Show();
        }

        private void tab1_menu2_Click(object sender, RoutedEventArgs e)
        {
            //탭2 삭제
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            INTERNATIONALRATE __rowview = (INTERNATIONALRATE)__lv.SelectedItem;

            internationalrates.REMOVE(__rowview);
        }

        private void tab2_menu0_Click(object sender, RoutedEventArgs e)
        {
            AddPrefixInterRegion __pop = new AddPrefixInterRegion();
            __pop.Owner = this;
            __pop.Show();
        }

        private void tab2_menu1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            CallingRateInterRegion __rowview = (CallingRateInterRegion)__lv.SelectedItem;

            AddPrefixInterRegion __pop = new AddPrefixInterRegion();
            __pop.Owner = this;
            __pop._propsValue = __rowview;
            __pop.Show();
        }

        private void tab2_menu2_Click(object sender, RoutedEventArgs e)
        {
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            CallingRateInterRegion __rowview = (CallingRateInterRegion)__lv.SelectedItem;
        }

        private void tab3_menu0_Click(object sender, RoutedEventArgs e)
        {
            AddPrefixException __pop = new AddPrefixException();
            __pop.Owner = this;
            __pop.Show();
        }

        private void tab3_menu1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            CallingRateException __rowview = (CallingRateException)__lv.SelectedItem;

            AddPrefixException __pop = new AddPrefixException();
            __pop.Owner = this;
            __pop._propsValue = __rowview;
            __pop.Show();
        }

        private void tab3_menu2_Click(object sender, RoutedEventArgs e)
        {
            MenuItem __menuItem = (MenuItem)e.Source;
            ContextMenu __contextMenu = (ContextMenu)__menuItem.Parent;
            ListView __lv = (ListView)__contextMenu.PlacementTarget;
            CallingRateException __rowview = (CallingRateException)__lv.SelectedItem;
        }
        #endregion 탭0 contextmenu e

        public void SaveProps()
        {
            Ini ini = new Ini(@".\cdr.ini");
            ini.IniWriteValue("ETC", "RUNSTART", _runstart.ToString());
        }

        private void props_etc_01_Checked(object sender, RoutedEventArgs e)
        {
            ModifyRegistry __reg = new ModifyRegistry(util.LoadProjectResource("REG_MAIN_RUN", "COMMONRES", "").ToString());
            string __curDirectory = Directory.GetCurrentDirectory();
            __reg.SetValue(RegKind.LocalMachine, "CdrAgent", string.Format("{0}\\CdrAgent.exe", __curDirectory));

            _runstart = true;
            this.SaveProps();
        }

        private void props_etc_01_Unchecked(object sender, RoutedEventArgs e)
        {
            ModifyRegistry __reg = new ModifyRegistry(util.LoadProjectResource("REG_MAIN_RUN", "COMMONRES", "").ToString());
            __reg.DeleteValue(RegKind.LocalMachine, "CdrAgent");

            _runstart = false;
            this.SaveProps();
        }
    }
}
