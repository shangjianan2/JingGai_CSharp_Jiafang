   M S B A M L   `   `   `  ���� B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  [ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration     WpfApp1.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:WpfApp1     Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     6sys&clr-namespace:System;assembly=mscorlib     _wfiOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration       � Title$1  +燃气管道井远程无线监控系统v1.1��$	��700��$
��1000�� � 	Resources  �� 
TargetType �� Property �� Value �� ImageSource   TextSelectionBrush �� Color  
FontFamily  FontSize  TextBackground  TextForeground  ControlBorderBrush  FocusBorderBrush  MouseOverBorderBrush  WatermarkOpacity 	 DisableOpacity 
 ReadonlyOpacity  TextLabelBackground  DefaultTextBox  TextBoxContextMenu#   WpfApp1.ControlAttachProperty �� 
TargetType �Name	 ��Name
 J� SnapsToDevicePixels   CornerRadius �� CornerRadius � ColumnDefinitions ��Name   LabelTemplate   Label ��Name ��Name �� Padding   	Watermark J� IsHitTestVisible J� Opacity   AttachContent �� Triggers �� Value �� Binding �� RelativeSource �� Path �� 
TargetName P� Property P� Value  LabelTextBox  �� BasedOn  ComboBoxItemStyle! ��Name  ItemSelectedBackground  ItemMouseOverBackground  EditableTextBoxStyle" �� Triggers  DefaultComboBox# K� ZIndex$ d�Name% �� 	ClickMode& d� 	IsChecked' �� Mode( �� TextAlignment) �� SelectionBoxItemTemplate* �� SelectionBoxItem#+ �� SelectionBoxItemStringFormat, �� ContentStringFormat- ��Name. �� 
IsReadOnly/ �� 
IsReadOnly0 �� Text1 �Name2 � AllowsTransparency3 ��Name-  'System.Windows.Input.KeyboardNavigation4   DirectionalNavigation  LabelComboBox  JieDians5 S� 
ObjectType   WpfApp1.JieDians  JieDians_tab6   WpfApp1.JieDians_tab6  ContextMenu  ContextMenu_DataGrid_tab2  ContextMenu_DataGrid_tab3  ContextMenu_tab4��%M  '��       &	  U     &	 h     &	 �     &	 �     &	 �     &	 �     &	 �     &	 �     &	 �     &		 �     &	
 �     &	     &	     7   7  7  7  7  7  7  7  7  7  7 	 7 
 7 
 7 
 &	 :    7  7  &	 �    7  7  &	 �	    7  7   7  7  7  7 
 7 	 &	 n    7  7  7  7  7  7  7  7  7  7  7 	 &	 �    7  7  &	     &	 )    &	 3    &	 h    &	     &	 E    �� " ��.���  � �� �� $ C:\NBIoT\button.png����  � ��
 ������� $ #8F8787��*�Microsoft YaHei)�X�15Q��� $ White���� $
 Blue���� $
 Blue���� $ #EBCECE���� $ #F6D1D1��X�0.4Q�X�0.5Q�X�0.88Q��� $ White���� " ��.���  �@�ContextMenu# �  ��  �@��SelectionBrush8   ��  � ��8  ��  � ��8  ��  � ��$ 1q���  � ��$ 26����  � ��$	 100����  � ��8  ��  � ��8  ��  � ��$ 0q���  � ��8  ��  �@ FocusBorderBrush8  ��  �@ MouseOverBorderBrush8  ��  � ��$ Center=���  �@J�SnapsToDevicePixels . ��  �@��
CaretBrush8  ��  � �� �� " ��.R�� $ 	PART_Root��.+��� $	 Bg��#
 z
 # z #��z��#��z��#��z��� $ PART_InnerGrid�� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $ Label��$��1q�#��z #��z �� $ PART_ContentHost��$��0q�$��1����.  $��2q�$��Stretch=���Z� �� $ Message��# z��$y�	Collapsed=�#��z $��1��#��z�� .  8 	 #��z��#��z��$��5,2,5,2q��� $	 PART_AttachContent��$��2��$��2q�$��Center=�$��Center=�._��� $��Center=�$��Center=�#��z  �� $  �� ��  �� *Self+$
 Text�.a��� $ Message�� � y�$ Visible=�P�  �@J�IsMouseOver . . ���  �� $8 2(local:ControlAttachProperty.MouseOverBorderBrush)� �� *Self+ � ��P�  �@J�	IsFocused . . ���  �� $4 .(local:ControlAttachProperty.FocusBorderBrush)� �� *Self+ � ��P�  � {� .  . ��� $ 	PART_Root��8 
  �@J�OpacityP�  �@��
IsReadOnly . . ��� $ PART_AttachContent�� � {� .  �� $ Bg��8   �@J�Opacity�� $ PART_ContentHost��8   �@J�Opacity�� $ Label��8   �@J�Opacity�� " ��8    .���  �@ LabelTemplate �� " ��.R��� $��90��8�� ._��� $��Center=�$��Right=�$��3q�#��z���� " ��.���  � ��$ Stretch=���  � ��$ Center=���  �@��ClearTypeHint$ Enabled=���  � ��$ 0q���  � ��$ 28����  � �� �� " ��.R�� #��z��$��0,0.5q�.+��� $	 ItemBackground�� .  #
 z
 #��z��#��z��#��z���� $! contentPresenter��#��z��#��z��#��z�� P�  �@��
IsSelected . . ��� $ ItemBackground��8    � ��P�  �@J�IsMouseOver . . ��� $ ItemBackground��8   � ���� " ��.���  � ��$ 1q���  � ��$ 0q���  � ��$ 0q���  � �� Z� ��  �@��	MaxLength$
 2048����  � ��8   ��  �@�ContextMenu# �  ��  �@��SelectionBrush8  ��  � ��8  ��  � ��8  ��  � |� . ��  �@��
CaretBrush8  ��  � ��$ Center=���  �@J�SnapsToDevicePixels . " P�  �@��
IsReadOnly . . ���  �@J�Opacity8  P�  � {� .  . ���  �@J�Opacity8  �� " ��.���  � ��8   ��  � ��8  ��  � ��$ Stretch=���  � ��$ Center=���  � ��$ 1q���  � ��8  ��  �@ FocusBorderBrush8  ��  �@ MouseOverBorderBrush8  ��  � ��8  ��  � ��8  ��  � ��8  ��  �@J�SnapsToDevicePixels . ��  �@��MaxDropDownHeight$	 400����  � �� .  ��  � �� �� " ��.R�� $ 	PART_Root��.+��� $	 Bg��#
 z
 # z #��z��#��z��#��z���  �� $��92 ��� $��* ��� $��20 �.+��� $ Label��$��1q�#��z #��z �� $	 PART_AttachContent��$# 2��$��2��$��Center=�$��Center=�._��� $��Center=�$��Center=�#��z d� $$ PART_DropDownToggle��$��2��$��2��|�.  $% Press=�& �� $ IsDropDownOpen�$' TwoWay=� �� *TemplatedParent+���� $ C:\NBIoT\button.png���� $��1��._��� $ Message��$ 0q�$y�	Collapsed=�#��z #��z�� .  8  $
��Left=�$( Center=�$��Center=�$��5,2,5,2q�� $ ContentSite��$��1��$��2 0 0 0q�.+��� $! PART_SelectedItem��#��z) #��z��#��z* #, z+ $��Stretch=�$��2,0,2,0q� .  #
 z
 #��z���� $- PART_EditableTextBox��8��	 $y�	Collapsed=�$��Stretch=�#
 z
  . #��z��#��z��#/ z. #��z��#��z��#��z0 #��z��� $1 
PART_Popup��2 . |�.  #��� .�$��Bottom=����� *IsDropDownOpen+ �� *TemplatedParent+.��� ���� *ActualWidth+ �� *TemplatedParent+���� *MaxDropDownHeight+ �� *TemplatedParent+.+��� $	 PopupBorder��#��z��$��Stretch=�$
��Auto��#��z��
��������� $ DropDownScrollViewer��$��0q�$��1q�.���� $3 ItemsPresenter��$4 	Contained=� �� $  �� ��  �� *Self+$
 Text�.a��� $ Message�� � y�$ Visible=�P�  �@��
IsEditable . . ��� $ PART_EditableTextBox�� � y�$ Visible=��� $ PART_SelectedItem�� � y�$ 	Collapsed=��� $ PART_DropDownToggle�� � ��$ 3���� $ PART_DropDownToggle�� � ��$ 1���� $ PART_DropDownToggle�� � ��
 ���� ��  � �� .  �� $ PART_DropDownToggle�� � |� .  P�  �@J�IsMouseOver . . ���  �� $8 2(local:ControlAttachProperty.MouseOverBorderBrush)� �� *Self+ � ��P�  �@J�	IsFocused . . ���  �� $4 .(local:ControlAttachProperty.FocusBorderBrush)� �� *Self+ � ��P�  �@J�IsKeyboardFocusWithin . . ���  �� $4 .(local:ControlAttachProperty.FocusBorderBrush)� �� *Self+ � ��P�  � {� .  . ��� $ 	PART_Root��8 
  �@J�Opacity�� " ��8    .���  � ��$	 260����  �@ LabelTemplate �� " ��.R��� $��92��8�� ._��� $��Center=�$��Right=�$��5q�#��z��S� "5  S� "5  �� .@�m� $��	map_Reset��-   $��恢复比例���� .@�m� $��hid_jiedian��-   $��隐藏节点��m� $��show_jiedian��-   $��显示节点��m� $��clear_datagrid_tab2��-   $��清空显示���� .@�m� $��clear_datagrid_tab3��-   $��清空显示���� .@�m� $��map_Reset_tab4��-   $��恢复��m� $��update_all_XY_tab4��-   $��更新所有节点坐标��.��� .+��� -   6 ��Name$6 
tabcontrol��$��Stretch=�$
��Auto��$��-6,-10,-6,-6q�$��Stretch=�$
��Auto��.@��� -	   7 ��Name$
7 Tab0��$��0��
���   �.���� $��C:\NBIoT\FirstPicture.png��$
��Fill=��� -
   $
7 Tab1��$��0��
���   �.��� .+�r� -   8 r�Name$8 SecondPicture_tab1��9 r� Source$ 9 C:\NBIoT\SecondPicture.mp4?�: r� LoadedBehavior$: Manual=�; r� Stretch$
; Fill=��� -   $
7 Tab2��$��0��
���   �.��� < � RowDefinitions< �� $	��100 ��� $��* �.+�� $��1�� �� $��* ��� $��* ��� $��* ��� $��* �.+��� -   = ��Name$= canvas_mine��$��3��$��0��$��3��> � ContextMenu#> [ z�V� .~��� -   sfr��T� -   tlt��.+��� -   ? ��Name$	? img��$��C:\NBIoT\timg.jpg��$��Uniform=�@ �� BitmapScalingMode$@ NearestNeighbor=�
A �� Top$A 0��B �� Left$B 0��� $��3��< �� $��50 ��� $��50 ��� $��* � �� $��* ��� $��* �.+��� -   C ��Name$C LieBiao_Tab2_Buttton��$��0��$��0��$��列表模式���� -   $C ChaXun_Tab2_Buttton��$��1��$��0��$��查询统计���� -   $C XiTong_Tab2_Buttton��$��0��$��1��$��系统设置���� -   $C TuiChu_Tab2_Buttton��$��1��$��1��$��退出系统���� $��2��$��0��$��2��
�������� $��2��$��0��$��2��< �� $��25 ��� $��* �.+��� $��0��$��Center=�$��Center=�
����  �实 时 信 息&   System.Windows.Controls.DataGrid  -   D  Name$D DataGrid��$��1��E   
IsReadOnlyE . #> [   DefaultDataGrid#��[ $��10, 0, 10, 10q�$��Hidden=�$��Hidden=����� F �� Source#F [ G   ColumnsG 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumnH   Width5  /System.Windows.Controls.DataGridLengthConverter$H 60 I   Header$I 编号��1	  +System.Windows.Controls.DataGridBoundColumnJ 	  BindingJ �� *ID+  $	H 100 $I 传感器状态��J �� *Status+  $	H 120 $I 时间��J �� *Date+�� 
����ح�$��0��$��1���� $��0��$��1��$��Center=�$��Center=�$��40(�
����  �)'燃气管道井远程无线监控系统�� -   $
7 Tab3��$��0��
���   �.��� < �� $	��100 ��� $��* �.+�� $��1�� �� $��* ��� $��* ��� $��* ��� $��* �.+�7
  1System.Windows.Forms.Integration.WindowsFormsHost
  $��0��$��3��K 
  Child.K #  System.Windows.Forms.ListView  -   listview_largeicon��� $��3��< �� $��50 ��� $��50 ��� $��* � �� $��* ��� $��* �.+��� -   $C DiTu_Tab3_Button��$��0��$��0��$��地图模式���� -   $C ChaXun_Tab3_Button��$��1��$��0��$��查询统计���� -   $C XiTong_Tab3_Button��$��0��$��1��$��系统设置���� -   $C TuiChu_Tab3_Button��$��1��$��1��$��退出系统��� $��2��$��0��$��2��< �� $��25 ��� $��* �.+��� $��0��$��Center=�$��Center=�
����  �实 时 信 息  -   $D DataGrid_tab3��$��1��E . #��[ #> [ $��10, 0, 10, 10q�$��Hidden=�$��Hidden=����� #F [ G   $H 60 $I 编号��J �� *ID+  $	H 100 $I 传感器状态��J �� *Status+  $	H 120 $I 时间��J �� *Date+� $��2��$��0��$��2���� 
����ح�$��0��$��1���� $��0��$��1��$��Center=�$��Center=�$��40(�
����  �)'燃气管道井远程无线监控系统�� -   $
7 Tab4��$��0��
���   �.��� < �� $	��100 ��� $��* �.+�� $��1�� �� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $= canvas_mine_tab4��$��3��$��0��$��3��#> [ z�V� .~��� -   sfr_tab4��T� -    tlt_tab4��.+��� -!   $? img_tab4��$��C:\NBIoT\timg.jpg��$��Uniform=�$@ NearestNeighbor=�$A 0��$B 0��� $��3��< �� $��50 ��� $��50 ��� $��50 ��� $��* � �� $��* ��� $��* �.+��� -"   $C DiTuMoShi_Tab4_Buttton��$��0��$��0��$��地图模式���� -#   $C LieBiaoMoShi_Tab4_Buttton��$��1��$��0��$��列表模式���� -$   $ C XinXiBianGeng_Tab4_Buttton��$��0��$��1��$��信息变更���� -%   $C ZengShan_Tab4_Buttton��$��1��$��1��$��增删点位���� -&   $C PuTongMoShi_Tab4_Button��$��0��$��2��$��返回���� -'   $!C Verify_PassWord_Tab4_Button��$��1��$��2��$��修改密码���� $��3��$��0��$��2��
�������� $��3��$��0��$��2�� �� $��* ��� $��* ��� $��* �< �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��30 �.+��� -(   $- BianHao��$��0��$��0��$��3��$
��Auto��$ 请输入编号��#��[ $ 编       号：��.���� -)   $- 
JianCeQiTi��/ . $��0��$��1��$��3��$
��Auto��$ 请输入气体类型��#��[ $ 检测气体：��.���� -*   $- AnZhuangWeiZhi��$��0��$��2��$��3��$
��Auto��$ 请输入安装位置��#��[ $ 安装位置：��.���� -+   $- AnZhuangShiJina��$��0��$��3��$��3��$
��Auto��$ 请输入安装时间��#��[ $ 安装时间：��.���� -,   $- GaoXianBaoJing��/ . $��0��$��4��$��3��$
��Auto��$ 请输入高限报警��#��[ $ 高限报警：��.���� --   $- DiXianBaoJing��/ . $��0��$��5��$��3��$
��Auto��$ 请输入低限报警��#��[ $ 低限报警：��.��� $��10��$��3�� �� $��* ��� $��* ��� $��* �.+��� -.   $C QueRenBianGeng_Button��$��0��$��确认变更���� -/   $C ZengJiaBianGeng_Button��$��1��$��增加点位���� -0   $C ShanChuDianWei��$��2��$��删除点位���� 
����ح�$��0��$��1���� $��0��$��1��$��Center=�$��Center=�$��40(�
����  �42燃气管道井远程无线监控系统(管理员)�� -1   $
7 Tab5��$��0��
���   �.��� < �� $	��100 ��� $��* �.+�� $��1�� �� $��* ��� $��* ��� $��* ��� $��* �.+�
  $��0��$��3��.K   -2   listview_largeicon_tab5��� $��3��< �� $��50 ��� $��50 ��� $��50 ��� $��* � �� $��* ��� $��* �.+��� -3   $C DiTuMoShi_Tab5_Buttton��$��0��$��0��$��地图模式���� -4   $C LieBianMoShi_Tab5_Buttton��$��1��$��0��$��列表模式���� -5   $ C XinXiBianGeng_Tab5_Buttton��$��0��$��1��$��信息更改���� -6   $C ZengShan_Tab5_Buttton��$��1��$��1��$��增删点位���� -7   $C PuTongMoShi_Tab5_Button��$��0��$��2��$��返回���� -8   $!C Verify_PassWord_Tab5_Button��$��1��$��2��$��修改密码���� $��3��$��0��$��2��
�������� $��3��$��0��$��2�� �� $��* ��� $��* ��� $��* �< �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��30 �.+��� -9   $- BianHao_tab5��$��0��$��0��$��3��$
��Auto��$ 请输入编号��#��[ $ 编       号：��.���� -:   $- JianCeQiTi_tab5��/ . $��0��$��1��$��3��$
��Auto��$ 请输入气体类型��#��[ $ 检测气体：��.���� -;   $- AnZhuangWeiZhi_tab5��$��0��$��2��$��3��$
��Auto��$ 请输入安装位置��#��[ $ 安装位置：��.���� -<   $- AnZhuangShiJina_tab5��$��0��$��3��$��3��$
��Auto��$ 请输入安装时间��#��[ $ 安装时间：��.���� -=   $- GaoXianBaoJing_tab5��/ . $��0��$��4��$��3��$
��Auto��$ 请输入高限报警��#��[ $ 高限报警：��.���� ->   $- DiXianBaoJing_tab5��/ . $��0��$��5��$��3��$
��Auto��$ 请输入低限报警��#��[ $ 低限报警：��.��� $��10��$��3�� �� $��* ��� $��* ��� $��* �.+��� -?   $ C QueRenBianGeng_Button_tab5��$��0��$��确认变更���� -@   $!C ZengJiaBianGeng_Button_tab5��$��1��$��增加点位���� -A   $ C ShanChuDianWei_Button_tab5��$��2��$��删除点位���� 
����ح�$��0��$��1���� $��0��$��1��$��Center=�$��Center=�$��40(�
����  �)'燃气管道井远程无线监控系统�� -B   $
7 Tab6��$��0��
���   �.��� < �� $	��100 ��� $��* �.+�� $��1�� �� $��* ��� $��* ��� $��* ��� $��* �.+�  -C   $D DataGrid_tab6��$��0��$��3��E . L   AutoGenerateColumnsL .  ���� #F [ G   $	H 120 $I 编号��J �� *ID+  $	H 120 $I 气体浓度��J �� *
Gas_Type+  $	H 120 $I 单位��J �� *DanWei+  $	H 120 $I 传感器状态��J �� *Status+  $	H 120 $I 浓度��J �� *NongDu+  $	H 120 $I 低限报警��J �� *DiXian+  $	H 120 $I 高限报警��J �� *	GaoXian+  $	H 120 $I 电量��J �� *	DianLiang+  $	H 120 $I 温度��J �� *WenDu+  $	H 120 $I 时间��J �� *Date+  $	H 120 $I 安装地点��J �� *
Location+  $	H 120 $I 安装时间��J �� *Time_of_install+� $��3��< �� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��* ��� $��50 � �� $��* ��� $��* ��� $��* �.+��� $��按时间查询��$��0��$��2���� -D   $C ChaXun1_tab6��$��查询��$��2���� -E   $- QiShi1_TextBox_tab6��$��0��$��1��$��3��$
��Auto��$ 请输入起始时间��#��[ $ 起       始：���� -F   $- ZhongZhi1_TextBox_tab6��$��0��$��2��$��3��$
��Auto��$ 请输入终止时间��#��[ $ 终       止：���� $��按探测器查询��$��3��$��0��$��2���� -G   $C ChaXun2_tab6��$��查询��$��3��$��2���� -H   $- BianHaoChaXun1_tab6��$��0��$��4��$��3��$
��Auto��$ 请输入节点编号��#��[ $ 编       号：���� $��精确查询��$��5��$��0��$��2���� -I   $C ChaXun3_tab6��$��查询��$��5��$��2���� -J   $- BianHaoChaXun2_tab6��$��0��$��6��$��3��$
��Auto��$ 请输入节点编号��#��[ $ 编       号：���� -K   $- QiShi2_TextBox_tab6��$��0��$��7��$��3��$
��Auto��$ 请输入起始时间��#��[ $ 起       始：���� -L   $- ZhongZhi2_TextBox_tab6��$��0��$��8��$��3��$
��Auto��$ 请输入终止时间��#��[ $ 终       止：���� -M   M ��Name$M WarningStyle_ComboBox��$��0��$��9��$��3��$
��Auto��$ 报警状态：��#��[ $ 请选择报警状态��.@��� $��气体报警���� $��低电量报警���� $��低温报警���� $��水位报警���� $��井盖被盗报警���� $��所有报警���� $��所有故障���� $��0��$��11���� -N   $C DaoChuExcel_Button��$��导出excel��$��1��$��11���� -O   $C 	Back_tab6��$��返回��$��2��$��11���� 
����ح�$��0��$��1���� $��0��$��1��$��Center=�$��Center=�$��40(�
����  �)'燃气管道井远程无线监控系统