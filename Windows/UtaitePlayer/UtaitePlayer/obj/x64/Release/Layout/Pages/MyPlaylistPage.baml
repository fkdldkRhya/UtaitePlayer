   M S B A M L   `   `   `  ���� S  OUtaitePlayer, Version=1.1.0.7, Culture=neutral, PublicKeyToken=1db8b1a767e76951E'clr-namespace:UtaitePlayer.Layout.PagesUtaitePlayer.Layout.Pages  P LVirtualizingWrapPanel, Version=1.5.7.0, Culture=neutral, PublicKeyToken=nullX@clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanelWpfToolkit.Controls .    (UtaitePlayer.Layout.Pages.MyPlaylistPage   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     b ^SharpVectors.Converters.Wpf, Version=1.7.7.0, Culture=neutral, PublicKeyToken=b532964b8548be771svgc&http://sharpvectors.codeplex.com/svgc/  S OHandyControl, Version=3.3.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  3local'clr-namespace:UtaitePlayer.Layout.Pages  Ocontrols@clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanel  -     N� Title$  MyPlaylistPage��.���  � RowDefinitions �� $
��auto �$��5���� $��40 ��� $��* �.+�� -    �Name$ LoadingProgressBar��$��0��$y�	Collapsed=�   HandyControlThemes � 	Resources# [    ProgressBarFlat#��[  � IsIndeterminate . $��Bottom=�$��5����Z� ��Z� �� $��나의 플레이리스트��$��Center=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�$��1��  Color_White1#���  $��10,0,0,0q�$
��Left=��� -   # [  $	��123��$��30��$��2��$��Right=�$��Center=�$��0,5,10,5q�.����  �� Orientation$ 
Horizontal=�.+��� $��15��$��15��$F��@/UtaitePlayer;component/Resources/drawable/ic_plus_for_white.png���� $��Center=�$%��/Resources/font/#esamanru Light)�$��9(�#���  $"��플레이리스트 만들기��$��5,0,0,0q�� -    �Name$ myPlaylistRootGrid��$��2��.+�� -   $ noResult��$��Center=�$��Center=�$
��0,15q�.+�(  "SharpVectors.Converters.SvgViewbox     Source$- '/Resources/drawable/img_character_1.svg?�$��0q�$	��225��$	��175��$	��Top=��� $@��:플레이리스트 데이터가 존재하지 않습니다.��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�#���  $��0q�$��Center=��� -    ��Name$ myPlaylistListBox��$��0q�# [  !	 %� IsVirtualizingWhenGrouping	 . 
 %� VirtualizationMode$
 	Recycling=� %� IsVirtualizing . $��0,10,0,0q����� .R�/  )WpfToolkit.Controls.VirtualizingWrapPanel     Orientation$ Vertical=� � ContextMenu �� .@�m� -   $��전체 재생��$%��/Resources/font/#esamanru Light)�$��12(� m� Icon F� $��12��$��16��������s�9� � >I �\ � u8  5  �$  j  �$ ��* � �$ ��� �$ `< u8 `q# >I �Iрs�l� ����D� ���a� �s�9� 	
������� �� Stretch$
 Fill=�m� -   $��플레이리스트 편집��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$<��6/UtaitePlayer;component/Resources/drawable/ic_edit.png��m� -	   $��플레이리스트 제거��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$>��8/UtaitePlayer;component/Resources/drawable/ic_delete.png������ .R�� $	��300��$	��110��$��10,10,10,10q� �� $
��auto ��� $��* � � ColumnDefinitions �� $	��110 ��� $��* �.+��� $	��110��$	��110��$
��Left=�$
��Fill=�$��2������ *image+�� $	��180��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��14(�#���  $��
15,10,15,0q�$��1������ *name+�� $	��Top=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��12(�  Color_White3#���  $��
15,10,15,0q�$��1��$��1������ *count+�� $	��180��$��Bottom=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��
15,0,15,10q�$��1��$��1��$��CharacterEllipsis=� �� TextAlignment$ Right=����� *	account+� -
   $$ x_PlaylistInfoLayout_RootPanel��$y�	Collapsed=�$��2��  Color_Gray3#���   �� $��40 ��� $��* �.+��� $��플레이리스트 정보��$��Center=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�#���  $��10,0,0,0q�$
��Left=��� -   $��16��$��16��$��Right=�$��Center=�$��0,0,15,0q�  �� 
TargetType'  !System.Windows.Shell.WindowChrome �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value��%M  '��       �� " ��.���   �@ IsHitTestVisibleInChrome . ��  � ��# �  ��  � ��$ 0q���  � ��
 ���� ��  � �� �� " ��.R��� #��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  �@J�Opacity$	 0.7Q�P�  � �� . . ���  �@J�Opacity$	 0.5Q�.��F� $
 Fill=�$��15��$��15��+���Ѐ��ا ���� -1Ѐ� -1�����ا 	$��2��$��Center=�$��Center=����� *
Foreground+ �� RelativeSource ��  �� AncestorType" ��� $��1�� �� $	��180 ��� $��* �.+��  �� $	��180 ��� $��* �.+��� -    ��Name$( "x_PlaylistInfoLayout_PlaylistImage��$��20,0,0,0q�$��UniformToFill=�$@��:/UtaitePlayer;component/Resources/drawable/img_no_data.png��$	��140��$	��140��$
��Left=�$��Center=��� $��1��$ Vertical=�.+��� -    ��Name$% x_PlaylistInfoLayout_MusicCount��$	��Top=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��	0,35,20,0q��� -   $( "x_PlaylistInfoLayout_PlaylistTitle��$	��Top=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��19(�#���  $��0,0,20,0q��� -   $&  x_PlaylistInfoLayout_AccountName��$	��Top=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��12(�  Color_Gray6#���  $��	0,20,20,0q��� -   $$ x_PlaylistInfoLayout_AccountID��$	��Top=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��14(�  Color_White2#���  $��0,0,20,0q��� $ 
Horizontal=�$��0,5,0,0q�.+��� -   # [  $	��183��$��35��$
��Left=�$��Center=�$��0q�.���� $ 
Horizontal=�.+��� $	��Top=�$
��Left=�$��12��$��12��$<��6/UtaitePlayer;component/Resources/drawable/ic_edit.png���� $��Center=�$%��/Resources/font/#esamanru Light)�$��11(�#���  $��플레이리스트 수정��$��10,0,0,0q��� -   # [  $	��183��$��35��$
��Left=�$��Center=�$��20,0,0,0q�.���� $ 
Horizontal=�.+��� $	��Top=�$
��Left=�$��12��$��12��$<��6/UtaitePlayer;component/Resources/drawable/ic_save.png���� $��Center=�$%��/Resources/font/#esamanru Light)�$��11(�#���  $��플레이리스트 저장��$��10,0,0,0q��� -   # [  $	��103��$��35��$
��Left=�$��Center=�$��20,0,0,0q�.���� $ 
Horizontal=�.+�F� $��10��$��12��������s�9� � >I �\ � u8  5  �$  j  �$ ��* � �$ ��� �$ `< u8 `q# >I �Iрs�l� ����D� ���a� �s�9� 	
�������$
 Fill=��� $��Center=�$%��/Resources/font/#esamanru Light)�$��11(�#���  $��전체 재생��$��10,0,0,0q�� $��1��.+�#  HandyControl.Controls.Divider     LineStrokeThickness$ 1Q�$
��Left=�# [  $	��Top=�$%��/Resources/font/#esamanru Light)�$��14(�#���  $
��10,0q�-  'HandyControl.Controls.CircleProgressBar  -     Name$ loadingProgressbar��# [     IsIndeterminate . $��60��$��60��� -   $ noResultPanelForPlaylist��$��Center=�$��Center=�.+�  $- '/Resources/drawable/img_character_1.svg?�$��0q�$	��225��$	��175��$	��Top=��� $6��0해당 플레이리스트는 비어있습니다.��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�#���  $��0q�$��Center=�&   System.Windows.Controls.DataGrid  -      Name$-  'x_PlaylistInfoLayout_MyPlaylistDataGrid��$��Stretch=�!   AutoGenerateColumns! .  $" �� IsSynchronizedWithCurrentItem$" FalseY�#   SelectionMode$# Single=�$   CanUserAddRows$ .  $��5,20,5,0q�$��1��$&�� /Resources/font/#esamanru Medium)�$��12(�	 . $
 	Recycling=� . # [  ��. $%   HorizontalScrollBarVisibility$% Disabled=���Z� ��Z�  �� .@�m� -   $)��#현재 플레이리스트에 담기��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$@��:/UtaitePlayer;component/Resources/drawable/ic_playlist.png��m� -   $��노래 정보 보기��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$<��6/UtaitePlayer;component/Resources/drawable/ic_info.png��m� -   $%��플레이리스트에서 삭제��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$>��8/UtaitePlayer;component/Resources/drawable/ic_delete.png��&   Columns& 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn'   Width5	  /System.Windows.Controls.DataGridLengthConverter$' 40	 (   
IsReadOnly( .  )   CanUserResize) .  *   CellTemplate* �� .R��� -   $��40��+ d� 	IsChecked+ �� *	isCheck+,   Header, �� -   $A��;x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox��$��40��  ( . ) .  * �� .R��� $��UniformToFill=�$��Center=�$��Center=�$��30������ *bitmapImage+0
  *System.Windows.Controls.DataGridTextColumn
  $
' 1.5*	 $, 제목��- 
  
FontFamily$%- /Resources/font/#esamanru Light)�. 
  FontSize$. 10Q�( . /   CanUserSort/ .  1  +System.Windows.Controls.DataGridBoundColumn0   Binding0 �� *	musicName+
  $' *	 $%- /Resources/font/#esamanru Light)�$. 10Q�$, 아티스트��( . / .  0 �� *
artistName+
  $' *	 $%- /Resources/font/#esamanru Light)�$. 10Q�$, 태그��( . / .  0 �� *
musicTag+
  $' *	 $%- /Resources/font/#esamanru Light)�$. 10Q�$, 	작곡가��( . / .  0 �� *musicWriter+  ( . ) .  * �� .R�� $��1��$��20��$��20��$��Center=�$��Bottom=�
������ .+�F� -   $��14��$��18��������s�9� � >I �\ � u8  5  �$  j  �$ ��* � �$ ��� �$ `< u8 `q# >I �Iрs�l� ����D� ���a� �s�9� 	
�������$
 Fill=�*  $HandyControl.Controls.AdornerElement1   Instance1 "  HandyControl.Controls.Poptip  # [  2   Content$2 재생��3   PlacementType$3 TopLeft=�$%��/Resources/font/#esamanru Light)�$��12(�#��[ 