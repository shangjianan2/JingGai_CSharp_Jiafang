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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml �Ľ����߼�
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<jiedian> os_tab6 = null;

        private void ChaXun1_tab6_Click(object sender, RoutedEventArgs e)
        {            
            string[] temp_str_begin_array = Split_Time(QiShi1_TextBox_tab6.Text);
            string[] temp_str_end_array = Split_Time(ZhongZhi1_TextBox_tab6.Text);

            string temp_str_begin = temp_str_begin_array[0] + " " + temp_str_begin_array[1] + " " + temp_str_begin_array[2] + " " + temp_str_begin_array[3];
            string temp_str_end = temp_str_end_array[0] + " " + temp_str_end_array[1] + " " + temp_str_end_array[2] + " " + temp_str_end_array[3];

            DateTime date_begin = DateTime.ParseExact(temp_str_begin, "yyyy M d H", null);
            DateTime date_end = DateTime.ParseExact(temp_str_end, "yyyy M d H", null);

            //�����
            string dataSet_temp_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where `Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" order by `Date` desc";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, dataSet_temp_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            //��DataGrid���������
            os_tab6 = (ObservableCollection<jiedian>)DataGrid_tab6.ItemsSource;

            for(int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }
        }

        private void ChaXun2_tab6_Click(object sender, RoutedEventArgs e)
        {
            os_tab6 = (ObservableCollection<jiedian>)DataGrid_tab6.ItemsSource;
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " order by date desc limit " + size_DataGrid_Display.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            for (int i = 0; i < size_DataGrid_Display; i++)
            {
                os_tab6.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }
        }

        private void ChaXun3_tab6_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Display_CurrentTime_on_TextBox()
        {
            string temp_str = null;
            string[] temp_str_array = null;//�м�ֵ�����ں����ʱ�����
            string current_time = DateTime.Now.ToString("yyyy-MM-dd HH");//��ȡ��ǰʱ����ַ���
            temp_str_array = Split_Time(current_time);
            temp_str_array[3] = (Convert.ToInt16(temp_str_array[3]) - 1).ToString();//��ʼʱ��Ϊ��ǰʱ���ǰһ��Сʱ
            temp_str = temp_str_array[0] + "��" + temp_str_array[1] + "��" + temp_str_array[2] + "��" + temp_str_array[3] + "ʱ";

            QiShi1_TextBox_tab6.Text = temp_str;

            temp_str_array[3] = (Convert.ToInt16(temp_str_array[3]) + 2).ToString();//��ʼʱ��Ϊ��ǰʱ���ǰһ��Сʱ
            temp_str = temp_str_array[0] + "��" + temp_str_array[1] + "��" + temp_str_array[2] + "��" + temp_str_array[3] + "ʱ";

            ZhongZhi1_TextBox_tab6.Text = temp_str;
        }

        public string[] Split_Time(string time_str)
        {
            if(time_str.Contains('��'))//�жϷָ�������ʽ
            {
                //�Ƚ����ַָ���ת��Ϊ�ո�����տո�ָ�
                time_str = time_str.Replace("��", " ");
                time_str = time_str.Replace("��", " ");
                time_str = time_str.Replace("��", " ");
                time_str = time_str.Replace("ʱ", " ");
                return time_str.Split(' ');
            }
            else
            {
                time_str = time_str.Replace(" ", "-");
                return time_str.Split('-');
            }
        }
    }
}
