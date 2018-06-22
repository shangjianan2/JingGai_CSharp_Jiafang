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

using System.Data;
using MySQL_Funtion;

using UDP_Thread;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using IP_PeiZhiWenJian_JieXi;
using Map_PeiZhiWenJian_JieXi;
using MySQL_PeiZhiWenJian_JieXi;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private System.Threading.Timer SendToIoT = null;//如果不加定时器不断的给谷雨平台发送信息，有可能造成掉线

        IP_PZWJ_JieXi IP_WJ_JieXi = null;

        #region//从配置文件中获取IP地址
        public void Init_PeiZhiIPAddress(ref byte[] temp_byte_array, ref UInt16 temp_duankou_int, ref byte[] remote_byte_array, ref UInt16 remote_duankou_int)
        {
            try
            {
                #region

                IP_WJ_JieXi = new IP_PZWJ_JieXi("C:\\NBIoT\\IP.txt");
                temp_byte_array = IP_WJ_JieXi.IP;
                temp_duankou_int = IP_WJ_JieXi.DuanKou;
                remote_byte_array = IP_WJ_JieXi.Remote_IP;
                remote_duankou_int = IP_WJ_JieXi.Remote_DuanKou;
                #endregion
            }
            catch
            {
                MessageBox.Show("IP配置文件加载失败", "加载失败");
                Application.Current.Shutdown();
            }
        }
        #endregion


        #region//监测ip地址是否可用
        public void Init_IP_right_or_not(byte[] array_byte_tt, UInt16 port_tt, byte[] NBIoT_IP_Byte_Array, UInt16 NBIoT_DuanKou, 
                                         ref UDP_Communication mysql_Thread_tt)
        {
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                IPAddress ip = new IPAddress(array_byte_tt);
                newsock.Bind(new IPEndPoint(ip, port_tt));//设定ip地址及端口号
            }
            catch
            {
                MessageBox.Show("本机ip地址配置出错", "error");
                Application.Current.Shutdown();
            }

            try
            {
                Init_NBIoT(NBIoT_IP_Byte_Array, NBIoT_DuanKou, ref mysql_Thread_tt);//将NBIoT的远程地址绑定在相应的"UDP_Communication"类里，并发送第一个注册码
            }
            catch
            {
                MessageBox.Show("远程ip地址配置出错", "error");
                Application.Current.Shutdown();
            }
            newsock.Dispose();//消除此套接字，因为此套接字只是用于检测
        }
        #endregion

        #region//NBIoT初始化，包含注册包的发送
        public void Init_NBIoT(byte[] NBIoT_IP_Byte_Array_tt, ushort NBIoT_DuanKou_tt, ref UDP_Communication mysql_Thread_tt)
        {
            string temp_str = "ep=J4JFAJUGYS3GGF7Z&pw=123456";
            byte[] buff = System.Text.Encoding.ASCII.GetBytes(temp_str);

            byte[] array_byte = NBIoT_IP_Byte_Array_tt;//设定远程ip地址
            IPAddress ip = new IPAddress(array_byte);
            IPEndPoint lep = new IPEndPoint(ip, NBIoT_DuanKou_tt);

            mysql_Thread_tt.newsock.Connect(lep);
            mysql_Thread_tt.newsock.Send(buff);
        }
        #endregion

        #region
        public void Init_UDP(ref UDP_Communication mysql_Thread_tt, 
                             ref byte[] Local_IP_Byte_Array_tt, ref UInt16 Local_DuanKou_tt, 
                             ref byte[] NBIoT_IP_Byte_Array_tt, ref UInt16 NBIoT_DuanKou_tt)
        {
            Init_PeiZhiIPAddress(ref Local_IP_Byte_Array_tt, ref Local_DuanKou_tt, ref NBIoT_IP_Byte_Array_tt, ref NBIoT_DuanKou_tt);//根据配置IP配置文件的内容给全局变量赋值
            //初始化udp通讯类
            mysql_Thread_tt = new UDP_Communication(Local_IP_Byte_Array_tt, Local_DuanKou_tt);
            //注册事件
            mysql_Thread_tt.rev_New2 += new recNewMessage2(rec_NewMessage);
            //mysql_Thread.recThread_Start();

            Init_NBIoT(NBIoT_IP_Byte_Array_tt, NBIoT_DuanKou_tt, ref mysql_Thread_tt);//将NBIoT的远程地址绑定在相应的"UDP_Communication"类里，并发送第一个注册码

            //添加定时器，因为长时间上位机不向下位机发送指令上位机与云平台会断线
            SendToIoT = new System.Threading.Timer(new System.Threading.TimerCallback(SendToIoTCall), this, 3000, 3000);
        }
        #endregion

        

        #region//有关数据库加载，监测数据库加载是否正常
        public void Init_MySQL(ref mysql_PZWJ_JieXi ShuJuKu_tt)
        {
            string[] array_str = mysql_PZWJ_JieXi.read_mysql_PeiZhiWenJian("C:\\NBIoT\\mysql.txt");
            if (array_str == null)
                throw new Exception("mysql.txt 配置文件的数据为空");
            ShuJuKu_tt = new mysql_PZWJ_JieXi(array_str[0], array_str[1], array_str[2], array_str[3]);
        }
        #endregion

        public void Init_QiDongJianCe( ref mysql_PZWJ_JieXi ShuJuKu_tt, ref UDP_Communication mysql_Thread_tt, 
                                       ref byte[] Local_IP_Byte_Array_tt, ref UInt16 Local_DuanKou_tt, 
                                       ref byte[] NBIoT_IP_Byte_Array_tt, ref UInt16 NBIoT_DuanKou_tt)
        {
            try
            {
                Init_MySQL(ref ShuJuKu_tt);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "error");
                Environment.Exit(0);//这里必须使用这个，不能用"Application.Current.Shutdown();"，"Application.Current.Shutdown();"不能将程序立刻关闭
            }

            try
            {
                Init_UDP(ref mysql_Thread_tt, ref Local_IP_Byte_Array_tt, ref Local_DuanKou_tt, ref NBIoT_IP_Byte_Array_tt, ref NBIoT_DuanKou_tt);//初始化udp通讯，

                byte[] recData = new byte[1024];
                EndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);
                //int n = mysql_Thread.newsock.ReceiveFrom(recData, ref senderRemote);//尝试第一次接收数据,只要有接收的数据（不论是什么内容），就可以继续运行，否则在此等待

                mysql_Thread_tt.recThread_Start();//开启类里的线程
            }
            catch
            {
                MessageBox.Show("UDP通讯初始化失败", "error");
                Application.Current.Shutdown();
            }

            
        }
    }
}