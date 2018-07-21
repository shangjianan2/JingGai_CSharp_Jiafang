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
        public const string path_dir = @"E:\biancheng\wpf_test\jar\WindowsFormsApp2\WindowsFormsApp2\bin\Debug\resource\cert\";

        public const string url_login = "https://180.101.147.89:8743/iocm/app/sec/v1.1.0/login";
        //public const string param_login = "{appId=xxlFd26ICnB18C6t2ePHHZXQQkUa, secret=0ggg4uI53fQC3aMrSQpmsqfTIb4a}";

        public const string appId = "xxlFd26ICnB18C6t2ePHHZXQQkUa";
        public const string secret = "0ggg4uI53fQC3aMrSQpmsqfTIb4a";

        public const string HEADER_APP_KEY = "app_key";
        public const string HEADER_APP_AUTH = "Authorization";

        public const string urlQueryDeviceData = "https://180.101.147.89:8743/iocm/app/dm/v1.3.0/devices/79ea83fc-1080-43fd-96c8-153381854c11";
        public const string urlQueryDevices = "https://180.101.147.89:8743/iocm/app/dm/v1.3.0/devices";

        public string accessToken_str = null;
        /*************************************************/


        public DianXinPingTai_Communication(DateTime dateTime_tt)
        {
            DateTime_NewestData = dateTime_tt;
            temp_DateTime_NewestData = dateTime_tt;
            Init_accessToken();

            Init_Thread();
            //Start_Thread();

            //这里应该初始化DateTime_NewestData
        }

        public void Init_accessToken()
        {
            MyHttpClient a = MyHttpClient.getInstance(path_dir + "outgoing.CertwithKey.pkcs12", "IoM@1234", path_dir + "ca.jks", "Huawei@123");

            Map paramLogin = new HashMap();

            paramLogin.put("appId", appId);
            paramLogin.put("secret", secret);

            ResponseMsg b = a.sendMsg(url_login, paramLogin);
            HttpsUtil http_utils_test = new HttpsUtil();

            /*
             * https://180.101.147.89:8743/iocm/app/dm/v1.3.0/devices/79ea83fc-1080-43fd-96c8-153381854c11      urlQueryDeviceData
             * appId=xxlFd26ICnB18C6t2ePHHZXQQkUa                                                               paramQueryDeviceData
             * Authorization=Bearer 7194309a7c8eb2f248474c36bee2faa, app_key=xxlFd26ICnB18C6t2ePHHZXQQkUa       header
             * 
             */
            //StreamClosedHttpResponse rec_http = http_utils_test.doGetWithParasGetStatusLine();

            string login_str = b.getContent().ToString();
            System.Diagnostics.Debug.WriteLine(login_str);
            accessToken_str = extract_data_from_json(login_str, "\"accessToken\"", 0).Replace("\"", "");//删除双引号
            System.Diagnostics.Debug.WriteLine(accessToken_str);


            string urlQueryDeviceData = "https://180.101.147.89:8743/iocm/app/dm/v1.3.0/devices/79ea83fc-1080-43fd-96c8-153381854c11";
            Map paramQueryDeviceData = new HashMap();
            paramQueryDeviceData.put("appId", appId);
            Map header = new HashMap();
            header.put("Authorization", "Bearer " + accessToken_str);
            header.put("app_key", appId);

            StreamClosedHttpResponse rec_http = http_utils_test.doGetWithParasGetStatusLine(urlQueryDeviceData, paramQueryDeviceData, header);
            string data_str = extract_data_from_json(rec_http.getContent(), "\"data\"", 0);
            System.Diagnostics.Debug.WriteLine(data_str);
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
            HttpsUtil http_utils_test = new HttpsUtil();
            Map paramQueryDeviceData = new HashMap();
            paramQueryDeviceData.put("pageNo", "0");
            paramQueryDeviceData.put("appId", appId);
            paramQueryDeviceData.put("pageSize", "10");
            Map header = new HashMap();
            header.put("Authorization", "Bearer " + accessToken_str);
            header.put("app_key", appId);
            while (true)
            {
                StreamClosedHttpResponse rec_http = http_utils_test.doGetWithParasGetStatusLine(urlQueryDevices, paramQueryDeviceData, header);
                dynamic json = Newtonsoft.Json.Linq.JToken.Parse(rec_http.getContent()) as dynamic;
                //int a = json.devices[0].services[0].data.DiZhiMa;
                int len_devices = json.totalCount;//获取本应用中共有几个节点

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
        //public bool updateDataGrid_or_not(dynamic dynamic_tt, ref DateTime dateTime_global)
        //{
        //    int len_devices = dynamic_tt.totalCount;
        //    List<int> list_int = new List<int>();
        //    DateTime datetime_to_update = dateTime_global;
        //    for (int i = 0; i < len_devices; i++)
        //    {
        //        if (compare_date_in_shuju(dynamic_tt.devices[i].services[0].eventTime.ToString(), dateTime_global))
        //        {
        //            os.Add(new EmployeeInfo(dynamic_tt.devices[i].services[0].data.DiZhiMa.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.ZiJieShu.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.JiQiMa.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.QiTiLeiXing.ToString(),

        //                                        dynamic_tt.devices[i].services[0].data.DanWei.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.TangCeQiZhuangTai.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.QiTiNongDu.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.DiXian.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.GaoXian.ToString(),

        //                                        dynamic_tt.devices[i].services[0].data.DianLiang.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.WenDuZhengShu.ToString(),
        //                                        dynamic_tt.devices[i].services[0].data.WenDuXiaoShu.ToString()));
        //            datetime_to_update = DateTime.ParseExact(dynamic_tt.devices[i].services[0].eventTime.ToString(),
        //                                                     "yyyyMMddThhmmssZ", null);
        //        }
        //    }

        //    dateTime_global = datetime_to_update;
        //    return false;
        //}
    }

    //public class EmployeeInfo : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private string id;
    //    private string name;
    //    private string type;
    //    private string gas_type;
    //    private string danwei;

    //    private string status;
    //    private string nongdu;
    //    private string dixian;
    //    private string gaoxian;
    //    private string dianliang;

    //    private string wendu;
    //    private string date;

    //    public string ID
    //    {
    //        get { return id; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                id = " ";
    //            }
    //            else
    //            {
    //                id = value;
    //            }
    //            OnPropertyChanged("ID");
    //        }
    //    }

    //    public string Name
    //    {
    //        get { return name; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                name = " ";
    //            }
    //            else
    //            {
    //                name = value;
    //            }
    //            OnPropertyChanged("Name");
    //        }
    //    }

    //    public string Type
    //    {
    //        get { return type; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                type = " ";
    //            }
    //            else
    //            {
    //                type = value;
    //            }
    //            OnPropertyChanged("Type");
    //        }
    //    }

    //    public string Gas_Type
    //    {
    //        get { return gas_type; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                gas_type = " ";
    //            }
    //            else
    //            {
    //                gas_type = value;
    //            }
    //            OnPropertyChanged("Gas_Type");
    //        }
    //    }

    //    public string DanWei
    //    {
    //        get { return danwei; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                danwei = " ";
    //            }
    //            else
    //            {
    //                danwei = value;
    //            }
    //            OnPropertyChanged("DanWei");
    //        }
    //    }

    //    public string Status
    //    {
    //        get { return status; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                status = " ";
    //            }
    //            else
    //            {
    //                status = value;
    //            }
    //            OnPropertyChanged("Status");
    //        }

    //    }

    //    public string NongDu
    //    {
    //        get { return nongdu; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                nongdu = " ";
    //            }
    //            else
    //            {
    //                nongdu = value;
    //            }
    //            OnPropertyChanged("NongDu");
    //        }
    //    }

    //    public string DiXian
    //    {
    //        get { return dixian; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                dixian = " ";
    //            }
    //            else
    //            {
    //                dixian = value;
    //            }
    //            OnPropertyChanged("DiXian");
    //        }
    //    }

    //    public string GaoXian
    //    {
    //        get { return gaoxian; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                gaoxian = " ";
    //            }
    //            else
    //            {
    //                gaoxian = value;
    //            }
    //            OnPropertyChanged("GaoXian");
    //        }
    //    }

    //    public string DianLiang
    //    {
    //        get { return dianliang; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                dianliang = " ";
    //            }
    //            else
    //            {
    //                dianliang = value;
    //            }
    //            OnPropertyChanged("DianLiang");
    //        }
    //    }

    //    public string WenDu
    //    {
    //        get { return wendu; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                wendu = " ";
    //            }
    //            else
    //            {
    //                wendu = value;
    //            }
    //            OnPropertyChanged("WenDu");
    //        }
    //    }

    //    public string Date
    //    {
    //        get { return date; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                date = " ";
    //            }
    //            else
    //            {
    //                date = value;
    //            }
    //            OnPropertyChanged("Date");
    //        }
    //    }

    //    public EmployeeInfo(string id_tt, string name_tt, string type_tt, string gas_type_tt, string danwei_tt,
    //        string status_tt, string nongdu_tt, string dixian_tt, string gaoxian_tt, string dianliang_tt,
    //        string wendu_tt, string date_tt)
    //    {
    //        id = id_tt;
    //        name = name_tt;
    //        type = type_tt;
    //        gas_type = gas_type_tt;
    //        danwei = danwei_tt;

    //        status = status_tt;
    //        nongdu = nongdu_tt;
    //        dixian = dixian_tt;
    //        gaoxian = gaoxian_tt;
    //        dianliang = dianliang_tt;

    //        wendu = wendu_tt;
    //        date = date_tt;
    //    }

    //    private void OnPropertyChanged(string strPropertyInfo)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
    //        }
    //    }
    //}
}
