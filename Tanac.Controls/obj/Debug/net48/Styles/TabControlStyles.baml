   M S B A M L   `   `   `  ����_  [MaterialDesignThemes.Wpf, Version=4.5.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a{Sclr-namespace:MaterialDesignThemes.Wpf.Converters;assembly=MaterialDesignThemes.Wpf#MaterialDesignThemes.Wpf.Converters  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35c _MaterialDesignThemes.MahApps, Version=0.2.2.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aR NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   MainTabItemStyle  �� 
TargetType �� Property  PrimaryHueMidBrush �� Value �� 
TargetType �Name J� SnapsToDevicePixels � Cursor'   !System.Windows.VisualStateManager    VisualStateGroups%  System.Windows.VisualStateGroup   Transitions%  System.Windows.VisualTransition	   GeneratedDuration	
   To   From   
Storyboard   GeneratedEasingFunction/  )System.Windows.Media.Animation.CircleEase7  1System.Windows.Media.Animation.EasingFunctionBase   
EasingMode   States   System.Windows.VisualState   
Storyboard g� Duration	 W� To �� TargetProperty �� 
TargetName ��Name J� Opacity �� Property �� 	Converter=   7MaterialDesignThemes.Wpf.Converters.BrushRoundConverter   LowValue   	HighValue J� RenderTransformOrigin ��Name%   MaterialDesignThemes.Wpf.Ripple  Name   Feedback ��Name  �� Triggers! P� Property" P� Value# P� 
SourceName  MainTabControlStyle-  'System.Windows.Input.KeyboardNavigation$   TabNavigation% � ColumnDefinitions& � RowDefinitions' K� IsItemsHost(   TabIndex) K� ZIndex*   DirectionalNavigation+ ��Name, �� 
TargetName  RightToLeftMovedStoryboard- g� 	BeginTime. W� From/ W� EasingFunction.	  (System.Windows.Media.Animation.CubicEase.
  (System.Windows.Media.Animation.PowerEase  LeftToRightMovedStoryboard�� 5      H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      HmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes    5      b
convertersSclr-namespace:MaterialDesignThemes.Wpf.Converters;assembly=MaterialDesignThemes.Wpf  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      %!  &	        7  7  7  &	 d	    '�� �    7  &	 �    &	     '�� s    �� 5      "  ��6%   .�5   
   ��  � ��8   6'   �� 5   
    � ��$ Stretch=�67   �� 5   
    � ��$ Stretch=�65   �� 5	   
    � �� 5
      �� 5      " ��6"   .R�5      � $
 Root��6   $
 true��6)   $��56��6D   $��68��6P   $
 Hand��6[    5        5      CommonStates�� 5   "     5   &   $	 0:0:0.6D�6G   $
 Normal��6y    67   Z�  6c   Z�  5   *     5   .   $ EaseOut=�69   5   '   . 5   "     
Normal��6.     5   "   	MouseOver��. 5   &   �� .��5   *   W� $ 0D�6:    0.46G   $ Opacity�6P   $ mouseOverBorder��6t   W� 5   *   $ 0D�6:    0.26G   $ Opacity�6P   $ mouseOverRectangle��6t   5   '   5   #     5   "   Disabled��6.   5        5      SelectionStates�� 5    "     5!   &   $	 0:0:0.4D�6G    67   Z�  6c   Z�  6�   Z� 
 6�   Z� . 5#   "     Selected��. 5$   &   �� .��5%   *   W� $ 0D�6:    0.86G   $ Opacity�6P   $ selectedBorder��6t   W� 5&   *   $ 0D�6:    16G   $ Opacity�6N   $ selectedRectangle��6r   5'   '   5(   #     5)   "   
Unselected��6.   5*      .+�5-      �� $ mouseOverBorder��6!   $ 0Q�6:   ��5.      �� 5/   "    � �� 50   &     51   *   8  
 �����6x   53   #   55      �� 56      $ selectedBorder��6!   $ 0Q�69   $ 0.5,0.5&�6E   ��57      �� 58   "    � �� 59   &     5:   *   8  
 �����6x   5<   #   5>      �� 5?      $ contentPresenter��6+   $��Header��6E   $|�False��6\   $��Center=�6n   $��Center=�6�   $��0q�5@   +   $
��True��66   # z 6Q     5A      $ Ripple��60   #��z��6@   $|�False��6t   #��z��6�   #��z��6�   # z 6�   #��z��60   5B      �� 5C   "    � �� 5D   &     5E   *   
 �����
 �   �6Z   5G   #   5I      �� 5K      $ selectedRectangle��6I   $��4��6$   $
��Left=�6.   ���#9FA4FE$ 0Q�6s   �� 5L      $ mouseOverRectangle��6I   $��4��6$   $
��Left=�6.   ���#9FA4FE$ 0Q�6t   5M        5N      P� 5O      ! � {�$" False��67   . �5P      ��  �@J�Opacity$
 0.56Q�68   5Q      P� 5R      $# contentPresenter��6"   ! �@J�IsMouseOver$
" True��6W   . �5S      ��  �@�Cursor$
 Hand��67   5T      5V      5X      5Y      �� 5Z      "  ��6(   .�5[   
   ��  �@��TabStripPlacement$
 Left=�6.   �� 5\   
    � ��$ Stretch=�67   �� 5]   
    � ��$ Stretch=�65   �� 5^   
    � �� 5_      �� 5`      " ��6"   .R�5a      � $
}�true��6   $
 true��6/   $$ Local=�6J   % 5b      �� 5c      $
��Auto �6/   �� 5d      & 5f      �� 5g      �� 5h      $	��112 �6,   .+�5j      �� $��0��6'   $��0��67   $
��Auto=�6D   .��5k      �� 
������ $
' true��6@   $( 1��6S   $) 1��6s   5l      �� 5m      $��0��6!   $��2��6.   $��1��6?   #��z��6O   $* 	Contained=�5n   !   $( 2��5o   !   $$ Local=�6A   ._�5p      �� $ PART_SelectedContentHost��6/   $��SelectedContent��6Q   $��0q�6q   # z 6|   5q      5r      5s      5u      5v      �� 5w      "  ��6   .�5x   
   ��  � ��8   6'   �� 5y   
    � ��$ Stretch=�67   �� 5z   
    � ��$ Stretch=�65   �� 5{   
    � �� 5|      �� 5}      " ��6"   .R�5~      � $
 Root��6   $
 true��6)    5        5�      SelectionStates�� 5�   "     5�   &   $	 0:0:0.6D�6G    67   Z�  6c   Z� 
 6y   Z�  5�   *     5�   .   $ EaseOut=�69   5�   '   . 5�   "     
Unselected��6.     5�   "   Selected��. 5�   &   �� .��5�   *   W�  16:   $ selectedUnderline��6A   $ Opacity�6k   5�   '   5�   #   5�        5�      CommonStates�� 5�   "     5�   &   $	 0:0:0.6D�6G    67   Z�  6c   Z�  6�   Z� 
 6�   Z� . 5�   "     
Normal��6.     5�   "   	MouseOver��. 5�   &   �� .��5�   *   W�  0.46:   $ mouseOverUnderline��6C   $ Opacity�6n   5�   '   5�   #   5�      .+�5�      �� $+ contentPresenter��6$   #��z��6>   $|�False��6_   $��8q�6q   # z 5�   $   �� 5�      $ selectedUnderline��6$   $	��8 0q�6?   $ 0Q�6L   $��4��6X   $|�False��6c   $��Bottom=�6u   #��z��6�   �� 5�      $ mouseOverUnderline��6$   $	��8 0q�6@   $ 0Q�6M   $��4��6Y   $|�False��6d   $��Bottom=�6v   #��z��6�   5�        5�      P� 5�      ! �@J�IsMouseOver$
" True��69   . �5�      ��  �@�Cursor$
 Hand��67   5�      P� 5�      ! �@��
IsSelected$
" True��68   . �5�      �� $, contentPresenter��6%    � ��$
 Bold"�6c   5�      5�      5�      5�      �� 5�      .��5�   
   W� - 00:00:006   $ 00:00:00.600D�6/   $ RenderTransform.X�6G   . -2006u    06�   / 5�      	  5�      $ EaseOut=�6   5�      W� 5�   
   - 00:00:006   $ 00:00:00.600D�6/   $ Opacity�6G   . 06k    16t   / 5�      
  5�      $ EaseIn=�6   5�      5�      �� 5�      .��5�   
   W� - 00:00:006   $ 00:00:00.600D�6/   $ RenderTransform.X�6G   . 2006u    06�   / 5�      	  5�      $ EaseOut=�6   5�      W� 5�   
   - 00:00:006   $ 00:00:00.600D�6/   $ Opacity�6G   . 06k    16t   / 5�      
  5�      $ EaseIn=�6   5�      5�      �� 5�      "  ��6   .�5�   
   ��  � ��$ Stretch=�67   �� 5�   
    � ��$ Stretch=�65   �� 5�   
    � �� 5�      �� 5�      " ��6"   .R�5�      � $
}�true��6   $
 true��6/   $$ Local=�6J   & 5�      �� 5�      $
��Auto �6,   �� 5�      .+�5�      �� 
������ $
' true��6<   $( 1��6O   $) 1��6o   �� 5�      $��1��6!   $��0q�6.   $* 	Contained=�5�   !   $( 2��5�   !   $$ Local=�6A   ._�5�      �� $ PART_SelectedContentHost��6/   $��SelectedContent��6Q   $��0q�6q   # z 6|   5�      5�      5�      5�      5�      5�      