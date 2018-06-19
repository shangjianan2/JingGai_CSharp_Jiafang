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
        private System.Drawing.Point pointView = new System.Drawing.Point(0, 0);
        public System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();

        ObservableCollection<jiedian> os_tab3 = null;

        public int size_datagrid = 10;

        
        public void Init_DataGrid_tab3()
        {
            os_tab3 = (ObservableCollection<jiedian>)DataGrid_tab3.ItemsSource;

            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from Table1_ShiJIan_JieDian order by date desc limit " + size_datagrid.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='NBIoT';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            for (int i = 0; i < size_datagrid; i++)
            {
                os_tab3.Add(new jiedian(temp_DataRow[i][0].ToString(), temp_DataRow[i][1].ToString(), temp_DataRow[i][2].ToString(),
                                   temp_DataRow[i][3].ToString(), temp_DataRow[i][4].ToString(), temp_DataRow[i][5].ToString(),
                                   temp_DataRow[i][6].ToString(), temp_DataRow[i][7].ToString(), temp_DataRow[i][8].ToString(),
                                   temp_DataRow[i][9].ToString(), temp_DataRow[i][10].ToString(), temp_DataRow[i][11].ToString()));
            }
        }

        private void listview_largeicon_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ListViewItem lv = this.listview_largeicon.GetItemAt(e.X, e.Y);

            if (lv != null)
            {
                if (pointView.X != e.X || pointView.Y != e.Y)//防止闪烁

                {

                    toolTip.Show(lv.SubItems[0].Text, listview_largeicon, new System.Drawing.Point(e.X, e.Y), 10000);

                    pointView.X = e.X;

                    pointView.Y = e.Y;

                    toolTip.Active = true;

                }
            }
            else
            {
                toolTip.Hide(listview_largeicon);
            }
        }
    }
    
    //public class JieDians_tab3 :
    //        ObservableCollection<jiedian>
    //{
    //    public JieDians_tab3()
    //    {
    //        Add(new jiedian("Jesper", "Aaberg", "1234567890", "Jesper", "Aaberg", "1234567890", "Jesper", "Aaberg", "1234567890", "Jesper", "Aaberg", "1234567890"));

    //    }
    //}
}
