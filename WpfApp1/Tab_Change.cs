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

using Excel = Microsoft.Office.Interop.Excel;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public List<byte> Tab_back_list = new List<byte>();

        /// <summary>
        /// ǰ��������
        /// </summary>
        /// <param name="current_index">��ǰ��������</param>
        /// <param name="goto_index">��Ҫ����Ľ�������</param>
        public void Tab_change_fore(byte current_index, byte goto_index)
        {
            Tab_back_list.Add(current_index);
            tabcontrol.SelectedIndex = goto_index;
        }

        /// <summary>
        /// ���˷����ϴεĽ���
        /// </summary>
        public void Tab_change_back()
        {
            int len = Tab_back_list.Count;
            tabcontrol.SelectedIndex = Tab_back_list[len - 1];
            Tab_back_list.RemoveAt(len - 1);
        }

        /// <summary>
        /// ���˷����ϴεĽ��棬��ָ�ӹ���Ա������ת����ͨ�û�����
        /// </summary>
        public void Tab_change_RootToUser()
        {
            for(int i = 0; Tab_back_list.Count > 0; i++)
            {
                if(Tab_back_list[Tab_back_list.Count - 1] == 2 || Tab_back_list[Tab_back_list.Count - 1] == 3)
                {
                    tabcontrol.SelectedIndex = Tab_back_list[Tab_back_list.Count - 1];
                    Tab_back_list.RemoveAt(Tab_back_list.Count - 1);
                    return;
                }
                Tab_back_list.RemoveAt(Tab_back_list.Count - 1);
            }
        }
    }
}
