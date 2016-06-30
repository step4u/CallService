using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Data;
using System.Windows.Controls.Primitives;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Com.Huen.Libs;
using Com.Huen.Sql;
using Com.Huen.DataModel;
using Com.Huen.Sockets;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CorePMS : MetroWindow
    {
        private RelayService relayservice = null;

        public CorePMS()
        {
            InitializeComponent();

            this.ReadIni();
            this.Loaded += CorePMS_Loaded;
            this.Closed += CorePMS_Closed;
        }

        private void CorePMS_Loaded(object sender, RoutedEventArgs e)
        {
            relayservice = new RelayService("14.63.171.190", RunningType.BOTH);
            relayservice.Device2CorePmsEvent += Relayservice_Device2CorePmsEvent;

            this.InitializeData();
        }

        private void Relayservice_Device2CorePmsEvent(object sender, _pms_data_type pmsdata)
        {
            RoomItem roomitem = null;

            foreach (var floor in floors)
            {
                roomitem = floor.list.FirstOrDefault(x => x.RoomNum.Equals(pmsdata.extension));
                if (roomitem == null)
                    continue;
                else
                    break;
            }

            if (roomitem == null)
            {
                pmsdata.status = pmsdata.status = STRUCTS.PMS_STATUS_FAIL;
                relayservice.SendReplay(pmsdata);
                return;
            }

            try
            {
                switch (pmsdata.cmd)
                {
                    case STRUCTS.PMS_SET_MORNING_CALL_REQ:
                        foreach (var floor in floors)
                        {
                            foreach (var room in floor.list)
                            {
                                if (room.RoomNum.Equals(pmsdata.extension))
                                {
                                    room.Hour = pmsdata.hour;
                                    room.Minutes = pmsdata.minutes;
                                    break;
                                }
                            }
                        }
                        break;
                    case STRUCTS.PMS_SET_LANGUAGE_REQ:
                        foreach (var floor in floors)
                        {
                            foreach (var room in floor.list)
                            {
                                if (room.RoomNum.Equals(pmsdata.extension))
                                {
                                    room.Languages = pmsdata.language.ToString();
                                    break;
                                }
                            }
                        }
                        break;
                    case STRUCTS.PMS_REPORT_FUNCTION_KEY_REQ:
                        if (pmsdata.function_key.Equals(fk_cleanroom))
                        {
                            switch (pmsdata.function_key_cmd)
                            {
                                case 1:
                                    // 청소 신청
                                    roomitem.States_Clean = "1";
                                    break;
                                case 2:
                                default:
                                    // 청소 신청취소
                                    roomitem.States_Clean = "0";
                                    break;
                            }
                        }
                        else if (pmsdata.function_key.Equals(fk_dnd))
                        {
                            switch (pmsdata.function_key_cmd)
                            {
                                case 1:
                                    // DND 신청
                                    roomitem.States_DnD = "1";
                                    break;
                                case 2:
                                default:
                                    // DND 해제
                                    roomitem.States_DnD = "0";
                                    break;
                            }
                        }
                        else if (pmsdata.function_key.Equals(fk_laundary))
                        {
                            switch (pmsdata.function_key_cmd)
                            {
                                case 1:
                                    // 세탁 신청
                                    roomitem.States_Laundary = "1";
                                    break;
                                case 2:
                                default:
                                    // 세탁 신청취소
                                    roomitem.States_Laundary = "0";
                                    break;
                            }
                        }
                        else if (pmsdata.function_key.Equals(fk_roomservice))
                        {
                            switch (pmsdata.function_key_cmd)
                            {
                                case 1:
                                    // 룸서비스 신청
                                    break;
                                case 2:
                                default:
                                    // 룸서비스 신청취소
                                    break;
                            }
                        }
                        //else if (pmsdata.function_key.Equals(fk_cleaningroom_complete))
                        //{
                        //    switch (pmsdata.function_key_cmd)
                        //    {
                        //        case 1:
                        //        case 2:
                        //        default:
                        //            // 청소 완료
                        //            break;
                        //    }
                        //}
                        //else if (pmsdata.function_key.Equals(fk_cleaningroom_inspection))
                        //{
                        //    switch (pmsdata.function_key_cmd)
                        //    {
                        //        case 1:
                        //        case 2:
                        //        default:
                        //            // 청소 확인
                        //            break;
                        //    }
                        //}
                        //else if (pmsdata.function_key.Equals(fk_emergency))
                        //{
                        //    switch (pmsdata.function_key_cmd)
                        //    {
                        //        case 1:
                        //            // 응급 신청
                        //            break;
                        //        case 2:
                        //        default:
                        //            // 응급 해제
                        //            break;
                        //    }
                        //}
                        break;
                    case STRUCTS.PMS_REPORT_MAKEUP_STATUS_REQ:
                        switch (pmsdata.makeup_room_status)
                        {
                            case 1:
                                // 청소 완료
                                roomitem.States_Clean = "2";
                                break;
                            case 2:
                                // 청소 완료 취소
                                roomitem.States_Clean = "1";
                                break;
                            case 3:
                                // 청소 점검 완료
                                roomitem.States_Clean = "3";
                                break;
                            case 4:
                                // 청소 점검 완료 취소
                                roomitem.States_Clean = "1";
                                break;
                        }
                        break;
                    default:
                        // Debug.WriteLine("Pmsserver_ReqPMSSetEvent: " + pmsdata.function_key + " // " + pmsdata.function_key_cmd);
                        break;
                }

                relayservice.SendReplay(pmsdata);
            }
            catch (Exception e)
            {
                pmsdata.status = pmsdata.status = STRUCTS.PMS_STATUS_FAIL;
                relayservice.SendReplay(pmsdata);
            }
        }

        private void CorePMS_Closed(object sender, EventArgs e)
        {
            this.SavePosition();
            Environment.Exit(0);
        }

        private void SavePosition()
        {
            Ini ini = new Ini(@".\pms.ini");
            ini.IniWriteValue("POSITION", "WIDTH", this.Width.ToString());
            ini.IniWriteValue("POSITION", "HEIGHT", this.Height.ToString());
            ini.IniWriteValue("POSITION", "LEFT", this.Left.ToString());
            ini.IniWriteValue("POSITION", "TOP", this.Top.ToString());

            ini.IniWriteValue("SERVER", "PBXIP", util.PBXIP);
            ini.IniWriteValue("DB", "IP", util.DBIP);
        }

        private Floors floors = null;
        private void InitializeData()
        {
            floors = new Floors();
            tabFloor.ItemsSource = floors;

            Languages _languages = new Languages();
            cmb_language.ItemsSource = _languages;
        }

        private string sitecode;
        private string fk_cleanroom;
        private string fk_dnd;
        private string fk_laundary;
        private string fk_roomservice;
        private string fk_cleaningroom_complete;
        private string fk_cleaningroom_inspection;
        private string fk_emergency;

        private void ReadIni()
        {
            Ini ini = new Ini(".\\pms.ini");
            this.Width = string.IsNullOrEmpty(ini.IniReadValue("POSITION", "WIDTH")) ? 800 : double.Parse(ini.IniReadValue("POSITION", "WIDTH"));
            this.Height = string.IsNullOrEmpty(ini.IniReadValue("POSITION", "HEIGHT")) ? 600 : double.Parse(ini.IniReadValue("POSITION", "HEIGHT"));
            this.Left = string.IsNullOrEmpty(ini.IniReadValue("POSITION", "LEFT")) ? 50 : double.Parse(ini.IniReadValue("POSITION", "LEFT"));
            this.Top = string.IsNullOrEmpty(ini.IniReadValue("POSITION", "TOP")) ? 50 : double.Parse(ini.IniReadValue("POSITION", "TOP"));

            util.PBXIP = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "PBXIP")) ? "127.0.0.1" : ini.IniReadValue("SERVER", "PBXIP");
            util.DBIP = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "DBIP")) ? "127.0.0.1" : ini.IniReadValue("SERVER", "DBIP");


            sitecode = ini.IniReadValue("SITE", "code");

            fk_cleanroom = ini.IniReadValue("FUNCSKEYS", "fk_cleanroom");
            fk_dnd = ini.IniReadValue("FUNCSKEYS", "fk_dnd");
            fk_laundary = ini.IniReadValue("FUNCSKEYS", "fk_laundary");
            fk_roomservice = ini.IniReadValue("FUNCSKEYS", "fk_roomservice");
            fk_cleaningroom_complete = ini.IniReadValue("FUNCSKEYS", "fk_cleaningroom_complete");
            fk_cleaningroom_inspection = ini.IniReadValue("FUNCSKEYS", "fk_cleaningroom_inspection");
            fk_emergency = ini.IniReadValue("FUNCSKEYS", "fk_emergency");
        }

        private void listrooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox _listbox = (ListBox)sender;
            if (_listbox.SelectedItems.Count > 1 && e.AddedItems.Count > 0)
            {
                if ((_listbox.SelectedItems[0] as RoomItem).States != (e.AddedItems[0] as RoomItem).States)
                {
                    ShowAlertDialog("같은 상태의 방만 선택할 수 있습니다.", (int)AlertDelaySec.Normal);
                    _listbox.SelectedItems.Remove(e.AddedItems[0]);
                    return;
                }
            }

            RoomItem addedItem = null;
            RoomItem removedItem = null;

            if (e.AddedItems.Count > 0)
                addedItem = (RoomItem)e.AddedItems[0];

            if (e.RemovedItems.Count > 0)
                removedItem = (RoomItem)e.RemovedItems[0];

            
            if (addedItem != null)
            {
                addedItem.IsChecked = true;
            }

            if (removedItem != null)
            {
                removedItem.IsChecked = false;
            }
        }

        private void listrooms_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void listrooms_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var lb = (ListBox)sender;
            if (lb.SelectedItems.Count < 1)
            {
                e.Handled = true;
                return;
            }

            ContextMenu cm = lb.ContextMenu;
            for (int i = 0; i < cm.Items.Count; i++)
            {
                if (cm.Items[i].GetType() == typeof(System.Windows.Controls.Separator))
                    continue;

                MenuItem mitem = (MenuItem)cm.Items[i];
                mitem.IsEnabled = true;
            }

            RoomItem sitem = (RoomItem)lb.SelectedItems[0];
            if (sitem.States.Equals("0"))
            {
                for (int i = 0; i < cm.Items.Count; i++)
                {
                    if (cm.Items[i].GetType() == typeof(System.Windows.Controls.Separator))
                        continue;

                    MenuItem mi = (MenuItem)cm.Items[i];
                    if (i > 0)
                    {
                        if (mi.Header.ToString().Contains("빌링"))
                        {
                            mi.IsEnabled = true;
                        }
                        else
                        {
                            mi.IsEnabled = false;
                        }
                    }
                }
            }
            else if (sitem.States.Equals("1"))
            {
                for (int i = 0; i < cm.Items.Count; i++)
                {
                    if (cm.Items[i].GetType() == typeof(System.Windows.Controls.Separator))
                        continue;

                    MenuItem mi = (MenuItem)cm.Items[i];
                    if (i == 0)
                    {
                        mi.IsEnabled = false;
                    }
                }
            }
        }

        private PMSBEH NOWACT = PMSBEH.NONE;
        private ListBox selectedListBox = null;
        // 체크인
        private void contextmenu0_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            ListBox lb = checkinFlyout.FindChild<ListBox>("checkinFlyout_lb");
            lb.ItemsSource = selectedListBox.SelectedItems;

            this.ToggleFlyout(0);

            NOWACT = PMSBEH.DO_CHECKIN;
        }

        // 수정
        private void contextmenu1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            ListBox lb = checkinFlyout.FindChild<ListBox>("checkinFlyout_lb");
            lb.ItemsSource = selectedListBox.SelectedItems;

            this.ToggleFlyout(0);

            NOWACT = PMSBEH.DO_MODIFY;
        }

        // 방변경
        private void contextmenu2_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // 빌링
        private void contextmenu3_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            string tmpext = string.Empty;
            foreach (RoomItem item in selectedListBox.SelectedItems)
            {
                if (string.IsNullOrEmpty(tmpext))
                    tmpext = item.RoomNum;
                else
                    tmpext += "," + item.RoomNum;
            }

            BILL bill = new BILL();
            bill._extnum = tmpext;
            bill.Owner = this;
            bill.Show();
        }

        // 체크아웃
        private void contextmenu4_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            List<RoomItem> _seccessRoom = new List<RoomItem>();
            string _failRoom = string.Empty;

            foreach (RoomItem item in selectedListBox.SelectedItems)
            {
                var pmsdata = item.PMSDATA;

                bool _result = false;
                using (HotelHelper hh = new HotelHelper(util.PBXIP))
                {
                    _result = hh.CheckOut(pmsdata.extension);
                }

                if (_result)
                {
                    DataTable dt = util.CreateDT2SP();
                    dt.Rows.Add("@I_ROOM", item.RoomNum);
                    dt.Rows.Add("@I_STATUS", "0");

                    using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            db.ExcuteSP("UDT_ROOM_STATUS", dt);
                            db.Commit();

                            item.PMSDATA = pmsdata;
                            item.IsChecked = false;
                            item.Languages = "0";
                            item.Hour = -1;
                            item.Minutes = -1;
                            item.States = "0";

                            _seccessRoom.Add(item);

                            util.Log2DB(item.RoomNum, string.Format("CHECK OUT : {0}", item.RoomNum), "0");
                        }
                        catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                        {
                            db.Rollback();
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("update hroom set states_clean=0, states_laundary=0, states_parcel=0 where room={0}", item.RoomNum);
                    using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            int c = db.GetEffectedCount();
                            db.Commit();
                        }
                        catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                        {
                            db.Rollback();
                        }
                    }
                }
                else
                {
                    item.IsChecked = false;
                    if (string.IsNullOrEmpty(_failRoom))
                        _failRoom = item.RoomNum;
                    else
                        _failRoom += string.Format(",{0}", item.RoomNum);
                }
            }

            var lbs = this.FindChild<ListBox>("listrooms");
            foreach (var itm in _seccessRoom)
            {
                lbs.SelectedItems.Remove(itm);
            }

            string alertmsg = string.Empty;
            int alertsec = 0;
            if (string.IsNullOrEmpty(_failRoom))
            {
                alertmsg = "체크아웃 설정이 완료 되었습니다.";
                alertsec = (int)AlertDelaySec.Success;
            }
            else
            {
                alertmsg = string.Format("체크아웃 실패\r\n방번호 : {0}", _failRoom);
                alertsec = (int)AlertDelaySec.Fail;
            }

            ShowAlertDialog(alertmsg, alertsec);
        }

        private void contextmenu5_sub0_Click(object sender, RoutedEventArgs e)
        {
            UpdateParcelStates(e, ParcelStates.GETREQUESTED);
        }

        private void contextmenu5_sub1_Click(object sender, RoutedEventArgs e)
        {
            // 우편물 수령 완료 (PMS > PBX > 객실전화)
            UpdateParcelStates(e, ParcelStates.NONE);
        }

        private void tabFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            switch (index)
            {
                case 0:
                    // checkin / modify
                    flyout.Width = 580;
                    break;
                case 1:
                    // options flyout
                    flyout.Width = 500;
                    flyout.Theme = FlyoutTheme.Dark;
                    break;
                case 2:
                    // logs
                    flyout.Width = 300;
                    break;
                case 3:
                    // absence call
                    flyout.Width = 300;
                    break;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void checkinFlyout_ClosingFinished(object sender, RoutedEventArgs e)
        {
            
        }

        private void switch1_IsCheckedChanged(object sender, EventArgs e)
        {
            ToggleSwitch tswitch = null;
            if (sender == null)
                return;
            else
                tswitch = (ToggleSwitch)sender;


            var switchoptions = swtoptions.FindChildren<ToggleSwitch>().ToList();
            int switchIdx = switchoptions.IndexOf(tswitch);

            int ischeckedcount = switchoptions.Where(x => x.IsChecked == true).Count();
            int count = switchoptions.Count();

            RoomItem selitem = (RoomItem)checkinFlyout_lb.SelectedItem;
            var pmsdata = selitem.PMSDATA;

            string _allowedPrefix = string.Empty;
            string _forbiddenPrefix = string.Empty;

            for (int i = 0; i < switchoptions.Count; i++)
            {
                string _token = string.Empty;

                switch (i)
                {
                    case 0:
                        _token = "00";
                        break;
                    case 1:
                        _token = "01";
                        break;
                    case 2:
                        _token = "02,03,04,05,06";
                        break;
                }

                if (switchoptions[i].IsChecked == true ? true : false)
                {
                    if (string.IsNullOrEmpty(_allowedPrefix))
                    {
                        _allowedPrefix = _token;
                    }
                    else
                    {
                        _allowedPrefix += string.Format(",{0}", _token);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_forbiddenPrefix))
                    {
                        _forbiddenPrefix = _token;
                    }
                    else
                    {
                        _forbiddenPrefix += string.Format(",{0}", _token);
                    }
                }
            }

            //if (ischeckedcount == 0)
            //{
            //    _forbiddenPrefix = "all";
            //}

            pmsdata.allowedPrefix = _allowedPrefix;
            pmsdata.forbiddenPrefix = _forbiddenPrefix;
            
            selitem.PMSDATA = pmsdata;
        }

        private void checkinFlyout_lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
                return;

            var addeditem = (RoomItem)e.AddedItems[0];

            _cgi_pms_data_type pmsdata = addeditem.PMSDATA;

            if (addeditem.States == "0")
            {
                pmsdata.allowedPrefix = "00,01,02,03,04,05,06";
                pmsdata.forbiddenPrefix = string.Empty;
                pmsdata.hour = -1;
                pmsdata.minutes = -1;
                pmsdata.language = 2;
                pmsdata.repeat_times = 3;
                pmsdata.ring_duration = 120;
                pmsdata.repeat_week = 0;
                pmsdata.week = "0000000";

                // 모닝콜 반복 초기값 설정
                char[] arrmcall = pmsdata.week.ToCharArray();
                var btnsweek = repeat_week.FindChildren<ToggleButton>().ToList();
                for (int j = 0; j < arrmcall.Length; j++)
                {
                    btnsweek[j].IsChecked = false;
                }

                // 모닝콜 시간, 반복주 선택, 언어 초기화
                mcall_hour.Value = pmsdata.hour;
                mcall_minutes.Value = pmsdata.minutes;
                repeat_week.Visibility = Visibility.Collapsed;
                cmb_language.SelectedValue = "2";

                addeditem.PMSDATA = pmsdata;
                return;
            }

            // 옵션 초기값 설정
            string _international = "00";
            string _cellphone = "01";
            string _internal = "02,03,04,05,06";

            var switchoptions = swtoptions.FindChildren<ToggleSwitch>().ToList();
            if (pmsdata.allowedPrefix.Contains(_cellphone))
            {
                switchoptions[1].IsChecked = true;
            }
            if (pmsdata.allowedPrefix.Contains(_internal))
            {
                switchoptions[2].IsChecked = true;
            }
            if (pmsdata.allowedPrefix.Contains(_international))
            {
                switchoptions[0].IsChecked = true;
            }
            if (pmsdata.allowedPrefix.Contains("all"))
            {
                switchoptions[0].IsChecked = true;
                switchoptions[1].IsChecked = true;
                switchoptions[2].IsChecked = true;
            }

            if (pmsdata.forbiddenPrefix.Contains(_cellphone))
            {
                switchoptions[1].IsChecked = false;
            }
            if (pmsdata.forbiddenPrefix.Contains(_internal))
            {
                switchoptions[2].IsChecked = false;
            }
            if (pmsdata.forbiddenPrefix.Contains(_international))
            {
                switchoptions[0].IsChecked = false;
            }
            if (pmsdata.forbiddenPrefix.Contains("all"))
            {
                switchoptions[0].IsChecked = false;
                switchoptions[1].IsChecked = false;
                switchoptions[2].IsChecked = false;
            }

            if (string.IsNullOrEmpty(pmsdata.allowedPrefix) && string.IsNullOrEmpty(pmsdata.forbiddenPrefix))
            {
                switchoptions[0].IsChecked = true;
                switchoptions[1].IsChecked = true;
                switchoptions[2].IsChecked = true;
            }

            // 모닝콜 반복 초기값 설정
            if (pmsdata.repeat_week == 1)
            {
                char[] arrmcall = pmsdata.week.ToCharArray();
                if (arrmcall.Length > 0)
                {
                    var btnsweek = repeat_week.FindChildren<ToggleButton>().ToList();
                    for (int j = 0; j < arrmcall.Length; j++)
                    {
                        if (arrmcall[j].Equals('1'))
                        {
                            btnsweek[j].IsChecked = true;
                        }
                        else
                        {
                            btnsweek[j].IsChecked = false;
                        }
                    }
                }
                mcallRepeat.IsChecked = true;
                repeat_week.Visibility = Visibility.Visible;
            }
            else
            {
                mcallRepeat.IsChecked = false;
                repeat_week.Visibility = Visibility.Collapsed;
            }

            // 모닝콜 시간 초기화
            mcall_hour.Value = pmsdata.hour;
            mcall_minutes.Value = pmsdata.minutes;

            addeditem.PMSDATA = pmsdata;
            if (pmsdata.language == 0)
            {
                cmb_language.SelectedIndex = 0;
            }
            else
            {
                cmb_language.SelectedValue = pmsdata.language;
            }
        }

        private void btn_checkin_save_Click(object sender, RoutedEventArgs e)
        {
            List<RoomItem> _seccessRoom = new List<RoomItem>();
            string _failRoom = string.Empty;

            foreach (RoomItem item in checkinFlyout_lb.ItemsSource)
            {
                var pmsdata = item.PMSDATA;
                pmsdata.cmd = STRUCTS.CGI_PMS_SET_ALL_REQ;
                pmsdata.repeat_times = 5;
                pmsdata.ring_duration = 120;
                pmsdata.try_interval = 3;

                bool _result = false;
                using (HotelHelper hh = new HotelHelper(util.PBXIP))
                {
                    _result = hh.CheckIn(pmsdata);
                }

                if (_result)
                {
                    if (NOWACT == PMSBEH.DO_MODIFY)
                    {
                        item.PMSDATA = pmsdata;
                        item.IsChecked = false;
                        item.Languages = pmsdata.language.ToString();
                        item.Hour = pmsdata.hour;
                        item.Minutes = pmsdata.minutes;
                        item.States = "1";

                        util.Log2DB(item.RoomNum, string.Format("MODIFY : {0}", item.RoomNum), "2");
                        continue;
                    }

                    DataTable dt = util.CreateDT2SP();
                    dt.Rows.Add("@I_ROOM", item.RoomNum);
                    dt.Rows.Add("@I_STATUS", "1");

                    using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            db.ExcuteSP("UDT_ROOM_STATUS", dt);
                            db.Commit();

                            item.PMSDATA = pmsdata;
                            item.IsChecked = false;
                            item.Languages = pmsdata.language.ToString();
                            item.Hour = pmsdata.hour;
                            item.Minutes = pmsdata.minutes;
                            item.States = "1";

                            item.States_Clean = "0";
                            item.States_DnD = "0";
                            item.States_Laundary = "0";
                            item.States_Parcel = "0";

                            _seccessRoom.Add(item);

                            util.Log2DB(item.RoomNum, string.Format("CHECK IN : {0}", item.RoomNum), "1");
                        }
                        catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                        {
                            db.Rollback();
                        }
                    }
                }
                else
                {
                    item.IsChecked = false;
                    if (string.IsNullOrEmpty(_failRoom))
                        _failRoom = item.RoomNum;
                    else
                        _failRoom += string.Format(",{0}", item.RoomNum);
                }
            }

            var lbs = this.FindChild<ListBox>("listrooms");
            foreach (var itm in _seccessRoom)
            {
                lbs.SelectedItems.Remove(itm);
            }

            ToggleFlyout(0);

            string alertmsg = string.Empty;
            int alertsec = 0;
            if (string.IsNullOrEmpty(_failRoom))
            {
                if (NOWACT == PMSBEH.DO_CHECKIN)
                {
                    alertmsg = "체크인 설정이 완료 되었습니다.";
                }
                else
                {
                    alertmsg = "설정 수정이 완료 되었습니다.";
                }
                
                alertsec = (int)AlertDelaySec.Success;
            }
            else
            {
                if (NOWACT == PMSBEH.DO_CHECKIN)
                {
                    alertmsg = string.Format("체크인 실패\r\n방번호 : {0}", _failRoom);
                }
                else
                {
                    alertmsg = string.Format("실패\r\n방번호 : {0}", _failRoom);
                }
                
                alertsec = (int)AlertDelaySec.Success;
            }

            ShowAlertDialog(alertmsg, alertsec);

            if (NOWACT == PMSBEH.DO_CHECKIN)
            {
                NOWACT = PMSBEH.NONE;
            }
            else if (NOWACT == PMSBEH.DO_MODIFY)
            {
                NOWACT = PMSBEH.NONE;
            }
        }

        private void cmb_language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checkinFlyout_lb.SelectedItems.Count < 1)
                return;

            var addeditem = (Language)e.AddedItems[0];

            var selecteditem = (RoomItem)checkinFlyout_lb.SelectedItem;
            var pmsdata = selecteditem.PMSDATA;
            _cgi_pms_data_type data = selecteditem.PMSDATA;
            pmsdata.language = addeditem.LangCode;
            selecteditem.PMSDATA = pmsdata;
        }

        private void checkinFlyout_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (Flyout)sender;
            
            var listbox = obj.FindChild<ListBox>("checkinFlyout_lb");

            if (obj.IsOpen)
            {
                if (tabFloor.IsEnabled)
                    tabFloor.IsEnabled = false;

                if (listbox.Items.Count > 0)
                {
                    if (listbox.SelectedItems.Count < 1)
                        listbox.SelectedIndex = 0;
                }
            }
            else
            {
                int count = 0;
                foreach (Flyout item in this.Flyouts.Items)
                {
                    if (item.IsVisible)
                        count++;
                }

                if (count < 1)
                    tabFloor.IsEnabled = true;
            }
        }

        private void mcallRepeat_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = (RoomItem)checkinFlyout_lb.SelectedItem;
            _cgi_pms_data_type pmsdata = selecteditem.PMSDATA;

            CheckBox cb = (CheckBox)sender;
            bool ischecked = cb.IsChecked == true ? true : false;
            if (ischecked)
            {
                if (mcall_hour.Value == null || mcall_minutes.Value == null)
                {
                    ShowAlertDialog("모닝콜 시간이 설정되지 않았습니다.\r\n시:분을 설정 후 반복 요일을 선택하세요.", (int)AlertDelaySec.Normal);
                    cb.IsChecked = false;
                    return;
                }

                repeat_week.Visibility = System.Windows.Visibility.Visible;
                pmsdata.repeat_week = 1;

                var arrmcall = pmsdata.week.ToCharArray();
                var btnsweek = repeat_week.FindChildren<ToggleButton>().ToList();
                for (int j = 0; j < arrmcall.Length; j++)
                {
                    if (arrmcall[j].Equals('1'))
                    {
                        btnsweek[j].IsChecked = true;
                    }
                    else
                    {
                        btnsweek[j].IsChecked = false;
                    }
                }
            }
            else
            {
                repeat_week.Visibility = System.Windows.Visibility.Collapsed;
                pmsdata.repeat_week = 0;
                pmsdata.week = "0000000";

                var btnsweek = repeat_week.FindChildren<ToggleButton>().ToList();
                for (int j = 0; j < btnsweek.Count; j++)
                {
                    btnsweek[j].IsChecked = false;
                }
            }

            selecteditem.PMSDATA = pmsdata;
        }

        private void toggleWeek0_Click(object sender, RoutedEventArgs e)
        {
            var btnsweek = repeat_week.FindChildren<ToggleButton>().ToList();
            ToggleButton togglebtn = (ToggleButton)sender;
            int idx = btnsweek.IndexOf(togglebtn);
            char[] chrweek = { '0','0','0','0','0','0','0' };
            var selecteditem = (RoomItem)checkinFlyout_lb.SelectedItem;
            _cgi_pms_data_type pmsdata = selecteditem.PMSDATA;
            if (!string.IsNullOrEmpty(pmsdata.week))
            {
                chrweek = pmsdata.week.ToCharArray();
            }

            chrweek[idx] = '1';
            string strweek = new string(chrweek);

            pmsdata.week = strweek;
            selecteditem.PMSDATA = pmsdata;
        }

        private void mcall_hour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            var obj = (NumericUpDown)sender;

            var selecteditem = (RoomItem)checkinFlyout_lb.SelectedItem;
            _cgi_pms_data_type pmsdata = selecteditem.PMSDATA;
            if (e.NewValue == null)
            {
                pmsdata.hour = -1;
                //selecteditem.Hour = -1;
            }
            else
            {
                if ((int)e.NewValue == -1)
                {
                    obj.Value = null;
                }

                pmsdata.hour = (int)e.NewValue;
                //selecteditem.Hour = (int)e.NewValue;
            }

            selecteditem.PMSDATA = pmsdata;
        }

        private void mcall_minutes_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            var obj = (NumericUpDown)sender;

            var selecteditem = (RoomItem)checkinFlyout_lb.SelectedItem;
            _cgi_pms_data_type pmsdata = selecteditem.PMSDATA;
            if (e.NewValue == null)
            {
                pmsdata.minutes = -1;
                //selecteditem.Minutes = -1;
            }
            else
            {
                if ((int)e.NewValue == -1)
                {
                    obj.Value = null;
                }

                pmsdata.minutes = (int)e.NewValue;
                //selecteditem.Minutes = (int)e.NewValue;
            }

            selecteditem.PMSDATA = pmsdata;
        }

        private async void ShowAlertDialog(string msg, int delaytime)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomDialog"];
            var children = dialog.FindChildren<TextBlock>();
            var txtobj = children.First();
            txtobj.Text = msg;
            await this.ShowMetroDialogAsync(dialog);
            await TaskEx.Delay(delaytime);
            await this.HideMetroDialogAsync(dialog);
        }

        private async void ShowLoginDialog()
        {
            //this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = string.Empty });
            if (result == null)
            {
                //User pressed cancel
                this.Close();
            }
            else
            {
                //MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication Information", String.Format("Username: {0}\nPassword: {1}", result.Username, result.Password));
                DataTable dt = null;
                StringBuilder query = new StringBuilder();
                query.Append(" select idx, identity, pwd, firm_idx, role_idx from users ");
                query.AppendFormat(" where identity='{0}' and pwd='{1}'", result.Username.Trim(), util.GetSHA1(result.Password));
                using (FirebirdDBHelper db = new FirebirdDBHelper(query.ToString(), util.strDBConn))
                {
                    try
                    {
                        dt = db.GetDataTable();
                    }
                    catch
                    {
                        dt = null;
                    }
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    // do when login failed
                    MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication", "Logon failed.\r\nPlease, identify your information one more time.");
                    ShowLoginDialog();
                }
                else
                {
                    // succeed
                }
            }
        }

        private void tmenu_options_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(1);
        }

        private void flyoutOptions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (Flyout)sender;

            if (obj.IsOpen)
            {
                if (tabFloor.IsEnabled)
                    tabFloor.IsEnabled = false;

                //list_floors = new FloorRs();
                //list_floors.CollectionChanged += list_floors_CollectionChanged;
                //options_floors.ItemsSource = list_floors;

                pbxip.Text = util.PBXIP;
                dbip.Text = util.DBIP;
            }
            else
            {
                int count = 0;
                foreach (Flyout item in this.Flyouts.Items)
                {
                    if (item.IsVisible)
                        count++;
                }

                if (count < 1)
                    tabFloor.IsEnabled = true;

                //options_floors.ItemsSource = null;
                //options_rooms.ItemsSource = null;
                //options_tels.ItemsSource = null;

                util.PBXIP = pbxip.Text.Trim();
                util.DBIP = dbip.Text.Trim();

                Ini ini = new Ini(@".\pms.ini");
                ini.IniWriteValue("SERVER", "PBXIP", util.PBXIP);
                ini.IniWriteValue("SERVER", "DBIP", util.DBIP);
            }
        }

        void list_floors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {

            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {

            }
        }

        private void flyoutViewlog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (Flyout)sender;

            if (obj.IsOpen)
            {
                if (tabFloor.IsEnabled)
                    tabFloor.IsEnabled = false;


                flyoutViewlog_grid.ItemsSource = new ActLogs();
            }
            else
            {
                int count = 0;
                foreach (Flyout item in this.Flyouts.Items)
                {
                    if (item.IsVisible)
                        count++;
                }

                if (count < 1)
                    tabFloor.IsEnabled = true;
            }
        }

        private void tmenu_log_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(2);
        }

        private void tmenu_absence_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(3);
        }

        private void contextmenu6_sub0_Click(object sender, RoutedEventArgs e)
        {
            // 프런트 > 청소요청
            UpdateCleanStates(e, "1");
        }

        private void contextmenu6_sub1_Click(object sender, RoutedEventArgs e)
        {
            // 프런트 > 청소완료
            UpdateCleanStates(e, "2");
        }

        private void contextmenu6_sub2_Click(object sender, RoutedEventArgs e)
        {
            // 프런트 > 청소컨펌
            UpdateCleanStates(e, "3");
        }

        private void contextmenu7_sub0_Click(object sender, RoutedEventArgs e)
        {
            // 프런트 > 세탁 요청
            UpdateLaundaryStates(e, "1");
        }

        private void contextmenu7_sub1_Click(object sender, RoutedEventArgs e)
        {
            // 프런트 > 세탁 완료
            UpdateLaundaryStates(e, "0");
        }

        private void UpdateCleanStates(RoutedEventArgs e, string states)
        {
            // 프런트 > 청소요청
            // states 1:요청, 2:완료, 3:컨펌

            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)((MenuItem)menuItem.Parent).Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            List<RoomItem> _successRoom = new List<RoomItem>();
            string _failRoom = string.Empty;

            foreach (RoomItem item in selectedListBox.SelectedItems)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("update hroom set");
                sb.AppendFormat(" states_clean={0}", states);
                sb.AppendFormat(" where room='{0}'", item.RoomNum);

                using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
                {
                    try
                    {
                        db.BeginTran();
                        int count = db.GetEffectedCount();
                        db.Commit();

                        item.States_Clean = states;

                        string _msg = string.Empty;

                        if (states == "1")
                        {
                            _msg = "REQUESTED";
                        }
                        else if (states == "2")
                        {
                            _msg = "COMPLETED";
                        }
                        else if (states == "3")
                        {
                            _msg = "CONFIRMED";
                        }

                        util.Log2DB(item.RoomNum, string.Format("CLEANING {0} : {1}", _msg, item.RoomNum), "1");
                    }
                    catch
                    {
                        db.Rollback();
                    }
                }
            }
        }

        private void UpdateLaundaryStates(RoutedEventArgs e, string states)
        {
            // 프런트 > 세탁
            // states 1:요청, 0:완료

            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)((MenuItem)menuItem.Parent).Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            List<RoomItem> _successRoom = new List<RoomItem>();
            string _failRoom = string.Empty;

            foreach (RoomItem item in selectedListBox.SelectedItems)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("update hroom set");
                sb.AppendFormat(" states_laundary={0}", states);
                sb.AppendFormat(" where room='{0}'", item.RoomNum);

                using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
                {
                    try
                    {
                        db.BeginTran();
                        int count = db.GetEffectedCount();
                        db.Commit();

                        item.States_Laundary = states;

                        string _msg = string.Empty;

                        if (states == "1")
                        {
                            _msg = "REQUESTED";
                        }
                        else if (states == "0")
                        {
                            _msg = "COMPLETED";
                        }

                        util.Log2DB(item.RoomNum, string.Format("LAUNDARY {0} : {1}", _msg, item.RoomNum), "1");
                    }
                    catch
                    {
                        db.Rollback();
                    }
                }
            }
        }

        private void UpdateParcelStates(RoutedEventArgs e, ParcelStates states)
        {
            // 프런트 > 우편물
            // states 1:요청, 0:완료

            // 우편물 수령 요청 (PMS > PBX > 객실전화)
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)((MenuItem)menuItem.Parent).Parent;
            selectedListBox = (ListBox)contextMenu.PlacementTarget;

            List<RoomItem> _successRoom = new List<RoomItem>();
            string _failRoom = string.Empty;

            foreach (RoomItem item in selectedListBox.SelectedItems)
            {
                bool result = false;
                using (HotelHelper hh = new HotelHelper(util.PBXIP))
                {
                    result = hh.SetParcel(item.RoomNum, (int)states);
                }

                if (result)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("update hroom set");
                    sb.AppendFormat(" states_parcel={0}", (int)states);
                    sb.AppendFormat(" where room='{0}'", item.RoomNum);

                    using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            int count = db.GetEffectedCount();
                            db.Commit();

                            item.States_Parcel = ((int)states).ToString();
                            _successRoom.Add(item);
                        }
                        catch
                        {
                            db.Rollback();
                        }
                    }

                    string _msg = string.Empty;
                    if (states == ParcelStates.GETREQUESTED)
                    {
                        _msg = "REQUESTED";
                    }
                    else if (states == ParcelStates.NONE)
                    {
                        _msg = "DELIVERED";
                    }

                    util.Log2DB(item.RoomNum, string.Format("PARCEL {0} : {1}", _msg, item.RoomNum), "1");
                }
                else
                {
                    if (string.IsNullOrEmpty(_failRoom))
                        _failRoom = item.RoomNum;
                    else
                        _failRoom += string.Format(",{0}", item.RoomNum);
                }
            }
        }

        private void flyoutAbsence_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (Flyout)sender;

            if (obj.IsOpen)
            {
                if (tabFloor.IsEnabled)
                    tabFloor.IsEnabled = false;


                flyoutAbsence_grid.ItemsSource = new Absences();
            }
            else
            {
                int count = 0;
                foreach (Flyout item in this.Flyouts.Items)
                {
                    if (item.IsVisible)
                        count++;
                }

                if (count < 1)
                    tabFloor.IsEnabled = true;

                flyoutAbsence_grid.ItemsSource = null;
            }
        }
    }
}
