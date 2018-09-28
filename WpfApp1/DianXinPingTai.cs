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

using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Threading;

using com.huawei.myhttpclient;
using java.util;


namespace DianXinPingTai
{
    public delegate void recNewMessage(string[] NewMessage);

    public partial class DianXinPingTai_Communication
    {
        public event recNewMessage rev_New;//至回传数据，不回传源地址及源端口

        public Thread p_Thread = null;
        public DateTime DateTime_NewestData = new DateTime();
        public DateTime temp_DateTime_NewestData = new DateTime();

        /******************************************/
        public string path_dir = null;

        public const string url_login = "https://180.101.147.89:8743/iocm/app/sec/v1.1.0/login";
        //public const string param_login = "{appId=xxlFd26ICnB18C6t2ePHHZXQQkUa, secret=0ggg4uI53fQC3aMrSQpmsqfTIb4a}";

        public const string appId = "xxlFd26ICnB18C6t2ePHHZXQQkUa";
        public const string secret = "kbg4dV8XOrt1wGUsBpD_TWJrZVga";

        public const string HEADER_APP_KEY = "app_key";
        public const string HEADER_APP_AUTH = "Authorization";
        
        public const string urlQueryDevices = "https://180.101.147.89:8743/iocm/app/dm/v1.3.0/devices";

        public string accessToken_str = null;
        /*************************************************/


        public DianXinPingTai_Communication(DateTime dateTime_tt)
        {
            DateTime_NewestData = dateTime_tt;
            temp_DateTime_NewestData = dateTime_tt;
            

            Init_Thread();
            //Start_Thread();

            path_dir = System.IO.Directory.GetCurrentDirectory() + "\\resource\\cert\\";
        }

        public void Init_accessToken()
        {
            MyHttpClient a = MyHttpClient.getInstance(path_dir + "outgoing.CertwithKey.pkcs12", "IoM@1234", path_dir + "ca.jks", "Huawei@123");

            Map paramLogin = new HashMap();

            paramLogin.put("appId", appId);
            paramLogin.put("secret", secret);

            ResponseMsg b = a.sendMsg(url_login, paramLogin);
            HttpsUtil http_utils_test = new HttpsUtil();

            string login_str = b.getContent().ToString();
            System.Diagnostics.Debug.WriteLine(login_str);
            accessToken_str = extract_data_from_json(login_str, "\"accessToken\"", 0).Replace("\"", "");//删除双引号，这里获取的accessToken以后会用到
            System.Diagnostics.Debug.WriteLine(accessToken_str);
        }

        public string extract_data_from_json(string json_str, string head_str, int startIndex)
        {
            int begin_index = json_str.IndexOf(head_str, startIndex);
            string temp_str = json_str.Substring(begin_index, json_str.Length - begin_index);

            char i_bianjie = temp_str.ElementAt(temp_str.IndexOf(":") + 1);
            int i_dakuohao = 1;//此变量用于判断(i_bianjie所代表的字符串)是否已经成对出现
            temp_str = temp_str.Substring(temp_str.IndexOf(":") + 1, temp_str.Length - (temp_str.IndexOf(":") + 1));
            int i = temp_str.IndexOf(i_bianjie);//此变量用于索引字符串中的char字符，初始为整个字符串中第一个i_bianjie所代表的字符串

            switch (i_bianjie)
            {
                case '{':
                    do
                    {
                        i++;
                        if (temp_str.ElementAt(i) == '{')
                        {
                            i_dakuohao++;
                        }
                        else if (temp_str.ElementAt(i) == '}')
                        {
                            i_dakuohao--;
                        }
                    } while (i_dakuohao > 0);
                    break;
                case '\"':
                    do
                    {
                        i++;
                        if (temp_str.ElementAt(i) == '\"')
                        {
                            i_dakuohao--;
                        }
                    } while (i_dakuohao > 0);
                    break;
                case '[':
                    do
                    {
                        i++;
                        if (temp_str.ElementAt(i) == '[')
                        {
                            i_dakuohao++;
                        }
                        else if (temp_str.ElementAt(i) == ']')
                        {
                            i_dakuohao--;
                        }
                    } while (i_dakuohao > 0);
                    break;

                default:
                    break;
            }


            int end_index = i + 1;
            return temp_str.Substring(0, end_index);
        }

        public void Init_Thread()
        {
            p_Thread = new Thread(recThread);
            p_Thread.IsBackground = true;
            p_Thread.Priority = ThreadPriority.AboveNormal;
        }
        public void Start_Thread()
        {
            p_Thread.Start();
        }

        public void recThread()
        {
            Init_accessToken();

            HttpsUtil http_utils_test = new HttpsUtil();
            Map paramQueryDeviceData = new HashMap();
            paramQueryDeviceData.put("pageNo", "0");
            paramQueryDeviceData.put("appId", appId);
            paramQueryDeviceData.put("pageSize", "64");
            Map header = new HashMap();
            header.put("Authorization", "Bearer " + accessToken_str);
            header.put("app_key", appId);
            while (true)
            {
                StreamClosedHttpResponse rec_http = null;
                dynamic json = null;
                try
                {
                    rec_http = http_utils_test.doGetWithParasGetStatusLine(urlQueryDevices, paramQueryDeviceData, header);
                    json = Newtonsoft.Json.Linq.JToken.Parse(rec_http.getContent()) as dynamic;
                }
                catch
                {
                    System.Threading.Thread.Sleep(3000);
                    continue;
                }
                
                //int a = json.devices[0].services[0].data.DiZhiMa;
                int len_devices = 0;
                try
                {
                    len_devices = json.totalCount;//获取本应用中共有几个节点
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("DianXin error!");

                    Init_accessToken();

                    http_utils_test = new HttpsUtil();
                    paramQueryDeviceData = new HashMap();
                    paramQueryDeviceData.put("pageNo", "0");
                    paramQueryDeviceData.put("appId", appId);
                    paramQueryDeviceData.put("pageSize", "64");
                    header = new HashMap();
                    header.put("Authorization", "Bearer " + accessToken_str);
                    header.put("app_key", appId);

                    continue;
                }
                //返回一个需要更新的节点的列表

                //遍历所有设备信息，如果发现有新的信息，就触发更新事件
                for(int i = 0; i < len_devices; i++)
                {
                    //判断时间新旧
                    DateTime temp_datetime = DateTime.ParseExact(json.devices[i].services[0].eventTime.ToString(), "yyyyMMddTHHmmssZ", null); //考虑到之后肯能多次使用这个节点的时间，所以先将其提取出来
                    if (DateTime.Compare(temp_datetime, DateTime_NewestData) > 0)
                    {
                        //如果需要更新，先将数据整理成所需格式，string数组
                        string[] temp_byte_array = new string[14];
                        temp_byte_array[0] = json.devices[i].services[0].data.DiZhiMa.ToString();
                        temp_byte_array[1] = json.devices[i].services[0].data.GongNengMa.ToString();
                        temp_byte_array[2] = json.devices[i].services[0].data.ZiJieShu.ToString();
                        temp_byte_array[3] = json.devices[i].services[0].data.JiQiMa.ToString();
                        temp_byte_array[4] = json.devices[i].services[0].data.QiTiLeiXing.ToString();

                        temp_byte_array[5] = json.devices[i].services[0].data.DanWei.ToString();
                        temp_byte_array[6] = json.devices[i].services[0].data.TangCeQiZhuangTai.ToString();
                        temp_byte_array[7] = json.devices[i].services[0].data.QiTiNongDu.ToString();
                        temp_byte_array[8] = json.devices[i].services[0].data.DiXian.ToString();
                        temp_byte_array[9] = json.devices[i].services[0].data.GaoXian.ToString();

                        temp_byte_array[10] = json.devices[i].services[0].data.DianLiang.ToString();
                        temp_byte_array[11] = json.devices[i].services[0].data.WenDuZhengShu.ToString();
                        temp_byte_array[12] = json.devices[i].services[0].data.WenDuXiaoShu.ToString();
                        temp_byte_array[13] = temp_datetime.ToString("yyyy-MM-dd HH:mm:ss");
                        //将整理后的数据传入rev_New中
                        rev_New(temp_byte_array);

                        //触发事件之后，比较当前设备是否比暂存区的时间要新，如果新，就更新
                        if (DateTime.Compare(temp_datetime, temp_DateTime_NewestData) > 0)
                        {
                            temp_DateTime_NewestData = temp_datetime;//保证temp_DateTime_NewestData是当前所有节点时间和DateTime_NewestData中最新的时间
                        }
                        
                    }
                }
                //所有节点检测并更新完成之后，用暂存版的datetime更新DateTime_NewestData
                DateTime_NewestData = temp_DateTime_NewestData;

                System.Threading.Thread.Sleep(1500);//连续发送请求可能引起程序错误
            }
        }

        public bool compare_date_in_shuju(string datatime_str, DateTime dateTime_global)
        {
            DateTime temp_datetime = DateTime.ParseExact(datatime_str, "yyyyMMddThhmmssZ", null);
            if (DateTime.Compare(temp_datetime, dateTime_global) > 0)//如果新输入的时间比已有的时间新
            {
                return true;//如果数据中的数据是新的，返回true
            }
            else
            {
                return false;//如果数据中的数据是旧的，返回false
            }
        }
        

    }

}
