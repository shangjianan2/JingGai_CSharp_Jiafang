using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace UDP_Thread
{
    public delegate void recNewMessage(byte[] NewMessage);//至回传数据，不回传源地址及源端口
    public delegate void recNewMessage2(byte[] NewMessage, ref EndPoint endPoint_tt);//回传源地址及源端口

    public class UDP_Communication
    {
        public Thread p_Thread = null;

        public event recNewMessage rev_New;//至回传数据，不回传源地址及源端口
        public event recNewMessage2 rev_New2;//回传源地址及源端口
        public Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public IPEndPoint ip = null;

        public UDP_Communication()
        {
            p_Thread = new Thread(recThread);
            p_Thread.IsBackground = true;
            p_Thread.Priority = ThreadPriority.AboveNormal;

            byte[] array_byte = new byte[4] { 192, 168, 1, 84 };//设定绑定的ip地址
            IPAddress ip = new IPAddress(array_byte);
            newsock.Bind(new IPEndPoint(ip, 2333));//设定ip地址及端口号
        }

        public UDP_Communication(byte[] array_byte_tt, int port_tt)
        {
            p_Thread = new Thread(recThread);
            p_Thread.IsBackground = true;
            p_Thread.Priority = ThreadPriority.AboveNormal;

            //byte[] array_byte = new byte[4] { 10, 137, 8, 15 };//设定绑定的ip地址
            IPAddress ip = new IPAddress(array_byte_tt);
            newsock.Bind(new IPEndPoint(ip, port_tt));//设定ip地址及端口号
        }

        public void recThread_Start()
        {
            p_Thread.Start();
        }

        public void recThread()
        {
            // Creates an IPEndPoint to capture the identity of the sending host.
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = sender;

            while (true)
            {
                byte[] recData = new byte[1024];
                //int n = newsock.Receive(recData);
                int n = newsock.ReceiveFrom(recData, ref senderRemote);
                if (n > 0)
                {
                    rev_New2(recData, ref senderRemote);
                }
            }
        }
    }
}