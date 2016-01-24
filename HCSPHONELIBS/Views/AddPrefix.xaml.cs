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
    public partial class AddPrefix : Window
    {
        public AddPrefixStates _mode = AddPrefixStates.Add;

        public DOMESTICRATE _domesticrate = null;
        private CdrAgent _parentWin = null;

        public AddPrefix()
        {
            InitializeComponent();

            this.Loaded += AddPrefix_Loaded;
        }

        public AddPrefix(AddPrefixStates mode)
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
                txtPrefix.Text = _domesticrate.prefix;
                txtPrefix.IsReadOnly = true;
                txtType.Text = _domesticrate.type;
                txtRate.Text = _domesticrate.rate.ToString();
                txtSec.Text = _domesticrate.sec.ToString();
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // 값 filter
            if (string.IsNullOrEmpty(txtPrefix.Text.Trim()))
            {
                MessageBox.Show("구분 번호를 입력해 주세요.");
                txtPrefix.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtType.Text.Trim()))
            {
                MessageBox.Show("구분명을 입력해 주세요.");
                txtType.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                MessageBox.Show("요율(원)을 입력해 주세요.");
                txtRate.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtSec.Text.Trim()))
            {
                MessageBox.Show("요율(초)을 입력해 주세요.");
                txtSec.Focus();
                return;
            }

            if (_mode == AddPrefixStates.Add)
            {
                _domesticrate = new DOMESTICRATE()
                {
                    prefix = txtPrefix.Text.Trim()
                    ,
                    type = txtType.Text.Trim()
                    ,
                    rate = float.Parse(txtRate.Text.Trim())
                    ,
                    sec = int.Parse(txtSec.Text.Trim())
                };

                _parentWin.domestics.ADD(_domesticrate);

                txtPrefix.Text = string.Empty;
                txtType.Text = string.Empty;
                txtRate.Text = "1";
                txtSec.Text = "1";
            }
            else if (_mode == AddPrefixStates.Modify)
            {
                _domesticrate.prefix = txtPrefix.Text.Trim();
                _domesticrate.type = txtType.Text.Trim();
                _domesticrate.rate = float.Parse(txtRate.Text.Trim());
                _domesticrate.sec = int.Parse(txtSec.Text.Trim());
                _parentWin.domestics.MODIFY(_domesticrate);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public enum AddPrefixStates
        {
            Add,
            Modify
        }

        private void txtPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            util.ValidateInteger(sender);
        }

        private void txtRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            util.ValidateDouble(sender);
        }

        private void txtSec_TextChanged(object sender, TextChangedEventArgs e)
        {
            util.ValidateInteger(sender);
        }
    }
}
