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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<jiedian> os = null;

        Point previousMousePoint = new Point(0, 0);

        const int size_chanel = 10;
        const int size_DataGrid_Display = 20;

        TranslateTransform[] translateTransform_Array_Tab3 = new TranslateTransform[size_chanel];
        ScaleTransform[] scaleTransform_Array_Tab3 = new ScaleTransform[size_chanel];
        Ellipse[] Ellipse_Array = new Ellipse[size_chanel];

        public UDP_Communication mysql_Thread = null;

        

        #region//udp接收中断
        public void rec_NewMessage(byte[] message, ref EndPoint endPoint_tt)
        {
            string temp_str = System.Text.Encoding.ASCII.GetString(message);
            //if (temp_str == "[iotxx:ok]" || temp_str == "[iotxx:update]")//[iotxx:ok]
            if (temp_str.Contains("[iotxx:"))
            {
                System.Diagnostics.Debug.WriteLine("[iotxx:ok]");
                return;
            }

            //显示源地址和源端口
            System.Diagnostics.Debug.WriteLine(endPoint_tt.ToString());


            string[] temp_array_str = ShuJuJieXi(message);
            string str = "INSERT INTO " + "Table1_ShiJIan_JieDian" + " ( `id`, `name`, `type`, `gas type`, `DanWei`,`status`, `NongDu`, `DiXian`, `GaoXian`, `DianLiang`, `WenDu`, `Date` ) " +
        "VALUES ( \"" + (message[0]).ToString() + "\",\"2\",\"3\",\"" + temp_array_str[0] + "\",\"" + temp_array_str[1] + "\",\"" + temp_array_str[2] + "\",\"" + temp_array_str[3] + "\",\"" + temp_array_str[4] + "\",\"" + temp_array_str[5] + "\",\"" + temp_array_str[6] + "\",\"" + temp_array_str[7] + "\",now());";
            MySqlHelper.GetDataSet("Database='NBIoT';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                    CommandType.Text, str, null);

            Action<bool> action = (x) =>//每次都对当前所有节点进行一次监测
            {
                DataSet dataSet_temp = new DataSet();
                string command_str = "select * from Table1_ShiJIan_JieDian order by date desc limit 1";
                dataSet_temp = MySqlHelper.GetDataSet("Database='NBIoT';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

                //更新DataGrid
                //os.Add(new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
                //                   temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
                //                   temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
                //                   temp_DataRow[0][9].ToString(), temp_DataRow[0][10].ToString(), temp_DataRow[0][11].ToString()));

                os.Insert(0, new jiedian(temp_DataRow[0][0].ToString(), temp_DataRow[0][1].ToString(), temp_DataRow[0][2].ToString(),
                                   temp_DataRow[0][3].ToString(), temp_DataRow[0][4].ToString(), temp_DataRow[0][5].ToString(),
                                   temp_DataRow[0][6].ToString(), temp_DataRow[0][7].ToString(), temp_DataRow[0][8].ToString(),
                                   temp_DataRow[0][9].ToString(), temp_DataRow[0][10].ToString(), temp_DataRow[0][11].ToString()));

                //更新ToolTip
                update_tooltip(ref Ellipse_Array, (message[0] - 1));
            };
            this.Dispatcher.Invoke(action, true);
        }
        #endregion

        public void jiedian_AutoZoom(ref Ellipse[] ellipse_array, int index)//从零开始索引
        {
            double x = Canvas.GetLeft(ellipse_array[index]);
            double y = Canvas.GetTop(ellipse_array[index]);

            double FangDaBeiShu = 2;

            Point centerPoint = new Point(x, y);
            Point pt = img.RenderTransform.Inverse.Transform(centerPoint);

            this.tlt.X = (centerPoint.X - pt.X) * this.sfr.ScaleX;
            this.tlt.Y = (centerPoint.Y - pt.Y) * this.sfr.ScaleY;
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            this.sfr.ScaleX += FangDaBeiShu;
            this.sfr.ScaleY += FangDaBeiShu;


            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                double x_temp = Canvas.GetLeft(ellipse_array[i]);
                double y_temp = Canvas.GetTop(ellipse_array[i]);

                Point centerPoint2 = new Point((x - x_temp), (y - y_temp));
                Point pt2 = Ellipse_Array[i].RenderTransform.Inverse.Transform(centerPoint2);
                translateTransform_Array_Tab3[i].X = (centerPoint2.X - pt2.X) * scaleTransform_Array_Tab3[i].ScaleX;
                translateTransform_Array_Tab3[i].Y = (centerPoint2.Y - pt2.Y) * scaleTransform_Array_Tab3[i].ScaleY;
                scaleTransform_Array_Tab3[i].CenterX = centerPoint2.X;
                scaleTransform_Array_Tab3[i].CenterY = centerPoint2.Y;
                scaleTransform_Array_Tab3[i].ScaleX += FangDaBeiShu;
                scaleTransform_Array_Tab3[i].ScaleY += FangDaBeiShu;

            }
        }

        public void jiedian_AutoMove(ref Ellipse[] ellipse_array, int index)//从零开始索引
        {
            map_Reset_Click(this, null);

            double x = Canvas.GetLeft(ellipse_array[index]);
            double y = Canvas.GetTop(ellipse_array[index]);

            double move_x = (this.Width) * 0.75 / 2 - x;// (img.Height / 2 - x) * sfr.ScaleX;
            double move_y = (this.Height) / 2 - y;// (img.Width / 2 - y) * sfr.ScaleY;


            tlt.X += move_x;
            tlt.Y += move_y;


            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                translateTransform_Array_Tab3[i].X += move_x * scaleTransform_Array_Tab3[i].ScaleX;
                translateTransform_Array_Tab3[i].Y += move_y * scaleTransform_Array_Tab3[i].ScaleY;
            }
        }

        private void AutoZoom_Click(object sender, RoutedEventArgs e)
        {
            jiedian_AutoMove(ref Ellipse_Array, 0);
            jiedian_AutoZoom(ref Ellipse_Array, 0);
        }

        public void Init_Ellipse_Array()
        {
            Ellipse_Array[0] = Ellipse1;
            Ellipse_Array[1] = Ellipse2;
            Ellipse_Array[2] = Ellipse3;
            Ellipse_Array[3] = Ellipse4;
            Ellipse_Array[4] = Ellipse5;
            Ellipse_Array[5] = Ellipse6;
            Ellipse_Array[6] = Ellipse7;
            Ellipse_Array[7] = Ellipse8;
            Ellipse_Array[8] = Ellipse9;
            Ellipse_Array[9] = Ellipse10;

            //rectangle_Array_Tab3[10] = rectangle11;
            //rectangle_Array_Tab3[11] = rectangle12;
            //rectangle_Array_Tab3[12] = rectangle13;
            //rectangle_Array_Tab3[13] = rectangle14;
            //rectangle_Array_Tab3[14] = rectangle15;
            //rectangle_Array_Tab3[15] = rectangle16;
            //rectangle_Array_Tab3[16] = rectangle17;
            //rectangle_Array_Tab3[17] = rectangle18;
            //rectangle_Array_Tab3[18] = rectangle19;
            //rectangle_Array_Tab3[19] = rectangle20;

            //rectangle_Array_Tab3[20] = rectangle21;
            //rectangle_Array_Tab3[21] = rectangle22;
            //rectangle_Array_Tab3[22] = rectangle23;
            //rectangle_Array_Tab3[23] = rectangle24;
            //rectangle_Array_Tab3[24] = rectangle25;
            //rectangle_Array_Tab3[25] = rectangle26;
            //rectangle_Array_Tab3[26] = rectangle27;
            //rectangle_Array_Tab3[27] = rectangle28;
            //rectangle_Array_Tab3[28] = rectangle29;
            //rectangle_Array_Tab3[29] = rectangle30;

            //rectangle_Array_Tab3[30] = rectangle31;
            //rectangle_Array_Tab3[31] = rectangle32;
            //rectangle_Array_Tab3[32] = rectangle33;
            //rectangle_Array_Tab3[33] = rectangle34;
            //rectangle_Array_Tab3[34] = rectangle35;
            //rectangle_Array_Tab3[35] = rectangle36;
            //rectangle_Array_Tab3[36] = rectangle37;
            //rectangle_Array_Tab3[37] = rectangle38;
            //rectangle_Array_Tab3[38] = rectangle39;
            //rectangle_Array_Tab3[39] = rectangle40;

            //rectangle_Array_Tab3[40] = rectangle41;
            //rectangle_Array_Tab3[41] = rectangle42;
            //rectangle_Array_Tab3[42] = rectangle43;
            //rectangle_Array_Tab3[43] = rectangle44;
            //rectangle_Array_Tab3[44] = rectangle45;
            //rectangle_Array_Tab3[45] = rectangle46;
            //rectangle_Array_Tab3[46] = rectangle47;
            //rectangle_Array_Tab3[47] = rectangle48;
            //rectangle_Array_Tab3[48] = rectangle49;
            //rectangle_Array_Tab3[49] = rectangle50;

            //rectangle_Array_Tab3[50] = rectangle51;
            //rectangle_Array_Tab3[51] = rectangle52;
            //rectangle_Array_Tab3[52] = rectangle53;
            //rectangle_Array_Tab3[53] = rectangle54;
            //rectangle_Array_Tab3[54] = rectangle55;
            //rectangle_Array_Tab3[55] = rectangle56;
            //rectangle_Array_Tab3[56] = rectangle57;
            //rectangle_Array_Tab3[57] = rectangle58;
            //rectangle_Array_Tab3[58] = rectangle59;
            //rectangle_Array_Tab3[59] = rectangle60;

            //rectangle_Array_Tab3[60] = rectangle61;
            //rectangle_Array_Tab3[61] = rectangle62;
            //rectangle_Array_Tab3[62] = rectangle63;
            //rectangle_Array_Tab3[63] = rectangle64;
        }

        public void Init_translateTransform_Array()
        {
            translateTransform_Array_Tab3[0] = tlt1;
            translateTransform_Array_Tab3[1] = tlt2;
            translateTransform_Array_Tab3[2] = tlt3;
            translateTransform_Array_Tab3[3] = tlt4;
            translateTransform_Array_Tab3[4] = tlt5;
            translateTransform_Array_Tab3[5] = tlt6;
            translateTransform_Array_Tab3[6] = tlt7;
            translateTransform_Array_Tab3[7] = tlt8;
            translateTransform_Array_Tab3[8] = tlt9;
            translateTransform_Array_Tab3[9] = tlt10;

            //translateTransform_Array_Tab3[10] = tlt11;
            //translateTransform_Array_Tab3[11] = tlt12;
            //translateTransform_Array_Tab3[12] = tlt13;
            //translateTransform_Array_Tab3[13] = tlt14;
            //translateTransform_Array_Tab3[14] = tlt15;
            //translateTransform_Array_Tab3[15] = tlt16;
            //translateTransform_Array_Tab3[16] = tlt17;
            //translateTransform_Array_Tab3[17] = tlt18;
            //translateTransform_Array_Tab3[18] = tlt19;
            //translateTransform_Array_Tab3[19] = tlt20;

            //translateTransform_Array_Tab3[20] = tlt21;
            //translateTransform_Array_Tab3[21] = tlt22;
            //translateTransform_Array_Tab3[22] = tlt23;
            //translateTransform_Array_Tab3[23] = tlt24;
            //translateTransform_Array_Tab3[24] = tlt25;
            //translateTransform_Array_Tab3[25] = tlt26;
            //translateTransform_Array_Tab3[26] = tlt27;
            //translateTransform_Array_Tab3[27] = tlt28;
            //translateTransform_Array_Tab3[28] = tlt29;
            //translateTransform_Array_Tab3[29] = tlt30;

            //translateTransform_Array_Tab3[30] = tlt31;
            //translateTransform_Array_Tab3[31] = tlt32;
            //translateTransform_Array_Tab3[32] = tlt33;
            //translateTransform_Array_Tab3[33] = tlt34;
            //translateTransform_Array_Tab3[34] = tlt35;
            //translateTransform_Array_Tab3[35] = tlt36;
            //translateTransform_Array_Tab3[36] = tlt37;
            //translateTransform_Array_Tab3[37] = tlt38;
            //translateTransform_Array_Tab3[38] = tlt39;
            //translateTransform_Array_Tab3[39] = tlt40;

            //translateTransform_Array_Tab3[40] = tlt41;
            //translateTransform_Array_Tab3[41] = tlt42;
            //translateTransform_Array_Tab3[42] = tlt43;
            //translateTransform_Array_Tab3[43] = tlt44;
            //translateTransform_Array_Tab3[44] = tlt45;
            //translateTransform_Array_Tab3[45] = tlt46;
            //translateTransform_Array_Tab3[46] = tlt47;
            //translateTransform_Array_Tab3[47] = tlt48;
            //translateTransform_Array_Tab3[48] = tlt49;
            //translateTransform_Array_Tab3[49] = tlt50;

            //translateTransform_Array_Tab3[50] = tlt51;
            //translateTransform_Array_Tab3[51] = tlt52;
            //translateTransform_Array_Tab3[52] = tlt53;
            //translateTransform_Array_Tab3[53] = tlt54;
            //translateTransform_Array_Tab3[54] = tlt55;
            //translateTransform_Array_Tab3[55] = tlt56;
            //translateTransform_Array_Tab3[56] = tlt57;
            //translateTransform_Array_Tab3[57] = tlt58;
            //translateTransform_Array_Tab3[58] = tlt59;
            //translateTransform_Array_Tab3[59] = tlt60;

            //translateTransform_Array_Tab3[60] = tlt61;
            //translateTransform_Array_Tab3[61] = tlt62;
            //translateTransform_Array_Tab3[62] = tlt63;
            //translateTransform_Array_Tab3[63] = tlt64;
        }

        public void Init_scaleTransform_Array()
        {
            scaleTransform_Array_Tab3[0] = sfr1;
            scaleTransform_Array_Tab3[1] = sfr2;
            scaleTransform_Array_Tab3[2] = sfr3;
            scaleTransform_Array_Tab3[3] = sfr4;
            scaleTransform_Array_Tab3[4] = sfr5;
            scaleTransform_Array_Tab3[5] = sfr6;
            scaleTransform_Array_Tab3[6] = sfr7;
            scaleTransform_Array_Tab3[7] = sfr8;
            scaleTransform_Array_Tab3[8] = sfr9;
            scaleTransform_Array_Tab3[9] = sfr10;

            //scaleTransform_Array_Tab3[10] = sfr11;
            //scaleTransform_Array_Tab3[11] = sfr12;
            //scaleTransform_Array_Tab3[12] = sfr13;
            //scaleTransform_Array_Tab3[13] = sfr14;
            //scaleTransform_Array_Tab3[14] = sfr15;
            //scaleTransform_Array_Tab3[15] = sfr16;
            //scaleTransform_Array_Tab3[16] = sfr17;
            //scaleTransform_Array_Tab3[17] = sfr18;
            //scaleTransform_Array_Tab3[18] = sfr19;
            //scaleTransform_Array_Tab3[19] = sfr20;

            //scaleTransform_Array_Tab3[20] = sfr21;
            //scaleTransform_Array_Tab3[21] = sfr22;
            //scaleTransform_Array_Tab3[22] = sfr23;
            //scaleTransform_Array_Tab3[23] = sfr24;
            //scaleTransform_Array_Tab3[24] = sfr25;
            //scaleTransform_Array_Tab3[25] = sfr26;
            //scaleTransform_Array_Tab3[26] = sfr27;
            //scaleTransform_Array_Tab3[27] = sfr28;
            //scaleTransform_Array_Tab3[28] = sfr29;
            //scaleTransform_Array_Tab3[29] = sfr30;

            //scaleTransform_Array_Tab3[30] = sfr31;
            //scaleTransform_Array_Tab3[31] = sfr32;
            //scaleTransform_Array_Tab3[32] = sfr33;
            //scaleTransform_Array_Tab3[33] = sfr34;
            //scaleTransform_Array_Tab3[34] = sfr35;
            //scaleTransform_Array_Tab3[35] = sfr36;
            //scaleTransform_Array_Tab3[36] = sfr37;
            //scaleTransform_Array_Tab3[37] = sfr38;
            //scaleTransform_Array_Tab3[38] = sfr39;
            //scaleTransform_Array_Tab3[39] = sfr40;

            //scaleTransform_Array_Tab3[40] = sfr41;
            //scaleTransform_Array_Tab3[41] = sfr42;
            //scaleTransform_Array_Tab3[42] = sfr43;
            //scaleTransform_Array_Tab3[43] = sfr44;
            //scaleTransform_Array_Tab3[44] = sfr45;
            //scaleTransform_Array_Tab3[45] = sfr46;
            //scaleTransform_Array_Tab3[46] = sfr47;
            //scaleTransform_Array_Tab3[47] = sfr48;
            //scaleTransform_Array_Tab3[48] = sfr49;
            //scaleTransform_Array_Tab3[49] = sfr50;

            //scaleTransform_Array_Tab3[50] = sfr51;
            //scaleTransform_Array_Tab3[51] = sfr52;
            //scaleTransform_Array_Tab3[52] = sfr53;
            //scaleTransform_Array_Tab3[53] = sfr54;
            //scaleTransform_Array_Tab3[54] = sfr55;
            //scaleTransform_Array_Tab3[55] = sfr56;
            //scaleTransform_Array_Tab3[56] = sfr57;
            //scaleTransform_Array_Tab3[57] = sfr58;
            //scaleTransform_Array_Tab3[58] = sfr59;
            //scaleTransform_Array_Tab3[59] = sfr60;

            //scaleTransform_Array_Tab3[60] = sfr61;
            //scaleTransform_Array_Tab3[61] = sfr62;
            //scaleTransform_Array_Tab3[62] = sfr63;
            //scaleTransform_Array_Tab3[63] = sfr64;
        }

        #region//地图功能的实现
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previousMousePoint = e.GetPosition(img);
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //isMouseLeftButtonDown = false;
        }

        private void img_MouseLeave(object sender, MouseEventArgs e)
        {
            //isMouseLeftButtonDown = false;
        }

        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            //if (isMouseLeftButtonDown == true)
            //{
            //    //Point position = e.GetPosition(img);
            //    //tlt.X += (position.X - this.previousMousePoint.X) * sfr.ScaleX;
            //    //tlt.Y += (position.Y - this.previousMousePoint.Y) * sfr.ScaleY;

            //    //tlt2.X += (position.X - this.previousMousePoint.X) * sfr.ScaleX;
            //    //tlt2.Y += (position.Y - this.previousMousePoint.Y) * sfr.ScaleY;

            //    Point position = e.GetPosition(img);
            //    tlt.X += (position.X - this.previousMousePoint.X) * sfr.ScaleX;
            //    tlt.Y += (position.Y - this.previousMousePoint.Y) * sfr.ScaleY;


            //}
        }

        private void img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sfr.ScaleX < 0.2 && sfr.ScaleY < 0.2 && e.Delta < 0)
            {
                return;
            }
            Point centerPoint = e.GetPosition(img);
            Point pt = img.RenderTransform.Inverse.Transform(centerPoint);
            this.tlt.X = (centerPoint.X - pt.X) * this.sfr.ScaleX;
            this.tlt.Y = (centerPoint.Y - pt.Y) * this.sfr.ScaleY;
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            this.sfr.ScaleX += e.Delta / 1000.0;
            this.sfr.ScaleY += e.Delta / 1000.0;


            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                Point centerPoint2 = e.GetPosition(Ellipse_Array[i]);
                Point pt2 = Ellipse_Array[i].RenderTransform.Inverse.Transform(centerPoint2);


                translateTransform_Array_Tab3[i].X = (centerPoint2.X - pt2.X) * scaleTransform_Array_Tab3[i].ScaleX;
                translateTransform_Array_Tab3[i].Y = (centerPoint2.Y - pt2.Y) * scaleTransform_Array_Tab3[i].ScaleY;
                scaleTransform_Array_Tab3[i].CenterX = centerPoint2.X;
                scaleTransform_Array_Tab3[i].CenterY = centerPoint2.Y;
                scaleTransform_Array_Tab3[i].ScaleX += e.Delta / 1000.0;
                scaleTransform_Array_Tab3[i].ScaleY += e.Delta / 1000.0;
            }
        }
        #endregion

        private void map_Reset_Click(object sender, RoutedEventArgs e)
        {
            //Canvas.SetTop(img, 0);
            //Canvas.SetLeft(img, 0);
            //this.sfr.CenterX = 0;
            //this.sfr.CenterY = 0;
            //this.sfr.ScaleX = 1;
            //this.sfr.ScaleY = 1;
            //this.sfr.CenterX = 0;
            //this.sfr.CenterY = 0;
            //this.sfr.ScaleX = 1;
            //this.sfr.ScaleY = 1;

            clear_img_canvas();
            clear_scale();
            clear_tlt();
        }

        public void clear_img_canvas()
        {
            Canvas.SetTop(img, 0);
            Canvas.SetLeft(img, 0);
        }

        public void clear_scale()
        {
            this.sfr.CenterX = 0;
            this.sfr.CenterY = 0;
            this.sfr.ScaleX = 1;
            this.sfr.ScaleY = 1;
            for (int i = 0; i < size_chanel; i++)
            {
                scaleTransform_Array_Tab3[i].CenterX = 0;
                scaleTransform_Array_Tab3[i].CenterY = 0;
                scaleTransform_Array_Tab3[i].ScaleX = 1;
                scaleTransform_Array_Tab3[i].ScaleY = 1;
            }
        }

        public void clear_tlt()
        {
            this.tlt.X = 0;
            this.tlt.Y = 0;
            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                translateTransform_Array_Tab3[i].X = 0;
                translateTransform_Array_Tab3[i].Y = 0;
            }
        }

        public void update_tooltip(ref Ellipse[] ellipse_array, int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from Table1_ShiJIan_JieDian where id = " + (index + 1).ToString() + " order by date desc limit 1";
            dataSet_temp = MySqlHelper.GetDataSet("Database='NBIoT';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//获取列

            if (temp_DataRow.Count <= 0)
                return;

            ellipse_array[index].ToolTip = "点位号      ：" + temp_DataRow[0][0].ToString() + "\n" +
                                           "实际位置    ：" + temp_DataRow[0][1].ToString() + "\n" +
                                           "实时浓度    ：" + temp_DataRow[0][2].ToString() + "\n" +
                                           "实时电量    ：" + temp_DataRow[0][3].ToString() + "\n" +
                                           "环境温度    ：" + temp_DataRow[0][4].ToString() + "\n" +
                                           "水位信息    ：" + temp_DataRow[0][5].ToString() + "\n" +
                                           "井盖信息    ：" + temp_DataRow[0][6].ToString() + "\n" +
                                           "数据更新时间：" + temp_DataRow[0][11].ToString();
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;

            jiedian_AutoMove(ref Ellipse_Array, (Convert.ToUInt16(a.ID) - 1));
            jiedian_AutoZoom(ref Ellipse_Array, (Convert.ToUInt16(a.ID) - 1));
        }

        private void hid_jiedian_Click(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            Ellipse_Array[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Hidden;
        }

        private void show_jiedian_Click(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            Ellipse_Array[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Visible;
        }
    }

    public class jiedian : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string id;
        private string name;
        private string type;
        private string gas_type;
        private string danwei;

        private string status;
        private string nongdu;
        private string dixian;
        private string gaoxian;
        private string dianliang;

        private string wendu;
        private string date;

        public string ID
        {
            get { return id; }
            set
            {
                if (value == null)
                {
                    id = " ";
                }
                else
                {
                    id = value;
                }
                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                {
                    name = " ";
                }
                else
                {
                    name = value;
                }
                OnPropertyChanged("Name");
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                if (value == null)
                {
                    type = " ";
                }
                else
                {
                    type = value;
                }
                OnPropertyChanged("Type");
            }
        }

        public string Gas_Type
        {
            get { return gas_type; }
            set
            {
                if (value == null)
                {
                    gas_type = " ";
                }
                else
                {
                    gas_type = value;
                }
                OnPropertyChanged("Gas_Type");
            }
        }

        public string DanWei
        {
            get { return danwei; }
            set
            {
                if (value == null)
                {
                    danwei = " ";
                }
                else
                {
                    danwei = value;
                }
                OnPropertyChanged("DanWei");
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                if (value == null)
                {
                    status = " ";
                }
                else
                {
                    status = value;
                }
                OnPropertyChanged("Status");
            }

        }

        public string NongDu
        {
            get { return nongdu; }
            set
            {
                if (value == null)
                {
                    nongdu = " ";
                }
                else
                {
                    nongdu = value;
                }
                OnPropertyChanged("NongDu");
            }
        }

        public string DiXian
        {
            get { return dixian; }
            set
            {
                if (value == null)
                {
                    dixian = " ";
                }
                else
                {
                    dixian = value;
                }
                OnPropertyChanged("DiXian");
            }
        }

        public string GaoXian
        {
            get { return gaoxian; }
            set
            {
                if (value == null)
                {
                    gaoxian = " ";
                }
                else
                {
                    gaoxian = value;
                }
                OnPropertyChanged("GaoXian");
            }
        }

        public string DianLiang
        {
            get { return dianliang; }
            set
            {
                if (value == null)
                {
                    dianliang = " ";
                }
                else
                {
                    dianliang = value;
                }
                OnPropertyChanged("DianLiang");
            }
        }

        public string WenDu
        {
            get { return wendu; }
            set
            {
                if (value == null)
                {
                    wendu = " ";
                }
                else
                {
                    wendu = value;
                }
                OnPropertyChanged("WenDu");
            }
        }

        public string Date
        {
            get { return date; }
            set
            {
                if (value == null)
                {
                    date = " ";
                }
                else
                {
                    date = value;
                }
                OnPropertyChanged("Date");
            }
        }

        public jiedian(string id_tt, string name_tt, string type_tt, string gas_type_tt, string danwei_tt,
            string status_tt, string nongdu_tt, string dixian_tt, string gaoxian_tt, string dianliang_tt,
            string wendu_tt, string date_tt)
        {
            id = id_tt;
            name = name_tt;
            type = type_tt;
            gas_type = gas_type_tt;
            danwei = danwei_tt;

            status = status_tt;
            nongdu = nongdu_tt;
            dixian = dixian_tt;
            gaoxian = gaoxian_tt;
            dianliang = dianliang_tt;

            wendu = wendu_tt;
            date = date_tt;
        }

        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }
    }
    public class JieDians :
            ObservableCollection<jiedian>
    {
        public JieDians()
        {
            //Add(new jiedian("Jesper", "Aaberg", "1234567890", "Jesper", "Aaberg", "1234567890", "Jesper", "Aaberg", "1234567890", "Jesper", "Aaberg", "1234567890"));

        }
    }
}
