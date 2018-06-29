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

        private void QueRenBianGeng_Button_tab5_Click(object sender, RoutedEventArgs e)
        {
            Update_Information_Jiedian(Ellipse_Array_tab4, Convert.ToInt16(BianHao_tab5.Text), JianCeQiTi_tab5.Text, AnZhuangWeiZhi_tab5.Text, AnZhuangShiJina_tab5.Text, GaoXianBaoJing_tab5.Text, DiXianBaoJing_tab5.Text);
            Init_Jiedian_DisplayOrNot();//刷新所有节点
        }

        private void DiTuMoShi_Tab5_Buttton_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 4;
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
