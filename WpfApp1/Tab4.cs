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
using System.Windows.Shapes;

using System.Data;
using MySQL_Funtion;

//using UDP_Thread;

using System.Collections.ObjectModel;

using System.ComponentModel;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml �Ľ����߼�
    /// </summary>
    public partial class MainWindow : Window
    {
        //JieDians os_tab4 = null;

        Point previousMousePoint_tab4 = new Point(0, 0);
        private bool isMouseLeftButtonDown_tab4 = false;

        const double jiedian_size = 10;

        TranslateTransform[] translateTransform_Array_tab4 = new TranslateTransform[size_chanel];
        ScaleTransform[] scaleTransform_Array_tab4 = new ScaleTransform[size_chanel];
        Ellipse[] Ellipse_Array_tab4 = new Ellipse[size_chanel];
        List<Ellipse> ellipse_list_tab4 = new List<Ellipse>();

        /// <summary>
        /// �����ݿ��л�ȡ��ǰ���ýڵ㣬�����䰴�����ݿ��е���Ϣ���л���
        /// </summary>
        public void Init_JieDian_Map_LieBiao()
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + ";";
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            for(int i = 0; i < temp_DataRow.Count; i++)
            {
                Draw_JieDian_on_Map(Convert.ToDouble(temp_DataRow[i][6]) - map_rightup_X, Convert.ToDouble(temp_DataRow[i][7]) - map_rightup_Y, ref ellipse_list_tab2, canvas_mine);
                Draw_JieDian_on_Map(Convert.ToDouble(temp_DataRow[i][6]) - map_rightup_X, Convert.ToDouble(temp_DataRow[i][7]) - map_rightup_Y, ref ellipse_list_tab4, canvas_mine_tab4);
            }
        }

        public void Draw_JieDian_on_Map(double x, double y, ref List<Ellipse> ellipse_list_tt, Canvas canvas_tt)
        {
            //�ڵ�ͼ�ϻ���
            Ellipse ellipse = new Ellipse();
            ellipse.Width = jiedian_size;
            ellipse.Height = jiedian_size;
            ellipse.Fill = Brushes.Red;
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            canvas_tt.Children.Add(ellipse);

            //���뵽�б���
            ellipse_list_tt.Add(ellipse);

            //����¼�
            ellipse.MouseMove += img_MouseMove_tab4;
            ellipse.MouseDown += img_MouseDown_tab4;
            ellipse.MouseUp += img_MouseUp_tab4;
            ellipse.MouseLeave += img_MouseLeave_tab4;
        }

        private void Verify_PassWord_Tab4_Button_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow passwd_Form2 = new PasswordWindow(this, 1);
            passwd_Form2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            passwd_Form2.Owner = this;
            passwd_Form2.ShowDialog();

            if (passwd_Form2.DialogResult == false)
                return;
        }

        private void XinXiBianGeng_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(true, false, false, 4);
        }

        private void ZengShan_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            Enable_verify_add_del(false, true, true, 4);
        }

        private void ShanChuDianWei_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//���TextBox��û�����ݾͲ������κβ���
                return;

            //ȷ�϶Ի���
            if (System.Windows.MessageBox.Show("ȷ��ɾ���ڵ㣿", "��ʾ", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            string command_str = "delete from table3_jiedian where id=" + BianHao.Text + ";";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ㣬������ͼģʽ���б�ģʽ
        }

        private void ZengJiaBianGeng_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//���TextBox��û�����ݾͲ������κβ���
                return;

            //ȷ�϶Ի���
            if (System.Windows.MessageBox.Show("ȷ�����ӽڵ㣿", "��ʾ", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            ZengJiaBianGeng(Convert.ToInt16(BianHao.Text), JianCeQiTi.Text, AnZhuangWeiZhi.Text, AnZhuangShiJina.Text, GaoXianBaoJing.Text, DiXianBaoJing.Text);

            //���ݵ�ǰ�����ڵ������������ݿ��е�����
            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ㣬������ͼģʽ���б�ģʽ


            //�ж��������нڵ��Ƿ����
            DiaoXian();
        }

        public void ZengJiaBianGeng(int index, string jianceqiti_tt, string anzhuangweizhi_tt, string anzhuangshijian_tt, string gaoxianbaojing_tt, string dixianbaojing_tt)
        {
            //������ݿ����Ƿ��Ѿ����ڴ˽ڵ�
            DataSet dataSet_temp = new DataSet();
            string exist_str = "select `id` from " + ShuJuKu.Table3_JieDian;
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, exist_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��
            for (int i = 0; i < temp_DataRow.Count; i++)
            {
                if (Convert.ToInt16(temp_DataRow[i][0]) == index)
                {
                    return;//������ݿ����Ѿ�������ڵ��id��ʱ��ֱ������
                }
            }

            //�����е�����˵������ڵ�ʹ���µģ�Ȼ���µ��������뵽���ݿ�
            string command_str = "INSERT INTO " + ShuJuKu.Table3_JieDian + " (`id`, `gas type`, `location`, `time of install`, `high to warning`, `low to warning`, `xmin`, `ymin` ) VALUES ( \"" +
                index.ToString() + "\",  \"" + jianceqiti_tt + "\", \"" + anzhuangweizhi_tt +
                "\", \"" + anzhuangshijian_tt + "\", \"" + gaoxianbaojing_tt + "\", \"" + dixianbaojing_tt + "\", \"" + map_rightup_X + "\", \"" + map_rightup_Y + "\");";
            MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);

            //�ڴ˴�����µĽڵ㣬�������ݵ���Ϣ

        }

        private void BianHao_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Show_Information_Jiedian(Convert.ToInt16(BianHao.Text));

                DataSet dataSet_temp = new DataSet();
                string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + BianHao.Text;
                dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                      CommandType.Text, command_str, null);
                DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

                if (temp_DataRow.Count <= 0)//û����Ӧ�ڵ����Ϣ�������µĽڵ�
                {
                    MessageBox.Show("�˽ڵ���δ����", "��ʾ");
                    return;
                }

                jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text) - 1);
                jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text) - 1);
            }
        }

        private void QueRenBianGeng_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BianHao.Text == "")//���TextBox��û�����ݾͲ������κβ���
                return;
            Update_Information_Jiedian(Ellipse_Array_tab4, Convert.ToInt16(BianHao.Text), JianCeQiTi.Text, AnZhuangWeiZhi.Text, AnZhuangShiJina.Text, GaoXianBaoJing.Text, DiXianBaoJing.Text);
            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ�
        }

        private void PuTongMoShi_Tab4_Button_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 2;
            Tab_change_fore(4, 2);
        }

        private void LieBiaoMoShi_Tab4_Buttton_Click(object sender, RoutedEventArgs e)
        {
            //tabcontrol.SelectedIndex = 5;
            Tab_change_fore(4, 5);
            Enable_verify_add_del(false, false, false, 5);
        }

        /// <summary>
        /// ��ʾ��ţ�������壬 ��װλ�ã���װʱ�䣬���ޱ��������ޱ���
        /// </summary>
        public void Show_Information_Jiedian(int index)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + index.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            if(temp_DataRow.Count <= 0)//û����Ӧ�ڵ����Ϣ���ͽ�������������ó�Ĭ��ֵ
            {
                JianCeQiTi.Text = "Ĭ��ֵ";
                AnZhuangWeiZhi.Text = "Ĭ��ֵ";
                AnZhuangShiJina.Text = DateTime.Now.ToString();//Ĭ��Ϊ��ǰʱ��
                GaoXianBaoJing.Text = "Ĭ��ֵ";
                DiXianBaoJing.Text = "Ĭ��ֵ";
            }
            else
            {
                JianCeQiTi.Text = temp_DataRow[0][1].ToString();
                AnZhuangWeiZhi.Text = temp_DataRow[0][2].ToString();
                AnZhuangShiJina.Text = temp_DataRow[0][3].ToString();
                GaoXianBaoJing.Text = temp_DataRow[0][4].ToString();
                DiXianBaoJing.Text = temp_DataRow[0][5].ToString();
            }
        }

        /// <summary>
        /// ���Ļ������ڵ���Ϣ����ţ�������壬 ��װλ�ã���װʱ�䣬���ޱ��������ޱ���
        /// </summary>
        public void Update_Information_Jiedian(Ellipse[] Ellipse_Array_tt, int index, string jianceqiti_tt, string anzhuangweizhi_tt, string anzhuangshijian_tt, string gaoxianbaojing_tt, string dixianbaojing_tt)
        {
            DataSet dataSet_temp = new DataSet();
            string command_str = "select * from " + ShuJuKu.Table3_JieDian + " where `id`=" + index.ToString();
            dataSet_temp = MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, command_str, null);
            DataRowCollection temp_DataRow = dataSet_temp.Tables[0].Rows;//��ȡ��

            if (temp_DataRow.Count <= 0)//û����Ӧ�ڵ����Ϣ�������µĽڵ�
            {
                MessageBox.Show("δ����¼��ķ�Χ��", "��ʾ");
            }
            else//�����еĽڵ���Ϣ����
            {
                Point point_temp = Ellipse_Array_tt[index - 1].TranslatePoint(new Point(0, 0), img_tab4);
                string UpdataCommand_str = "UPDATE Table3_JieDian SET `gas type` = '" + jianceqiti_tt +
                    "', location = '" + anzhuangweizhi_tt + "', `time of install` = '" + anzhuangshijian_tt +
                    "', `high to warning` = '" + gaoxianbaojing_tt + "', `low to warning` = '" + dixianbaojing_tt +
                    "', `xmin` = '" + point_temp.X.ToString() + "', `ymin` = '" + point_temp.Y.ToString() +
                    "' WHERE id = " + index;
                MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true",
                                                  CommandType.Text, UpdataCommand_str, null);
            }
        }


        public void jiedian_AutoZoom_tab4(ref Ellipse[] ellipse_array, int index)//���㿪ʼ����
        {
            double x = Canvas.GetLeft(ellipse_array[index]);
            double y = Canvas.GetTop(ellipse_array[index]);

            double FangDaBeiShu = 2;

            Point centerPoint = new Point(x, y);
            Point pt = img_tab4.RenderTransform.Inverse.Transform(centerPoint);

            this.tlt_tab4.X = (centerPoint.X - pt.X) * this.sfr_tab4.ScaleX;
            this.tlt_tab4.Y = (centerPoint.Y - pt.Y) * this.sfr_tab4.ScaleY;
            this.sfr_tab4.CenterX = centerPoint.X;
            this.sfr_tab4.CenterY = centerPoint.Y;
            this.sfr_tab4.ScaleX += FangDaBeiShu;
            this.sfr_tab4.ScaleY += FangDaBeiShu;
            
        }

        public void jiedian_AutoMove_tab4(ref Ellipse[] ellipse_array, int index)//���㿪ʼ����
        {
            map_Reset_Click_tab4(this, null);
            
            double move_x = 0;
            double move_y = 0;

            if (this.WindowState == WindowState.Maximized)
            {
                double x = Canvas.GetLeft(ellipse_array[index]) - map_rightup_X;
                double y = Canvas.GetTop(ellipse_array[index]) - map_rightup_Y;

                double x1 = SystemParameters.PrimaryScreenWidth;//�õ���Ļ������
                double y1 = SystemParameters.PrimaryScreenHeight;//�õ���Ļ����߶�

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


            tlt_tab4.X += move_x;
            tlt_tab4.Y += move_y;
            
        }

        //���ֻ�����ڲ��ԣ�����ʵ������
        private void update_all_XY_tab4_Click(object sender, RoutedEventArgs e)
        {
            List<int> ids_list = get_exit_jiedian_id_list();
            foreach (int mem in ids_list)
            {
                Point point_temp = Ellipse_Array_tab4[(mem - 1)].TranslatePoint(new Point(0, 0), img_tab4);
                string command_str = "update " + ShuJuKu.Table3_JieDian + " SET xmin = '" + point_temp.X.ToString() + "', ymin = '" + point_temp.Y.ToString() + "' WHERE id = " + (mem).ToString() + ";";
                MySqlHelper.GetDataSet("Database='" + ShuJuKu.ShuJuKu_Name + "';Data Source='localhost';User Id='root';Password='123456';charset='utf8';pooling=true", CommandType.Text, command_str, null);
            }
            Init_Jiedian_DisplayOrNot();//ˢ�����нڵ�
        }

        public void Init_Ellipse_Array_tab4()
        {
            
        }

        public void Init_translateTransform_Array_tab4()
        {
            
        }

        public void Init_scaleTransform_Array_tab4()
        {
            
        }

        #region//��ͼ���ܵ�ʵ��
        private void img_MouseDown_tab4(object sender, MouseButtonEventArgs e)
        {
            if (sender.ToString() == "System.Windows.Shapes.Ellipse")
            {
                isMouseLeftButtonDown_tab4 = true;//ֻ���ڽڵ㴦���°������㰴��
                                                  //System.Windows.Shapes.Ellipse ellipse_tt = (System.Windows.Shapes.Ellipse)sender;
                                                  
            }
        }

        public string extract_id_from_ToolTip(string tooltip_tt)
        {
            string temp_str = tooltip_tt.Replace("�� λ �ţ�", "");
            int index_first_eol = temp_str.IndexOf('\n');
            return temp_str.Remove(index_first_eol);
        }

        private void img_MouseUp_tab4(object sender, MouseButtonEventArgs e)
        {
            
            isMouseLeftButtonDown_tab4 = false;
            //System.Diagnostics.Debug.WriteLine(e.GetPosition(canvas_mine_tab4).X.ToString(), e.GetPosition(canvas_mine_tab4).Y.ToString());
        }

        public void follow_mouse(object sender, MouseEventArgs e)
        {
            Ellipse Ellipse_temp = (Ellipse)sender;

            double x_move = e.GetPosition(canvas_mine_tab4).X;
            double y_move = e.GetPosition(canvas_mine_tab4).Y;
            Canvas.SetLeft(Ellipse_temp, (x_move - (jiedian_size / 2)));
            Canvas.SetTop(Ellipse_temp, (y_move - (jiedian_size / 2)));
        }

        private void img_MouseLeave_tab4(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown_tab4 == false)
                return;
            if (sender.ToString() == "System.Windows.Shapes.Ellipse")
            {
                //System.Windows.Shapes.Ellipse Ellipse_temp = (System.Windows.Shapes.Ellipse)sender;

                follow_mouse(sender, e);
            }
        }

        private void img_MouseMove_tab4(object sender, MouseEventArgs e)
        {
            if (isMouseLeftButtonDown_tab4 == true)
            {
                if (sender.ToString() == "System.Windows.Shapes.Ellipse")
                {
                    follow_mouse(sender, e);
                }

            }
        }

        private void img_MouseWheel_tab4(object sender, MouseWheelEventArgs e)
        {
            if (sfr_tab4.ScaleX < 0.2 && sfr_tab4.ScaleY < 0.2 && e.Delta < 0)
            {
                return;
            }
            Point centerPoint = e.GetPosition(canvas_mine_tab4);
            Point pt = canvas_mine_tab4.RenderTransform.Inverse.Transform(centerPoint);
            this.tlt_tab4.X = (centerPoint.X - pt.X) * this.sfr_tab4.ScaleX;
            this.tlt_tab4.Y = (centerPoint.Y - pt.Y) * this.sfr_tab4.ScaleY;
            this.sfr_tab4.CenterX = centerPoint.X;
            this.sfr_tab4.CenterY = centerPoint.Y;
            this.sfr_tab4.ScaleX += e.Delta / 1000.0;
            this.sfr_tab4.ScaleY += e.Delta / 1000.0;

            //��֤ScaleX��ScaleY������ֵ���ڵ���1
            if (this.sfr_tab4.ScaleX < 1 || this.sfr_tab4.ScaleY < 1)
            {
                this.sfr_tab4.ScaleX = 1;
                this.sfr_tab4.ScaleY = 1;
                
            }


            
        }
        #endregion

        private void map_Reset_Click_tab4(object sender, RoutedEventArgs e)
        {
            Init_map_location_XY(map_rightup_X, map_rightup_Y, 4);
        }

        public void clear_img_canvas_tab4()
        {
            Canvas.SetTop(img_tab4, 0);
            Canvas.SetLeft(img_tab4, 0);
        }

        public void clear_scale_tab4()
        {
            this.sfr_tab4.CenterX = 0;
            this.sfr_tab4.CenterY = 0;
            this.sfr_tab4.ScaleX = 1;
            this.sfr_tab4.ScaleY = 1;
            
        }

        public void clear_tlt_tab4()
        {
            this.tlt_tab4.X = 0;
            this.tlt_tab4.Y = 0;
            for (int i = 0; i < size_chanel; i++)//size_chanel
            {
                translateTransform_Array_tab4[i].X = 0;
                translateTransform_Array_tab4[i].Y = 0;
            }
        }



        //private void DataGrid_SelectedCellsChanged_tab4(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    jiedian a = (jiedian)this.DataGrid_tab4.SelectedItem;

        //    jiedian_AutoMove_tab4(ref Ellipse_Array_tab4, (Convert.ToUInt16(a.ID) - 1));
        //    jiedian_AutoZoom_tab4(ref Ellipse_Array_tab4, (Convert.ToUInt16(a.ID) - 1));
        //}

        private void hid_jiedian_Click_tab4(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            Ellipse_Array_tab4[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Hidden;
        }

        private void show_jiedian_Click_tab4(object sender, RoutedEventArgs e)
        {
            jiedian a = (jiedian)this.DataGrid.SelectedItem;
            Ellipse_Array_tab4[Convert.ToInt16(a.ID) - 1].Visibility = Visibility.Visible;
        }
    }
}