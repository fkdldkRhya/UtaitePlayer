   M S B A M L   `   `   `  ���� Y  URHYANetwork.UtaitePlayer.Setup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullm;clr-namespace:RHYANetwork.UtaitePlayer.Setup.Layout.Windows-RHYANetwork.UtaitePlayer.Setup.Layout.Windows  >    8RHYANetwork.UtaitePlayer.Setup.Layout.Windows.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Mlocal;clr-namespace:RHYANetwork.UtaitePlayer.Setup.Layout.Windows     S OHandyControl, Version=3.4.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  -     � Title$?  9우타이테 플레이어 (Utaite Player) Install Manager�� � WindowStyle$ 
ToolWindow=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=�$��131.333��$	��500������  �� ResourceKey$ CustomWindowTitleBarStyle��.���    Color_Gray3#���    � RowDefinitions �� $
��Auto ��� .+�]� ��.  $��2�� � 	Resources  �� 
TargetType �� Property	 �� Value'  !System.Windows.Shell.WindowChrome  Color_White1
 �� 
TargetType �� Triggers P� Property P� Value  Color_Gray2��%�  '��       '�� �    �� " ��.���  � ��$)	 #/Resources/Font/#Malgun Gothic Bold)���  � ��$
	 Bold"���  � ��$	 Right=���  � ��$	 40����   �@ IsHitTestVisibleInChrome	 . ��  � ��#	 �  ��  � ��$	 0q���  � ��
	 ���� ��  � ��	 �� "
 ��.R��� #��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��#	 �  P�  � �� . . ���  � ��#	 �   �� " ��.���  � ��$	 5,0,0,0q���  � ��$	 Center=���  � ��$
	 Left=���  � ��$&	  /Resources/Font/#esamanru Medium)���  � ��$
	 Auto����  � ��
	 �������  � ��$	 0q���  � ��
	 ���� .+��� -    ��Name$ closeButton��$
��auto��$
��auto��.��F� $��46��$��32��+���Ѐ��ا ���� -1Ѐ� -1�����ا 	$��1������ *
Foreground+ �� RelativeSource ��  �� AncestorType" ���� -   $ minimizeButton��$
��auto��$
��auto�� �� EdgeMode$ Aliased=�.��F� $��46��$��32�����Ѐ���� � ?���� 	$��1������ *
Foreground+ �� " ���� $?��9우타이테 플레이어 (Utaite Player) Install Manager��$��11(�#���  $
��Left=�� $��1��.+��� -    ��Name$ SetupProgressBar_Message��$��Initializing...��$	��Top=�$
��Wrap=�$%��/Resources/Font/#esamanru Light)�$��11(�$��CharacterEllipsis=�#���  $��
26,19,26,0q�� -    �Name$ SetupProgressBar��  HandyControlThemes# [ $��15��$��
25,36,25,0q�$	��Top=�