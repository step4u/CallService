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

using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.DataModel;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for AddPrefix.xaml
    /// </summary>
    public partial class AddPrefixInternational : Window
    {
        private AddPrefixStates _mode = AddPrefixStates.Add;

        public INTERNATIONALRATE _obj = null;
        private CdrAgent _parentWin = null;

        public AddPrefixInternational()
        {
            InitializeComponent();

            this.Loaded += AddPrefix_Loaded;
        }

        public AddPrefixInternational(AddPrefixStates mode)
        {
            InitializeComponent();

            _mode = mode;

            this.Loaded += AddPrefix_Loaded;
        }

        void AddPrefix_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            _parentWin = (CdrAgent)this.Owner;

            if (_mode == AddPrefixStates.Modify)
            {
                txtKind.Text = _obj.areacode.ToString();
                txtLRate.Text = _obj.lrate.ToString();
                txtLSec.Text = _obj.lsec.ToString();
                txtMRate.Text = _obj.mrate.ToString();
                txtMSec.Text = _obj.msec.ToString();
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // 값 filter
            if (string.IsNullOrEmpty(txtKind.Text.Trim()))
            {
                MessageBox.Show("대역 번호를 입력하세요.");
                txtKind.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtLRate.Text.Trim()))
            {
                MessageBox.Show("유선 착신 요율(원)을 입력하세요.");
                txtLRate.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtLSec.Text.Trim()))
            {
                MessageBox.Show("유선 착신 요율(초)을 입력하세요.");
                txtLSec.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtMRate.Text.Trim()))
            {
                MessageBox.Show("무선 착신 요율(원)을 입력하세요.");
                txtMRate.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtMSec.Text.Trim()))
            {
                MessageBox.Show("무선 착신 요율(초)을 입력하세요.");
                txtMSec.Focus();
                return;
            }

            if (_mode == AddPrefixStates.Add)
            {

                _obj = new INTERNATIONALRATE()
                {
                    areacode = int.Parse(txtKind.Text.Trim())
                    ,
                    lrate = float.Parse(txtLRate.Text.Trim())
                    ,
                    lsec = int.Parse(txtLSec.Text.Trim())
                    ,
                    mrate = float.Parse(txtMRate.Text.Trim())
                    ,
                    msec = int.Parse(txtMSec.Text.Trim())
                };

                txtKind.Text = string.Empty;
                txtLRate.Text = "50";
                txtLSec.Text = "60";
                txtMRate.Text = "50";
                txtMSec.Text = "60";

                _parentWin.internationalrates.ADD(_obj);
            }
            else if (_mode == AddPrefixStates.Modify)
            {
                //_obj.areacode = int.Parse(txtKind.Text.Trim());
                //_obj.lrate = float.Parse(txtLRate.Text.Trim());
                //_obj.lsec = int.Parse(txtLSec.Text.Trim());
                //_obj.mrate = float.Parse(txtMRate.Text.Trim());
                //_obj.msec = int.Parse(txtMSec.Text.Trim());
                _parentWin.internationalrates.MODIFY(_obj);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtInteger_TextChanged(object sender, TextChangedEventArgs e)
        {
            util.ValidateInteger(sender);
        }

        private void txtDouble_TextChanged(object sender, TextChangedEventArgs e)
        {
            util.ValidateDouble(sender);
        }
    }
}
