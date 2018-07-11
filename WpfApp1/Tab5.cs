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
    /// MainWindow.xaml 的交互逻辑
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
            if (BianHao_tab5.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;
            Update_Information_Jiedian(Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text), JianCeQiTi_tab5.Text, AnZhuangWeiZhi_tab5.Text, AnZhuangShiJina_tab5.Text, GaoXianBaoJing_tab5.Text, DiXianBaoJing_tab5.Text);
            Init_Jiedian_DisplayOrNot();//刷新所有节点
        }

        private void ZengJiaBianGeng_Button_tab5_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao_tab5.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;

            //确认对话框
            if (System.Windows.MessageBox.Show("确定增加节点？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            ZengJiaBianGeng(Convert.ToInt16(BianHao_tab5.Text), JianCeQiTi_tab5.Text, AnZhuangWeiZhi_tab5.Text, AnZhuangShiJina_tab5.Text, GaoXianBaoJing_tab5.Text, DiXianBaoJing_tab5.Text);

            //根据当前新增节点的坐标更新数据库中的数据
            Init_Jiedian_DisplayOrNot();//刷新所有节点，包括地图模式和列表模式

            //判断现有所有节点是否掉线
            DiaoXian();
        }

        private void ShanChuDianWei_Button_tab5_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao_tab5.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;

            //确认对话框
            if (System.Windows.MessageBox.Show("确定删除节点？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            string command_str = "delete from table3_jiedian where id=" + BianHao_tab5.Text + ";";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            Init_Jiedian_DisplayOrNot();//刷新所有节点，包括地图模式和列表模式
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
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

                if (temp_DataRow.Count <= 0)//没有相应节点的信息，建立新的节点
                {
                    System.Windows.MessageBox.Show("此节点尚未创建", "提示");
                    return;
                }

                //jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text) - 1);
                //jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text) - 1);
            }
        }

        /// <summary>
        /// 显示编号，检测气体， 安装位置，安装时间，高限报警，低限报警
        /// </summary>
        public void Show_Information_Jiedian_tab5(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + index.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            if (temp_DataRow.Count <= 0)//没有相应节点的信息，就将下面的数据设置成默认值
            {
                JianCeQiTi_tab5.Text = "默认值";
                AnZhuangWeiZhi_tab5.Text = "默认值";
                AnZhuangShiJina_tab5.Text = DateTime.Now.ToString();//默认为当前时间
                GaoXianBaoJing_tab5.Text = "默认值";
                DiXianBaoJing_tab5.Text = "默认值";
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
                if (pointView_tab5.X != e.X || pointView_tab5.Y != e.Y)//防止闪烁

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
