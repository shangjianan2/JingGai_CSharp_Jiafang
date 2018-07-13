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
        /// <summary>
        /// 从数据库中获取当前可用节点，并将其按照数据库中的信息进行绘制
        /// </summary>
        public void Init_JieDian_Map()
        {
            removeAll_jiedian_in_map(canvas_mine);
            removeAll_jiedian_in_map(canvas_mine_tab4);
            ellipse_list_tab2.Clear();
            ellipse_list_tab4.Clear();

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + ";";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                int jiedian_id = Convert.ToInt16(temp_DataRow[i][0]);//获取所更新的节点的id
                Draw_JieDian_on_Map(jiedian_id, Convert.ToDouble(temp_DataRow[i][6]), Convert.ToDouble(temp_DataRow[i][7]), ref ellipse_list_tab2, canvas_mine, false);
                Draw_JieDian_on_Map(jiedian_id, Convert.ToDouble(temp_DataRow[i][6]), Convert.ToDouble(temp_DataRow[i][7]), ref ellipse_list_tab4, canvas_mine_tab4, true);
            }
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 2);//将canvas整体移动
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 4);
        }

        public int mysqlID_to_listID(List<Ellipse> list_ellipse, int mysqlID)
        {
            for (int i = 0; i < list_ellipse.Count; i++)
            {
                if (Convert.ToInt16(extract_id_from_ToolTip(list_ellipse[i].ToolTip.ToString())) == mysqlID)
                {
                    return i;
                }
            }
            return -1;//不存在此节点
        }

        public int mysqlID_to_listID(System.Windows.Forms.ListView list_LargeIcon, int mysqlID)
        {
            for (int i = 0; i < list_LargeIcon.Items.Count; i++)
            {
                string temp_str = listview_largeicon.Items[i].Text.Replace("#", "");
                if (Convert.ToInt16(temp_str) == mysqlID)
                {
                    return i;
                }
            }
            return -1;//不存在此节点
        }

        public void removeAll_jiedian_in_map(Canvas canvas_tt)
        {
            canvas_tt.Children.RemoveRange(1, (canvas_tt.Children.Count - 1));
        }

        public void Draw_JieDian_on_Map(int jiedian_id, double x, double y, ref List<Ellipse> ellipse_list_tt, Canvas canvas_tt, bool shijian_or_not)
        {
            //在地图上绘制
            Ellipse ellipse = new Ellipse();
            ellipse.Width = jiedian_size;
            ellipse.Height = jiedian_size;
            ellipse.Fill = System.Windows.Media.Brushes.Blue;
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            canvas_tt.Children.Add(ellipse);

            //加入到列表中
            ellipse_list_tt.Add(ellipse);

            //添加事件
            if (shijian_or_not == true)
            {
                //现减再加时为了防止重复注册
                ellipse.MouseMove -= img_MouseMove_tab4;
                ellipse.MouseDown -= img_MouseDown_tab4;
                ellipse.MouseUp -= img_MouseUp_tab4;
                ellipse.MouseLeave -= img_MouseLeave_tab4;

                ellipse.MouseMove += img_MouseMove_tab4;
                ellipse.MouseDown += img_MouseDown_tab4;
                ellipse.MouseUp += img_MouseUp_tab4;
                ellipse.MouseLeave += img_MouseLeave_tab4;
            }

            //更新toolTip
            update_tooltip(ref ellipse, jiedian_id, false);
        }

        public void Remove_JieDian_on_Map(string jiedian_id, ref List<Ellipse> ellipse_list_tt, Canvas canvas_tt)
        {
            //遍历ellipse_list_tt中所有节点，查找是否是否有节点的id为jiedian_id
            foreach (Ellipse mem in ellipse_list_tt)
            {
                string jiedian_id_str = extract_id_from_ToolTip(mem.ToolTip.ToString());
                if (jiedian_id == jiedian_id_str)
                {
                    canvas_tt.Children.Remove(mem);
                    return;
                }
            }

            //如果有id等于jiedian_id的节点，将其从地图中删除
        }


        public void change_jiedian_status_DiTu(List<Ellipse> ellipse_list, int index_tt, int BaoJing)
        {
            int index = mysqlID_to_listID(ellipse_list, index_tt);
            if (BaoJing == 0)
            {
                ellipse_list[index].Fill = System.Windows.Media.Brushes.Blue;
            }
            else if (BaoJing == 1)//暂定为浓度超出界限
            {
                ellipse_list[index].Fill = System.Windows.Media.Brushes.Red;
            }
            else if (BaoJing == 2)//暂定为掉线，丢失
            {
                ellipse_list[index].Fill = System.Windows.Media.Brushes.Green;
            }

        }
    }
}
