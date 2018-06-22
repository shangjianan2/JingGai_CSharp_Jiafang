using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Map_PeiZhiWenJian_JieXi
{
    public class map_PZWJ_JieXi
    {
        /// <summary>
        ///  根据配置地图配置文件的内容解析出每个节点的坐标信息
        /// </summary>
        /// <param name="FileName">文件的完整路径</param>
        /// <param name="size">解析之后的长度</param>
        /// <param name="JieDian_ZuoBiao_Array">输出解析之后的节点坐标数组</param>
        /// <returns>正常解析输出零，否则输出一</returns>
        public static bool get_JieDianZuoBiao(string FileName, int size, ref int[,] JieDian_ZuoBiao_Array)
        {
            try
            {
                System.IO.StreamReader rd = System.IO.File.OpenText(FileName);
                string s = rd.ReadToEnd();
                s = s.Replace("\r\n", " ");

                string[] s_Array = s.Split(' ');
                if(s_Array.Length != (size * 3 + 1))
                {
                    throw (new System.Exception("the length of s_Array is wrong"));
                }

                if(JieDian_ZuoBiao_Array.Length != (size * 2))
                {
                    throw (new System.Exception("the length of JieDian_ZuoBiao_Array is wrong"));
                }

                for(int i = 0; i < size; i++)
                {
                    JieDian_ZuoBiao_Array[i, 0] = Convert.ToInt16(s_Array[i * 3 + 1]);
                    JieDian_ZuoBiao_Array[i, 1] = Convert.ToInt16(s_Array[i * 3 + 2]);
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///  根据配置地图配置文件的内容解析出要加载的地图的路径
        /// </summary>
        /// <param name="FileName">文件的完整路径</param>
        /// <param name="size">解析之后的长度</param>
        /// <param name="DiTuLuJing">输出解析之后地图的路径</param>
        /// <returns>正常解析输出零，否则输出一</returns>
        public static bool get_DiTuLuJing(string FileName, int size, ref string DiTuLuJing)
        {
            try
            {
                System.IO.StreamReader rd = System.IO.File.OpenText(FileName);
                string s = rd.ReadToEnd();
                s = s.Replace("\r\n", " ");

                string[] s_Array = s.Split(' ');

                DiTuLuJing = s_Array[s_Array.Length - 1];


                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}