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
    public partial class AddPrefixInterRegion : Window
    {
        private AddPrefixStates _mode = AddPrefixStates.Add;

        public CallingRateInterRegion _propsValue = null;
        private CdrAgent _parentWin = null;

        public AddPrefixInterRegion()
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

            //txtKind.ItemsSource = _parentWin._cdrprops.CDRInternational;
            txtKind.SelectedValuePath = "Kind";
            txtKind.DisplayMemberPath = "Kind";
            txtKind.SelectedIndex = 0;
            txtNationCode.Focus();

            if (_propsValue == null)
            {
                _mode = AddPrefixStates.Add;
                return;
            }
            else
            {
                _mode = AddPrefixStates.Modify;
                txtKind.SelectedValue = _propsValue.Kind;
                txtNationCode.Text = _propsValue.NationCode;
                txtAreaCode.Text = _propsValue.AreaCode;
                txtNationNameKO.Text = _propsValue.NameKo;
                txtNationNameEN.Text = _propsValue.NameEn;
                txtLM.Text = _propsValue.LM;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // 값 filter
            //if (txtKind.SelectedIndex == 0)
            //{
            //    MessageBox.Show("대역 번호를 선택하세요.");
            //    txtKind.Focus();
            //    return;
            //}
            if (string.IsNullOrEmpty(txtNationCode.Text.Trim()))
            {
                MessageBox.Show("국가 번호를 입력하세요.");
                txtNationCode.Focus();
                return;
            }
            //if (string.IsNullOrEmpty(txtAreaCode.Text.Trim()))
            //{
            //    MessageBox.Show("지역 번호를 입력하세요.");
            //    txtAreaCode.Focus();
            //    return;
            //}
            if (string.IsNullOrEmpty(txtNationNameKO.Text.Trim()))
            {
                MessageBox.Show("나라 이름(한글)을 입력하세요.");
                txtNationNameKO.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtNationNameEN.Text.Trim()))
            {
                MessageBox.Show("나라 이름(영어)을 입력하세요.");
                txtNationNameEN.Focus();
                return;
            }

            if (_mode == AddPrefixStates.Add)
            {
                CallingRateInterRegion __row = new CallingRateInterRegion() {
                    Kind = txtKind.Text.Trim()
                    ,
                    NationCode = txtNationCode.Text.Trim()
                    ,
                    AreaCode = txtAreaCode.Text.Trim()
                    ,
                    NameKo = txtNationNameKO.Text.Trim()
                    ,
                    NameEn = txtNationNameEN.Text.Trim()
                    ,
                    LM = txtLM.Text
                };

                txtNationCode.Text = string.Empty;
                txtAreaCode.Text = string.Empty;
                txtNationNameKO.Text = string.Empty;
                txtNationNameEN.Text = string.Empty;
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
