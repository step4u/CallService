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

using System.Data;
using System.Printing;
using System.Collections.ObjectModel;

using Com.Huen.Commands;
using Com.Huen.Libs;
using Com.Huen.DataModel;
using Com.Huen.Sql;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BILL : Window
    {
        private string inifilepath = @".\bill.ini";
        private ObservableCollection<DOMESTICRATE> DomesticRates;
        private ObservableCollection<INTERNATIONALRATE> InternationalRates;
        private ObservableCollection<INTERNATIONAL> Internationals;
        private ObservableCollection<BILLException> BillExceptions;
        private string[] _internationalnumber = new string[] { "001", "002", "003", "005", "006", "007", "008" };
        private DataTable dt = null;
        private PrintingOrientation printorientation = PrintingOrientation.Portrait;

        public string _extnum = string.Empty;

        public BILL()
        {
            InitializeComponent();

            InitializeWindow();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        public BILL(string ext)
        {
            InitializeComponent();

            InitializeWindow();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PageInitialize();

            DomesticRates = new DOMESTICRATES().GETLIST();
            InternationalRates = new INTERNATIONALRATES().GETLIST();
            Internationals = new INTERNATIONALS().GETLIST();

            if (CDRInfoValidate(out dt))
            {
                sdate.Value = string.IsNullOrEmpty(dt.Rows[1][4].ToString()) ? DateTime.Now : DateTime.Parse(dt.Rows[0][4].ToString());
            }
            else
            {
                sdate.Value = DateTime.Now;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.WriteIni();
        }

        //private DocumentPaginator docViewer;
        private FlowDocument flowDoc;
        private void InitializeWindow()
        {
            //this.ReadIni();

            flowDoc = new FlowDocument();
            docs.Document = flowDoc;
        }

        private void ReadIni()
        {
            Ini inifile = new Ini(inifilepath);
            string _top = inifile.IniReadValue("POSITION", "TOP");
            string _left = inifile.IniReadValue("POSITION", "LEFT");
            string _width = inifile.IniReadValue("SIZE", "WIDTH");
            string _height = inifile.IniReadValue("SIZE", "HEIGHT");
            string _state = inifile.IniReadValue("ETC", "STATE");

            this.Top = string.IsNullOrEmpty(_top) ? 50.0d : double.Parse(_top);
            this.Left = string.IsNullOrEmpty(_left) ? 50.0d : double.Parse(_left);
            this.Width = string.IsNullOrEmpty(_width) ? 800.0d : double.Parse(_width);
            this.Height = string.IsNullOrEmpty(_height) ? 600.0d : double.Parse(_height);
            this.WindowState = string.IsNullOrEmpty(_state) ? System.Windows.WindowState.Normal : (System.Windows.WindowState)Enum.Parse(typeof(System.Windows.WindowState), _state);
        }

        private void WriteIni()
        {
            Ini inifile = new Ini(inifilepath);
            inifile.IniWriteValue("POSITION", "TOP", this.Top.ToString());
            inifile.IniWriteValue("POSITION", "LEFT", this.Left.ToString());
            inifile.IniWriteValue("SIZE", "WIDTH", this.Width.ToString());
            inifile.IniWriteValue("SIZE", "HEIGHT", this.Height.ToString());
            inifile.IniWriteValue("ETC", "STATE", this.WindowState.ToString());
        }

        private void PageInitialize()
        {
            if (printorientation == PrintingOrientation.Portrait)
            {
                FlowDocument doc = docs.Document;
                doc.PageHeight = PrintLayout.A4.Size.Height;
                doc.PageWidth = PrintLayout.A4.Size.Width;
                //doc.PagePadding = PrintLayout.A4.Margin;
                doc.ColumnGap = 0.0d;
                doc.ColumnWidth = PrintLayout.A4.ColumnWidth;
            }
            else
            {
                LengthConverter converter = new LengthConverter();

                FlowDocument doc = docs.Document;
                doc.PageHeight = PrintLayout.A4Landscape.Size.Height;
                //doc.PageWidth = PrintLayout.A4Landscape.Size.Width;
                doc.PageWidth = (double)converter.ConvertFromInvariantString(docs.Width.ToString());
                //doc.PagePadding = PrintLayout.A4.Margin;
                doc.ColumnGap = 0.0d;
                doc.ColumnWidth = PrintLayout.A4Landscape.ColumnWidth;
            }
        }

        private void cmdSave2Excel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dt == null) return;

            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xls)|*.xls";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataSet _ds = new DataSet();

                DataTable TOTAL = new DataTable();
                TOTAL.TableName = "TOTAL";
                TOTAL.Columns.Add("GUBUN", typeof(string));
                TOTAL.Columns.Add("COUNT", typeof(string));
                TOTAL.Columns.Add("USEDTIME", typeof(string));
                TOTAL.Columns.Add("RATE", typeof(string));

                DataTable DETAIL = new DataTable();
                DETAIL.TableName = "DETAIL";
                DETAIL.Columns.Add("Seq", typeof(string));
                DETAIL.Columns.Add("Caller", typeof(string));
                DETAIL.Columns.Add("Callee", typeof(string));
                DETAIL.Columns.Add("Start Date", typeof(string));
                DETAIL.Columns.Add("End Date", typeof(string));
                DETAIL.Columns.Add("Used Time", typeof(string));
                DETAIL.Columns.Add("Result", typeof(string));
                DETAIL.Columns.Add("Rate", typeof(string));
                DETAIL.Columns.Add("Etc", typeof(string));
                DETAIL.Rows.Add("Seq", "Caller", "Callee", "Start Date", "End Date", "Used Time", "Result", "Rate", "Etc");

                TOTALCALL _innercall = new TOTALCALL();
                TOTALCALL _outercall = new TOTALCALL();
                TOTALCALL _cellphonecall = new TOTALCALL();
                TOTALCALL _internationalcall = new TOTALCALL();
                TOTALCALL _totalcall = new TOTALCALL();

                foreach (DataRow row in dt.Rows)
                {
                    DateTime _sdate = DateTime.Parse(row[3].ToString());
                    DateTime _edate = DateTime.Parse(row[4].ToString());
                    TimeSpan ts = _edate - _sdate;

                    AMOUNTINFO _amt = this.GetAmounts(row, ts);

                    if (_amt.domestictype == "EXT")
                    {
                        // inner call
                        _innercall.count++;
                        _innercall.usedtime += ts.TotalSeconds;
                        _innercall.rate = 0;
                    }
                    else if (_amt.domestictype == "O")
                    {
                        // outer call
                        _outercall.count++;
                        _outercall.usedtime += ts.TotalSeconds;
                        _outercall.rate += _amt.fee;
                    }
                    else if (_amt.domestictype == "M")
                    {
                        // cellphone call
                        _cellphonecall.count++;
                        _cellphonecall.usedtime += ts.TotalSeconds;
                        _cellphonecall.rate += _amt.fee;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_amt.domestictype))
                        {
                            // Internationalcall
                            _internationalcall.count++;
                            _internationalcall.usedtime += ts.TotalSeconds;
                            _internationalcall.rate += _amt.fee;
                        }
                    }

                    DETAIL.Rows.Add(
                        row[0].ToString()
                        , row[1].ToString()
                        , row[2].ToString()
                        , _sdate.ToString("MM-dd-yyyy HH:mm:ss")
                        , _edate.ToString("MM-dd-yyyy HH:mm:ss")
                        , ts.TotalSeconds.ToString()
                        , row[5].ToString()
                        , _amt.fee.ToString()
                        , _amt.natione
                        );
                }

                TOTAL.Rows.Add("", "Count", "UsedTime", "Rate");
                TOTAL.Rows.Add("Inner", _innercall.count.ToString(), _innercall.usedtime.ToString(), _innercall.rate.ToString());
                TOTAL.Rows.Add("Outer", _outercall.count.ToString(), _outercall.usedtime.ToString(), _outercall.rate.ToString());
                TOTAL.Rows.Add("Cell Phone", _cellphonecall.count.ToString(), _cellphonecall.usedtime.ToString(), _cellphonecall.rate.ToString());
                TOTAL.Rows.Add("International", _internationalcall.count.ToString(), _internationalcall.usedtime.ToString(), _internationalcall.rate.ToString());
                TOTAL.Rows.Add("Total"
                        , (_innercall.count + _outercall.count + _cellphonecall.count + _internationalcall.count).ToString()
                        , (_innercall.usedtime + _outercall.usedtime + _cellphonecall.usedtime + _internationalcall.usedtime).ToString()
                        , (_innercall.rate + _outercall.rate + _cellphonecall.rate + _internationalcall.rate).ToString()
                        );

                _ds.Tables.Add(TOTAL);
                _ds.Tables.Add(DETAIL);

                //ExcelHelper.SaveExcelDB(saveDialog.FileName, _ds, true);
                CodentQ.excelconnect excel = new CodentQ.excelconnect();
                excel.saveexcel(_ds, saveDialog.FileName);
            }

           
//            ExcelHelper.SaveExcelDB("d:\\111.xlsx", ds);
        }

        private void cmdPrint_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            docs.Print();

            /*
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = docs.Document;
                double pageHeight = doc.PageHeight;
                double pageWidth = doc.PageWidth;
                Thickness pagePadding = doc.PagePadding;
                double columnGap = doc.ColumnGap;
                double columnWidth = doc.ColumnWidth;

                //printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                //printorientation = PrintingOrientation.Landscape;
                //this.PageInitialize();
                printDialog.PrintDocument(((IDocumentPaginatorSource)docs.Document).DocumentPaginator, "ReportBill");

                doc.PageHeight = pageHeight;
                doc.PageWidth = pageWidth;
                doc.PagePadding = pagePadding;
                doc.ColumnGap = columnGap;
                doc.ColumnWidth = columnWidth;
            }
            */

            /*
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = docs.Document;
                double pageHeight = doc.PageHeight;
                double pageWidth = doc.PageWidth;
                Thickness pagePadding = doc.PagePadding;
                double columnGap = doc.ColumnGap;
                double columnWidth = doc.ColumnWidth;

                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);

                //doc.ColumnGap = 25;
                //doc.ColumnWidth = (doc.PageWidth - doc.ColumnGap - doc.PagePadding.Left - doc.PagePadding.Right) / 2;
                doc.ColumnGap = 00;
                doc.ColumnWidth = (doc.PageWidth - doc.ColumnGap - doc.PagePadding.Left - doc.PagePadding.Right);
                
                printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
                printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                //printDialog.PrintDocument(((IDocumentPaginatorSource)docs.Document).DocumentPaginator, "ReportBill");

                doc.PageHeight = pageHeight;
                doc.PageWidth = pageWidth;
                doc.PagePadding = pagePadding;
                doc.ColumnGap = columnGap;
                doc.ColumnWidth = columnWidth;
            }
            */
        }

        private void cmdExit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void cmdOptions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Com.Huen.Views.CdrAgent optionsWin = new Com.Huen.Views.CdrAgent(false) { Owner = this, ShowInTaskbar = false, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner };
            optionsWin.Show();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.CreateContents();
        }

        private void CreateContents()
        {
            flowDoc.Blocks.Clear();

            if (!CDRInfoValidate(out dt)) return;

            this.GetCDRInfo2(out dt);

            // CDR 데이터 가져오기

            //if (chkout.IsChecked == true ? true : false)
            //{
            //    if (!valid)
            //    {
            //       // MessageBox.Show(string.Format("Room \"{0}\". You didin't checkout yet. You should chkeckout the first", extnum.Text.Trim()));
            //        return;
            //    }                
            //}

            // create title
            Paragraph contents = new Paragraph() { Margin = new Thickness(0, 0, 0, 0), FontSize = 35.0d, TextAlignment = TextAlignment.Center };
            string txt = "BILL";
            Run titlerun = new Run(txt) { BaselineAlignment = BaselineAlignment.Baseline };
            contents.Inlines.Add(new Bold(titlerun));
            //flowDoc.Blocks.Clear();
            flowDoc.Blocks.Add(contents);

            // create ext, date
            contents = new Paragraph() { Margin = new Thickness(20, 30, 0, 0), FontSize = 16.0d, TextAlignment = TextAlignment.Left };
            txt = string.Format("Date : {0} - {1}\tExtention : {2}", ((DateTime)sdate.Value).ToShortDateString(), ((DateTime)edate.Value).ToShortDateString(), string.IsNullOrEmpty(_extnum) ? "ALL" : _extnum);
            contents.Inlines.Add(new Run(txt));
            flowDoc.Blocks.Add(contents);

            // create 2nd table
            Table tbl2 = new Table() { Padding = new Thickness(0, 0, 0, 0), CellSpacing = 0.0d, Margin = new Thickness(10, 40, 0, 0) };
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(50) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(60) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(160) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(180) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(180) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(90) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(70) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(90) });
            tbl2.Columns.Add(new TableColumn() { Width = new GridLength(120) });

            docs.Document.PageWidth = 1050;

            // 1st row
            TableRow tblrow = new TableRow();

            // 1st column cell
            Run run = new Run("Seq");
            Paragraph paragraph = new Paragraph(run);
            TableCell tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Caller");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Callee");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Start Date");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("End Date");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Used Time");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Result");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Rate");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            run = new Run("Etc");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            TableRowGroup rowgroup = new TableRowGroup() { Background = util.Str2Brush("#CCCCCC") };
            rowgroup.Rows.Add(tblrow);
            tbl2.RowGroups.Add(rowgroup);


            // 2nd row ~ 끝
            TOTALCALL _innercall = new TOTALCALL();
            TOTALCALL _outercall = new TOTALCALL();
            TOTALCALL _cellphonecall = new TOTALCALL();
            TOTALCALL _internationalcall = new TOTALCALL();
            TOTALCALL _totalcall = new TOTALCALL();
            int evenchk = 0;
            foreach (DataRow row in dt.Rows)
            {
                tblrow = new TableRow();

                // 1st column cell
                run = new Run(row[0].ToString());
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                run = new Run(row[1].ToString());
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                run = new Run(row[2].ToString());
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                DateTime _sdate = DateTime.Parse(row[3].ToString());
                run = new Run(_sdate.ToString("MM/dd/yyyy HH:mm:ss"));
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                DateTime _edate = DateTime.Parse(row[4].ToString());
                run = new Run(_edate.ToString("MM/dd/yyyy HH:mm:ss"));
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                TimeSpan ts = _edate - _sdate;
                run = new Run(ts.TotalSeconds.ToString());
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                run = new Run(row[5].ToString());
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                AMOUNTINFO _amt = this.GetAmounts(row, ts);

                run = new Run(_amt.fee.ToString());
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                run = new Run(_amt.natione);
                paragraph = new Paragraph(run);
                tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
                tblrow.Cells.Add(tblcell);

                Brush _background = util.Str2Brush("#EEEEEE");
                if (evenchk % 2 == 0)
                {
                    _background = util.Str2Brush("#FFFFFF");
                }

                rowgroup = new TableRowGroup() { Background = _background };
                rowgroup.Rows.Add(tblrow);
                tbl2.RowGroups.Add(rowgroup);

                evenchk++;

                if (_amt.domestictype == "EXT")
                {
                    // inner call
                    _innercall.count++;
                    _innercall.usedtime += ts.TotalSeconds;
                    _innercall.rate = 0;
                }
                else if (_amt.domestictype == "O")
                {
                    // outer call
                    _outercall.count++;
                    _outercall.usedtime += ts.TotalSeconds;
                    _outercall.rate += _amt.fee;
                }
                else if (_amt.domestictype == "M")
                {
                    // cellphone call
                    _cellphonecall.count++;
                    _cellphonecall.usedtime += ts.TotalSeconds;
                    _cellphonecall.rate += _amt.fee;
                }
                else
                {
                    if (string.IsNullOrEmpty(_amt.domestictype))
                    {
                        // Internationalcall
                        _internationalcall.count++;
                        _internationalcall.usedtime += ts.TotalSeconds;
                        _internationalcall.rate += _amt.fee;
                    }
                }
            }

            flowDoc.Blocks.Add(tbl2);




            // create 1st table
            Table tbl1 = new Table() { Padding = new Thickness(0, 0, 0, 0), CellSpacing = 0.0d, Margin = new Thickness(10, 10, 0, 0) };
            tbl1.Columns.Add(new TableColumn() { Width = new GridLength(130) });
            tbl1.Columns.Add(new TableColumn() { Width = new GridLength(80) });
            tbl1.Columns.Add(new TableColumn() { Width = new GridLength(230) });
            tbl1.Columns.Add(new TableColumn() { Width = new GridLength(230) });

            // 1st row
            tblrow = new TableRow();
            
            // 1st column cell
            run = new Run("");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 2nd column cell
            run = new Run("Count");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 3rd column cell
            run = new Run("Used Time");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 4th column cell
            run = new Run("Rate");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 2, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 16.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            rowgroup = new TableRowGroup() { Background = util.Str2Brush("#CCCCCC") };
            rowgroup.Rows.Add(tblrow);
            tbl1.RowGroups.Add(rowgroup);


            // 2nd row
            // 1st column cell
            tblrow = new TableRow();
            run = new Run("Inner");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 2nd column cell
            run = new Run(_innercall.count.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 3rd column cell
            run = new Run(_innercall.usedtime.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 4th column cell
            run = new Run(_innercall.rate.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            rowgroup = new TableRowGroup() { Background = util.Str2Brush("#FFFFFF") };
            rowgroup.Rows.Add(tblrow);
            tbl1.RowGroups.Add(rowgroup);


            // 3rd row
            // 1st column cell
            tblrow = new TableRow();
            run = new Run("Outer");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 2nd column cell
            run = new Run(_outercall.count.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 3rd column cell
            run = new Run(_outercall.usedtime.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 4th column cell
            run = new Run(_outercall.rate.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            rowgroup = new TableRowGroup() { Background = util.Str2Brush("#EEEEEE") };
            rowgroup.Rows.Add(tblrow);
            tbl1.RowGroups.Add(rowgroup);


            // 4th row
            // 1st column cell
            tblrow = new TableRow();
            run = new Run("Cell Phone");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 2nd column cell
            run = new Run(_cellphonecall.count.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 3rd column cell
            run = new Run(_cellphonecall.usedtime.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 4th column cell
            run = new Run(_cellphonecall.rate.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            rowgroup = new TableRowGroup() { Background = util.Str2Brush("#FFFFFF") };
            rowgroup.Rows.Add(tblrow);
            tbl1.RowGroups.Add(rowgroup);


            // 5th row
            // 1st column cell
            tblrow = new TableRow();
            run = new Run("International");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 2nd column cell
            run = new Run(_internationalcall.count.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 3rd column cell
            run = new Run(_internationalcall.usedtime.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 4th column cell
            run = new Run(_internationalcall.rate.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            rowgroup = new TableRowGroup() { Background = util.Str2Brush("#EEEEEE") };
            rowgroup.Rows.Add(tblrow);
            tbl1.RowGroups.Add(rowgroup);


            // 6th row
            // 1st column cell
            _totalcall.count = _innercall.count + _outercall.count + _cellphonecall.count + _internationalcall.count;
            _totalcall.usedtime = _innercall.usedtime + _outercall.usedtime + _cellphonecall.usedtime + _internationalcall.usedtime;
            _totalcall.rate = _innercall.rate + _outercall.rate + _cellphonecall.rate + _internationalcall.rate;

            tblrow = new TableRow();
            run = new Run("Total");
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(2, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 2nd column cell
            run = new Run(_totalcall.count.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 3rd column cell
            run = new Run(_totalcall.usedtime.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            // 4th column cell
            run = new Run(_totalcall.rate.ToString());
            paragraph = new Paragraph(run);
            tblcell = new TableCell(paragraph) { TextAlignment = TextAlignment.Center, BorderBrush = util.Str2Brush("#000000"), BorderThickness = new Thickness(0, 0, 2, 2), Padding = new Thickness(3, 3, 3, 3), Foreground = util.Str2Brush("#000000"), FontSize = 14.0d, LineHeight = 20.0d };
            tblrow.Cells.Add(tblcell);

            rowgroup = new TableRowGroup() { Background = util.Str2Brush("#CCCCCC") };
            rowgroup.Rows.Add(tblrow);
            tbl1.RowGroups.Add(rowgroup);

            flowDoc.Blocks.InsertBefore(tbl2, tbl1);
        }

        private bool GetCDRInfo(out DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            dt = null;

            //if (chkout.IsChecked == true ? true : false)
            //{
                bool valid = CDRInfoValidate(out dt);
                if (!valid)
                {
                    return false;
                }
                else
                {
                    sb.Append("select seq, caller, callee, startdate, enddate, result, caller_type, callee_type from cdrinfo where 1=1");
                    if (!string.IsNullOrEmpty(_extnum))
                    {
                        var tmparr = _extnum.Split(',');
                        string tmpext = string.Empty;
                        for (int k = 0; k < tmparr.Length; k++)
                        {
                            if (string.IsNullOrEmpty(tmpext))
                            {
                                tmpext = "'" + tmparr[k] + "'";
                            }
                            else
                            {
                                tmpext += ",'" + tmparr[k] + "'";
                            }
                        }
                        sb.AppendFormat(" and caller in ({0})", tmpext);
                    }
                    sb.AppendFormat(" and startdate between cast('{0}' as timestamp) and cast('{1}' as timestamp)", DateTime.Parse(dt.Rows[1][4].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Parse(dt.Rows[0][4].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append(" and caller_type=0");
                    sb.Append(" and result=0");
                    sb.Append(" order by caller asc, startdate asc");

                    using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
                    {
                        try
                        {
                            dt = db.GetDataTable();
                        }
                        catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                        {
                            util.WriteLog(string.Format("{0} INIT ERR : {1}", this.GetType(), fex.Message));
                        }
                    }
                    return true;
                }
            //}

            /*
            sb.Append("select seq, caller, callee, startdate, enddate, result, caller_type, callee_type from cdrinfo where 1=1");
            if (!string.IsNullOrEmpty(_extnum))
            {
                var tmparr = _extnum.Split(',');
                string tmpext = string.Empty;
                for (int k = 0; k < tmparr.Length; k++)
                {
                    if (string.IsNullOrEmpty(tmpext))
                    {
                        tmpext = "'" + tmparr[k] + "'";
                    }
                    else
                    {
                        tmpext += ",'" + tmparr[k] + "'";
                    }
                }
                sb.AppendFormat(" and caller in ({0})", tmpext);
            }
            sb.AppendFormat(" and cast(startdate as date) between cast('{0}' as date) and cast('{1}' as date)", ((DateTime)sdate.Value).ToShortDateString(), ((DateTime)edate.Value).ToShortDateString());
            sb.Append(" and caller_type=0");
            sb.Append(" and result=0");
            sb.Append(" order by caller asc, startdate asc");

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTable();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    util.WriteLog(string.Format("{0} INIT ERR : {1}", this.GetType(), fex.Message));
                }
            }

            return true;
            */
        }

        private void GetCDRInfo2(out DataTable dt)
        {
            dt = null;
            StringBuilder sb = new StringBuilder();

            sb.Append("select seq, caller, callee, startdate, enddate, result, caller_type, callee_type from cdrinfo where 1=1");
            if (!string.IsNullOrEmpty(_extnum))
            {
                var tmparr = _extnum.Split(',');
                string tmpext = string.Empty;
                for (int k = 0; k < tmparr.Length; k++)
                {
                    if (string.IsNullOrEmpty(tmpext))
                    {
                        tmpext = "'" + tmparr[k] + "'";
                    }
                    else
                    {
                        tmpext += ",'" + tmparr[k] + "'";
                    }
                }
                sb.AppendFormat(" and caller in ({0})", tmpext);
            }
            sb.AppendFormat(" and startdate between cast('{0}' as timestamp) and cast('{1}' as timestamp)", ((DateTime)sdate.Value).ToShortDateString(), ((DateTime)edate.Value).ToShortDateString());
            sb.Append(" and caller_type=0");
            sb.Append(" and result=0");
            sb.Append(" order by caller asc, startdate asc");

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTable();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    util.WriteLog(string.Format("{0} INIT ERR : {1}", this.GetType(), fex.Message));
                }
            }
        }

        private bool CDRInfoValidate(out DataTable dt)
        {
            dt = null;
            bool result = false;

            //DataTable dt = null;

            var tmparr = _extnum.Split(',');
            string tmpext = string.Empty;
            if (!string.IsNullOrEmpty(_extnum))
            {
                for (int k = 0; k < tmparr.Length; k++)
                {
                    if (string.IsNullOrEmpty(tmpext))
                    {
                        tmpext = "'" + tmparr[k] + "'";
                    }
                    else
                    {
                        tmpext += ",'" + tmparr[k] + "'";
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select first 2 * from tbl_log where 1=1 and room in ({0}) and (chk=1 or chk=0) order by room asc, regdate desc", tmpext);

            using (FirebirdDBHelper db = new FirebirdDBHelper(sb.ToString(), util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTable();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fex)
                {
                    util.WriteLog(string.Format("{0} INIT ERR : {1}", this.GetType(), fex.Message));
                }
            }

            if (dt.Rows.Count <= 0)
            {
                result = false;
            }
            else
            {
                if (dt.Rows[0][3].ToString().Equals("0"))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private AMOUNTINFO GetAmounts(DataRow row, TimeSpan ts)
        {
            int multiply = 1;
            string callee = row[2].ToString();
            int caller_type = int.Parse(row[6].ToString());
            int callee_type = int.Parse(row[7].ToString());
            AMOUNTINFO _amountinfo = new AMOUNTINFO() { natione = "KOREA", domestictype = string.Empty, fee = 0.0d };
            double __duration = ts.TotalSeconds;
            double __fee = 0.0d;
            string __tmpcallee = callee.Substring(0, 2);

            if (caller_type == 0 && callee_type == 0)
            {
                _amountinfo.domestictype = "EXT";
                return _amountinfo;
            }

            if (__tmpcallee == "00")
            {
                // 국제 통화
                for (int i = 0; i < _internationalnumber.Length; i++)
                {
                    string __internumber = _internationalnumber[i];
                    if (callee.IndexOf(__internumber) == 0)
                    {
                        string __inum = string.Empty;

                        if (__internumber == "003" || __internumber == "007")
                        {
                            __inum = callee.Substring(__internumber.Length + 2, callee.Length - (__internumber.Length + 2));
                        }
                        else
                        {
                            __inum = callee.Substring(__internumber.Length, callee.Length - __internumber.Length);
                        }

                        // 나라 코드 분리 및 확인
                        int __kind = -1;
                        string __nationcode = string.Empty;
                        string __areacode = string.Empty;
                        string __lm = string.Empty;

                        foreach (INTERNATIONAL _data in Internationals)
                        {
                            if (__inum.IndexOf(_data.nation_num) == 0 && string.IsNullOrEmpty(_data.nation_local_num))
                            {
                                _amountinfo.natione = _data.natione;
                                __nationcode = _data.nation_num;
                                __kind = _data.areacode;
                                __lm = _data.lm;
                                break;
                            }
                        }

                        // 나라 코드를 제외한 번호
                        __inum = __inum.Substring(__nationcode.Length, __inum.Length - __nationcode.Length);

                        // 지역 코드 분리 및 확인
                        foreach (INTERNATIONAL _data in Internationals.Where(x => x.nation_num == __nationcode))
                        {
                            if (!string.IsNullOrEmpty(_data.nation_local_num))
                            {
                                if (__inum.IndexOf(_data.nation_local_num) == 0)
                                {
                                    string __itum = __inum.Substring(_data.nation_local_num.Length, __inum.Length - _data.nation_local_num.Length);

                                    if (__itum.Length > 6)
                                    {
                                        __areacode = _data.nation_local_num;
                                        __kind = _data.areacode;
                                        __lm = _data.lm;
                                    }
                                }
                            }
                        }

                        // 나라코드, 지역코드 제외한 번호
                        //__inum = __inum.Substring(__areacode.Length, __inum.Length - __areacode.Length);

                        INTERNATIONALRATE __region = InternationalRates.FirstOrDefault(x => x.areacode == __kind);
                        if (__region == null)
                        {
                            __fee = (700 * multiply) * Math.Ceiling(__duration / 60);
                        }
                        else
                        {
                            if (__region.lrate == __region.mrate)
                            {
                                // L,M 동일 과금
                                __fee = (__region.lrate * multiply) * Math.Ceiling(__duration / __region.lsec);
                            }
                            else
                            {
                                // L,M 구분 과금
                                if (__lm == "L")
                                {
                                    __fee = (__region.lrate * multiply) * Math.Ceiling(__duration / __region.lsec);
                                }
                                else if (__lm == "M")
                                {
                                    __fee = (__region.mrate * multiply) * Math.Ceiling(__duration / __region.msec);
                                }
                            }
                        }
                    }
                }
            }
            else if (__tmpcallee == "01")
            {
                // 국내 통화, 이동전화
                DOMESTICRATE __tmpcallrate = DomesticRates.FirstOrDefault(x => x.prefix == __tmpcallee);
                __fee = (__tmpcallrate.rate * multiply) * Math.Ceiling(__duration / __tmpcallrate.sec);

                _amountinfo.domestictype = "M";
            }
            else if (__tmpcallee == "02")
            {
                // 국내 통화, 서울
                DOMESTICRATE __tmpcallrate = DomesticRates.FirstOrDefault(x => x.prefix == __tmpcallee);
                __fee = (__tmpcallrate.rate * multiply) * Math.Ceiling(__duration / __tmpcallrate.sec);

                _amountinfo.domestictype = "O";
            }
            else
            {
                // 그 외 시외
                __tmpcallee = callee.Substring(0, 3);
                DOMESTICRATE __tmpcallrate = DomesticRates.FirstOrDefault(x => x.prefix == __tmpcallee);
                if (__tmpcallrate == null)
                {
                    __fee = (38 * multiply) * Math.Ceiling(__duration / 180);
                }
                else
                {
                    __fee = (__tmpcallrate.rate * multiply) * Math.Ceiling(__duration / __tmpcallrate.sec);
                }

                _amountinfo.domestictype = "O";
            }

            _amountinfo.fee = Math.Round(__fee, 2);

            return _amountinfo;
        }
    }

    public class AMOUNTINFO
    {
        public string natione = string.Empty;
        public string domestictype = string.Empty;
        public double fee = 0.0d;
    }

    public class TOTALCALL
    {
        public int count = 0;
        public double usedtime = 0.0d;
        public double rate = 0.0d;
    }

    public enum PrintingOrientation
    {
        Portrait
        , Landscape
    }
}
