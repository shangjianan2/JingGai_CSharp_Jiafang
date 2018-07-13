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
        //JieDians os_tab4 = null;

        Point previousMousePoint_tab4 = new Point(0, 0);
        private bool isMouseLeftButtonDown_tab4 = false;

        const double jiedian_size = 10;
        
        List<Ellipse> ellipse_list_tab4 = new List<Ellipse>();

        

        private void Verify_PassWord_Tab4_Button_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow passwd_Form2 = new PasswordWindow(this, 1);
            passwd_Form2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            passwd_Form2.Owner = this;
            passwd_Form2.ShowDialog();

            if (passwd_Form2.DialogResult == false)
                return;
        }

        private void XinXiBianGeng_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(true, false, false, 4);
        }

        private void ZengShan_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(false, true, true, 4);
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

            //删除相应节点
            Remove_JieDian_on_Map(BianHao.Text, ref ellipse_list_tab2, canvas_mine);
            Remove_JieDian_on_Map(BianHao.Text, ref ellipse_list_tab4, canvas_mine_tab4);

            //刷新所有列表
            Init_ImageList(ref imageListLarge);//初始化imagelist
            Init_LieBiao(listview_largeicon, ref imageListLarge);
            Init_LieBiao(listview_largeicon_tab5, ref imageListLarge);
        }

        private void ZengJiaBianGeng_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;

            //确认对话框
            if (System.Windows.MessageBox.Show("确定增加节点？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            bool newJiedian_or_not = ZengJiaBianGeng(Convert.ToInt16(BianHao.Text), JianCeQiTi.Text, AnZhuangWeiZhi.Text, AnZhuangShiJina.Text, GaoXianBaoJing.Text, DiXianBaoJing.Text);

            //刷新相关界面
            Init_JieDian_Map();
            Init_ImageList(ref imageListLarge);//初始化imagelist
            Init_LieBiao(listview_largeicon, ref imageListLarge);
            Init_LieBiao(listview_largeicon_tab5, ref imageListLarge);

            //判断现有所有节点是否掉线
            //DiaoXian();
        }

        /// <summary>
        /// 添加节点，对数据库进行操作，不涉及界面的显示
        /// </summary>
        /// <param name="index"></param>
        /// <param name="jianceqiti_tt"></param>
        /// <param name="anzhuangweizhi_tt"></param>
        /// <param name="anzhuangshijian_tt"></param>
        /// <param name="gaoxianbaojing_tt"></param>
        /// <param name="dixianbaojing_tt"></param>
        public bool ZengJiaBianGeng(int index, string jianceqiti_tt, string anzhuangweizhi_tt, string anzhuangshijian_tt, string gaoxianbaojing_tt, string dixianbaojing_tt)
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
                    return false;//如果数据库中已经有这个节点的id的时候，直接跳出,并返回false
                }
            }

            //能运行到这里说明这个节点使个新的，然后将新的数据输入到数据库
            string command_str = "INSERT INTO " + ShuJuKu.Table3_JieDian + " (`id`, `gas type`, `location`, `time of install`, `high to warning`, `low to warning`, `xmin`, `ymin` ) VALUES ( \"" +
                index.ToString() + "\",  \"" + jianceqiti_tt + "\", \"" + anzhuangweizhi_tt +
                "\", \"" + anzhuangshijian_tt + "\", \"" + gaoxianbaojing_tt + "\", \"" + dixianbaojing_tt + "\", \"" + map_rightup_X + "\", \"" + map_rightup_Y + "\");";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            return true;//这是个新的节点，已向数据库中新增新的节点信息

        }

        private void BianHao_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Show_Information_Jiedian(Convert.ToInt16(BianHao.Text));

                DataSet dataSet_temp = new DataSet();
                string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + BianHao.Text;
                dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                      CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

                if (temp_DataRow.Count <= 0)//没有相应节点的信息，建立新的节点
                {
                    MessageBox.Show("此节点尚未创建", "提示");
                    return;
                }

                jiedian_AutoMove_tab4(ref ellipse_list_tab4, Convert.ToInt16(BianHao.Text));
                jiedian_AutoZoom_tab4(ref ellipse_list_tab4, Convert.ToInt16(BianHao.Text));
            }
        }

        private void QueRenBianGeng_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//如果TextBox中没有数据就不进行任何操作
                return;
            Update_Information_Jiedian(ellipse_list_tab4, Convert.ToInt16(BianHao.Text), JianCeQiTi.Text, AnZhuangWeiZhi.Text, AnZhuangShiJina.Text, GaoXianBaoJing.Text, DiXianBaoJing.Text);
            //刷新相关界面
            Init_JieDian_Map();
            Init_ImageList(ref imageListLarge);//初始化imagelist
            Init_LieBiao(listview_largeicon, ref imageListLarge);
            Init_LieBiao(listview_largeicon_tab5, ref imageListLarge);
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
            Enable_verify_add_del(false, false, false, 5);
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
        public void Update_Information_Jiedian(List<Ellipse> Ellipse_Array_tt, int index, string jianceqiti_tt, string anzhuangweizhi_tt, string anzhuangshijian_tt, string gaoxianbaojing_tt, string dixianbaojing_tt)
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
                int i_temp = mysqlID_to_listID(Ellipse_Array_tt, index);
                Point point_temp = get_ellipse_XY(Ellipse_Array_tt[i_temp]);
                string UpdataCommand_str = "UPDATE Table3_JieDian SET `gas type` = '" + jianceqiti_tt +
                    "', location = '" + anzhuangweizhi_tt + "', `time of install` = '" + anzhuangshijian_tt +
                    "', `high to warning` = '" + gaoxianbaojing_tt + "', `low to warning` = '" + dixianbaojing_tt +
                    "', `xmin` = '" + point_temp.X.ToString() + "', `ymin` = '" + point_temp.Y.ToString() +
                    "' WHERE id = " + index.ToString();
                MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, UpdataCommand_str, null);
            }
        }

        /// <summary>
        /// 获取指定椭圆的坐标
        /// </summary>
        /// <param name="ellipse_tt"></param>
        /// <returns></returns>
        public Point get_ellipse_XY(Ellipse ellipse_tt)
        {
            return new Point(Canvas.GetLeft(ellipse_tt), Canvas.GetTop(ellipse_tt));
        }


        public void jiedian_AutoZoom_tab4(ref List<Ellipse> ellipse_array, int index)
        {
            int i_list = mysqlID_to_listID(ellipse_list_tab4, index);//从数据库id转换为列表id

            double x = Canvas.GetLeft(ellipse_array[i_list]);
            double y = Canvas.GetTop(ellipse_array[i_list]);

            double FangDaBeiShu = 2;

            Point centerPoint = new Point(x, y);

            this.sfr_tab4.CenterX = centerPoint.X;
            this.sfr_tab4.CenterY = centerPoint.Y;
            this.sfr_tab4.ScaleX = FangDaBeiShu;
            this.sfr_tab4.ScaleY = FangDaBeiShu;
            
        }

        public void jiedian_AutoMove_tab4(ref List<Ellipse> ellipse_array, int index)//从零开始索引
        {
            map_Reset_Click_tab4(this, null);
            
            double move_x = 0;
            double move_y = 0;

            int i_list = mysqlID_to_listID(ellipse_list_tab4, index);//从数据库id转换为列表id

            if (this.WindowState == WindowState.Maximized)
            {
                double x = Canvas.GetLeft(ellipse_array[i_list]) - map_rightup_X;
                double y = Canvas.GetTop(ellipse_array[i_list]) - map_rightup_Y;

                double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
                double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度

                move_x = x1 * 0.75 / 2 - x;// (img.Height / 2 - x) * sfr.ScaleX;
                move_y = (y1 - 100) / 2 - y;// (img.Width / 2 - y) * sfr.ScaleY;
            }
            else
            {
                double x = Canvas.GetLeft(ellipse_array[i_list]) - map_rightup_X;
                double y = Canvas.GetTop(ellipse_array[i_list]) - map_rightup_Y;

                move_x = (this.Width) * 0.75 / 2 - x;// (img.Height / 2 - x) * sfr.ScaleX;
                move_y = (this.Height - 100) / 2 - y;// (img.Width / 2 - y) * sfr.ScaleY;

            }


            tlt_tab4.X += move_x;
            tlt_tab4.Y += move_y;
            
        }

        //
        private void update_all_XY_tab4_Click(object sender, RoutedEventArgs e)
        {
            foreach(Ellipse mem in ellipse_list_tab4)
            {
                string jiedian_id_str_temp = extract_id_from_ToolTip(mem.ToolTip.ToString());
                string command_str = "update " + ShuJuKu.Table3_JieDian + " SET xmin = '" + Canvas.GetLeft(mem).ToString() + "', ymin = '" + Canvas.GetTop(mem).ToString() + "' WHERE id = " + jiedian_id_str_temp + ";";
                MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            }
            Init_JieDian_Map();//刷新所有节点
        }

        
        #region//地图功能的实现
        private void img_MouseDown_tab4(object sender, MouseButtonEventArgs e)
        {
            if (sender.ToString() == "System.Windows.Shapes.Ellipse")
            {
                isMouseLeftButtonDown_tab4 = true;//只有在节点处按下按键才算按下
                                                  //System.Windows.Shapes.Ellipse ellipse_tt = (System.Windows.Shapes.Ellipse)sender;

                BianHao.Text = extract_id_from_ToolTip(((Ellipse)sender).ToolTip.ToString());

                Show_Information_Jiedian(Convert.ToInt16(BianHao.Text));
            }
        }

        public string extract_id_from_ToolTip(string tooltip_tt)
        {
            string temp_str = tooltip_tt.Replace("点 位 号：", "");
            int index_first_eol = temp_str.IndexOf('\n');
            return temp_str.Remove(index_first_eol);
        }

        private void img_MouseUp_tab4(object sender, MouseButtonEventArgs e)
        {
            
            isMouseLeftButtonDown_tab4 = false;
            //System.Diagnostics.Debug.WriteLine(e.GetPosition(canvas_mine_tab4).X.ToString(), e.GetPosition(canvas_mine_tab4).Y.ToString());
        }

        public void follow_mouse(object sender, MouseEventArgs e)
        {
            Ellipse Ellipse_temp = (Ellipse)sender;

            double x_move = e.GetPosition(canvas_mine_tab4).X;
            double y_move = e.GetPosition(canvas_mine_tab4).Y;
            Canvas.SetLeft(Ellipse_temp, (x_move - (jiedian_size / 2)));
            Canvas.SetTop(Ellipse_temp, (y_move - (jiedian_size / 2)));
        }

        private void img_MouseLeave_tab4(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown_tab4 == false)
                return;
            if (sender.ToString() == "System.Windows.Shapes.Ellipse")
            {
                //System.Windows.Shapes.Ellipse Ellipse_temp = (System.Windows.Shapes.Ellipse)sender;

                follow_mouse(sender, e);
            }
        }

        private void img_MouseMove_tab4(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown_tab4 == true)
            {
                if (sender.ToString() == "System.Windows.Shapes.Ellipse")
                {
                    follow_mouse(sender, e);
                }

            }
        }

        private void img_MouseWheel_tab4(object sender, MouseWheelEventArgs e)
        {
            if (sfr_tab4.ScaleX < 0.2 && sfr_tab4.ScaleY < 0.2 && e.Delta < 0)
            {
                return;
            }
            Point centerPoint = e.GetPosition(canvas_mine_tab4);
            Point pt = canvas_mine_tab4.RenderTransform.Inverse.Transform(centerPoint);
            this.tlt_tab4.X = (centerPoint.X - pt.X) * this.sfr_tab4.ScaleX;
            this.tlt_tab4.Y = (centerPoint.Y - pt.Y) * this.sfr_tab4.ScaleY;
            this.sfr_tab4.CenterX = centerPoint.X;
            this.sfr_tab4.CenterY = centerPoint.Y;
            this.sfr_tab4.ScaleX += e.Delta / 1000.0;
            this.sfr_tab4.ScaleY += e.Delta / 1000.0;

            //保证ScaleX和ScaleY这两个值大于等于1
            if (this.sfr_tab4.ScaleX < 1 || this.sfr_tab4.ScaleY < 1)
            {
                this.sfr_tab4.ScaleX = 1;
                this.sfr_tab4.ScaleY = 1;
                
            }


            
        }
        #endregion

        private void map_Reset_Click_tab4(object sender, RoutedEventArgs e)
        {
            Init_JieDian_Map();//重新加载地图
        }

        //public void clear_img_canvas_tab4()
        //{
        //    Canvas.SetTop(img_tab4, 0);
        //    Canvas.SetLeft(img_tab4, 0);
        //}

        public void clear_scale_tab4()
        {
            this.sfr_tab4.CenterX = 0;
            this.sfr_tab4.CenterY = 0;
            this.sfr_tab4.ScaleX = 1;
            this.sfr_tab4.ScaleY = 1;
            
        }

        public void clear_tlt_tab4()
        {
            this.tlt_tab4.X = 0;
            this.tlt_tab4.Y = 0;
            //for (int i = 0; i < size_chanel; i++)//size_chanel
            //{
            //    translateTransform_Array_tab4[i].X = 0;
            //    translateTransform_Array_tab4[i].Y = 0;
            //}
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
            ellipse_list_tab4[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Hidden;
        }

        private void show_jiedian_Click_tab4(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            ellipse_list_tab4[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Visible;
        }
    }
}