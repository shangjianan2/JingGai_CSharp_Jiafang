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

using System.Windows.Media.Animation;

using System.Collections;

namespace WpfApp1
{
    /// <summary>
    /// MultiComboBox.xaml �Ľ����߼�
    /// </summary>
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public partial class MultiComboBox : ComboBox
    {
        /// <summary>
        /// ��ȡѡ�����
        /// </summary>
        public IList SelectedItems
        {
            get { return this._ListBox.SelectedItems; }
        }

        /// <summary>
        /// ���û��ȡѡ����
        /// </summary>
        public new int SelectedIndex
        {
            get { return this._ListBox.SelectedIndex; }
            set { this._ListBox.SelectedIndex = value; }
        }

        private ListBox _ListBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._ListBox = Template.FindName("PART_ListBox", this) as ListBox;
            this._ListBox.SelectionChanged += _ListBox_SelectionChanged;
        }

        void _ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this.SelectedItems)
            {
                sb.Append(item.ToString()).Append(";");
            }
            this.Text = sb.ToString().TrimEnd(';');
        }


        static MultiComboBox()
        {
            //OverridesDefaultStyleProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(typeof(MultiComboBox)));
        }

        public MultiComboBox()
        {
            ListBox ls = new ListBox();
            //ls.SelectedItems
        }

        public void SelectAll()
        {
            this._ListBox.SelectAll();

        }

        public void UnselectAll()
        {
            this._ListBox.UnselectAll();
        }
    }
}
