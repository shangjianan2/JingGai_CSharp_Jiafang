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
    public delegate void recNewMessage(byte[] NewMessage);//���ش����ݣ����ش�Դ��ַ��Դ�˿�
    public delegate void recNewMessage2(byte[] NewMessage, ref EndPoint endPoint_tt);//�ش�Դ��ַ��Դ�˿�

    public class UDP_Communication
    {
        public Thread p_Thread = null;

        public event recNewMessage rev_New;//���ش����ݣ����ش�Դ��ַ��Դ�˿�
        public event recNewMessage2 rev_New2;//�ش�Դ��ַ��Դ�˿�
        public Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public IPEndPoint ip = null;

        public UDP_Communication()
        {
            p_Thread = new Thread(recThread);
            p_Thread.IsBackground = true;
            p_Thread.Priority = ThreadPriority.AboveNormal;

            byte[] array_byte = new byte[4] { 192, 168, 1, 84 };//�趨�󶨵�ip��ַ
            IPAddress ip = new IPAddress(array_byte);
            newsock.Bind(new IPEndPoint(ip, 2333));//�趨ip��ַ���˿ں�
        }

        public UDP_Communication(byte[] array_byte_tt, int port_tt)
        {
            p_Thread = new Thread(recThread);
            p_Thread.IsBackground = true;
            p_Thread.Priority = ThreadPriority.AboveNormal;

            //byte[] array_byte = new byte[4] { 10, 137, 8, 15 };//�趨�󶨵�ip��ַ
            IPAddress ip = new IPAddress(array_byte_tt);
            newsock.Bind(new IPEndPoint(ip, port_tt));//�趨ip��ַ���˿ں�
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