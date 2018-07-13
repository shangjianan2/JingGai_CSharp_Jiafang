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
        public System.Windows.Forms.ToolTip toolTip_tab3 = new System.Windows.Forms.ToolTip();

        
        private void listview_largeicon_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ListViewItem lv = this.listview_largeicon.GetItemAt(e.X, e.Y);

            if (lv != null)
            {
                if (pointView.X != e.X || pointView.Y != e.Y)//防止闪烁

                {

                    string str_temp = lv.SubItems[0].Text.Replace("#", "");
                    int index_temp = mysqlID_to_listID(ellipse_list_tab4, Convert.ToInt16(str_temp));

                    toolTip_tab3.Show(ellipse_list_tab2[index_temp].ToolTip.ToString(), listview_largeicon, new System.Drawing.Point(e.X, e.Y), 10000);

                    pointView.X = e.X;

                    pointView.Y = e.Y;

                    toolTip_tab3.Active = true;

                }
            }
            else
            {
                toolTip_tab3.Hide(listview_largeicon);
            }
        }

        private void ChaXun_Tab3_Button_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 6;
            System.Windows.Controls.Button sender_Button = (System.Windows.Controls.Button)sender;

            if(sender_Button.Name.Contains("ab2"))//如果是Tab2
            {
                Tab_change_fore(2, 6);
            }
            else if(sender_Button.Name.Contains("ab3"))//如果是Tab3
            {
                Tab_change_fore(3, 6);
            }

            Display_CurrentTime_on_TextBox();
        }

    }

    
}
