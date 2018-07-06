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
        JieDians os = null;

        Point previousMousePoint = new Point(0, 0);

        const int size_chanel = 64;
        const int size_DataGrid_Display = 20;

        TranslateTransform[] translateTransform_Array_Tab3 = new TranslateTransform[size_chanel];
        ScaleTransform[] scaleTransform_Array_Tab3 = new ScaleTransform[size_chanel];
        Ellipse[] Ellipse_Array = new Ellipse[size_chanel];



        private void TuiChu_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            //确认对话框
            if (MessageBox.Show("确定退出系统？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            Application.Current.Shutdown();//关闭程序
        }

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

            

            double move_x = 0;
            double move_y = 0;

            if (this.WindowState == WindowState.Maximized)
            {
                double x = Canvas.GetLeft(ellipse_array[index]) - map_rightup_X;
                double y = Canvas.GetTop(ellipse_array[index]) - map_rightup_Y;

                double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
                double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度

                move_x = x1 * 0.75 / 2 - x;// (img.Height / 2 - x) * sfr.ScaleX;
                move_y = (y1 - 100) / 2 - y;// (img.Width / 2 - y) * sfr.ScaleY;
            }
            else
            {
                double x = Canvas.GetLeft(ellipse_array[index]) - map_rightup_X;
                double y = Canvas.GetTop(ellipse_array[index]) - map_rightup_Y;

                move_x = (this.Width) * 0.75 / 2 - x;// (img.Height / 2 - x) * sfr.ScaleX;
                move_y = (this.Height - 100) / 2 - y;// (img.Width / 2 - y) * sfr.ScaleY;

            }

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

            Ellipse_Array[10] = Ellipse11;
            Ellipse_Array[11] = Ellipse12;
            Ellipse_Array[12] = Ellipse13;
            Ellipse_Array[13] = Ellipse14;
            Ellipse_Array[14] = Ellipse15;
            Ellipse_Array[15] = Ellipse16;
            Ellipse_Array[16] = Ellipse17;
            Ellipse_Array[17] = Ellipse18;
            Ellipse_Array[18] = Ellipse19;
            Ellipse_Array[19] = Ellipse20;

            Ellipse_Array[20] = Ellipse21;
            Ellipse_Array[21] = Ellipse22;
            Ellipse_Array[22] = Ellipse23;
            Ellipse_Array[23] = Ellipse24;
            Ellipse_Array[24] = Ellipse25;
            Ellipse_Array[25] = Ellipse26;
            Ellipse_Array[26] = Ellipse27;
            Ellipse_Array[27] = Ellipse28;
            Ellipse_Array[28] = Ellipse29;
            Ellipse_Array[29] = Ellipse30;

            Ellipse_Array[30] = Ellipse31;
            Ellipse_Array[31] = Ellipse32;
            Ellipse_Array[32] = Ellipse33;
            Ellipse_Array[33] = Ellipse34;
            Ellipse_Array[34] = Ellipse35;
            Ellipse_Array[35] = Ellipse36;
            Ellipse_Array[36] = Ellipse37;
            Ellipse_Array[37] = Ellipse38;
            Ellipse_Array[38] = Ellipse39;
            Ellipse_Array[39] = Ellipse40;

            Ellipse_Array[40] = Ellipse41;
            Ellipse_Array[41] = Ellipse42;
            Ellipse_Array[42] = Ellipse43;
            Ellipse_Array[43] = Ellipse44;
            Ellipse_Array[44] = Ellipse45;
            Ellipse_Array[45] = Ellipse46;
            Ellipse_Array[46] = Ellipse47;
            Ellipse_Array[47] = Ellipse48;
            Ellipse_Array[48] = Ellipse49;
            Ellipse_Array[49] = Ellipse50;

            Ellipse_Array[50] = Ellipse51;
            Ellipse_Array[51] = Ellipse52;
            Ellipse_Array[52] = Ellipse53;
            Ellipse_Array[53] = Ellipse54;
            Ellipse_Array[54] = Ellipse55;
            Ellipse_Array[55] = Ellipse56;
            Ellipse_Array[56] = Ellipse57;
            Ellipse_Array[57] = Ellipse58;
            Ellipse_Array[58] = Ellipse59;
            Ellipse_Array[59] = Ellipse60;

            Ellipse_Array[60] = Ellipse61;
            Ellipse_Array[61] = Ellipse62;
            Ellipse_Array[62] = Ellipse63;
            Ellipse_Array[63] = Ellipse64;
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

            translateTransform_Array_Tab3[10] = tlt11;
            translateTransform_Array_Tab3[11] = tlt12;
            translateTransform_Array_Tab3[12] = tlt13;
            translateTransform_Array_Tab3[13] = tlt14;
            translateTransform_Array_Tab3[14] = tlt15;
            translateTransform_Array_Tab3[15] = tlt16;
            translateTransform_Array_Tab3[16] = tlt17;
            translateTransform_Array_Tab3[17] = tlt18;
            translateTransform_Array_Tab3[18] = tlt19;
            translateTransform_Array_Tab3[19] = tlt20;

            translateTransform_Array_Tab3[20] = tlt21;
            translateTransform_Array_Tab3[21] = tlt22;
            translateTransform_Array_Tab3[22] = tlt23;
            translateTransform_Array_Tab3[23] = tlt24;
            translateTransform_Array_Tab3[24] = tlt25;
            translateTransform_Array_Tab3[25] = tlt26;
            translateTransform_Array_Tab3[26] = tlt27;
            translateTransform_Array_Tab3[27] = tlt28;
            translateTransform_Array_Tab3[28] = tlt29;
            translateTransform_Array_Tab3[29] = tlt30;

            translateTransform_Array_Tab3[30] = tlt31;
            translateTransform_Array_Tab3[31] = tlt32;
            translateTransform_Array_Tab3[32] = tlt33;
            translateTransform_Array_Tab3[33] = tlt34;
            translateTransform_Array_Tab3[34] = tlt35;
            translateTransform_Array_Tab3[35] = tlt36;
            translateTransform_Array_Tab3[36] = tlt37;
            translateTransform_Array_Tab3[37] = tlt38;
            translateTransform_Array_Tab3[38] = tlt39;
            translateTransform_Array_Tab3[39] = tlt40;

            translateTransform_Array_Tab3[40] = tlt41;
            translateTransform_Array_Tab3[41] = tlt42;
            translateTransform_Array_Tab3[42] = tlt43;
            translateTransform_Array_Tab3[43] = tlt44;
            translateTransform_Array_Tab3[44] = tlt45;
            translateTransform_Array_Tab3[45] = tlt46;
            translateTransform_Array_Tab3[46] = tlt47;
            translateTransform_Array_Tab3[47] = tlt48;
            translateTransform_Array_Tab3[48] = tlt49;
            translateTransform_Array_Tab3[49] = tlt50;

            translateTransform_Array_Tab3[50] = tlt51;
            translateTransform_Array_Tab3[51] = tlt52;
            translateTransform_Array_Tab3[52] = tlt53;
            translateTransform_Array_Tab3[53] = tlt54;
            translateTransform_Array_Tab3[54] = tlt55;
            translateTransform_Array_Tab3[55] = tlt56;
            translateTransform_Array_Tab3[56] = tlt57;
            translateTransform_Array_Tab3[57] = tlt58;
            translateTransform_Array_Tab3[58] = tlt59;
            translateTransform_Array_Tab3[59] = tlt60;

            translateTransform_Array_Tab3[60] = tlt61;
            translateTransform_Array_Tab3[61] = tlt62;
            translateTransform_Array_Tab3[62] = tlt63;
            translateTransform_Array_Tab3[63] = tlt64;
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

            scaleTransform_Array_Tab3[10] = sfr11;
            scaleTransform_Array_Tab3[11] = sfr12;
            scaleTransform_Array_Tab3[12] = sfr13;
            scaleTransform_Array_Tab3[13] = sfr14;
            scaleTransform_Array_Tab3[14] = sfr15;
            scaleTransform_Array_Tab3[15] = sfr16;
            scaleTransform_Array_Tab3[16] = sfr17;
            scaleTransform_Array_Tab3[17] = sfr18;
            scaleTransform_Array_Tab3[18] = sfr19;
            scaleTransform_Array_Tab3[19] = sfr20;

            scaleTransform_Array_Tab3[20] = sfr21;
            scaleTransform_Array_Tab3[21] = sfr22;
            scaleTransform_Array_Tab3[22] = sfr23;
            scaleTransform_Array_Tab3[23] = sfr24;
            scaleTransform_Array_Tab3[24] = sfr25;
            scaleTransform_Array_Tab3[25] = sfr26;
            scaleTransform_Array_Tab3[26] = sfr27;
            scaleTransform_Array_Tab3[27] = sfr28;
            scaleTransform_Array_Tab3[28] = sfr29;
            scaleTransform_Array_Tab3[29] = sfr30;

            scaleTransform_Array_Tab3[30] = sfr31;
            scaleTransform_Array_Tab3[31] = sfr32;
            scaleTransform_Array_Tab3[32] = sfr33;
            scaleTransform_Array_Tab3[33] = sfr34;
            scaleTransform_Array_Tab3[34] = sfr35;
            scaleTransform_Array_Tab3[35] = sfr36;
            scaleTransform_Array_Tab3[36] = sfr37;
            scaleTransform_Array_Tab3[37] = sfr38;
            scaleTransform_Array_Tab3[38] = sfr39;
            scaleTransform_Array_Tab3[39] = sfr40;

            scaleTransform_Array_Tab3[40] = sfr41;
            scaleTransform_Array_Tab3[41] = sfr42;
            scaleTransform_Array_Tab3[42] = sfr43;
            scaleTransform_Array_Tab3[43] = sfr44;
            scaleTransform_Array_Tab3[44] = sfr45;
            scaleTransform_Array_Tab3[45] = sfr46;
            scaleTransform_Array_Tab3[46] = sfr47;
            scaleTransform_Array_Tab3[47] = sfr48;
            scaleTransform_Array_Tab3[48] = sfr49;
            scaleTransform_Array_Tab3[49] = sfr50;

            scaleTransform_Array_Tab3[50] = sfr51;
            scaleTransform_Array_Tab3[51] = sfr52;
            scaleTransform_Array_Tab3[52] = sfr53;
            scaleTransform_Array_Tab3[53] = sfr54;
            scaleTransform_Array_Tab3[54] = sfr55;
            scaleTransform_Array_Tab3[55] = sfr56;
            scaleTransform_Array_Tab3[56] = sfr57;
            scaleTransform_Array_Tab3[57] = sfr58;
            scaleTransform_Array_Tab3[58] = sfr59;
            scaleTransform_Array_Tab3[59] = sfr60;

            scaleTransform_Array_Tab3[60] = sfr61;
            scaleTransform_Array_Tab3[61] = sfr62;
            scaleTransform_Array_Tab3[62] = sfr63;
            scaleTransform_Array_Tab3[63] = sfr64;
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

            //保证ScaleX和ScaleY这两个值大于等于1
            if (this.sfr.ScaleX < 1 || this.sfr.ScaleY < 1)
            {
                this.sfr.ScaleX = 1;
                this.sfr.ScaleY = 1;

                for (int i = 0; i < size_chanel; i++)//size_chanel
                {
                    Point centerPoint2 = e.GetPosition(Ellipse_Array[i]);
                    Point pt2 = Ellipse_Array[i].RenderTransform.Inverse.Transform(centerPoint2);


                    translateTransform_Array_Tab3[i].X = (centerPoint2.X - pt2.X) * scaleTransform_Array_Tab3[i].ScaleX;
                    translateTransform_Array_Tab3[i].Y = (centerPoint2.Y - pt2.Y) * scaleTransform_Array_Tab3[i].ScaleY;
                    scaleTransform_Array_Tab3[i].CenterX = centerPoint2.X;
                    scaleTransform_Array_Tab3[i].CenterY = centerPoint2.Y;
                    scaleTransform_Array_Tab3[i].ScaleX = 1;
                    scaleTransform_Array_Tab3[i].ScaleY = 1;
                }
            }
            else
            {
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


            
        }
        #endregion

        private void map_Reset_Click(object sender, RoutedEventArgs e)
        {
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 2);
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

        protected string id;
        protected string gas_type;
        protected string danwei;

        protected string status;
        protected string nongdu;
        protected string dixian;
        protected string gaoxian;
        protected string dianliang;

        protected string wendu;
        protected string date;

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

        public jiedian(string id_tt, string gas_type_tt, string danwei_tt,
            string status_tt, string nongdu_tt, string dixian_tt, string gaoxian_tt, string dianliang_tt,
            string wendu_tt, string date_tt)
        {
            id = id_tt;
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

        protected void OnPropertyChanged(string strPropertyInfo)
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
        }

        public void add_size_DataGrid_Display(jiedian jiedian_tt, int max_size)
        {
            while (this.Count > (max_size - 1))//从尾部移除多余数据，保证数据长度不超过max_size
                this.RemoveAt(this.Count - 1);
            this.Insert(0, jiedian_tt);//在头部添加新的数据
        }
    }
}
