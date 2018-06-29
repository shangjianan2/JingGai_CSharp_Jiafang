using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Forms;
using System.Drawing;

using System.Collections.ObjectModel;
using System.ComponentModel;
using MySQL_Funtion;
using System.Data;

using Excel = Microsoft.Office.Interop.Excel;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<jiedian_tab6> os_tab6 = null;
        string chaxun_command_str = null;

        private void DaoChuExcel_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.DefaultExt = "xls";
                sfd.Filter = "文件(*.xls)|*.xls";
                DaoChuExcel_Button.Content = "保存中...";
                if (sfd.ShowDialog() == true)
                {
                    output_excel(sfd.FileName);
                    DaoChuExcel_Button.Content = "";
                }
                #endregion
            }
            catch
            {
                System.Windows.MessageBox.Show("日期格式错误", "error");
                return;
            }
        }

        private void ChaXun1_tab6_Click(object sender, RoutedEventArgs e)
        {            
            string[] temp_str_begin_array = Split_Time(QiShi1_TextBox_tab6.Text);
            string[] temp_str_end_array = Split_Time(ZhongZhi1_TextBox_tab6.Text);

            string temp_str_begin = temp_str_begin_array[0] + " " + temp_str_begin_array[1] + " " + temp_str_begin_array[2] + " " + temp_str_begin_array[3];
            string temp_str_end = temp_str_end_array[0] + " " + temp_str_end_array[1] + " " + temp_str_end_array[2] + " " + temp_str_end_array[3];

            DateTime date_begin = DateTime.ParseExact(temp_str_begin, "yyyy M d H", null);
            DateTime date_end = DateTime.ParseExact(temp_str_end, "yyyy M d H", null);

            //添加列
            //chaxun_command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + ", " + ShuJuKu.Table3_JieDian + 
            //    " where " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=" + ShuJuKu.Table3_JieDian + ".id and" + "`Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" order by `Date` desc";
            chaxun_command_str = "select " + ShuJuKu.Table1_ShiJIna_JieDian + ".id, " + ShuJuKu.Table1_ShiJIna_JieDian + ".`gas type`, danwei, status, nongdu, dixian, gaoxian, dianliang, wendu, `Date`, `location`, `time of install` from " + ShuJuKu.Table1_ShiJIna_JieDian + 
                ", " + ShuJuKu.Table3_JieDian + " where " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=" + ShuJuKu.Table3_JieDian + ".id and" + "`Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" order by `Date` desc;";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            //想DataGrid中添加数据
            os_tab6 = (ObservableCollection<jiedian_tab6>)DataGrid_tab6.ItemsSource;
            os_tab6.Clear();//清除上次的数据

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian_tab6(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }
        }

        private void ChaXun2_tab6_Click(object sender, RoutedEventArgs e)
        {
            string[] temp_str_begin_array = Split_Time(QiShi1_TextBox_tab6.Text);
            string[] temp_str_end_array = Split_Time(ZhongZhi1_TextBox_tab6.Text);

            string temp_str_begin = temp_str_begin_array[0] + " " + temp_str_begin_array[1] + " " + temp_str_begin_array[2] + " " + temp_str_begin_array[3];
            string temp_str_end = temp_str_end_array[0] + " " + temp_str_end_array[1] + " " + temp_str_end_array[2] + " " + temp_str_end_array[3];

            DateTime date_begin = DateTime.ParseExact(temp_str_begin, "yyyy M d H", null);
            DateTime date_end = DateTime.ParseExact(temp_str_end, "yyyy M d H", null);

            //添加列
            chaxun_command_str = "select " + ShuJuKu.Table1_ShiJIna_JieDian + ".id, " + ShuJuKu.Table1_ShiJIna_JieDian + ".`gas type`, danwei, status, nongdu, dixian, gaoxian, dianliang, wendu, `Date`, `location`, `time of install` from " + ShuJuKu.Table1_ShiJIna_JieDian +
                ", " + ShuJuKu.Table3_JieDian + " where " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=" + ShuJuKu.Table3_JieDian + ".id and " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=\"" + BianHaoChaXun1_tab6.Text + "\" order by `Date` desc;";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            //想DataGrid中添加数据
            os_tab6 = (ObservableCollection<jiedian_tab6>)DataGrid_tab6.ItemsSource;
            os_tab6.Clear();//清除上次的数据

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian_tab6(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }
        }

        private void ChaXun3_tab6_Click(object sender, RoutedEventArgs e)
        {
            string[] temp_str_begin_array = Split_Time(QiShi2_TextBox_tab6.Text);
            string[] temp_str_end_array = Split_Time(ZhongZhi2_TextBox_tab6.Text);

            string temp_str_begin = temp_str_begin_array[0] + " " + temp_str_begin_array[1] + " " + temp_str_begin_array[2] + " " + temp_str_begin_array[3];
            string temp_str_end = temp_str_end_array[0] + " " + temp_str_end_array[1] + " " + temp_str_end_array[2] + " " + temp_str_end_array[3];

            DateTime date_begin = DateTime.ParseExact(temp_str_begin, "yyyy M d H", null);
            DateTime date_end = DateTime.ParseExact(temp_str_end, "yyyy M d H", null);

            //添加列
            chaxun_command_str = "select " + ShuJuKu.Table1_ShiJIna_JieDian + ".id, " + ShuJuKu.Table1_ShiJIna_JieDian + ".`gas type`, danwei, status, nongdu, dixian, gaoxian, dianliang, wendu, `Date`, `location`, `time of install` from " + ShuJuKu.Table1_ShiJIna_JieDian +
                ", " + ShuJuKu.Table3_JieDian + " where " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=" + ShuJuKu.Table3_JieDian + ".id and" + "`Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" and " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=\"" + BianHaoChaXun1_tab6.Text + "\" order by `Date` desc;";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            //想DataGrid中添加数据
            os_tab6 = (ObservableCollection<jiedian_tab6>)DataGrid_tab6.ItemsSource;
            os_tab6.Clear();//清除上次的数据

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian_tab6(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }
        }

        public void Display_CurrentTime_on_TextBox()
        {
            string temp_str = null;
            string[] temp_str_array = null;//中间值，用于后面的时间计算
            string current_time = DateTime.Now.ToString("yyyy-MM-dd HH");//获取当前时间的字符串
            temp_str_array = Split_Time(current_time);
            temp_str_array[3] = (Convert.ToInt16(temp_str_array[3]) - 1).ToString();//起始时间为当前时间的前一个小时
            temp_str = temp_str_array[0] + "年" + temp_str_array[1] + "月" + temp_str_array[2] + "日" + temp_str_array[3] + "时";

            QiShi1_TextBox_tab6.Text = temp_str;
            QiShi2_TextBox_tab6.Text = temp_str;

            temp_str_array[3] = (Convert.ToInt16(temp_str_array[3]) + 2).ToString();//起始时间为当前时间的前一个小时
            temp_str = temp_str_array[0] + "年" + temp_str_array[1] + "月" + temp_str_array[2] + "日" + temp_str_array[3] + "时";

            ZhongZhi1_TextBox_tab6.Text = temp_str;
            ZhongZhi2_TextBox_tab6.Text = temp_str;
        }

        public string[] Split_Time(string time_str)
        {
            if(time_str.Contains('年'))//判断分隔符的形式
            {
                //先将文字分隔符转换为空格，最后按照空格分隔
                time_str = time_str.Replace("年", " ");
                time_str = time_str.Replace("月", " ");
                time_str = time_str.Replace("日", " ");
                time_str = time_str.Replace("时", " ");
                return time_str.Split(' ');
            }
            else
            {
                time_str = time_str.Replace(" ", "-");
                return time_str.Split('-');
            }
        }

        public void output_excel(string strFileName)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Excel.Application();

            if (xlApp == null)
            {

                System.Windows.MessageBox.Show("无法创建excel对象，可能您的系统没有安装excel");

                return;

            }

            xlApp.DefaultFilePath = "";

            xlApp.DisplayAlerts = true;

            xlApp.SheetsInNewWorkbook = 1;

            Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);

            //添加列表头
            DataSet temp_dataset = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, "show columns from " + ShuJuKu.Table1_ShiJIna_JieDian + ";", null);
            DataRowCollection temp_dataRow = temp_dataset.Tables[0].Rows;
            for (int i = 0; i < temp_dataRow.Count; i++)
            {
                //listView_tt.Columns.Add(temp_dataRow[i][0].ToString(), 50);
                xlApp.Cells[1, (i + 1)] = temp_dataRow[i][0].ToString();
            }

            //string dataSet_temp_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where `Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" order by `Date` desc";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            //将ListView中的数据导入Excel中
            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                for (int j = 0; j < temp_dataRow.Count; j++)
                {
                    //注意这个在导出的时候加了“\t” 的目的就是避免导出的数据显示为科学计数法。可以放在每行的首尾。

                    xlApp.Cells[(i + 2), (j + 1)] = temp_DataRow[i][j].ToString() + "\t";

                }

            }

            //例外需要说明的是用strFileName,Excel.XlFileFormat.xlExcel9795保存方式时 当你的Excel版本不是95、97 而是2003、2007 时导出的时候会报一个错误：异常来自 HRESULT:0x800A03EC。 解决办法就是换成strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal。

            xlBook.SaveAs(strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, false, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            xlBook.Close();
            xlApp = null;

            xlBook = null;

            System.Windows.MessageBox.Show("生成报表完成");
        }

    }

    public class jiedian_tab6 : jiedian
    {
        public string location;
        public string time_of_install;

        public string Location
        {
            get { return location; }
            set
            {
                if (value == null)
                {
                    location = " ";
                }
                else
                {
                    location = value;
                }
                OnPropertyChanged("Location");
            }
        }

        public string Time_of_install
        {
            get { return time_of_install; }
            set
            {
                if (value == null)
                {
                    time_of_install = " ";
                }
                else
                {
                    time_of_install = value;
                }
                OnPropertyChanged("Time_of_install");
            }
        }


        public jiedian_tab6(string id_tt, string gas_type_tt, string danwei_tt,
            string status_tt, string nongdu_tt, string dixian_tt, string gaoxian_tt, string dianliang_tt,
            string wendu_tt, string date_tt, string location_tt, string time_of_install_tt) : base(id_tt, gas_type_tt, danwei_tt,
            status_tt, nongdu_tt, dixian_tt, gaoxian_tt, dianliang_tt,
            wendu_tt, date_tt)
        {
            id = id_tt;
            gas_type = gas_type_tt;
            danwei = danwei_tt;

            status = status_tt;
            nongdu = nongdu_tt;
            dixian = dixian_tt;
            gaoxian = gaoxian_tt;
            dianliang = dianliang_tt;

            wendu = wendu_tt;
            date = date_tt;

            location = location_tt;
            time_of_install = time_of_install_tt;
        }
    }

    public class JieDians_tab6 :
            ObservableCollection<jiedian_tab6>
    {
        public JieDians_tab6()
        {

        }
    }
}
