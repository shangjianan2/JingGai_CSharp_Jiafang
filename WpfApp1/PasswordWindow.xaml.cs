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
using System.Windows.Shapes;

using MySQL_Funtion;

namespace WpfApp1
{
    /// <summary>
    /// PasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordWindow : Window
    {
        MainWindow mainWindow = null;
        public int module_index = 0;



        public PasswordWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow_tt">用于调用变量</param>
        /// <param name="module_index">选择对话框模式</param>
        public PasswordWindow(MainWindow mainWindow_tt, int module_index_tt)
        {
            InitializeComponent();
            mainWindow = mainWindow_tt;
            module_index = module_index_tt;
        }

        /// <summary>
        /// 确认密码时需要获取DialogResult的值判断密码是否正确
        /// 更改密码时不需要DialogResult
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Passwd_Button_Click(object sender, EventArgs e)
        {
            if (Passwd_TextBox.Password == "")
                return;
            
            if(module_index == 0)
            {
                if (mainWindow.passwd_str == Passwd_TextBox.Password)
                {
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
            else if(module_index == 1)
            {
                if (MessageBox.Show("确认更改密码？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    this.DialogResult = true;
                    return;
                }

                string command_str = "update " + mainWindow.ShuJuKu .Table2_YongHu + " set `password`=\"" + Passwd_TextBox.Password + "\" where `name`=\"root\";";
                MySqlHelper.GetDataSet("Database='" + mainWindow.ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", System.Data.CommandType.Text, command_str, null);

                this.DialogResult = true;
            }
        }

        private void Passwd_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Passwd_Button_Click(sender, e);
            }
        }
    }
}
