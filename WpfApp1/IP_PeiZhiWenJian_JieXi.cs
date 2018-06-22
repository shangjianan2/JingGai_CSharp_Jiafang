using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IP_PeiZhiWenJian_JieXi
{
    public class IP_PZWJ_JieXi
    {
        private byte[] ip_private = new byte[4];
        private UInt16 DuanKou_private;
        private byte[] remote_IP = new byte[4];
        private UInt16 remote_DuanKou_private;
        /// <summary>
        ///  根据配置IP配置文件的内容解析出IP地址及端口号，并将其存放在这个类中的相应变量中
        /// </summary>
        /// <param name="FileName">文件的完整路径</param>
        /// <returns>无</returns>
        public IP_PZWJ_JieXi(string FileName)
        {
            //Point point = rectangle1_Tab6.TranslatePoint(new Point(150,200), Tab6_Canvas);//获取点坐标
            //Canvas.SetLeft(rectangle2_Tab6, 500);//测试使用，无实际意义
            //Canvas.SetTop(rectangle2_Tab6, 500);//测试使用，无实际意义
            #region
            System.IO.StreamReader rd = System.IO.File.OpenText(FileName);
            string s = rd.ReadToEnd();
            s = s.Replace("\r\n", ";");//将回车符("\r\n")换成";"
            string[] s_Array_str = s.Split('.', ':', ';');//以分号将配置文件中的多个ip地址及相应端口号分割

            if(s_Array_str.Length < 10)//监测长度，长度最起码是10,包含本地地址和远程地址
            {
                throw (new System.Exception("IP_PZWJ_JieXi error"));
            }

            for(int i = 0; i < 4; i++)//将前四个作为ip地址
            {
                ip_private[i] = Convert.ToByte(s_Array_str[i]);
            }
            DuanKou_private = Convert.ToUInt16(s_Array_str[4]);//将第五个作为端口

            //远程地址
            for (int i = 0; i < 4; i++)//将前四个作为ip地址
            {
                remote_IP[i] = Convert.ToByte(s_Array_str[i + 5]);
            }
            remote_DuanKou_private = Convert.ToUInt16(s_Array_str[9]);//将第10个作为端口
            #endregion
        }

        public byte[] IP
        {
            get { return ip_private; }
            set
            {
                if (value == null)
                {
                    ip_private = new byte[] {127, 0, 0, 1};
                }
                else
                {
                    ip_private = value;
                }
            }
        }

        public UInt16 DuanKou
        {
            get { return DuanKou_private; }
            set
            {
                if (value < 0)
                {
                    DuanKou_private = 8080;
                }
                else
                {
                    DuanKou_private = value;
                }
            }
        }

        public byte[] Remote_IP
        {
            get { return remote_IP; }
            set
            {
                if(value == null)
                {
                    remote_IP = new byte[] { 127, 0, 0, 1 };
                }
                else
                {
                    remote_IP = value;
                }
            }
        }

        public UInt16 Remote_DuanKou
        {
            get { return remote_DuanKou_private; }
            set
            {
                if(value < 0)
                {
                    remote_DuanKou_private = 8081;
                }
                else
                {
                    remote_DuanKou_private = value;
                }
            }
        }
    }
}