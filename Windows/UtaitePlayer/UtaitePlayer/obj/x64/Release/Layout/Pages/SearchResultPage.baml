   M S B A M L   `   `   `  ���� S  OUtaitePlayer, Version=1.1.0.7, Culture=neutral, PublicKeyToken=1db8b1a767e76951E'clr-namespace:UtaitePlayer.Layout.PagesUtaitePlayer.Layout.Pages  0    *UtaitePlayer.Layout.Pages.SearchResultPage   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     b ^SharpVectors.Converters.Wpf, Version=1.7.7.0, Culture=neutral, PublicKeyToken=b532964b8548be771svgc&http://sharpvectors.codeplex.com/svgc/  S OHandyControl, Version=3.3.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  3local'clr-namespace:UtaitePlayer.Layout.Pages  -     N� Title$  SearchResultPage��.��� -   .+�(  "HandyControl.Controls.ScrollViewer  -   $"��rootSearchResultScrollViewer��   HandyControlThemes � 	Resources# [  .���  � RowDefinitions �� $
��auto ��� $
��auto �.+�� $	��Top=�$��0�� �� $��30 ��� $
��auto ��� $��40 �.+��� $=��7/UtaitePlayer;component/Resources/drawable/ic_music.png��$
��Left=�$��16��$��16��$��5,0,0,2q�$��Bottom=��� -    ��Name$ searchResultTitleForMusic��$��	노래(0)��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�  Color_White1#���  $��25,0,0,0q�$
��Left=�� -    �Name$ noSearchResultForMusic��$��1��$��Center=�$��Center=�$	��200��$	��160��$
��0,15q�.+�(  "SharpVectors.Converters.SvgViewbox     Source$- '/Resources/drawable/img_character_1.svg?�$��0q�$	��Top=��� $*��$검색된 데이터가 없습니다.��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�#���  $��0q�$��Center=�&   System.Windows.Controls.DataGrid  -     Name$" searchResultDataGridForMusic��$��Stretch=�   AutoGenerateColumns .  $ �� IsSynchronizedWithCurrentItem$ FalseY�	   SelectionMode$	 Single=�
   CanUserAddRows
 .  $
��5,10q�$��1��$&�� /Resources/font/#esamanru Medium)�$��12(�# [  ��.  $   HorizontalScrollBarVisibility$ Disabled=�"   VerticalScrollBarVisibility$ Disabled=���Z� ��Z�  � ContextMenu �� .@�m� -   $)��#현재 플레이리스트에 담기��$%��/Resources/font/#esamanru Light)�$��12(� m� Icon �� $��15��$��15��$@��:/UtaitePlayer;component/Resources/drawable/ic_playlist.png��m� -   $,��&사용자 플레이리스트에 담기��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$@��:/UtaitePlayer;component/Resources/drawable/ic_playlist.png��m� -	   $��노래 정보 보기��$%��/Resources/font/#esamanru Light)�$��12(� �� $��15��$��15��$<��6/UtaitePlayer;component/Resources/drawable/ic_info.png��   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    CanUserResize .     CellTemplate �� .R��� $��UniformToFill=�$��Center=�$��Center=�$��30������ *
musicImage+0  *System.Windows.Controls.DataGridTextColumn     Width5  /System.Windows.Controls.DataGridLengthConverter$
 1.5*    Header$ 제목��   
FontFamily$% /Resources/font/#esamanru Light)�   FontSize$ 10Q� .    CanUserSort .  1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *	musicName+  $ * $% /Resources/font/#esamanru Light)�$ 10Q�$ 아티스트�� .  .   �� *
artistName+  $ * $% /Resources/font/#esamanru Light)�$ 10Q�$ 태그�� .  .   �� *
musicTag+  $ * $% /Resources/font/#esamanru Light)�$ 10Q�$ 	작곡가�� .  .   �� *musicWriter+   .  .   �� .R�� $��1��$��20��$��20��$��Center=�$��Bottom=�
������ .+�F� -
   $��14��$��18��������s�9� � >I �\ � u8  5  �$  j  �$ ��* � �$ ��� �$ `< u8 `q# >I �Iрs�l� ����D� ���a� �s�9� 	
������� �� Stretch$
 Fill=�*	  $HandyControl.Controls.AdornerElement 	  Instance "
  HandyControl.Controls.Poptip
  # [   
  Content$ 재생�� 
  PlacementType$ TopLeft=�$%��/Resources/font/#esamanru Light)�$��12(�#��[ �� -    ��Name$% pageIndexPerviousButtonForMusic��$��이전��$��Right=�$��0,0,95,0q�$%��/Resources/font/#esamanru Light)�$��13(�$��2��$��Center=�$��75��� $��2��$��	0,0,180,0q� � ColumnDefinitions �� $��* ��� $
��auto �.+��� -   $  pageIndexTextBlockForMusic��$��/0��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��13(�$��1��#���  $��
2.5,0,15,0q�$
��Left=�#  HandyControl.Controls.TextBox  -     Name$ pageIndexTextboxForMusic��$��Center=�$��13(�$��Right=�$%��/Resources/font/#esamanru Light)�$��70��$��0��$��	0,0,2.5,0q�  �� TextAlignment$  Center=�.�� �� .@�m� $��잘라내기��$%��/Resources/font/#esamanru Light)�$��12(�! m� Command$! ApplicationCommands.Cut��m� $��복사��$%��/Resources/font/#esamanru Light)�$��12(�$! ApplicationCommands.Copy��m� $��붙여넣기��$%��/Resources/font/#esamanru Light)�$��12(�$! ApplicationCommands.Paste��.���� -   $! pageIndexNextButtonForMusic��$��다음��$��Right=�$��0,0,15,0q�$%��/Resources/font/#esamanru Light)�$��13(�$��2��$��Center=�$��75��� $��1��$	��Top=� �� $��30 ��� $
��auto ��� $��40 ��� $��20 �.+��� $B��</UtaitePlayer;component/Resources/drawable/ic_microphone.png��$
��Left=�$��16��$��16��$��5,0,0,2q�$��Bottom=��� -   $  searchResultTitleForArtist��$��아티스트(0)��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�#���  $��25,0,0,0q�$
��Left=�� -   $ noSearchResultForArtist��$��1��$��Center=�$��Center=�$	��200��$	��160��$
��0,15q�.+�  $- '/Resources/drawable/img_character_1.svg?�$��0q�$	��Top=��� $*��$검색된 데이터가 없습니다.��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��16(�#���  $��0q�$��Center=�  -   $# searchResultDataGridForArtist��# [  $��Stretch=� .  $ FalseY�$	 Single=�
 .  $
��5,10q�$��1��$&�� /Resources/font/#esamanru Medium)�$��12(�$ Disabled=�$ Disabled=���.  ��Z� ��Z�     .  .   �� .R��� $��30��$��30��$��UniformToFill=�$��Center=�$��Center=����� *artistImage+  $ * $ 아티스트��$% /Resources/font/#esamanru Light)�$ 10Q� .   .  �� *
artistName+   .  .   �� .R�� .+��� -   $��30��$��70��$%��/Resources/font/#esamanru Light)�$��10(�$��Center=�$��Center=�y��� *!artistSubscribeButtonVisibility+���� *artistSubscribeData+-  'HandyControl.Controls.CircleProgressBar  # [  "   IsIndeterminate" . $��30��$��30��y��� *artistLoadingVisibility+�� -   $&  pageIndexPerviousButtonForArtist��$��이전��$��Right=�$��0,0,95,0q�$%��/Resources/font/#esamanru Light)�$��13(�$��2��$��Center=�$��75��� $��2��$��	0,0,180,0q� �� $��* ��� $
��auto �.+��� -   $! pageIndexTextBlockForArtist��$��/0��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��13(�$��1��#���  $��
2.5,0,15,0q�$
��Left=�  -   $ pageIndexTextboxForArtist��$��Center=�$��13(�$��Right=�$%��/Resources/font/#esamanru Light)�$��70��$��0��$��	0,0,2.5,0q�$  Center=�.�� �� .@�m� $��잘라내기��$%��/Resources/font/#esamanru Light)�$��12(�$! ApplicationCommands.Cut��m� $��복사��$%��/Resources/font/#esamanru Light)�$��12(�$! ApplicationCommands.Copy��m� $��붙여넣기��$%��/Resources/font/#esamanru Light)�$��12(�$! ApplicationCommands.Paste��.���� -   $" pageIndexNextButtonForArtist��$��다음��$��Right=�$��0,0,15,0q�$%��/Resources/font/#esamanru Light)�$��13(�$��2��$��Center=�$��75��#  HandyControl.Controls.GotoTop  # [  #   Animated# . $   
AutoHiding$ . %   AnimationTime$
% 2000Q�$
��Left=�$��Bottom=�$��	20,0,0,20q�&   Target& �� ' �� ElementName$"' rootSearchResultScrollViewer��� -   $ loadingPanel��$y�	Collapsed=�  Color_Gray3#���  .+��� ( �� Orientation$( Vertical=�$��Center=�$��Center=�.+�  # [  " . $��60��$��60���� $��검색 중...��$��Bottom=�$
��Wrap=�$&�� /Resources/font/#esamanru Medium)�$��18(�#���  $��0,15,0,0q�$��Center=�