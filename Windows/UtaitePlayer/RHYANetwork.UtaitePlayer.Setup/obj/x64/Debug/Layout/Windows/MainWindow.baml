   M S B A M L   `   `   `  ����Y  URHYANetwork.UtaitePlayer.Setup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullm;clr-namespace:RHYANetwork.UtaitePlayer.Setup.Layout.Windows-RHYANetwork.UtaitePlayer.Setup.Layout.Windows  5   	   >    8RHYANetwork.UtaitePlayer.Setup.Layout.Windows.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Mlocal;clr-namespace:RHYANetwork.UtaitePlayer.Setup.Layout.Windows     5   	   S OHandyControl, Version=3.4.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  5   	   -   5          � Title$?  9우타이테 플레이어 (Utaite Player) Install Manager��5   	    � WindowStyle$ 
ToolWindow=�5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   $��131.333��5   	   $	��500��6   ��5      �� 5   
    �� ResourceKey$ CustomWindowTitleBarStyle��6   .��5      �    Color_Gray3#���   6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5"   
   ]� ��.  $��2��5#       � 	Resources 5$       �� 
TargetType �� Property	 �� Value'  !System.Windows.Shell.WindowChrome  Color_White1
 �� 
TargetType �� Triggers P� Property P� Value  Color_Gray2��%a  '��       '�� �    �� 5&      " ��6   .�5'      ��  � ��$)	 #/Resources/Font/#Malgun Gothic Bold)�63   �� 5(       � ��$
	 Bold"�63   �� 5)       � ��$	 Right=�67   �� 5*       � ��$	 40��6.   �� 5+        �@ IsHitTestVisibleInChrome	 . 6N   �� 5,       � ��#	 �  63   �� 5-       � ��$	 0q�68   �� 5.       � ��
	 ���� 63   �� 5/       � ��	 50      �� 51      "
 ��6.   .R�52   "   �� #��z��6)   ._�53   &   �� $��Center=�67   $��Center=�54   7   55   #   56      58       5:      P� 5;       �@J�IsMouseOver . . �5<      ��  � ��#	 �  6;   5=      P� 5>       � �� . . �5?      ��  � ��#	 �   6;   5@      5B      �� 5D      " ��6   .�5E      ��  � ��$	 5,0,0,0q�6/   �� 5F       � ��$	 Center=�6:   �� 5G       � ��$
	 Left=�67   �� 5H       � ��$&	  /Resources/Font/#esamanru Medium)�63   �� 5I       � ��$
	 Auto��6.   �� 5J       � ��
	 �����63   �� 5K       � ��$	 0q�68   �� 5L       � ��
	 ���� 63   5M      5N      .+�5Q      �� -   5         ��Name$ closeButton��5R      $
��auto��5S      $
��auto��5T      .��5W      F� $��46��5X      $��32��5Y      +���Ѐ��ا ���� -1Ѐ� -1�����ا 	$��1��5\      ��5[      �� *
Foreground+ �� RelativeSource ��  �� AncestorType" ��5\      5]      �� 5`      -   5        $ minimizeButton��5a      $
��auto��5b      $
��auto��5c       �� EdgeMode$ Aliased=�5d      .��5g      F� $��46��5h      $��32��5i      ���Ѐ���� � ?���� 	$��1��5l      ��5k      �� *
Foreground+ �� " ��5l      5m      �� 5p      $?��9우타이테 플레이어 (Utaite Player) Install Manager��5q      $��11(�5r      #���  5s      $
��Left=�5t      5u      � 5w   
   $��1��6   .+�5x      �� -   5         ��Name$ SetupProgressBar_Message��5y      $��Initializing...��5z      $	��Top=�5{      $
��Wrap=�5|      $%��/Resources/Font/#esamanru Light)�5}      $��11(�5~      $��CharacterEllipsis=�5      #���  5�      $��
26,19,26,0q�5�      � 5�      -   5         �Name$ SetupProgressBar��5�        HandyControlThemes# [ 5�      $��15��5�      $��
25,36,25,0q�5�      $	��Top=�5�      5�      5�      5�      