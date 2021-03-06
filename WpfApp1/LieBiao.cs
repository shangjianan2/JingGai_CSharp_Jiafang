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


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private void listview_largeicon_tab5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listview_largeicon_tab5.SelectedItems.Count == 0)//注意：切换选中项的以瞬间会出现没有选中项的情况，切换选中项的时候，第一步撤销旧有选中项（此时出现没有选中项的情况），第二部添加新的选中项
                return;

            BianHao_tab5.Text = listview_largeicon_tab5.SelectedItems[0].Text.Replace("#", "");
            Show_Information_Jiedian_tab5(Convert.ToInt16(BianHao_tab5.Text));
        }

        public void Init_LieBiao(System.Windows.Forms.ListView listView_tt, ref ImageList imageList_tt)
        {
            //初始化listview
            listView_tt.View = View.LargeIcon;

            listView_tt.BackColor = System.Drawing.Color.FromArgb(255, 237, 237, 237);

            listView_tt.LargeImageList = imageList_tt;

            listView_tt.Items.Clear();


            List<int> list_int = get_exit_jiedian_id_list();
            
            DataSet dataSet_temp = new DataSet();


            listView_tt.BeginUpdate();//开是更新listview

            foreach (int index in list_int)
            {
                //从数据获取数据
                string command_str = "select * from " + ShuJuKu.Table1_ShiJIna_JieDian + " where id = " + index.ToString() + " order by date desc limit 1";
                dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

                //绘制节点
                System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();
                lvi.ImageIndex = Convert.ToInt16(temp_DataRow[0][0]);
                lvi.Text = lvi.ImageIndex.ToString() + "#";
                listView_tt.Items.Add(lvi);
                //listView_tt.LargeImageList.Images.Add(Bitmap.FromFile("jiedian.png"));
            }

            listView_tt.EndUpdate();//更新结束
        }

        public void Init_ImageList(ref ImageList imageList)
        {
            imageList.Images.Clear();
            imageList.ImageSize = new System.Drawing.Size(50, 75);

            for (int i = 0; i < size_chanel; i++)
            {
                System.Drawing.Image image = Bitmap.FromFile(".\\jiedian_green.png");


                imageList.Images.Add(image);
                //imageList.ImageSize = new System.Drawing.Size(50, 75);
            }
        }

        public void change_jiedian_status_LieBiao(System.Windows.Forms.ListView listView_tt, int index_tt, int BaoJing)
        {

            if (BaoJing == 0)//正常
            {
                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index_tt] = Bitmap.FromFile("jiedian_green.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if (BaoJing == 1)//报警
            {

                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index_tt] = Bitmap.FromFile("jiedian_red.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if (BaoJing == 2)//故障
            {
                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index_tt] = Bitmap.FromFile("jiedian_yellow.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if (BaoJing == 3)//掉线
            {
                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index_tt] = Bitmap.FromFile("jiedian_gray.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }

        }
    }
}
