﻿using System;
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

using System.Collections.ObjectModel;

using System.ComponentModel;

using MySQL_Funtion;
using System.Data;
using UDP_Thread;
using System.Net;

using System.Windows.Forms;
using System.Drawing;


using MySQL_PeiZhiWenJian_JieXi;
using Map_PeiZhiWenJian_JieXi;

using DianXinPingTai;

using System.Runtime.InteropServices;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public byte tab0_tab1_or_not = 0;//6种状态0 1 2 3 4 5，分别代表着tab0的三秒，tab2的9秒,5代表结束tab0 tab1

        ImageList imageListLarge = new ImageList();
        ImageList imageListLarge_tab5 = new ImageList();

        public mysql_PZWJ_JieXi ShuJuKu = null;//在处定义，但是是在Test_Enviroment中初始化的
        //public UDP_Communication mysql_Thread = null;
        public DianXinPingTai_Communication mysql_Thread = null;

        public byte[] Local_IP_Byte_Array = new byte[4];
        public UInt16 Local_DuanKou;
        public byte[] NBIoT_IP_Byte_Array = new byte[4];
        public UInt16 NBIoT_DuanKou;

        //
        public int[,] JieDianZuoBiao_Array_int = new int[size_chanel, 2];
        public string map_LuJing = null;

        public const int DiaoXian_ShiJian = 3;//掉线时间的判定天数

        const double map_rightup_X = 1500;
        const double map_rightup_Y = 1500;

        public string passwd_str = "1";

        SolidColorBrush NongDu_BaoJing_Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
        SolidColorBrush ZhengChang_Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
        SolidColorBrush QiTa_BaoJing_Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));

        //蜂鸣器报警函数
        [DllImport("user32.dll")]
        public static extern int MessageBeep(uint uType);

        //掉线检测时间计时器
        int DiaoXian_timer = 0;//在中断服务函数中用于记录“检测掉线状态”的定时器

        public MainWindow()
        {
            InitializeComponent();
            
            
            Init_QiDongJianCe(ref ShuJuKu);

            //建立与电信平台的连接，初始化某些信息（这些信息对于判断获取到的数据应该进行怎样的操作有很大的作用）
            Init_DianXinPingTai();
            
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "config/timg.jpg", UriKind.RelativeOrAbsolute));
            img_tab4.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "config/timg.jpg", UriKind.RelativeOrAbsolute));

            if (tab0_tab1_or_not == 0)//如果显示tab0和tab1
            {
                tabcontrol.SelectedIndex = 0;//显示tab2
            }
            else
            {
                tabcontrol.SelectedIndex = 2;//显示tab0
            }
            
            os = (JieDians)DataGrid.ItemsSource;

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " order by date desc limit " + size_DataGrid_Display.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for (int i = 0; i < size_DataGrid_Display; i++)
            {
                os.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   ((DateTime)temp_DataRow[i][9]).ToString("yyyy-MM-dd HH:mm:ss")));
            }



            update_map_liebiao();

            //开启定时器
            SendToIoT = new System.Threading.Timer(new System.Threading.TimerCallback(SendToIoTCall_1), this, 3000, 3000);

            update_jiedians_DiaoXian();
        }

        public void Init_DianXinPingTai()
        {
            //读取数据库中table1_shijian_jiedian中的最新时间
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `Date` from table1_shijian_jiedian order by `date` desc limit 1;";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            mysql_Thread = new DianXinPingTai_Communication((DateTime)temp_DataRow[0][0]);
            mysql_Thread.rev_New += rec_NewMessage_str;
            mysql_Thread.Start_Thread();
        }

        public void update_map_liebiao()
        {
            Init_JieDian_Map();


            Init_ImageList(ref imageListLarge);//初始化imagelist
            Init_LieBiao(listview_largeicon, ref imageListLarge);
            Init_ImageList(ref imageListLarge_tab5);
            Init_LieBiao(listview_largeicon_tab5, ref imageListLarge_tab5);

            //更新所有节点的报警状态
            List<int> temp_int_list = get_exit_jiedian_id_list();
            foreach (int mem in temp_int_list)
            {
                Update_BaoJingStatus(mem);
            }
        }

        public void Init_map_location_XY(double X, double Y, int tab)
        {
            if(tab == 2)
            {
                //clear_img_canvas();
                clear_scale();
                clear_tlt();

                tlt.X = -X;
                tlt.Y = -Y;
                
            }
            else if(tab == 4)
            {
                //clear_img_canvas_tab4();
                clear_scale_tab4();
                clear_tlt_tab4();

                tlt_tab4.X = -X;
                tlt_tab4.Y = -Y;
                
            }
        }

        /// <summary>
        /// 从数据库中获取所有信息，重绘节点
        /// 判断所有节点是否显示，并判断是否让其处于报警状态
        /// </summary>
        public void Init_Jiedian_DisplayOrNot()
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select id, xmin, ymin from " + ShuJuKu.Table3_JieDian;
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            
            //clear_img_canvas_tab4();
            clear_scale_tab4();
            clear_tlt_tab4();
            clear_img_canvas();
            clear_scale();
            clear_tlt();
            

            for(int i = 0; i < temp_DataRow.Count; i++)
            {
                int temp_index = Convert.ToInt16(temp_DataRow[i][0]);
                //if (GaoDiXian_BaiJing_PanDuan(temp_index) == 1 || GaoDiXian_BaiJing_PanDuan(temp_index) == 2)
                //{
                //    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 1);
                //    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 1);
                //}
                //else if(GaoDiXian_BaiJing_PanDuan(temp_index) == 3)
                //{
                //    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 2);
                //    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 2);
                //}
                //else
                //{
                //    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 0);
                //    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 0);
                //}
            }

            //添加地图偏置
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 2);
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 4);
        }
        
        

        public void Init_LargeIcon_ListView(System.Windows.Forms.ListView listView_tt)
        {
            listView_tt.View = View.LargeIcon;

            listView_tt.BackColor = System.Drawing.Color.FromArgb(255, 237, 237, 237);

            listView_tt.LargeImageList = imageListLarge;

            listView_tt.BeginUpdate();

            for (int i = 0; i < size_chanel; i++)
            {
                System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();

                lvi.ImageIndex = i;

                lvi.Text = (i + 1).ToString() + "#";

                listView_tt.Items.Add(lvi);
            }

            listView_tt.EndUpdate();
        }

        #region//更新所有可用节点是否处于掉线状态
        /// <summary>
        /// 判断某个节点是否掉线
        /// </summary>
        /// <param name="index">数据库中的节点号</param>
        /// <returns>如果掉线返回true，否则false</returns>
        public bool jiedian_DiaoXian(int index)
        {
            DataSet dataSet_temp_action = new DataSet();
            string command_str_action = "select `Date` from " + ShuJuKu.Table1_ShiJIna_JieDian + " where id = " + index.ToString() + " order by date desc limit 1";
            dataSet_temp_action = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str_action, null);
            DataRowCollection temp_DataRow_action = dataSet_temp_action.Tables[0].Rows;//获取列

            DateTime dateTime = DateTime.Now.AddDays(-DiaoXian_ShiJian);
            
            return (DateTime.Compare((DateTime)temp_DataRow_action[0][0], dateTime) < 0) ? true : false;
        }

        public void update_jiedians_DiaoXian()
        {
            List<int> list_temp = get_exit_jiedian_id_list();

            foreach(int index in list_temp)
            {
                if (jiedian_DiaoXian(index))//如果确实掉线了，注意这里输入的是数据库中的节点号
                {
                    change_jiedian_status_DiTu(ellipse_list_tab2, index, 3);//注意这里输入的是数据库中的节点号，下同
                    change_jiedian_status_LieBiao(listview_largeicon, index, 3);

                    change_jiedian_status_DiTu(ellipse_list_tab4, index, 3);
                    change_jiedian_status_LieBiao(listview_largeicon_tab5, index, 3);

                    update_tooltip_to_DiaoXian(index);
                }
                else//如果没有掉线
                {
                    //从逻辑上来看，如果没有掉线，我应该不需要对显示界面进行任何修改

                    //change_jiedian_status_DiTu(ellipse_list_tab2, index, 0);
                    //change_jiedian_status_LieBiao(listview_largeicon, index, 0);

                    //change_jiedian_status_DiTu(ellipse_list_tab4, index, 0);
                    //change_jiedian_status_LieBiao(listview_largeicon_tab5, index, 0);

                    ////更新ToolTip
                    //update_tooltip(ref ellipse_list_tab2, index, false);
                    //update_tooltip(ref ellipse_list_tab4, index, false);
                }
            }
        }

        public void update_tooltip_to_DiaoXian(int index)
        {
            int index_jiemian = mysqlID_to_listID(ellipse_list_tab2, index);//将数据库中的节点转换为界面中的节点
            ellipse_list_tab2[index_jiemian].ToolTip = "点 位 号：" + index.ToString() + "\n" + "掉线\n";
            ellipse_list_tab4[index_jiemian].ToolTip = "点 位 号：" + index.ToString() + "\n" + "掉线\n";
        }

        #endregion

        #region//udp接收中断
        public void rec_NewMessage_str(string[] string_array)
        {//只有当电信平台中有新的数据的时候才会进入到这个函数中
            System.Diagnostics.Debug.WriteLine(string_array[0]);

            /************************判断此次数据是否有效（正常状态下每天传感器被激活一次，每次发送四组数据，这四组数据内容大体相同，所以只记录其中一组）*******************************/
            //如果此地址码所对应的节点的状态与上次的状态都是正常（这种无效数据通常是在节点状态正常的时候发生）
            if (string_array[6] == "0")
            {
                //从数据库中获取此地址码对应的节点的最新更新时间
                DataSet date_temp = new DataSet();
                string command_str = "select date from table1_shijian_jiedian where id=" + string_array[0] + " order by date desc limit 1;";
                date_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = date_temp.Tables[0].Rows;//获取列
                DateTime dateTime_shujuku = (DateTime)temp_DataRow[0][0];
                DateTime dateTime_string_array = DateTime.ParseExact(string_array[13], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

                if (temp_DataRow.Count > 0)//如果数据库中含有此地址码所对应节点的数据
                {
                    //如果此地址码所对应的数据在几分钟之内已经更新过
                    TimeSpan timeSpan_temp = dateTime_string_array - dateTime_shujuku;
                    if (timeSpan_temp.TotalMinutes < 1)//一分钟
                    {
                        //运行到这里说明数据无效，抛弃
                        return;
                    }
                }
                
            }

            string str = "INSERT INTO " + ShuJuKu.Table1_ShiJIna_JieDian + " ( `id`, `gas type`, `DanWei`,`status`, `NongDu`, `DiXian`, `GaoXian`, `DianLiang`, `WenDu`, `Date` ) " +
            "VALUES ( \"" + string_array[0] + "\",\"" + trans_gas_type(string_array[4]) + "\",\"" + trans_DanWei(string_array[5]) + "\",\"" + trans_status(string_array[6]) + "\",\"" + 
            trans_nongdu(string_array[7]) + "\",\"" + trans_nongdu(string_array[8]) + "\",\"" + trans_nongdu(string_array[9]) + "\",\"" + string_array[10] + "\",\"" + trans_wendu(string_array[11], string_array[12]) + "\",\"" + string_array[13] + "\");";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                    CommandType.Text, str, null);

            Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测，从数据库中重新获取数据
            {
                //更新DataGrid
                //update_DataGrid(ref os);
                os.add_size_DataGrid_Display(new jiedian(string_array[0], trans_gas_type(string_array[4]), trans_DanWei(string_array[5]), trans_status(string_array[6]), trans_nongdu(string_array[7]),
                    trans_nongdu(string_array[8]), trans_nongdu(string_array[9]), string_array[10], (string_array[11] + "." + string_array[12]), string_array[13]), size_DataGrid_Display);

                //更新ToolTip
                update_tooltip(ref ellipse_list_tab2, Convert.ToInt16(string_array[0]), false);
                update_tooltip(ref ellipse_list_tab4, Convert.ToInt16(string_array[0]), false);

                //更新报警状态
                Update_BaoJingStatus(Convert.ToInt16(string_array[0]));
            };
            this.Dispatcher.Invoke(action, true);

        }


        //public void rec_NewMessage(byte[] message, ref EndPoint endPoint_tt)
        //{
        //    string temp_str = System.Text.Encoding.ASCII.GetString(message);
        //    //if (temp_str == "[iotxx:ok]" || temp_str == "[iotxx:update]")//[iotxx:ok]
        //    if (temp_str.Contains("[iotxx:"))
        //    {
        //        System.Diagnostics.Debug.WriteLine("[iotxx:ok]");
        //        return;
        //    }

        //    //显示源地址和源端口
        //    System.Diagnostics.Debug.WriteLine(endPoint_tt.ToString());

        //    //检测数据中的节点id是否在数据库中存在
        //    if (ID_Exsit_or_Not(message[0]) == false)
        //        return;


        //    string[] temp_array_str = ShuJuJieXi(message);
        //    bool change_or_not = remember_to_databases_or_not(temp_array_str, message[0]);//有选择性的将数据输入到数据库
        //    if (change_or_not == false)//如果数据库没有更新，就不进行下面的运算
        //        return;

        ////    string str = "INSERT INTO " + ShuJuKu.Table1_ShiJIna_JieDian + " ( `id`, `gas type`, `DanWei`,`status`, `NongDu`, `DiXian`, `GaoXian`, `DianLiang`, `WenDu`, `Date` ) " +
        ////"VALUES ( \"" + (message[0]).ToString() + "\",\"" + temp_array_str[0] + "\",\"" + temp_array_str[1] + "\",\"" + temp_array_str[2] + "\",\"" + temp_array_str[3] + "\",\"" + temp_array_str[4] + "\",\"" + temp_array_str[5] + "\",\"" + temp_array_str[6] + "\",\"" + temp_array_str[7] + "\",now());";
        ////    MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
        ////                            CommandType.Text, str, null);

        //    Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
        //    {
        //        DataSet dataSet_temp = new DataSet();
        //        string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " order by date desc limit 1";
        //        dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
        //        DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

        //        //更新DataGrid
        //        //os.Add(new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
        //        //                   temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
        //        //                   temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
        //        //                   temp_DataRow[0][9].ToString(), temp_DataRow[0][10].ToString(), temp_DataRow[0][11].ToString()));

        //        os.add_size_DataGrid_Display(new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
        //                           temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
        //                           temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
        //                           temp_DataRow[0][9].ToString()), size_DataGrid_Display);

        //        //更新ToolTip
        //        update_tooltip(ref ellipse_list_tab2, (message[0]), false);
        //        update_tooltip(ref ellipse_list_tab4, (message[0]), false);

        //        //更新报警状态
        //        Update_BaoJingStatus(message[0]);
        //    };
        //    this.Dispatcher.Invoke(action, true);
        //}

        public void Update_BaoJingStatus(int index)
        {
            int temp_index = Convert.ToInt16(index);
            //if (GaoDiXian_BaiJing_PanDuan(temp_index) == 1 || GaoDiXian_BaiJing_PanDuan(temp_index) == 2)
            //{
            //    change_jiedian_status_DiTu(ellipse_list_tab2, temp_index, 1);
            //    change_jiedian_status_LieBiao(listview_largeicon, temp_index, 1);

            //    change_jiedian_status_DiTu(ellipse_list_tab4, temp_index, 1);
            //    change_jiedian_status_LieBiao(listview_largeicon_tab5, temp_index, 1);
            //}
            //else if (GaoDiXian_BaiJing_PanDuan(temp_index) == 3)
            //{
            //    change_jiedian_status_DiTu(ellipse_list_tab2, temp_index, 2);
            //    change_jiedian_status_LieBiao(listview_largeicon, temp_index, 2);

            //    change_jiedian_status_DiTu(ellipse_list_tab4, temp_index, 2);
            //    change_jiedian_status_LieBiao(listview_largeicon_tab5, temp_index, 2);
            //}
            //else
            //{
            //    change_jiedian_status_DiTu(ellipse_list_tab2, temp_index, 0);
            //    change_jiedian_status_LieBiao(listview_largeicon, temp_index, 0);

            //    change_jiedian_status_DiTu(ellipse_list_tab4, temp_index, 0);
            //    change_jiedian_status_LieBiao(listview_largeicon_tab5, temp_index, 0);
            //}

            if(GaoDiXian_BaiJing_PanDuan(temp_index) == 0)//正常
            {
                change_jiedian_status_DiTu(ellipse_list_tab2, temp_index, 0);
                change_jiedian_status_LieBiao(listview_largeicon, temp_index, 0);

                change_jiedian_status_DiTu(ellipse_list_tab4, temp_index, 0);
                change_jiedian_status_LieBiao(listview_largeicon_tab5, temp_index, 0);
            }
            else if(GaoDiXian_BaiJing_PanDuan(temp_index) == 1)//报警 高报 低报 有水 盗窃
            {
                change_jiedian_status_DiTu(ellipse_list_tab2, temp_index, 1);
                change_jiedian_status_LieBiao(listview_largeicon, temp_index, 1);

                change_jiedian_status_DiTu(ellipse_list_tab4, temp_index, 1);
                change_jiedian_status_LieBiao(listview_largeicon_tab5, temp_index, 1);
            }
            else if(GaoDiXian_BaiJing_PanDuan(temp_index) == 2)//故障 传感器故障 电池电压低
            {
                change_jiedian_status_DiTu(ellipse_list_tab2, temp_index, 2);
                change_jiedian_status_LieBiao(listview_largeicon, temp_index, 2);

                change_jiedian_status_DiTu(ellipse_list_tab4, temp_index, 2);
                change_jiedian_status_LieBiao(listview_largeicon_tab5, temp_index, 2);
            }
        }

        public bool remember_to_databases_or_not(string[] temp_array_str_tt, int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `status`, `date` from " + ShuJuKu.Table1_ShiJIna_JieDian + " where to_days(now())-to_days(date) < 1 and id=" + index.ToString() + " order by date desc limit 1";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            

            //如果有对于此节点来说今日已经有数据了，且本次数据和上次数据的状态相同，就不进行任何操作
            if (temp_DataRow.Count != 0 && temp_DataRow[0][0].ToString() == temp_array_str_tt[2])
                return false;

            string str = "INSERT INTO " + ShuJuKu.Table1_ShiJIna_JieDian + " ( `id`, `gas type`, `DanWei`,`status`, `NongDu`, `DiXian`, `GaoXian`, `DianLiang`, `WenDu`, `Date` ) " +
            "VALUES ( \"" + (index).ToString() + "\",\"" + temp_array_str_tt[0] + "\",\"" + temp_array_str_tt[1] + "\",\"" + temp_array_str_tt[2] + "\",\"" + temp_array_str_tt[3] + "\",\"" + temp_array_str_tt[4] + "\",\"" + temp_array_str_tt[5] + "\",\"" + temp_array_str_tt[6] + "\",\"" + temp_array_str_tt[7] + "\",now());";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                    CommandType.Text, str, null);
            return true;
        }
        #endregion

        #region//udp出错处理
        public void udp_warning_to_shutdown(Exception e)
        {
            System.Windows.MessageBox.Show("网络连接错误", "程序即将关闭");

            Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
            {
                System.Windows.Application.Current.Shutdown();//关闭程序
            };
            this.Dispatcher.Invoke(action, true);            
        }
        #endregion

        public bool ID_Exsit_or_Not(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `id` from " + ShuJuKu.Table3_JieDian + ";";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for(int i = 0; i < temp_DataRow.Count; i++)
            {
                if(Convert.ToInt16(temp_DataRow[i][0]) == index)
                {
                    return true;
                }
            }
            return false;
        }

        public int GaoDiXian_BaiJing_PanDuan(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `NongDu`, `GaoXian`, `DiXian`, `status` from " + ShuJuKu.Table1_ShiJIna_JieDian + " where `id`=" + index.ToString() + " order by date desc limit 1;";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            double NongDu = Convert.ToDouble(temp_DataRow[0][0]);
            double GaoXian = Convert.ToDouble(temp_DataRow[0][1]);
            double DiXian = Convert.ToDouble(temp_DataRow[0][2]);
            string Status = temp_DataRow[0][3].ToString();
            if(Status == "")//正常状态
            {
                return 0;
            }
            else if(Status.Contains("报") || Status.Contains("水") || Status.Contains("盗"))//高报 低报 有水 盗窃
            {
                return 1;
            }
            else if(Status.Contains("故") || Status.Contains("压"))//传感器故障 电池电压低
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        #region//有关地图加载
        //public void Init_Map(int num_tab)
        //{
        //    map_PZWJ_JieXi.get_JieDianZuoBiao("C:\\NBIoT\\map.txt", size_chanel, ref JieDianZuoBiao_Array_int);
        //    map_PZWJ_JieXi.get_DiTuLuJing("C:\\NBIoT\\map.txt", size_chanel, ref map_LuJing);

        //    if(num_tab == 2)//更新tab2中的地图
        //    {
        //        clear_img_canvas();//将地图的图片位置归零
        //        clear_tlt();//将每个矩形的tlt清零
        //        clear_scale();//将所有的放大倍数归零（具体是不是放大倍数我也不知道，反正就是将之前所有因为操作而更改的数据全部复位，其中放大倍数应该为1）


        //        //for (int i = 0; i < size_chanel; i++)
        //        //{
        //        //    change_XY_rectangle(Ellipse_Array[i], map_rightup_X, map_rightup_Y);
        //        //}
        //        img.Source = new BitmapImage(new Uri(map_LuJing));

        //    }
        //    else if(num_tab == 4)//更新他爸中的地图
        //    {
        //        //clear_img_canvas_tab4();//将地图的图片位置归零
        //        clear_tlt_tab4();//将每个矩形的tlt清零
        //        clear_scale_tab4();//将所有的放大倍数归零（具体是不是放大倍数我也不知道，反正就是将之前所有因为操作而更改的数据全部复位，其中放大倍数应该为1）


        //        for (int i = 0; i < size_chanel; i++)
        //        {
        //            change_XY_rectangle(ellipse_list_tab4[i], map_rightup_X, map_rightup_Y);
        //        }
        //        img_tab4.Source = new BitmapImage(new Uri(map_LuJing));
        //    }

        //}

        #region//导入地图的配置文件
        public void change_XY_rectangle(Ellipse rectangle_tt, double x, double y)
        {
            Canvas.SetLeft(rectangle_tt, x);
            Canvas.SetTop(rectangle_tt, y);
        }
        #endregion
        #endregion

        #region//定时器中断
        public void SendToIoTCall_1(object state)
        {
            //tab0的暂停效果
            if (tab0_tab1_or_not != 5)//如果等于5，就说明tab0和tab1已经完事了
            {
                Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
                {
                    switch (tab0_tab1_or_not)
                    {
                        case 0:
                            tab0_tab1_or_not = 5;
                            SecondPicture_tab1.Play();
                            tabcontrol.SelectedIndex = 1;
                            break;
                    }
                };
                this.Dispatcher.Invoke(action, true);
            }

            if(DiaoXian_timer <= 1200)//此定时器是三秒触发一次，所以此处的实际时间是一个小时左右
            {
                ++DiaoXian_timer;
            }
            else
            {
                DiaoXian_timer = 0;
                Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
                {
                    update_jiedians_DiaoXian();
                };
                this.Dispatcher.Invoke(action, true);
            }

            //节点报警声音            
            Action<bool> action_warning = (x) =>//每次都对当前所有节点进行一次监测
            {
                bool flag_warning = false;
                for (int i = 0; i < ellipse_list_tab2.Count; ++i)
                {
                    if (ellipse_list_tab2[i].Fill == System.Windows.Media.Brushes.Red || ellipse_list_tab2[i].Fill == System.Windows.Media.Brushes.Yellow)
                    {
                        flag_warning = true;
                        break;
                    }
                }
                if (flag_warning == true)
                {
                    MessageBeep((uint)0x00000010);
                }
            };
            this.Dispatcher.Invoke(action_warning, true);
            
        }
        public void SendToIoTCall(object state)
        {
            //tab0的暂停效果
            if(tab0_tab1_or_not != 5)//如果等于5，就说明tab0和tab1已经完事了
            {
                Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
                {
                    switch (tab0_tab1_or_not)
                    {
                        case 0:
                            tab0_tab1_or_not = 5;
                            SecondPicture_tab1.Play();
                            tabcontrol.SelectedIndex = 1;
                            break;
                    }
                };
                this.Dispatcher.Invoke(action, true);
            }           


            //string temp_str = "ep=J4JFAJUGYS3GGF7Z&pw=123456";
            //byte[] buff = System.Text.Encoding.ASCII.GetBytes(temp_str);

            //byte[] array_byte = new byte[4] { 115, 29, 240, 46 };//设定远程ip地址
            //IPAddress ip = new IPAddress(array_byte);
            //IPEndPoint lep = new IPEndPoint(ip, 6000);

            //mysql_Thread.newsock.Connect(lep);
            //mysql_Thread.newsock.Send(buff);

            //如果程序长期开启，会在每小时检测有无节点掉线
            if(DateTime.Now.ToString("mm:ss") == "00:00")
            {
                update_jiedians_DiaoXian();
            }
        }
        #endregion

        public void update_DataGrid(ref JieDians os_tt)
        {
            DataSet dataSet_temp_action = new DataSet();
            string command_str_action = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " order by date desc limit 1";
            dataSet_temp_action = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str_action, null);
            DataRowCollection temp_DataRow_action = dataSet_temp_action.Tables[0].Rows;//获取列

            os_tt.add_size_DataGrid_Display(new jiedian(temp_DataRow_action[0][0].ToString(), temp_DataRow_action[0][1].ToString(), temp_DataRow_action[0][2].ToString(),
                               temp_DataRow_action[0][3].ToString(), temp_DataRow_action[0][4].ToString(), temp_DataRow_action[0][5].ToString(),
                               temp_DataRow_action[0][6].ToString(), temp_DataRow_action[0][7].ToString(), temp_DataRow_action[0][8].ToString(),
                               temp_DataRow_action[0][9].ToString()), size_DataGrid_Display);
        }

        public void update_tooltip(ref List<Ellipse> ellipse_array, int index_tt, bool diaoxian)
        {
            int index = mysqlID_to_listID(ellipse_array, index_tt);

            if (index == -1)
                return;//说明不存在这个节点

            if (diaoxian == true)
            {
                ellipse_array[index].ToolTip = "点 位 号：" + (index_tt).ToString() + "\n" + "掉线";
                return;
            }

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where id = " + (index_tt).ToString() + " order by date desc limit 1";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            if (temp_DataRow.Count <= 0)
                return;

            ellipse_array[index].ToolTip = "点 位 号：" + temp_DataRow[0][0].ToString() + "\n" +
                                           "气体类型：" + temp_DataRow[0][1].ToString() + "\n" +
                                           "单    位：" + temp_DataRow[0][2].ToString() + "\n" +
                                           "状    态：" + (temp_DataRow[0][3].ToString() == "" ? "正常" : temp_DataRow[0][3].ToString()) + "\n" +
                                           "气体浓度：" + temp_DataRow[0][4].ToString() + "\n" +
                                           "低限浓度：" + temp_DataRow[0][5].ToString() + "\n" +
                                           "高限浓度：" + temp_DataRow[0][6].ToString() + "\n" +
                                           "电    量：" + temp_DataRow[0][7].ToString() + "%\n" +
                                           "温    度：" + temp_DataRow[0][8].ToString() + "\n" +
                                           "更新时间：" + temp_DataRow[0][9].ToString();
        }

        public void update_tooltip(ref Ellipse ellipse, int jiedian_id, bool diaoxian)
        {
            if (diaoxian == true)
            {
                ellipse.ToolTip = "点 位 号：" + jiedian_id.ToString() + "\n" + "掉线";
                return;
            }

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where id = " + jiedian_id.ToString() + " order by date desc limit 1";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            if (temp_DataRow.Count <= 0)
                return;

            ellipse.ToolTip = "点 位 号：" + temp_DataRow[0][0].ToString() + "\n" +
                                           "气体类型：" + temp_DataRow[0][1].ToString() + "\n" +
                                           "单    位：" + temp_DataRow[0][2].ToString() + "\n" +
                                           "状    态：" + (temp_DataRow[0][3].ToString() == "" ? "正常" : temp_DataRow[0][3].ToString()) + "\n" +
                                           "气体浓度：" + temp_DataRow[0][4].ToString() + "\n" +
                                           "低限浓度：" + temp_DataRow[0][5].ToString() + "\n" +
                                           "高限浓度：" + temp_DataRow[0][6].ToString() + "\n" +
                                           "电    量：" + temp_DataRow[0][7].ToString() + "%\n" +
                                           "温    度：" + temp_DataRow[0][8].ToString() + "\n" +
                                           "更新时间：" + temp_DataRow[0][9].ToString();
        }

        //public void change_jiedian_status(ref List<Ellipse> ellipse_array, System.Windows.Forms.ListView listView_tt,  int index, int BaoJing)
        //{
        //    if(BaoJing == 0)
        //    {
        //        ellipse_array[index].Fill = ZhengChang_Color;
                

        //        listView_tt.BeginUpdate();
        //        listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian.png");//这个列表的表头貌似是从1开始索引
        //        listView_tt.EndUpdate();
        //    }
        //    else if(BaoJing == 1)//暂定为浓度超出界限
        //    {
        //        ellipse_array[index].Fill = NongDu_BaoJing_Color;

                

        //        listView_tt.BeginUpdate();
        //        listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian_warning.png");//这个列表的表头貌似是从1开始索引
        //        listView_tt.EndUpdate();
        //    }
        //    else if(BaoJing == 2)//暂定为掉线，丢失
        //    {
        //        ellipse_array[index].Fill = QiTa_BaoJing_Color;



        //        listView_tt.BeginUpdate();
        //        listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian_warning_diaoxian.png");//这个列表的表头貌似是从1开始索引
        //        listView_tt.EndUpdate();
        //    }
            
        //}

        private void LieBiao_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 3;
            Tab_change_fore(2, 3);
        }

        private void DiTu_Tab3_Button_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 2;
            Tab_change_fore(3, 2);
        }

        private void XiTong_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            passwd_str = get_password_from_databases("root");//从数据库中获取相应用户的密码并返回

            PasswordWindow passwd_Form2 = new PasswordWindow(this, 0);
            passwd_Form2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            passwd_Form2.Owner = this;
            passwd_Form2.ShowDialog();
                        
            if (passwd_Form2.DialogResult == false)
                return;


            System.Windows.Controls.Button sender_Button = (System.Windows.Controls.Button)sender;
            if(sender_Button.Name.Contains("ab2"))//如果是Tab2的按键
            {
                Tab_change_fore(2, 4);

                //失能tab4右下防的三个按键
                Enable_verify_add_del(false, false, false, 4);
            }
            else if(sender_Button.Name.Contains("ab3"))//如果是Tab3的按键
            {
                Tab_change_fore(3, 5);

                //失能tab5右下防的三个按键
                Enable_verify_add_del(false, false, false, 5);
            }
        }

        public string get_password_from_databases(string name)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `password` from table2_yonghu where name=\"" + name + "\";";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            return temp_DataRow[0][0].ToString();
        }

        /// <summary>
        /// 使能或是使能tab4和tab5右下的按键
        /// </summary>
        /// <param name="queren">确认变更 按键</param>
        /// <param name="zengjia">增加点位 按键</param>
        /// <param name="shanchu">删除点位 按键</param>
        public void Enable_verify_add_del(bool queren, bool zengjia, bool shanchu, int tab)
        {
            if(tab == 4)
            {
                QueRenBianGeng_Button.IsEnabled = queren;
                ZengJiaBianGeng_Button.IsEnabled = zengjia;
                ShanChuDianWei.IsEnabled = shanchu;
            }
            else if(tab == 5)
            {
                QueRenBianGeng_Button_tab5.IsEnabled = queren;
                ZengJiaBianGeng_Button_tab5.IsEnabled = zengjia;
                ShanChuDianWei_Button_tab5.IsEnabled = shanchu;
            }
        }


        //增删点位
        private void TuiChu_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 2;
        }

        
    }
}
