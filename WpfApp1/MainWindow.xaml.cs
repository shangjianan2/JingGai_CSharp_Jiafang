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

            os = (ObservableCollection<jiedian>)DataGrid.ItemsSource;

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " order by date desc limit " + size_DataGrid_Display.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for (int i = 0; i < size_DataGrid_Display; i++)
            {
                os.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
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


            //**************************************************tab3*******************8
            // Create two ImageList objects.
            

            // Initialize the ImageList objects with bitmaps.
            for (int i = 0; i < size_chanel; i++)
            {
                System.Drawing.Image image = Bitmap.FromFile(".\\jiedian.png");


                imageListLarge.Images.Add(image);
                imageListLarge.ImageSize = new System.Drawing.Size(50, 75);
            }



            this.listview_largeicon.View = View.LargeIcon;

            this.listview_largeicon.BackColor = System.Drawing.Color.FromArgb(255, 237, 237, 237);

            this.listview_largeicon.LargeImageList = imageListLarge;

            this.listview_largeicon.BeginUpdate();

            for (int i = 0; i < size_chanel; i++)
            {
                System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();

                lvi.ImageIndex = i;

                lvi.Text = (i + 1).ToString() + "#";

                this.listview_largeicon.Items.Add(lvi);
            }

            this.listview_largeicon.EndUpdate();

            //Init_DataGrid_tab3();
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


            string[] temp_array_str = ShuJuJieXi(message);
            string str = "INSERT INTO " + ShuJuKu.Table1_ShiJIna_JieDian + " ( `id`, `name`, `type`, `gas type`, `DanWei`,`status`, `NongDu`, `DiXian`, `GaoXian`, `DianLiang`, `WenDu`, `Date` ) " +
        "VALUES ( \"" + (message[0]).ToString() + "\",\"2\",\"3\",\"" + temp_array_str[0] + "\",\"" + temp_array_str[1] + "\",\"" + temp_array_str[2] + "\",\"" + temp_array_str[3] + "\",\"" + temp_array_str[4] + "\",\"" + temp_array_str[5] + "\",\"" + temp_array_str[6] + "\",\"" + temp_array_str[7] + "\",now());";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                    CommandType.Text, str, null);

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

                os.Insert(0, new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
                                   temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
                                   temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
                                   temp_DataRow[0][9].ToString(), temp_DataRow[0][10].ToString(), temp_DataRow[0][11].ToString()));

                //更新ToolTip
                update_tooltip(ref Ellipse_Array, (message[0] - 1));

                /////////
                if(Convert.ToDouble(temp_DataRow[0][10]) > 30.0)
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (message[0] - 1), true);
                }
                else
                {
                    change_jiedian_status(ref Ellipse_Array, listview_largeicon, (message[0] - 1), false);
                }
            };
            this.Dispatcher.Invoke(action, true);
        }
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

            ellipse_array[index].ToolTip = "点位号      ：" + temp_DataRow[0][0].ToString() + "\n" +
                                           "实际位置    ：" + temp_DataRow[0][1].ToString() + "\n" +
                                           "实时浓度    ：" + temp_DataRow[0][2].ToString() + "\n" +
                                           "实时电量    ：" + temp_DataRow[0][3].ToString() + "\n" +
                                           "环境温度    ：" + temp_DataRow[0][10].ToString() + "\n" +
                                           "水位信息    ：" + temp_DataRow[0][5].ToString() + "\n" +
                                           "井盖信息    ：" + temp_DataRow[0][6].ToString() + "\n" +
                                           "数据更新时间：" + temp_DataRow[0][11].ToString();
        }

        public void change_jiedian_status(ref Ellipse[] ellipse_array, System.Windows.Forms.ListView listView_tt,  int index, bool BaoJing)
        {
            if(BaoJing == true)
            {
                ellipse_array[index].Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                

                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index] = Bitmap.FromFile("jiedian_warning.png");
                listView_tt.EndUpdate();
            }
            else
            {
                ellipse_array[index].Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));

                

                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index] = Bitmap.FromFile("jiedian.png");
                listView_tt.EndUpdate();
            }
            
        }

        private void LieBiao_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 3;
        }

        private void DiTu_Tab3_Button_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 2;
        }

        private void XiTong_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 4;
        }


        //增删点位
        private void TuiChu_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 2;
        }
    }
}
