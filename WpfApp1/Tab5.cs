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
        private System.Drawing.Point pointView_tab5 = new System.Drawing.Point(0, 0);
        public System.Windows.Forms.ToolTip toolTip_tab5 = new System.Windows.Forms.ToolTip();

        private void XinXiBianGeng_Tab5_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(true, false, false, 5);
        }

        private void ZengShan_Tab5_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(false, true, true, 5);
        }

        private void PuTongMoShi_Tab5_Button_Click(object sender, RoutedEventArgs e)
        {
            Tab_change_fore(5, 3);
        }

        private void QueRenBianGeng_Button_tab5_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao_tab5.Text == "")//���TextBox��û�����ݾͲ������κβ���
                return;
            Update_Information_Jiedian(Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text), JianCeQiTi_tab5.Text, AnZhuangWeiZhi_tab5.Text, AnZhuangShiJina_tab5.Text, GaoXianBaoJing_tab5.Text, DiXianBaoJing_tab5.Text);
            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ�
        }

        private void ZengJiaBianGeng_Button_tab5_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao_tab5.Text == "")//���TextBox��û�����ݾͲ������κβ���
                return;

            //ȷ�϶Ի���
            if (System.Windows.MessageBox.Show("ȷ�����ӽڵ㣿", "��ʾ", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            ZengJiaBianGeng(Convert.ToInt16(BianHao_tab5.Text), JianCeQiTi_tab5.Text, AnZhuangWeiZhi_tab5.Text, AnZhuangShiJina_tab5.Text, GaoXianBaoJing_tab5.Text, DiXianBaoJing_tab5.Text);

            //���ݵ�ǰ�����ڵ������������ݿ��е�����
            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ㣬������ͼģʽ���б�ģʽ

            //�ж��������нڵ��Ƿ����
            DiaoXian();
        }

        private void ShanChuDianWei_Button_tab5_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao_tab5.Text == "")//���TextBox��û�����ݾͲ������κβ���
                return;

            //ȷ�϶Ի���
            if (System.Windows.MessageBox.Show("ȷ��ɾ���ڵ㣿", "��ʾ", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            string command_str = "delete from table3_jiedian where id=" + BianHao_tab5.Text + ";";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ㣬������ͼģʽ���б�ģʽ
        }

        private void BianHao_tab5_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Show_Information_Jiedian_tab5(Convert.ToInt16(BianHao_tab5.Text));

                DataSet dataSet_temp = new DataSet();
                string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + BianHao_tab5.Text;
                dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                      CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

                if (temp_DataRow.Count <= 0)//û����Ӧ�ڵ����Ϣ�������µĽڵ�
                {
                    System.Windows.MessageBox.Show("�˽ڵ���δ����", "��ʾ");
                    return;
                }

                //jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text) - 1);
                //jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text) - 1);
            }
        }

        /// <summary>
        /// ��ʾ��ţ�������壬 ��װλ�ã���װʱ�䣬���ޱ��������ޱ���
        /// </summary>
        public void Show_Information_Jiedian_tab5(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + index.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            if (temp_DataRow.Count <= 0)//û����Ӧ�ڵ����Ϣ���ͽ�������������ó�Ĭ��ֵ
            {
                JianCeQiTi_tab5.Text = "Ĭ��ֵ";
                AnZhuangWeiZhi_tab5.Text = "Ĭ��ֵ";
                AnZhuangShiJina_tab5.Text = DateTime.Now.ToString();//Ĭ��Ϊ��ǰʱ��
                GaoXianBaoJing_tab5.Text = "Ĭ��ֵ";
                DiXianBaoJing_tab5.Text = "Ĭ��ֵ";
            }
            else
            {
                JianCeQiTi_tab5.Text = temp_DataRow[0][1].ToString();
                AnZhuangWeiZhi_tab5.Text = temp_DataRow[0][2].ToString();
                AnZhuangShiJina_tab5.Text = temp_DataRow[0][3].ToString();
                GaoXianBaoJing_tab5.Text = temp_DataRow[0][4].ToString();
                DiXianBaoJing_tab5.Text = temp_DataRow[0][5].ToString();
            }
        }

        private void DiTuMoShi_Tab5_Buttton_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 4;
            Tab_change_fore(5, 4);
            Enable_verify_add_del(false, false, false, 4);
        }

        private void listview_largeicon_MouseMove_tab5(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ListViewItem lv = this.listview_largeicon_tab5.GetItemAt(e.X, e.Y);

            if (lv != null)
            {
                if (pointView_tab5.X != e.X || pointView_tab5.Y != e.Y)//��ֹ��˸

                {

                    string str_temp = lv.SubItems[0].Text.Replace("#", "");

                    toolTip_tab5.Show(Ellipse_Array[Convert.ToInt16(str_temp) - 1].ToolTip.ToString(), listview_largeicon_tab5, new System.Drawing.Point(e.X, e.Y), 10000);

                    pointView_tab5.X = e.X;

                    pointView_tab5.Y = e.Y;

                    toolTip_tab5.Active = true;

                }
            }
            else
            {
                toolTip_tab5.Hide(listview_largeicon_tab5);
            }
        }
    }
}
