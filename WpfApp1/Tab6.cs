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
    /// MainWindow.xaml �Ľ����߼�
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<jiedian> os_tab6 = null;
        string chaxun_command_str = null;

        private void DaoChuExcel_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.DefaultExt = "xls";
                sfd.Filter = "�ļ�(*.xls)|*.xls";
                DaoChuExcel_Button.Content = "������...";
                if (sfd.ShowDialog() == true)
                {
                    output_excel(sfd.FileName);
                    DaoChuExcel_Button.Content = "";
                }
                #endregion
            }
            catch
            {
                System.Windows.MessageBox.Show("���ڸ�ʽ����", "error");
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

            //�����
            chaxun_command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + ", " + ShuJuKu.Table3_JieDian + 
                " where " + ShuJuKu.Table1_ShiJIna_JieDian + ".id=" + ShuJuKu.Table3_JieDian + ".id and" + "`Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" order by `Date` desc";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            //��DataGrid���������
            os_tab6 = (ObservableCollection<jiedian>)DataGrid_tab6.ItemsSource;
            os_tab6.Clear();//����ϴε�����

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString()));
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

            //�����
            chaxun_command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where `id`=\"" + BianHaoChaXun1_tab6.Text + "\" order by `Date` desc";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            //��DataGrid���������
            os_tab6 = (ObservableCollection<jiedian>)DataGrid_tab6.ItemsSource;
            os_tab6.Clear();//����ϴε�����

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString()));
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

            //�����
            chaxun_command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where `Date`>=\"" + date_begin.ToString() + "\" and `Date`<=\"" + date_end.ToString() + "\" and `id`=\"" +BianHaoChaXun2_tab6.Text + "\" order by `Date` desc";
            //string dataSet_temp_str = "select * from test5 order by `Date` desc";
            DataSet dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, chaxun_command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            //��DataGrid���������
            os_tab6 = (ObservableCollection<jiedian>)DataGrid_tab6.ItemsSource;
            os_tab6.Clear();//����ϴε�����

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                os_tab6.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString()));
            }
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
            QiShi2_TextBox_tab6.Text = temp_str;

            temp_str_array[3] = (Convert.ToInt16(temp_str_array[3]) + 2).ToString();//��ʼʱ��Ϊ��ǰʱ���ǰһ��Сʱ
            temp_str = temp_str_array[0] + "��" + temp_str_array[1] + "��" + temp_str_array[2] + "��" + temp_str_array[3] + "ʱ";

            ZhongZhi1_TextBox_tab6.Text = temp_str;
            ZhongZhi2_TextBox_tab6.Text = temp_str;
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

        public void output_excel(string strFileName)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Excel.Application();

            if (xlApp == null)
            {

                System.Windows.MessageBox.Show("�޷�����excel���󣬿�������ϵͳû�а�װexcel");

                return;

            }

            xlApp.DefaultFilePath = "";

            xlApp.DisplayAlerts = true;

            xlApp.SheetsInNewWorkbook = 1;

            Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);

            //����б�ͷ
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
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            //��ListView�е����ݵ���Excel��
            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                for (int j = 0; j < temp_dataRow.Count; j++)
                {
                    //ע������ڵ�����ʱ����ˡ�\t�� ��Ŀ�ľ��Ǳ��⵼����������ʾΪ��ѧ�����������Է���ÿ�е���β��

                    xlApp.Cells[(i + 2), (j + 1)] = temp_DataRow[i][j].ToString() + "\t";

                }

            }

            //������Ҫ˵��������strFileName,Excel.XlFileFormat.xlExcel9795���淽ʽʱ �����Excel�汾����95��97 ����2003��2007 ʱ������ʱ��ᱨһ�������쳣���� HRESULT:0x800A03EC�� ����취���ǻ���strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal��

            xlBook.SaveAs(strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, false, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            xlBook.Close();
            xlApp = null;

            xlBook = null;

            System.Windows.MessageBox.Show("���ɱ������");
        }

    }
}
