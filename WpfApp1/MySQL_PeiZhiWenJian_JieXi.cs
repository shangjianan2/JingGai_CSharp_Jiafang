using System;
using System.Windows;

using MySQL_Funtion;
using System.Data;

namespace MySQL_PeiZhiWenJian_JieXi
{
    public class mysql_PZWJ_JieXi
    {
        private string shujuku_name = null;
        private string table1_shijian_jiedian = null;
        private string table2_yonghu = null;
        private string table3_jiedian = null;

        public mysql_PZWJ_JieXi(string shujuku_name_tt, string table1_shijian_jiedian_tt, string table2_yonghu_tt, string table3_jiedian_tt)
        {
            ShuJuKu_Name = shujuku_name_tt;//ע�⸳ֵ��˳��֮��ı����ĸ�ֵ����鸳ֵ����ȷ�ԣ����ķ��������ڵ�һ������
            Table1_ShiJIna_JieDian = table1_shijian_jiedian_tt;
            Table2_YongHu = table2_yonghu_tt;
            Table3_JieDian = table3_jiedian_tt;
        }

        //ע������ĸ�ֵ˳��һ��Ҫ�ȸ����ݿ�"ShuJuKu_Name"��ֵ����Ϊ֮��ı�ļ���Ҫ������"ShuJuKu_Name"
        public string ShuJuKu_Name
        {
            get
            {
                if (shujuku_name == null)
                {
                    throw new Exception("by lunge: shujuku_name is null");
                }
                else
                {
                    return shujuku_name;
                }
            }
            private set
            {
                if(value is string && (!(value is null)))
                {
                    string temp_str = value;

                    try
                    {
                        string conn = "Database='" + temp_str + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true";
                        MySqlHelper.GetDataSet(conn, CommandType.Text, "show tables", null);

                        shujuku_name = temp_str;
                    }
                    catch
                    {
                        throw new Exception("this shujuku seem not exist");//˵��������ݲ�����
                    }
                }
                else
                {
                    throw new Exception("can not set value");
                }
            }
        }

        public string Table1_ShiJIna_JieDian
        {
            get
            {
                if (table1_shijian_jiedian == null)
                {
                    throw new Exception("by lunge: table1_shijian_jiedian is null");
                }
                else
                {
                    return table1_shijian_jiedian;
                }
            }
            private set
            {
                if (value is string && (!(value is null)))
                {
                    string temp_str = value;

                    try
                    {
                        string conn = "Database='" + this.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true";
                        MySqlHelper.GetDataSet(conn, CommandType.Text, "show columns from " + temp_str, null);

                        table1_shijian_jiedian = temp_str;
                    }
                    catch
                    {
                        throw new Exception("this table1 seem not exist");//˵��������ݲ�����
                    }
                }
                else
                {
                    throw new Exception("can not set value");
                }
            }
        }

        public string Table2_YongHu
        {
            get
            {
                if(table2_yonghu == null)
                {
                    throw new Exception("by lunge: table2_YongHu is null");
                }
                else
                {
                    return table2_yonghu;
                }
            }
            private set
            {
                if (value is string && (!(value is null)))
                {
                    string temp_str = value;

                    try
                    {
                        string conn = "Database='" + this.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true";
                        MySqlHelper.GetDataSet(conn, CommandType.Text, "show columns from " + temp_str, null);

                        table2_yonghu = temp_str;
                    }
                    catch
                    {
                        throw new Exception("this table2 seem not exist");//˵��������ݲ�����
                    }
                }
                else
                {
                    throw new Exception("can not set value");
                }
            }
        }

        public string Table3_JieDian
        {
            get
            {
                if(table3_jiedian == null)
                {
                    throw new Exception("by lunge: table3_jiedian is null");
                }
                else
                {
                    return table3_jiedian;
                }
            }
            private set
            {
                if (value is string && (!(value is null)))
                {
                    string temp_str = value;

                    try
                    {
                        string conn = "Database='" + this.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true";
                        MySqlHelper.GetDataSet(conn, CommandType.Text, "show columns from " + temp_str, null);

                        table3_jiedian = temp_str;
                    }
                    catch
                    {
                        throw new Exception("this table3 seem not exist");//˵��������ݲ�����
                    }
                }
                else
                {
                    throw new Exception("can not set value");
                }
            }
        }

        public static string[] read_mysql_PeiZhiWenJian(string FileName)
        {
            try
            {
                System.IO.StreamReader rd = System.IO.File.OpenText(FileName);
                string s = rd.ReadToEnd();
                s = s.Replace("\r\n", " ");


                return s.Split(' ');
            }
            catch
            {
                return null;
            }

            
        }
    }
}