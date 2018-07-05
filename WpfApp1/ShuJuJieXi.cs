using System.Windows;
//using System.Windows.Forms;

using System.Diagnostics;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        const int message_length = 21; 
        public string[] ShuJuJieXi(byte[] temp_byte_array)
        {
            int[] array_int = new int[message_length];//前面数据可能两个数据放一个字节了

            int JianCeQiTiLeiXing = 0;              //检测气体类型
            int DanWei = 0;                         //单位
            int TanCeQiZhuangTai = 0;              //探测器状态
            int QiTiNongDu = 0;                     //气体浓度
            int DiXianJingBao = 0;                  //低限警报
            int GaoXianJingBao = 0;                 //高限警报
            int LiDianChiDianLiang = 0;             //锂电池电量
            float WenDu = 0;                          //温度

            string[] output = new string[8];

            for (int i = 0; i < message_length; i++)
            {
                array_int[i] = temp_byte_array[i];
            }

            JianCeQiTiLeiXing = array_int[4] & 0x0f;
            DanWei = array_int[5];
            TanCeQiZhuangTai = array_int[6];

            JieXinQiTiNongDu(ref QiTiNongDu, array_int[7], array_int[8]);
            JieXinQiTiNongDu(ref DiXianJingBao, array_int[9], array_int[10]);
            JieXinQiTiNongDu(ref GaoXianJingBao, array_int[11], array_int[12]);

            LiDianChiDianLiang = array_int[13];

            JieXiWenDu(ref WenDu, array_int[14], array_int[15]);

            //准备打印
            string temp_Display_str;
            if (JianCeQiTiLeiXing == 1)
            {
                output[0] = "CH4";
            }
            else
            {
                output[0] = "CO";
            }

            switch (DanWei)
            {
                case 1:
                    output[1] = "V/V";
                    break;
                case 2:
                    output[1] = "LEL";
                    break;
                case 3:
                    output[1] = "PPM";
                    break;
            }

            switch (TanCeQiZhuangTai)
            {
                case 0x00:
                    output[2] = "";
                    break;
                case 0x02:
                    output[2] = "低报";
                    break;
                case 0x04:
                    output[2] = "高报";
                    break;
                case 0x08:
                    output[2] = "传感器故障";
                    break;
                case 0x10:
                    output[2] = "电池电压低";
                    break;
                case 0x20:
                    output[2] = "有水";
                    break;
                case 0x40:
                    output[2] = "温度低";
                    break;
                case 0x80:
                    output[2] = "盗窃";
                    break;
            }

            output[3] = QiTiNongDu.ToString();
            output[4] = DiXianJingBao.ToString();
            output[5] =  GaoXianJingBao.ToString();

            output[6] =  LiDianChiDianLiang.ToString();

            output[7] = WenDu.ToString();

            return output;
        }

        void JieXinQiTiNongDu(ref int temp_int, int GaoWei, int DiWei)
        {
            int temp_GaoLiangWei = 1;//最高两位
            switch (GaoWei & 0xc0)
            {
                case 0x00:
                    temp_GaoLiangWei = 1;
                    break;
                case 0x40:
                    temp_GaoLiangWei = 10;
                    break;
                case 0x80:
                    temp_GaoLiangWei = 100;
                    break;
            }
            temp_int = ((GaoWei & 0x3f) * 256 + DiWei) / temp_GaoLiangWei;
        }

        void JieXiWenDu(ref float temp_wendu, int GaoWei, int DiWei)
        {
            if ((GaoWei & 0x80) == 0)
            {
                temp_wendu = GaoWei + (float)DiWei / 10;
            }
            else
            {
                temp_wendu = -((GaoWei & 0x7f) + (float)DiWei / 10);
            }
        }
    }
}