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
using System.Windows.Shapes;

using System.Data;
using MySQL_Funtion;

//using UDP_Thread;

using System.Collections.ObjectModel;

using System.ComponentModel;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<jiedian> os_tab4 = null;

        Point previousMousePoint_tab4 = new Point(0, 0);
        private bool isMouseLeftButtonDown_tab4 = false;

        //const int size_chanel = 64;
        //const int size_DataGrid_Display= 20;

        TranslateTransform[] translateTransform_Array_tab4 = new TranslateTransform[size_chanel];
        ScaleTransform[] scaleTransform_Array_tab4 = new ScaleTransform[size_chanel];
        Ellipse[] Ellipse_Array_tab4 = new Ellipse[size_chanel];

        private void XinXiBianGeng_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(true, false, false);
        }

        private void ZengShan_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(false, true, true);
        }

        private void ShanChuDianWei_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;

            //确认对话框
            if (System.Windows.MessageBox.Show("确定删除节点？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            string command_str = "delete from table3_jiedian where id=" + BianHao.Text + ";";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            Init_Jiedian_DisplayOrNot();//刷新所有节点，包括地图模式和列表模式
        }

        private void ZengJiaBianGeng_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;

            //确认对话框
            if (System.Windows.MessageBox.Show("确定增加节点？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            ZengJiaBianGeng(Convert.ToInt16(BianHao.Text), JianCeQiTi.Text, AnZhuangWeiZhi.Text, AnZhuangShiJina.Text, GaoXianBaoJing.Text, DiXianBaoJing.Text);

            //根据当前新增节点的坐标更新数据库中的数据
            Init_Jiedian_DisplayOrNot();//刷新所有节点，包括地图模式和列表模式
            //Update_Information_Jiedian(Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text));
        }

        public void ZengJiaBianGeng(int index, string jianceqiti_tt, string anzhuangweizhi_tt, string anzhuangshijian_tt, string gaoxianbaojing_tt, string dixianbaojing_tt)
        {
            //检测数据库中是否已经存在此节点
            DataSet dataSet_temp = new DataSet();
            string exist_str = "select `id` from " + ShuJuKu.Table3_JieDian;
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, exist_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                if (Convert.ToInt16(temp_DataRow[i][0]) == index)
                {
                    return;//如果数据库中已经有这个节点的id的时候，直接跳出
                }
            }

            //能运行到这里说明这个节点使个新的，然后将新的数据输入到数据库
            string command_str = "INSERT INTO " + ShuJuKu.Table3_JieDian + " (`id`, `gas type`, `location`, `time of install`, `high to warning`, `low to warning` ) VALUES ( \"" +
                index.ToString() + "\",  \"" + jianceqiti_tt + "\", \"" + anzhuangweizhi_tt +
                "\", \"" + anzhuangshijian_tt + "\", \"" + gaoxianbaojing_tt + "\", \"" + dixianbaojing_tt + "\");";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            //在管理员地图界面和普通地图界面中显示新增节点，初始节点位置为右上角
            Ellipse_Array[index].Visibility = Visibility.Visible;
            Ellipse_Array_tab4[index].Visibility = Visibility.Visible;
        }

        private void BianHao_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Show_Information_Jiedian(Convert.ToInt16(BianHao.Text));
                jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text) - 1);
                jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text) - 1);
            }
        }

        private void QueRenBianGeng_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;
            Update_Information_Jiedian(Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text), JianCeQiTi.Text, AnZhuangWeiZhi.Text, AnZhuangShiJina.Text, GaoXianBaoJing.Text, DiXianBaoJing.Text);
            Init_Jiedian_DisplayOrNot();//刷新所有节点
        }

        private void PuTongMoShi_Tab4_Button_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 2;
            Tab_change_fore(4, 2);
        }

        private void LieBiaoMoShi_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 5;
            Tab_change_fore(4, 5);
        }

        /// <summary>
        /// 显示编号，检测气体， 安装位置，安装时间，高限报警，低限报警
        /// </summary>
        public void Show_Information_Jiedian(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + index.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            if(temp_DataRow.Count <= 0)//没有相应节点的信息，就将下面的数据设置成默认值
            {
                JianCeQiTi.Text = "默认值";
                AnZhuangWeiZhi.Text = "默认值";
                AnZhuangShiJina.Text = DateTime.Now.ToString();//默认为当前时间
                GaoXianBaoJing.Text = "默认值";
                DiXianBaoJing.Text = "默认值";
            }
            else
            {
                JianCeQiTi.Text = temp_DataRow[0][1].ToString();
                AnZhuangWeiZhi.Text = temp_DataRow[0][2].ToString();
                AnZhuangShiJina.Text = temp_DataRow[0][3].ToString();
                GaoXianBaoJing.Text = temp_DataRow[0][4].ToString();
                DiXianBaoJing.Text = temp_DataRow[0][5].ToString();
            }
        }

        /// <summary>
        /// 更改或新增节点信息。编号，检测气体， 安装位置，安装时间，高限报警，低限报警
        /// </summary>
        public void Update_Information_Jiedian(Ellipse[] Ellipse_Array_tt, int index, string jianceqiti_tt, string anzhuangweizhi_tt, string anzhuangshijian_tt, string gaoxianbaojing_tt, string dixianbaojing_tt)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + index.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            if (temp_DataRow.Count <= 0)//没有相应节点的信息，建立新的节点
            {
                MessageBox.Show("未在已录入的范围内", "提示");
            }
            else//将已有的节点信息更改
            {
                Point point_temp = Ellipse_Array_tt[index - 1].TranslatePoint(new Point(0, 0), img_tab4);
                string UpdataCommand_str = "UPDATE Table3_JieDian SET `gas type` = '" + jianceqiti_tt +
                    "', location = '" + anzhuangweizhi_tt + "', `time of install` = '" + anzhuangshijian_tt +
                    "', `high to warning` = '" + gaoxianbaojing_tt + "', `low to warning` = '" + dixianbaojing_tt +
                    "', `xmin` = '" + point_temp.X.ToString() + "', `ymin` = '" + point_temp.Y.ToString() +
                    "' WHERE id = " + index;
                MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, UpdataCommand_str, null);
            }
        }


        public void jiedian_AutoZoom_tab4(ref Ellipse[] ellipse_array, int index)//从零开始索引
        {
            double x = Canvas.GetLeft(ellipse_array[index]);
            double y = Canvas.GetTop(ellipse_array[index]);

            double FangDaBeiShu = 2;

            Point centerPoint = new Point(x, y);
            Point pt = img_tab4.RenderTransform.Inverse.Transform(centerPoint);

            this.tlt_tab4.X = (centerPoint.X - pt.X) * this.sfr_tab4.ScaleX;
            this.tlt_tab4.Y = (centerPoint.Y - pt.Y) * this.sfr_tab4.ScaleY;
            this.sfr_tab4.CenterX = centerPoint.X;
            this.sfr_tab4.CenterY = centerPoint.Y;
            this.sfr_tab4.ScaleX += FangDaBeiShu;
            this.sfr_tab4.ScaleY += FangDaBeiShu;


            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                double x_temp = Canvas.GetLeft(ellipse_array[i]);
                double y_temp = Canvas.GetTop(ellipse_array[i]);

                Point centerPoint2 = new Point((x - x_temp), (y - y_temp));
                Point pt2 = Ellipse_Array_tab4[i].RenderTransform.Inverse.Transform(centerPoint2);
                translateTransform_Array_tab4[i].X = (centerPoint2.X - pt2.X) * scaleTransform_Array_tab4[i].ScaleX;
                translateTransform_Array_tab4[i].Y = (centerPoint2.Y - pt2.Y) * scaleTransform_Array_tab4[i].ScaleY;
                scaleTransform_Array_tab4[i].CenterX = centerPoint2.X;
                scaleTransform_Array_tab4[i].CenterY = centerPoint2.Y;
                scaleTransform_Array_tab4[i].ScaleX += FangDaBeiShu;
                scaleTransform_Array_tab4[i].ScaleY += FangDaBeiShu;

            }
        }

        public void jiedian_AutoMove_tab4(ref Ellipse[] ellipse_array, int index)//从零开始索引
        {
            map_Reset_Click_tab4(this, null);

            double x = Canvas.GetLeft(ellipse_array[index]) - map_rightup_X;
            double y = Canvas.GetTop(ellipse_array[index]) - map_rightup_Y;

            double move_x = (this.Width) * 0.75 / 2 - x;// (img.Height / 2 - x) * sfr.ScaleX;
            double move_y = (this.Height) / 2 - y;// (img.Width / 2 - y) * sfr.ScaleY;


            tlt_tab4.X += move_x;
            tlt_tab4.Y += move_y;


            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                translateTransform_Array_tab4[i].X += move_x * scaleTransform_Array_tab4[i].ScaleX;
                translateTransform_Array_tab4[i].Y += move_y * scaleTransform_Array_tab4[i].ScaleY;
            }
        }

        //这个只是用于测试，并无实际作用
        private void AutoZoom_Click_tab4(object sender, RoutedEventArgs e)
        {
            jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, 0);
            jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, 0);
        }

        public void Init_Ellipse_Array_tab4()
        {
            Ellipse_Array_tab4[0] = Ellipse1_tab4;
            Ellipse_Array_tab4[1] = Ellipse2_tab4;
            Ellipse_Array_tab4[2] = Ellipse3_tab4;
            Ellipse_Array_tab4[3] = Ellipse4_tab4;
            Ellipse_Array_tab4[4] = Ellipse5_tab4;
            Ellipse_Array_tab4[5] = Ellipse6_tab4;
            Ellipse_Array_tab4[6] = Ellipse7_tab4;
            Ellipse_Array_tab4[7] = Ellipse8_tab4;
            Ellipse_Array_tab4[8] = Ellipse9_tab4;
            Ellipse_Array_tab4[9] = Ellipse10_tab4;

            Ellipse_Array_tab4[10] = Ellipse11_tab4;
            Ellipse_Array_tab4[11] = Ellipse12_tab4;
            Ellipse_Array_tab4[12] = Ellipse13_tab4;
            Ellipse_Array_tab4[13] = Ellipse14_tab4;
            Ellipse_Array_tab4[14] = Ellipse15_tab4;
            Ellipse_Array_tab4[15] = Ellipse16_tab4;
            Ellipse_Array_tab4[16] = Ellipse17_tab4;
            Ellipse_Array_tab4[17] = Ellipse18_tab4;
            Ellipse_Array_tab4[18] = Ellipse19_tab4;
            Ellipse_Array_tab4[19] = Ellipse20_tab4;

            Ellipse_Array_tab4[20] = Ellipse21_tab4;
            Ellipse_Array_tab4[21] = Ellipse22_tab4;
            Ellipse_Array_tab4[22] = Ellipse23_tab4;
            Ellipse_Array_tab4[23] = Ellipse24_tab4;
            Ellipse_Array_tab4[24] = Ellipse25_tab4;
            Ellipse_Array_tab4[25] = Ellipse26_tab4;
            Ellipse_Array_tab4[26] = Ellipse27_tab4;
            Ellipse_Array_tab4[27] = Ellipse28_tab4;
            Ellipse_Array_tab4[28] = Ellipse29_tab4;
            Ellipse_Array_tab4[29] = Ellipse30_tab4;

            Ellipse_Array_tab4[30] = Ellipse31_tab4;
            Ellipse_Array_tab4[31] = Ellipse32_tab4;
            Ellipse_Array_tab4[32] = Ellipse33_tab4;
            Ellipse_Array_tab4[33] = Ellipse34_tab4;
            Ellipse_Array_tab4[34] = Ellipse35_tab4;
            Ellipse_Array_tab4[35] = Ellipse36_tab4;
            Ellipse_Array_tab4[36] = Ellipse37_tab4;
            Ellipse_Array_tab4[37] = Ellipse38_tab4;
            Ellipse_Array_tab4[38] = Ellipse39_tab4;
            Ellipse_Array_tab4[39] = Ellipse40_tab4;

            Ellipse_Array_tab4[40] = Ellipse41_tab4;
            Ellipse_Array_tab4[41] = Ellipse42_tab4;
            Ellipse_Array_tab4[42] = Ellipse43_tab4;
            Ellipse_Array_tab4[43] = Ellipse44_tab4;
            Ellipse_Array_tab4[44] = Ellipse45_tab4;
            Ellipse_Array_tab4[45] = Ellipse46_tab4;
            Ellipse_Array_tab4[46] = Ellipse47_tab4;
            Ellipse_Array_tab4[47] = Ellipse48_tab4;
            Ellipse_Array_tab4[48] = Ellipse49_tab4;
            Ellipse_Array_tab4[49] = Ellipse50_tab4;

            Ellipse_Array_tab4[50] = Ellipse51_tab4;
            Ellipse_Array_tab4[51] = Ellipse52_tab4;
            Ellipse_Array_tab4[52] = Ellipse53_tab4;
            Ellipse_Array_tab4[53] = Ellipse54_tab4;
            Ellipse_Array_tab4[54] = Ellipse55_tab4;
            Ellipse_Array_tab4[55] = Ellipse56_tab4;
            Ellipse_Array_tab4[56] = Ellipse57_tab4;
            Ellipse_Array_tab4[57] = Ellipse58_tab4;
            Ellipse_Array_tab4[58] = Ellipse59_tab4;
            Ellipse_Array_tab4[59] = Ellipse60_tab4;

            Ellipse_Array_tab4[60] = Ellipse61_tab4;
            Ellipse_Array_tab4[61] = Ellipse62_tab4;
            Ellipse_Array_tab4[62] = Ellipse63_tab4;
            Ellipse_Array_tab4[63] = Ellipse64_tab4;
        }

        public void Init_translateTransform_Array_tab4()
        {
            translateTransform_Array_tab4[0] = tlt1_tab4;
            translateTransform_Array_tab4[1] = tlt2_tab4;
            translateTransform_Array_tab4[2] = tlt3_tab4;
            translateTransform_Array_tab4[3] = tlt4_tab4;
            translateTransform_Array_tab4[4] = tlt5_tab4;
            translateTransform_Array_tab4[5] = tlt6_tab4;
            translateTransform_Array_tab4[6] = tlt7_tab4;
            translateTransform_Array_tab4[7] = tlt8_tab4;
            translateTransform_Array_tab4[8] = tlt9_tab4;
            translateTransform_Array_tab4[9] = tlt10_tab4;

            translateTransform_Array_tab4[10] = tlt11_tab4;
            translateTransform_Array_tab4[11] = tlt12_tab4;
            translateTransform_Array_tab4[12] = tlt13_tab4;
            translateTransform_Array_tab4[13] = tlt14_tab4;
            translateTransform_Array_tab4[14] = tlt15_tab4;
            translateTransform_Array_tab4[15] = tlt16_tab4;
            translateTransform_Array_tab4[16] = tlt17_tab4;
            translateTransform_Array_tab4[17] = tlt18_tab4;
            translateTransform_Array_tab4[18] = tlt19_tab4;
            translateTransform_Array_tab4[19] = tlt20_tab4;

            translateTransform_Array_tab4[20] = tlt21_tab4;
            translateTransform_Array_tab4[21] = tlt22_tab4;
            translateTransform_Array_tab4[22] = tlt23_tab4;
            translateTransform_Array_tab4[23] = tlt24_tab4;
            translateTransform_Array_tab4[24] = tlt25_tab4;
            translateTransform_Array_tab4[25] = tlt26_tab4;
            translateTransform_Array_tab4[26] = tlt27_tab4;
            translateTransform_Array_tab4[27] = tlt28_tab4;
            translateTransform_Array_tab4[28] = tlt29_tab4;
            translateTransform_Array_tab4[29] = tlt30_tab4;

            translateTransform_Array_tab4[30] = tlt31_tab4;
            translateTransform_Array_tab4[31] = tlt32_tab4;
            translateTransform_Array_tab4[32] = tlt33_tab4;
            translateTransform_Array_tab4[33] = tlt34_tab4;
            translateTransform_Array_tab4[34] = tlt35_tab4;
            translateTransform_Array_tab4[35] = tlt36_tab4;
            translateTransform_Array_tab4[36] = tlt37_tab4;
            translateTransform_Array_tab4[37] = tlt38_tab4;
            translateTransform_Array_tab4[38] = tlt39_tab4;
            translateTransform_Array_tab4[39] = tlt40_tab4;

            translateTransform_Array_tab4[40] = tlt41_tab4;
            translateTransform_Array_tab4[41] = tlt42_tab4;
            translateTransform_Array_tab4[42] = tlt43_tab4;
            translateTransform_Array_tab4[43] = tlt44_tab4;
            translateTransform_Array_tab4[44] = tlt45_tab4;
            translateTransform_Array_tab4[45] = tlt46_tab4;
            translateTransform_Array_tab4[46] = tlt47_tab4;
            translateTransform_Array_tab4[47] = tlt48_tab4;
            translateTransform_Array_tab4[48] = tlt49_tab4;
            translateTransform_Array_tab4[49] = tlt50_tab4;

            translateTransform_Array_tab4[50] = tlt51_tab4;
            translateTransform_Array_tab4[51] = tlt52_tab4;
            translateTransform_Array_tab4[52] = tlt53_tab4;
            translateTransform_Array_tab4[53] = tlt54_tab4;
            translateTransform_Array_tab4[54] = tlt55_tab4;
            translateTransform_Array_tab4[55] = tlt56_tab4;
            translateTransform_Array_tab4[56] = tlt57_tab4;
            translateTransform_Array_tab4[57] = tlt58_tab4;
            translateTransform_Array_tab4[58] = tlt59_tab4;
            translateTransform_Array_tab4[59] = tlt60_tab4;

            translateTransform_Array_tab4[60] = tlt61_tab4;
            translateTransform_Array_tab4[61] = tlt62_tab4;
            translateTransform_Array_tab4[62] = tlt63_tab4;
            translateTransform_Array_tab4[63] = tlt64_tab4;
        }

        public void Init_scaleTransform_Array_tab4()
        {
            scaleTransform_Array_tab4[0] = sfr1_tab4;
            scaleTransform_Array_tab4[1] = sfr2_tab4;
            scaleTransform_Array_tab4[2] = sfr3_tab4;
            scaleTransform_Array_tab4[3] = sfr4_tab4;
            scaleTransform_Array_tab4[4] = sfr5_tab4;
            scaleTransform_Array_tab4[5] = sfr6_tab4;
            scaleTransform_Array_tab4[6] = sfr7_tab4;
            scaleTransform_Array_tab4[7] = sfr8_tab4;
            scaleTransform_Array_tab4[8] = sfr9_tab4;
            scaleTransform_Array_tab4[9] = sfr10_tab4;

            scaleTransform_Array_tab4[10] = sfr11_tab4;
            scaleTransform_Array_tab4[11] = sfr12_tab4;
            scaleTransform_Array_tab4[12] = sfr13_tab4;
            scaleTransform_Array_tab4[13] = sfr14_tab4;
            scaleTransform_Array_tab4[14] = sfr15_tab4;
            scaleTransform_Array_tab4[15] = sfr16_tab4;
            scaleTransform_Array_tab4[16] = sfr17_tab4;
            scaleTransform_Array_tab4[17] = sfr18_tab4;
            scaleTransform_Array_tab4[18] = sfr19_tab4;
            scaleTransform_Array_tab4[19] = sfr20_tab4;

            scaleTransform_Array_tab4[20] = sfr21_tab4;
            scaleTransform_Array_tab4[21] = sfr22_tab4;
            scaleTransform_Array_tab4[22] = sfr23_tab4;
            scaleTransform_Array_tab4[23] = sfr24_tab4;
            scaleTransform_Array_tab4[24] = sfr25_tab4;
            scaleTransform_Array_tab4[25] = sfr26_tab4;
            scaleTransform_Array_tab4[26] = sfr27_tab4;
            scaleTransform_Array_tab4[27] = sfr28_tab4;
            scaleTransform_Array_tab4[28] = sfr29_tab4;
            scaleTransform_Array_tab4[29] = sfr30_tab4;

            scaleTransform_Array_tab4[30] = sfr31_tab4;
            scaleTransform_Array_tab4[31] = sfr32_tab4;
            scaleTransform_Array_tab4[32] = sfr33_tab4;
            scaleTransform_Array_tab4[33] = sfr34_tab4;
            scaleTransform_Array_tab4[34] = sfr35_tab4;
            scaleTransform_Array_tab4[35] = sfr36_tab4;
            scaleTransform_Array_tab4[36] = sfr37_tab4;
            scaleTransform_Array_tab4[37] = sfr38_tab4;
            scaleTransform_Array_tab4[38] = sfr39_tab4;
            scaleTransform_Array_tab4[39] = sfr40_tab4;

            scaleTransform_Array_tab4[40] = sfr41_tab4;
            scaleTransform_Array_tab4[41] = sfr42_tab4;
            scaleTransform_Array_tab4[42] = sfr43_tab4;
            scaleTransform_Array_tab4[43] = sfr44_tab4;
            scaleTransform_Array_tab4[44] = sfr45_tab4;
            scaleTransform_Array_tab4[45] = sfr46_tab4;
            scaleTransform_Array_tab4[46] = sfr47_tab4;
            scaleTransform_Array_tab4[47] = sfr48_tab4;
            scaleTransform_Array_tab4[48] = sfr49_tab4;
            scaleTransform_Array_tab4[49] = sfr50_tab4;

            scaleTransform_Array_tab4[50] = sfr51_tab4;
            scaleTransform_Array_tab4[51] = sfr52_tab4;
            scaleTransform_Array_tab4[52] = sfr53_tab4;
            scaleTransform_Array_tab4[53] = sfr54_tab4;
            scaleTransform_Array_tab4[54] = sfr55_tab4;
            scaleTransform_Array_tab4[55] = sfr56_tab4;
            scaleTransform_Array_tab4[56] = sfr57_tab4;
            scaleTransform_Array_tab4[57] = sfr58_tab4;
            scaleTransform_Array_tab4[58] = sfr59_tab4;
            scaleTransform_Array_tab4[59] = sfr60_tab4;

            scaleTransform_Array_tab4[60] = sfr61_tab4;
            scaleTransform_Array_tab4[61] = sfr62_tab4;
            scaleTransform_Array_tab4[62] = sfr63_tab4;
            scaleTransform_Array_tab4[63] = sfr64_tab4;
        }

        #region//地图功能的实现
        private void img_MouseDown_tab4(object sender, MouseButtonEventArgs e)
        {
            //previousMousePoint_tab4 = e.GetPosition(img_tab4);

            


            if (sender.ToString() == "System.Windows.Shapes.Ellipse")
            {
                isMouseLeftButtonDown_tab4 = true;//只有在节点处按下按键才算按下
                previousMousePoint_tab4 = e.GetPosition((System.Windows.Shapes.Ellipse)sender);
            }
            else
            {
                previousMousePoint_tab4 = e.GetPosition(img_tab4);
            }
        }

        private void img_MouseUp_tab4(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown_tab4 = false;
        }

        private void img_MouseLeave_tab4(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown_tab4 == false)
                return;
            if (sender.ToString() == "System.Windows.Shapes.Ellipse")
            {
                Point position_tab4;
                System.Windows.Shapes.Ellipse ellipse = (System.Windows.Shapes.Ellipse)sender;
                position_tab4 = e.GetPosition(ellipse);
                //单独拖拽节点只会更改节点的在Canvas中的相对位置（left top），不会更改其他变量
                Canvas.SetLeft(ellipse, Canvas.GetLeft(ellipse) + (position_tab4.X - this.previousMousePoint_tab4.X) * sfr_tab4.ScaleX);
                Canvas.SetTop(ellipse, Canvas.GetTop(ellipse) + (position_tab4.Y - this.previousMousePoint_tab4.Y) * sfr_tab4.ScaleY);
            }
        }

        private void img_MouseMove_tab4(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown_tab4 == true)
            {
                if (sender.ToString() == "System.Windows.Shapes.Ellipse")
                {
                    System.Windows.Shapes.Ellipse Ellipse_temp = (System.Windows.Shapes.Ellipse)sender;

                    Point position_tab4;

                    for (int i = 0; i < size_chanel; i++)
                    {
                        if (Ellipse_Array_tab4[i].Name == Ellipse_temp.Name)
                        {
                            position_tab4 = e.GetPosition(Ellipse_Array_tab4[i]);
                            //单独拖拽节点只会更改节点的在Canvas中的相对位置（left top），不会更改其他变量
                            Canvas.SetLeft(Ellipse_Array_tab4[i], Canvas.GetLeft(Ellipse_Array_tab4[i]) + (position_tab4.X - this.previousMousePoint_tab4.X) * scaleTransform_Array_tab4[i].ScaleX);
                            Canvas.SetTop(Ellipse_Array_tab4[i], Canvas.GetTop(Ellipse_Array_tab4[i]) + (position_tab4.Y - this.previousMousePoint_tab4.Y) * scaleTransform_Array_tab4[i].ScaleY);
                            //System.Diagnostics.Debug.WriteLine("GetLeft {0} GetTop {1}", Canvas.GetLeft(rectangle_Array[i]), Canvas.GetTop(rectangle_Array[i]));此处显示改变
                            //System.Diagnostics.Debug.WriteLine("translateTransform_Array {0} {1}", translateTransform_Array[i].X, translateTransform_Array[i].Y);此处显示不变
                            break;
                        }
                    }
                }

            }
        }

        private void img_MouseWheel_tab4(object sender, MouseWheelEventArgs e)
        {
            if (sfr_tab4.ScaleX < 0.2 && sfr_tab4.ScaleY < 0.2 && e.Delta < 0)
            {
                return;
            }
            Point centerPoint = e.GetPosition(img_tab4);
            Point pt = img_tab4.RenderTransform.Inverse.Transform(centerPoint);
            this.tlt_tab4.X = (centerPoint.X - pt.X) * this.sfr_tab4.ScaleX;
            this.tlt_tab4.Y = (centerPoint.Y - pt.Y) * this.sfr_tab4.ScaleY;
            this.sfr_tab4.CenterX = centerPoint.X;
            this.sfr_tab4.CenterY = centerPoint.Y;
            this.sfr_tab4.ScaleX += e.Delta / 1000.0;
            this.sfr_tab4.ScaleY += e.Delta / 1000.0;


            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                Point centerPoint2 = e.GetPosition(Ellipse_Array_tab4[i]);
                Point pt2 = Ellipse_Array_tab4[i].RenderTransform.Inverse.Transform(centerPoint2);


                translateTransform_Array_tab4[i].X = (centerPoint2.X - pt2.X) * scaleTransform_Array_tab4[i].ScaleX;
                translateTransform_Array_tab4[i].Y = (centerPoint2.Y - pt2.Y) * scaleTransform_Array_tab4[i].ScaleY;
                scaleTransform_Array_tab4[i].CenterX = centerPoint2.X;
                scaleTransform_Array_tab4[i].CenterY = centerPoint2.Y;
                scaleTransform_Array_tab4[i].ScaleX += e.Delta / 1000.0;
                scaleTransform_Array_tab4[i].ScaleY += e.Delta / 1000.0;
            }
        }
        #endregion

        private void map_Reset_Click_tab4(object sender, RoutedEventArgs e)
        {
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 4);
        }

        public void clear_img_canvas_tab4()
        {
            Canvas.SetTop(img_tab4, 0);
            Canvas.SetLeft(img_tab4, 0);
        }

        public void clear_scale_tab4()
        {
            this.sfr_tab4.CenterX = 0;
            this.sfr_tab4.CenterY = 0;
            this.sfr_tab4.ScaleX = 1;
            this.sfr_tab4.ScaleY = 1;
            for (int i = 0; i < size_chanel; i++)
            {
                scaleTransform_Array_tab4[i].CenterX = 0;
                scaleTransform_Array_tab4[i].CenterY = 0;
                scaleTransform_Array_tab4[i].ScaleX = 1;
                scaleTransform_Array_tab4[i].ScaleY = 1;
            }
        }

        public void clear_tlt_tab4()
        {
            this.tlt_tab4.X = 0;
            this.tlt_tab4.Y = 0;
            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                translateTransform_Array_tab4[i].X = 0;
                translateTransform_Array_tab4[i].Y = 0;
            }
        }



        //private void DataGrid_SelectedCellsChanged_tab4(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    jiedian a = (jiedian)this.DataGrid_tab4.SelectedItem;

        //    jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, (Convert.ToUInt16(a.ID) - 1));
        //    jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, (Convert.ToUInt16(a.ID) - 1));
        //}

        private void hid_jiedian_Click_tab4(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            Ellipse_Array_tab4[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Hidden;
        }

        private void show_jiedian_Click_tab4(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            Ellipse_Array_tab4[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Visible;
        }
    }
}