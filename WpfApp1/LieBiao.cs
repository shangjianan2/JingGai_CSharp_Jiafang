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
                listView_tt.LargeImageList.Images.Add(Bitmap.FromFile("jiedian.png"));
            }

            listView_tt.EndUpdate();//更新结束
        }

        public void Init_ImageList(ref ImageList imageList)
        {
            for (int i = 0; i < size_chanel; i++)
            {
                System.Drawing.Image image = Bitmap.FromFile(".\\jiedian.png");


                imageList.Images.Add(image);
                imageList.ImageSize = new System.Drawing.Size(50, 75);
            }
        }

        public void change_jiedian_status_LieBiao(System.Windows.Forms.ListView listView_tt, int index_tt, int BaoJing)
        {
            int index = mysqlID_to_listID(listView_tt, index_tt);

            if (BaoJing == 0)
            {
                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if (BaoJing == 1)//暂定为浓度超出界限
            {

                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian_warning.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }
            else if (BaoJing == 2)//暂定为掉线，丢失
            {
                listView_tt.BeginUpdate();
                listView_tt.LargeImageList.Images[index + 1] = Bitmap.FromFile("jiedian_warning_diaoxian.png");//这个列表的表头貌似是从1开始索引
                listView_tt.EndUpdate();
            }

        }
    }
}
