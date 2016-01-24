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
    public partial class AddPrefixException : Window
    {
        private AddPrefixStates _mode = AddPrefixStates.Add;

        public CallingRateException _propsValue = null;
        private CdrAgent _parentWin = null;

        public AddPrefixException()
        {
            InitializeComponent();

            this.Loaded += AddPrefix_Loaded;
        }

        void AddPrefix_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            _parentWin = (CdrAgent)this.Owner;

            if (_propsValue == null)
            {
                _mode = AddPrefixStates.Add;
            }
            else
            {
                _mode = AddPrefixStates.Modify;

                txtTelnum.Text = _propsValue.TelNum;
                txtMemo.Text = _propsValue.Memo;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // 값 filter
            if (string.IsNullOrEmpty(txtTelnum.Text.Trim()))
            {
                MessageBox.Show("내선 번호를 입력하세요.");
                txtTelnum.Focus();
                return;
            }

            if (_mode == AddPrefixStates.Add)
            {

            }
            else if (_mode == AddPrefixStates.Modify)
            {

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
