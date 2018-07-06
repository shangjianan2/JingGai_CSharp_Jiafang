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


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageList imageListLarge = new ImageList();

        public mysql_PZWJ_JieXi ShuJuKu = null;//在处定义，但是是在Test_Enviroment中初始化的
        public UDP_Communication mysql_Thread = null;
        public byte[] Local_IP_Byte_Array = new byte[4];
        public UInt16 Local_DuanKou;
        public byte[] NBIoT_IP_Byte_Array = new byte[4];
        public UInt16 NBIoT_DuanKou;

        //
        public int[,] JieDianZuoBiao_Array_int = new int[size_chanel, 2];
        public string map_LuJing = null;

        const double map_rightup_X = 1500;
        const double map_rightup_Y = 1500;

        public string passwd_str = "1";

        public MainWindow()
        {
            InitializeComponent();

            Init_QiDongJianCe(ref ShuJuKu, ref mysql_Thread, ref Local_IP_Byte_Array, ref Local_DuanKou, 
                                                             ref NBIoT_IP_Byte_Array, ref NBIoT_DuanKou);

            

            tabcontrol.SelectedIndex = 2;//显示地图模式

            

            #region//udp通讯
            //byte[] array_byte = new byte[4] { 192, 168, 1, 84 };
            //mysql_Thread = new UDP_Communication(array_byte, 2333);
            ////注册事件
            //mysql_Thread.rev_New2 += new recNewMessage2(rec_NewMessage);
            //mysql_Thread.recThread_Start();//开启类里的线程
            #endregion

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
                                   temp_DataRow[i][9].ToString()));
            }



            //Tab2地图初始化
            Init_Ellipse_Array();
            Init_translateTransform_Array();
            Init_scaleTransform_Array();

            for (int i = 0; i < size_chanel; i++)
            {
                update_tooltip(ref Ellipse_Array, i);
            }

            //Tab4地图初始化
            Init_Ellipse_Array_tab4();
            Init_translateTransform_Array_tab4();
            Init_scaleTransform_Array_tab4();

            for (int i = 0; i < size_chanel; i++)
            {
                update_tooltip(ref Ellipse_Array_tab4, i);
            }


            //初始化地图位置
            map_Reset_Click(this, null);

            //加载地图配置文件
            Init_Map(2);//更新tab2中的地图
            Init_Map(4);//更新tab4中的地图


            //**************************************************tab3*******************8
            // Create two ImageList objects.


            // Initialize the ImageList objects with bitmaps.
            for (int i = 0; i < size_chanel; i++)
            {
                System.Drawing.Image image = Bitmap.FromFile(".\\jiedian.png");


                imageListLarge.Images.Add(image);
                imageListLarge.ImageSize = new System.Drawing.Size(50, 75);
            }



            //Init_LargeIcon_ListView(listview_largeicon);
            //Init_LargeIcon_ListView(listview_largeicon_tab5);



            //Ellipse_Array[63].Visibility = Visibility.Collapsed;
            Init_Jiedian_DisplayOrNot();

            //每次重新绘制地图的时候都会有一定的偏置
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 2);
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 4);
        }

        public void Init_map_location_XY(double X, double Y, int tab)
        {
            if(tab == 2)
            {
                clear_img_canvas();
                clear_scale();
                clear_tlt();

                tlt.X = -X;
                tlt.Y = -Y;


                for (int i = 0; i < size_chanel; i++)//size_chanel
                {
                    translateTransform_Array_Tab3[i].X = -X * scaleTransform_Array_Tab3[i].ScaleX;
                    translateTransform_Array_Tab3[i].Y = -Y * scaleTransform_Array_Tab3[i].ScaleY;
                }
            }
            else if(tab == 4)
            {
                clear_img_canvas_tab4();
                clear_scale_tab4();
                clear_tlt_tab4();

                tlt_tab4.X = -X;
                tlt_tab4.Y = -Y;


                for (int i = 0; i < size_chanel; i++)//size_chanel
                {
                    translateTransform_Array_tab4[i].X = -X * scaleTransform_Array_tab4[i].ScaleX;
                    translateTransform_Array_tab4[i].Y = -Y * scaleTransform_Array_tab4[i].ScaleY;
                }
            }
        }

        /// <summary>
        /// 判断所有节点是否显示，并判断是否让其处于报警状态
        /// </summary>
        public void Init_Jiedian_DisplayOrNot()
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select id, xmin, ymin from " + ShuJuKu.Table3_JieDian;
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            
            clear_img_canvas_tab4();
            clear_scale_tab4();
            clear_tlt_tab4();
            clear_img_canvas();
            clear_scale();
            clear_tlt();


            HideOrShow_jiedian_map(temp_DataRow, ref Ellipse_Array);
            HideOrShow_jiedian_map(temp_DataRow, ref Ellipse_Array_tab4);
            HideOrShow_jiedian_liebiao(temp_DataRow, listview_largeicon);
            HideOrShow_jiedian_liebiao(temp_DataRow, listview_largeicon_tab5);

            for(int i = 0; i < temp_DataRow.Count; i++)
            {
                int temp_index = Convert.ToInt16(temp_DataRow[i][0]);
                if (GaoDiXian_BaiJing_PanDuan(temp_index) == 1 || GaoDiXian_BaiJing_PanDuan(temp_index) == 2)
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 1);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 1);
                }
                else if(GaoDiXian_BaiJing_PanDuan(temp_index) == 3)
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 2);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 2);
                }
                else
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 0);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 0);
                }
            }

            //添加地图偏置
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 2);
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 4);
        }


        /// <summary>
        /// 无论数据库中的数据是否是顺序排列，都会检测出哪些节点应该显示，并按照数据库中的信息调整节点位置
        /// </summary>
        /// <param name="temp_DataRow_tt">
        /// 从数据库中获取的集合
        /// </param>
        public void HideOrShow_jiedian_map(DataRowCollection temp_DataRow_tt, ref Ellipse[] Ellipse_Array_tt)
        {
            //将所有节点隐藏
            foreach (Ellipse mem in Ellipse_Array_tt)
            {
                mem.Visibility = Visibility.Collapsed;
            }
            for(int i = 0; i < temp_DataRow_tt.Count; i++)
            {
                int index_tempj = Convert.ToInt16(temp_DataRow_tt[i][0]) - 1;
                Ellipse_Array_tt[index_tempj].Visibility = Visibility.Visible;

                change_XY_rectangle(Ellipse_Array_tt[index_tempj], 
                                    Convert.ToDouble(temp_DataRow_tt[i][1]), 
                                    Convert.ToDouble(temp_DataRow_tt[i][2]));
            }
        }

        /// <summary>
        /// 根据数据库中的实际情况在列表模式下显示相应的节点
        /// </summary>
        /// <param name="temp_DataRow_tt">
        /// 从数据库中获取的集合
        /// </param>
        public void HideOrShow_jiedian_liebiao(DataRowCollection temp_DataRow_tt, System.Windows.Forms.ListView listView_tt)
        {
            listView_tt.View = View.LargeIcon;

            listView_tt.BackColor = System.Drawing.Color.FromArgb(255, 237, 237, 237);

            listView_tt.LargeImageList = imageListLarge;

            listView_tt.BeginUpdate();

            listView_tt.Items.Clear();

            for (int i = 0; i < temp_DataRow_tt.Count; i++)
            {
                System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();

                lvi.ImageIndex = Convert.ToInt16(temp_DataRow_tt[i][0]);
                lvi.Text = lvi.ImageIndex.ToString() + "#";
                listView_tt.Items.Add(lvi);
            }


            listView_tt.EndUpdate();
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

        #region//udp接收中断
        public void rec_NewMessage(byte[] message, ref EndPoint endPoint_tt)
        {
            string temp_str = System.Text.Encoding.ASCII.GetString(message);
            //if (temp_str == "[iotxx:ok]" || temp_str == "[iotxx:update]")//[iotxx:ok]
            if (temp_str.Contains("[iotxx:"))
            {
                System.Diagnostics.Debug.WriteLine("[iotxx:ok]");
                return;
            }

            //显示源地址和源端口
            System.Diagnostics.Debug.WriteLine(endPoint_tt.ToString());

            //检测数据中的节点id是否在数据库中存在
            if (ID_Exsit_or_Not(message[0]) == false)
                return;


            string[] temp_array_str = ShuJuJieXi(message);
            bool change_or_not = remember_to_databases_or_not(temp_array_str, message[0]);//有选择性的将数据输入到数据库
            if (change_or_not == false)//如果数据库没有更新，就不进行下面的运算
                return;

        //    string str = "INSERT INTO " + ShuJuKu.Table1_ShiJIna_JieDian + " ( `id`, `gas type`, `DanWei`,`status`, `NongDu`, `DiXian`, `GaoXian`, `DianLiang`, `WenDu`, `Date` ) " +
        //"VALUES ( \"" + (message[0]).ToString() + "\",\"" + temp_array_str[0] + "\",\"" + temp_array_str[1] + "\",\"" + temp_array_str[2] + "\",\"" + temp_array_str[3] + "\",\"" + temp_array_str[4] + "\",\"" + temp_array_str[5] + "\",\"" + temp_array_str[6] + "\",\"" + temp_array_str[7] + "\",now());";
        //    MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
        //                            CommandType.Text, str, null);

            Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
            {
                DataSet dataSet_temp = new DataSet();
                string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " order by date desc limit 1";
                dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

                //更新DataGrid
                //os.Add(new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
                //                   temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
                //                   temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
                //                   temp_DataRow[0][9].ToString(), temp_DataRow[0][10].ToString(), temp_DataRow[0][11].ToString()));

                os.add_size_DataGrid_Display(new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
                                   temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
                                   temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
                                   temp_DataRow[0][9].ToString()), size_DataGrid_Display);

                //更新ToolTip
                update_tooltip(ref Ellipse_Array, (message[0] - 1));

                /////////
                int temp_index = Convert.ToInt16(message[0]);
                if (GaoDiXian_BaiJing_PanDuan(temp_index) == 1 || GaoDiXian_BaiJing_PanDuan(temp_index) == 2)
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 1);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 1);
                }
                else if (GaoDiXian_BaiJing_PanDuan(temp_index) == 3)
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 2);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 2);
                }
                else
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 0);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 0);
                }
            };
            this.Dispatcher.Invoke(action, true);
        }

        public bool remember_to_databases_or_not(string[] temp_array_str_tt, int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `status`, `date` from table1_shijian_jiedian where to_days(now())-to_days(date) < 1 and id=" + index.ToString() + " order by date desc limit 1";
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
            if (NongDu > GaoXian)//如果大于高限程序返回1
            {
                return 1;
            }
            else if(NongDu < DiXian)//小于低限程序返回2
            {
                return 2;
            }
            else if(Status != "")
            {
                return 3;
            }
            else//如果浓度在正常范围之内程序返回0
            {
                return 0;
            }
        }

        #region//有关地图加载
        public void Init_Map(int num_tab)
        {
            map_PZWJ_JieXi.get_JieDianZuoBiao("C:\\NBIoT\\map.txt", size_chanel, ref JieDianZuoBiao_Array_int);
            map_PZWJ_JieXi.get_DiTuLuJing("C:\\NBIoT\\map.txt", size_chanel, ref map_LuJing);

            if(num_tab == 2)//更新tab2中的地图
            {
                clear_img_canvas();//将地图的图片位置归零
                clear_tlt();//将每个矩形的tlt清零
                clear_scale();//将所有的放大倍数归零（具体是不是放大倍数我也不知道，反正就是将之前所有因为操作而更改的数据全部复位，其中放大倍数应该为1）


                for (int i = 0; i < size_chanel; i++)
                {
                    change_XY_rectangle(Ellipse_Array[i], map_rightup_X, map_rightup_Y);
                }
                img.Source = new BitmapImage(new Uri(map_LuJing));

            }
            else if(num_tab == 4)//更新他爸中的地图
            {
                clear_img_canvas_tab4();//将地图的图片位置归零
                clear_tlt_tab4();//将每个矩形的tlt清零
                clear_scale_tab4();//将所有的放大倍数归零（具体是不是放大倍数我也不知道，反正就是将之前所有因为操作而更改的数据全部复位，其中放大倍数应该为1）


                for (int i = 0; i < size_chanel; i++)
                {
                    change_XY_rectangle(Ellipse_Array_tab4[i], map_rightup_X, map_rightup_Y);
                }
                img_tab4.Source = new BitmapImage(new Uri(map_LuJing));
            }

        }

        #region//导入地图的配置文件
        public void change_XY_rectangle(Ellipse rectangle_tt, double x, double y)
        {
            Canvas.SetLeft(rectangle_tt, x);
            Canvas.SetTop(rectangle_tt, y);
        }
        #endregion
        #endregion

        #region//定时器中断
        public void SendToIoTCall(object state)
        {
            string temp_str = "ep=J4JFAJUGYS3GGF7Z&pw=123456";
            byte[] buff = System.Text.Encoding.ASCII.GetBytes(temp_str);

            //byte[] array_byte = new byte[4] { 115, 29, 240, 46 };//设定远程ip地址
            //IPAddress ip = new IPAddress(array_byte);
            //IPEndPoint lep = new IPEndPoint(ip, 6000);

            //mysql_Thread.newsock.Connect(lep);
            mysql_Thread.newsock.Send(buff);

#if YanShi
            if(flag_Tab8 >= 2)
            {
                
            }
            else if(flag_Tab8 < 1)
            {
                flag_Tab8++;
            }
            else
            {
                Action<bool> action_tt = (x) =>
                {
                    tabcontrol.SelectedIndex = 0;//开启登陆界面
                };
                this.Dispatcher.Invoke(action_tt, true);
                
                flag_Tab8++;
            }
#endif
        }
        #endregion

        public void update_tooltip(ref Ellipse[] ellipse_array, int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where id = " + (index + 1).ToString() + " order by date desc limit 1";
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

        public void change_jiedian_status(ref Ellipse[] ellipse_array, System.Windows.Forms.ListView listView_tt,  int index, int BaoJing)
        {
            if(BaoJing == 0)
            {
                ellipse_array[index].Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
                

                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if(BaoJing == 1)//暂定为浓度超出界限
            {
                ellipse_array[index].Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));

                

                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian_warning.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if(BaoJing == 2)//暂定为掉线，丢失
            {
                ellipse_array[index].Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128));



                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian_warning_diaoxian.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            
        }

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

            PasswordWindow passwd_Form2 = new PasswordWindow(this);
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
