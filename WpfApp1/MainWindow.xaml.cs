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
        public MainWindow()
        {
            InitializeComponent();

            tabcontrol.SelectedIndex = 2;//显示地图模式

            #region//udp通讯
            byte[] array_byte = new byte[4] { 192, 168, 1, 84 };
            mysql_Thread = new UDP_Communication(array_byte, 2333);
            //注册事件
            mysql_Thread.rev_New2 += new recNewMessage2(rec_NewMessage);
            mysql_Thread.recThread_Start();//开启类里的线程
            #endregion

            os = (ObservableCollection<jiedian>)DataGrid.ItemsSource;

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from Table1_ShiJIan_JieDian order by date desc limit " + size_DataGrid_Display.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='NBIoT';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for (int i = 0; i < size_DataGrid_Display; i++)
            {
                os.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }



            Init_Ellipse_Array();
            Init_translateTransform_Array();
            Init_scaleTransform_Array();

            for (int i = 0; i < size_chanel; i++)
            {
                update_tooltip(ref Ellipse_Array, i);
            }

            //初始化地图位置
            map_Reset_Click(this, null);


            //**************************************************tab3*******************8
            // Create two ImageList objects.
            ImageList imageListLarge = new ImageList();

            // Initialize the ImageList objects with bitmaps.
            for (int i = 0; i < 64; i++)
            {
                System.Drawing.Image image = Bitmap.FromFile(".\\jiedian.png");


                imageListLarge.Images.Add(image);
                imageListLarge.ImageSize = new System.Drawing.Size(100, 150);
            }



            this.listview_largeicon.View = View.LargeIcon;

            this.listview_largeicon.BackColor = System.Drawing.Color.FromArgb(255, 237, 237, 237);

            this.listview_largeicon.LargeImageList = imageListLarge;

            this.listview_largeicon.BeginUpdate();

            for (int i = 0; i < 64; i++)
            {
                System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();

                lvi.ImageIndex = i;

                lvi.Text = "item" + i;

                this.listview_largeicon.Items.Add(lvi);
            }

            this.listview_largeicon.EndUpdate();

            Init_DataGrid_tab3();
        }

        private void LieBiao_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 3;
        }

        private void DiTu_Tab3_Button_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 2;
        }
    }
}
