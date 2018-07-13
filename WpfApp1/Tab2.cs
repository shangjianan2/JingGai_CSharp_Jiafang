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
        const int size_DataGrid_Display = 25;

        List<Ellipse> ellipse_list_tab2 = new List<Ellipse>();



        private void TuiChu_Tab2_Buttton_Click(object sender, RoutedEventArgs e)
        {
            //确认对话框
            if (MessageBox.Show("确定退出系统？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            Application.Current.Shutdown();//关闭程序
        }

        public void jiedian_AutoZoom(ref List<Ellipse> ellipse_array, int index_tt)//从零开始索引
        {
            int index = mysqlID_to_listID(ellipse_array, index_tt);

            double x = Canvas.GetLeft(ellipse_array[index]) + (jiedian_size / 2);//加上半径的长度，获取圆心
            double y = Canvas.GetTop(ellipse_array[index]) + (jiedian_size / 2);

            double FangDaBeiShu = 2;

            Point centerPoint = new Point(x, y);
            //Point pt = canvas_mine.RenderTransform.Inverse.Transform(centerPoint);

            //this.tlt.X = (centerPoint.X - pt.X) * this.sfr.ScaleX;
            //this.tlt.Y = (centerPoint.Y - pt.Y) * this.sfr.ScaleY;
            this.sfr.CenterX = centerPoint.X;
            this.sfr.CenterY = centerPoint.Y;
            this.sfr.ScaleX = FangDaBeiShu;//此处的放大倍数应该直接赋值，而不是递加
            this.sfr.ScaleY = FangDaBeiShu;
            
        }

        public void jiedian_AutoMove(ref List<Ellipse> ellipse_array, int index_tt)//从零开始索引
        {
            map_Reset_Click(this, null);

            int index = mysqlID_to_listID(ellipse_array, index_tt);

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


            //for (int i = 0; i < size_chanel; i++)//size_chanel
            //{
            //    translateTransform_Array_Tab3[i].X += move_x * scaleTransform_Array_Tab3[i].ScaleX;
            //    translateTransform_Array_Tab3[i].Y += move_y * scaleTransform_Array_Tab3[i].ScaleY;
            //}

            
        }

        

        #region//地图功能的实现
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
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
            Point centerPoint = e.GetPosition(canvas_mine);
            Point pt = canvas_mine.RenderTransform.Inverse.Transform(centerPoint);
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
            //for (int i = 0; i < size_chanel; i++)
            //{
            //    scaleTransform_Array_Tab3[i].CenterX = 0;
            //    scaleTransform_Array_Tab3[i].CenterY = 0;
            //    scaleTransform_Array_Tab3[i].ScaleX = 1;
            //    scaleTransform_Array_Tab3[i].ScaleY = 1;
            //}
        }

        public void clear_tlt()
        {
            this.tlt.X = 0;
            this.tlt.Y = 0;
            //for (int i = 0; i < size_chanel; i++)//size_chanel
            //{
            //    translateTransform_Array_Tab3[i].X = 0;
            //    translateTransform_Array_Tab3[i].Y = 0;
            //}
        }

        

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            
            jiedian_AutoMove(ref ellipse_list_tab2, (Convert.ToUInt16(a.ID)));
            jiedian_AutoZoom(ref ellipse_list_tab2, (Convert.ToUInt16(a.ID)));
        }

        //为MouseLeftButtonUp创造的重载,目的是可以区分鼠标在DataGrid上的左键右键
        private void DataGrid_SelectedCellsChanged(object sender, MouseButtonEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;

            jiedian_AutoMove(ref ellipse_list_tab2, (Convert.ToUInt16(a.ID)));
            jiedian_AutoZoom(ref ellipse_list_tab2, (Convert.ToUInt16(a.ID)));
        }

        private void hid_jiedian_Click(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            ellipse_list_tab2[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Hidden;
        }

        private void show_jiedian_Click(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            ellipse_list_tab2[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Visible;
        }


        private void clear_datagrid_Click(object sender, RoutedEventArgs e)
        {
            os.Clear();
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
