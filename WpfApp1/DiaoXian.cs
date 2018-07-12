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
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>掉线true 没掉线false</returns>
        public bool jiedian_diaoxian_or_not(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `id` from " + ShuJuKu.Table1_ShiJIna_JieDian + " where to_days(now())-to_days(date) < 3 and id=" + index.ToString() + " order by date desc limit 1";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列
            return (temp_DataRow.Count == 0) ? true : false;//判断三天内是否有数据
        }

        public List<int> get_exit_jiedian_id_list()
        {
            List<int> id_list = new List<int>();
            DataSet dataSet_temp = new DataSet();
            string command_str = "select `id` from " + ShuJuKu.Table3_JieDian + ";";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for(int i = 0; i < temp_DataRow.Count; i++)
            {
                id_list.Add(Convert.ToInt16(temp_DataRow[i][0]));
            }

            return id_list;
        }

        public void DiaoXian()
        {
            List<int> ids_list = get_exit_jiedian_id_list();
            foreach(int mem in ids_list)
            {
                if(jiedian_diaoxian_or_not(mem))
                {
                    //update_tooltip(ref Ellipse_Array, (mem - 1), true);
                    update_tooltip(ref Ellipse_Array_tab4, (mem - 1), true);

                    /////////
                    int temp_index = (mem - 1);
                    //change_jiedian_status(ref Ellipse_Array, listview_largeicon, (temp_index - 1), 2);
                    change_jiedian_status(ref Ellipse_Array_tab4, listview_largeicon_tab5, (temp_index - 1), 2);
                }
            }
        }
    }
}
