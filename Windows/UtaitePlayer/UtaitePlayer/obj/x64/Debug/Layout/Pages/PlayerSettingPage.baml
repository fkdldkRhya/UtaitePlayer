   M S B A M L   `   `   `  ����S  OUtaitePlayer, Version=1.1.0.5, Culture=neutral, PublicKeyToken=1db8b1a767e76951E'clr-namespace:UtaitePlayer.Layout.PagesUtaitePlayer.Layout.Pages  5      1    +UtaitePlayer.Layout.Pages.PlayerSettingPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      b ^SharpVectors.Converters.Wpf, Version=1.7.7.0, Culture=neutral, PublicKeyToken=b532964b8548be771svgc&http://sharpvectors.codeplex.com/svgc/  5      S OHandyControl, Version=3.3.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  5	      3local'clr-namespace:UtaitePlayer.Layout.Pages  5      -   5          N� Title$  PlayerSettingPage��5      .��5      � .+�5   
   (  "HandyControl.Controls.ScrollViewer  -   5        $��rootResultScrollViewer��5         HandyControlThemes � 	Resources# [  5      .��5      �  � RowDefinitions 5      �� 5       $
��auto �6$   �� 5!      $
��auto �6$   .+�5%      � .+�5&      �� $<��6/UtaitePlayer;component/Resources/drawable/ic_user.png��5'      $��40��5(      $��40��5)      $
��Left=�5*      $	��Top=�5+      $��	10,20,0,0q�5,      �� 5/      -   5         ��Name$ accountIDTextBlock��50      $
��Null��51      $	��Top=�52      $��NoWrap=�53      $��CharacterEllipsis=�54      $&�� /Resources/font/#esamanru Medium)�55      $��20(�56        Color_White1#���  57      $��60,20,130,0q�58      �� 5;      -   5        $ accountEmailTextBlock��5<      $
��Null��5=      $��Bottom=�5>      $��NoWrap=�5?      $��CharacterEllipsis=�5@      $%��/Resources/font/#esamanru Light)�5A      $��14(�5B      #���  5C      $��
60,0,130,0q�5D      �� 5G      $��Bottom=�5H      $��NoWrap=�5I      $��CharacterEllipsis=�5J      $%��/Resources/font/#esamanru Light)�5K      $��12(�5L        Color_White3#���  5M      $��	60,0,10,0q�5N      $��Right=�5O      �� 5Q      -   5          Color_Blue1#���  5R      .�5S   ;   로그아웃5U      5V      �� 5Y      $��Bottom=�5Z      $��NoWrap=�5[      $��CharacterEllipsis=�5\      $%��/Resources/font/#esamanru Light)�5]      $��12(�5^      #���  5_      $��	60,0,65,0q�5`      $��Right=�5a      �� 5c      -   5        #���  5d      .�5e   C   계정 설정5g      5h      5i      � 5l      $��1��5m      $��0,20,0,0q�5n      .+�5q      ��  �� Orientation$ Vertical=�5r      $��	10,0,10,0q�5s      .+�5v      #  HandyControl.Controls.Divider     LineStrokeThickness$ 1Q�5w      $��Right=�5x      # [  5y         Content$ 일반 설정��5z      $	��Top=�5{      $%��/Resources/font/#esamanru Light)�5|      $��15(�5}      #���  5~      $��
10,10,10,0q�5      � 5�      $��0,10,0,0q�6   .+�5�      d� -   5         d�Name$' !BootStartUtaitePlayerToggleButton��5�   !     ToggleButtonSwitch#���  5�   !   $	��Top=�5�   !   $
��Left=�5�   !   $��40��5�   !   $��0q�5�   !   5�   !   �� 5�      $ ��컴퓨터 부팅시 실행��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��12(�5�   !   #���  5�   !   $��50,0,0,0q�5�   !   $
��Left=�5�   !   �� 5�      $U��O(우타이테 플레이어를 Windows가 시작될 때 같이 실행합니다.)��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��10(�5�   !   #���  5�   !   $��	155,0,0,0q�5�   !   $
��Left=�5�   !   5�      � 5�      $��0,10,0,0q�6   .+�5�      d� -   5        $! CrashHandlerRunToggleButton��5�   !   #���  5�   !   $��Stretch=�5�   !   $
��Left=�5�   !   $��40��5�   !   $��0q�5�   !   5�   !   �� 5�      $#��비정상적인 종료 감지��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��12(�5�   !   #���  5�   !   $��50,0,0,0q�5�   !   $
��Left=�5�   !   �� 5�      $Q��K(우타이테 플레이어 비정상 종료 감시 서비스 작동 여부)��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��10(�5�   !   #���  5�   !   $��	168,0,0,0q�5�   !   $
��Left=�5�   !   5�      � 5�      $��0,10,0,0q�6   .+�5�      d� -	   5        $$ CloseButtonSettingToggleButton��5�   !   #���  5�   !   $��Center=�5�   !   $
��Left=�5�   !   $��40��5�   !   $��0q�5�   !   5�   !   �� 5�      $-��'닫기 버튼으로 바로 종료하기��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��12(�5�   !   #���  5�   !   $��50,0,0,0q�5�   !   $
��Left=�5�   !   �� 5�      $B��<(창 닫기 버튼으로 바로 종료 기능 사용 여부)��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��10(�5�   !   #���  5�   !   $��	202,9,0,9q�5�   !   $
��Left=�5�   !   5�      � 5�      $��0,10,0,0q�6   .+�5�      d� -
   5        $$ ReloadButtonEnableToggleButton��5�   !   #���  5�   !   $��Center=�5�   !   $
��Left=�5�   !   $��40��5�   !   $��0q�5�   !   5�   !   �� 5�      $#��새로고침 버튼 활성화��5�   !   $��Center=�5�   !   $
��Wrap=�5�   !   $%��/Resources/font/#esamanru Light)�5�   !   $��12(�5�   !   #���  5�   !   $��50,0,0,0q�5�   !   $
��Left=�5�   !   5�      � 5�      $��0,10,0,0q�6   .+�5�      d� -   5        $ TopMostToggleButton��5�   !   #���  5�   !   $��Center=�5�   !   $
��Left=�5�   !   $��40��5�   !   $��0q�5�   !   5   !   �� 5     $>��8우타이테 플레이어를 가장 위에 있게 하기��5  !   $��Center=�5  !   $
��Wrap=�5  !   $%��/Resources/font/#esamanru Light)�5  !   $��12(�5  !   #���  5  !   $��50,0,0,0q�5	  !   $
��Left=�5
  !   5     � 5     $��0,10,0,0q�6   .+�5     �� $ ��프로그램 시작 모드��5  !   $	��Top=�5  !   $
��Wrap=�5  !   $%��/Resources/font/#esamanru Light)�5  !   $��12(�5  !   #���  5  !   $��0q�5  !   $
��Left=�5  !   �� 5     $��0,20,0,0q�5  !   $ 
Horizontal=�5  !   .+�5  "   	� -   5         	�Name$% StartModForWindowMODRadioButton��5  %   $%��/Resources/font/#esamanru Light)�5  %   $��11(�5   %   $��
Window MOD��5!  %   $
��Left=�5"  %   5#  %   	� 5%  "   -   5        $) #StartModForBackgroundMODRadioButton��5&  %   $%��/Resources/font/#esamanru Light)�5'  %   $��11(�5(  %   $��Background MOD��5)  %   $
��Left=�5*  %   $��15,0,0,0q�5+  %   5,  %   5-     5.     � 51     $��0,10,0,0q�6   .+�52     �� -   5        # [  53  !   $	��120��54  !   $��30��55  !   $
��Left=�56  !   $	��Top=�57  !   $��0,3,10,0q�58  !   .��5;  "   �� $ 
Horizontal=�5<  %   .+�5>  &   F� $��10��5?  )   $��10��5@      ����p !������0@S�gfffff/@@���� ��     ,@S �     (@gfffffJ@`*� �B������#@S ^     #@ �vhfffff%@��|hfffff)@S R�gfffff0@�y�     4@���43333�7@�����YN@43333�9@�l������L;@`am�$�S@�4433333<@��� t����`"��`�@��� �`����� v��433333E@������������7@��� b�@� m] l��_Z`O� �U�����|C ͷ �8��� -1�fffff&@@ �`H�Pt@���433333/@���gfffff7@�؈� ]�S �������O@�9d�[��`F�Ӏl��� �����y� Ϛ�����N@43333�D@������N@`m���43333sD@Ӏ����t������P@�����LE@fffff�P@    @F@ffffffP@�����H@�����P@    �I@�����?O@�����LK@������L@43333�N@������I@43333�P@����̌E@ 	������;@gffff�Q@������+@ �033333'@`��������"@@�������2@gfffff1@������?@������+@�����LC@433333(@������F@������)@������I@�����0@� u�� @� ��  !������0@	
�������	 �� Stretch$
	 Fill=�5C  )   �� 5E  &   $��Bottom=�5F  )   $%��/Resources/font/#esamanru Light)�5G  )   $��9(�5H  )   #���  5I  )   $��리소스 새로고침��5J  )   $��5,0,0,0q�5K  )   5L  #   5M     �� 5O     $j��d(다운로드 받은 노래 데이터를 제거하고 우타이테 플레이어가 종료됩니다.)��5P  !   $��Bottom=�5Q  !   $
��Wrap=�5R  !   $%��/Resources/font/#esamanru Light)�5S  !   $��10(�5T  !   #���  5U  !   $��	134,0,0,5q�5V  !   $
��Left=�5W  !   5X       5[     $ 1Q�5\     $��Right=�5]     # [  5^     $ 재생 설정��5_     $	��Top=�5`     $%��/Resources/font/#esamanru Light)�5a     $��15(�5b     #���  5c     $��
10,10,10,0q�5d     � 5g     $��0,10,0,0q�6   .+�5h     �� $��출력 장치��5i  !   $	��Top=�5j  !   $
��Wrap=�5k  !   $%��/Resources/font/#esamanru Light)�5l  !   $��12(�5m  !   #���  5n  !   $��0q�5o  !   $
��Left=�5p  !   $  HandyControl.Controls.ComboBox  5r     -   5        
  Name$
 deviceComboBox��5s  !   $��0,20,0,0q�5t  !   $	��260��5u  !   $%��/Resources/font/#esamanru Light)�5v  !   $��12(�5w  !   $
��Left=�5x  !   5y  !   5z     � 5}     $��0,10,0,0q�6   .+�5~     �� $��예약 정지��5  !   $	��Top=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��12(�5�  !   #���  5�  !   $��0q�5�  !   $
��Left=�5�  !   �� 5�     $	��Top=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��10(�5�  !   #���  5�  !   $��59,2,0,0q�5�  !   $
��Left=�5�  !   $	��447��6<   $|��v(지정된 시간에 노래 재생을 멈춥니다. 0시간 0분으로 설정할 경우 타이머가 종료됩니다.)��6H   )  #HandyControl.Controls.NumericUpDown  5�     -   5          Name$) #ReservationStopForHourNumericUpDown��5�  !      Minimum$ 0Q�5�  !      Maximum$ 12Q�5�  !      Value$ 00Q�5�  !   $��80��5�  !   $
��Left=�5�  !   $	��Top=�5�  !   $��0,25,0,0q�5�  !      ValueFormat 5�  !   Z� 5�  !   �� 5�     $��시간��5�  !   $	��Top=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��14(�5�  !   #���  5�  !   $��	90,32,0,0q�5�  !   $
��Left=�5�  !     5�     -   5        $+ %ReservationStopForMinuteNumericUpDown��5�  !   $ 0Q�5�  !   $ 60Q�5�  !   $ 00Q�5�  !   $��80��5�  !   $
��Left=�5�  !   $	��Top=�5�  !   $��
135,25,0,0q�5�  !    5�  !   Z� 5�  !   �� 5�     $	��분��5�  !   $	��Top=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��14(�5�  !   #���  5�  !   $��
228,32,0,0q�5�  !   $
��Left=�5�  !   �� 5�     -   5        # [  5�  !   $��98��5�  !   $��30��5�  !   $
��Left=�5�  !   $	��Top=�5�  !   $��0,70,0,0q�5�  !   .��5�  "   �� $ 
Horizontal=�5�  %   .+�5�  &   �� $��10��5�  )   $��10��5�  )   $<��6/UtaitePlayer;component/Resources/drawable/ic_save.png��5�  )   �� 5�  &   $��Bottom=�5�  )   $%��/Resources/font/#esamanru Light)�5�  )   $��9(�5�  )   #���  5�  )   $��적용��5�  )   $��5,0,0,0q�5�  )   5�  #   5�     �� 5�     $��남은 시간: ��5�  !   $��Bottom=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��10(�5�  !   #���  5�  !   $��	112,0,0,5q�5�  !   $
��Left=�5�  !   �� 5�     -   5        $ ReservationStopTime��5�  !   $��00:00��5�  !   $��Bottom=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��10(�5�  !   #���  5�  !   $��	160,0,0,5q�5�  !   $
��Left=�5�  !   �� 5�     -   5        $ IsEnableTimerTextBlock��5�  !   $��타이머 멈춤��5�  !   $��Bottom=�5�  !   $
��Wrap=�5�  !   $%��/Resources/font/#esamanru Light)�5�  !   $��10(�5�  !   #���  5�  !   $��	221,0,0,5q�5�  !   $
��Left=�5�  !   5�       5�     $ 1Q�5�     $��Right=�5�     # [  5�     $ 프로그램 정보��5�     $	��Top=�5�     $%��/Resources/font/#esamanru Light)�5�     $��15(�5�     #���  5      $��
10,10,10,0q�5     �� 5     $E��?/UtaitePlayer;component/Resources/drawable/img_up_logo_main.png��5     $	��150��5     $
��Left=�5     $	��Top=�5     $��0,0,0,15q�5	     � 5     $
��Left=�6   .+�5     �� $��프로그램 버전��5  !   $	��Top=�5  !   $
��Wrap=�5  !   $%��/Resources/font/#esamanru Light)�5  !   $��12(�5  !   #���  5  !   $��0q�5  !   $
��Left=�5  !   �� 5     -   5        $ programVersionTextBlock��5  !   $��Center=�5  !   $
��Wrap=�5  !   $%��/Resources/font/#esamanru Light)�5  !   $��10(�5  !   #���  5  !   $��	120,0,0,0q�5  !   $
��Left=�5  !   5      � 5#     $
��Left=�6   .+�5$     �� $��개발자 정보��5%  !   $	��Top=�5&  !   $
��Wrap=�5'  !   $%��/Resources/font/#esamanru Light)�5(  !   $��12(�5)  !   #���  5*  !   $��0q�5+  !   $
��Left=�5,  !   �� 5.     $��CHOI SI-HUN (최시훈)��5/  !   $��Center=�50  !   $
��Wrap=�51  !   $%��/Resources/font/#esamanru Light)�52  !   $��10(�53  !   #���  54  !   $
��Left=�56  !   ��55  !   ��  �� Path$ Margin� �� ElementName$ programVersionTextBlock��56  !   57     �� 5:     $ Vertical=�6%   .+�5;     �� $��업데이트 내역��5<  !   $	��Top=�5=  !   $
��Wrap=�5>  !   $%��/Resources/font/#esamanru Light)�5?  !   $��12(�5@  !   #���  5A  !   $��0q�5B  !   $
��Left=�5C  !   �� 5E     -   5         ��Name$ updateTextRichTextBox��5F  !   $%��/Resources/font/#esamanru Light)�5G  !   $��10(�5H  !    �� 
IsReadOnly . $
��Left=�5J  !   $	��Top=�5K  !   $��0,10,0,0q�5L  !   $	��300��5M  !   $	��130��5N  !   5O       5R     $ 1Q�5S     $��Right=�5T     # [  5U     $ 라이센스 정보��5V     $	��Top=�5W     $%��/Resources/font/#esamanru Light)�5X     $��15(�5Y     #���  5Z     $��
10,10,10,0q�5[     � 5^     $��0,10,0,0q�6   .+�5_     �� $��MIT License��5`  !   $	��Top=�5a  !   $
��Wrap=�5b  !   $%��/Resources/font/#esamanru Light)�5c  !   $��10(�5d  !   #���  5e  !   $��0q�5f  !   �� 5h     $$��Copyright (c) 2022 CHOI SI-HUN��5i  !   $	��Top=�5j  !   $
��Wrap=�5k  !   $%��/Resources/font/#esamanru Light)�5l  !   $��10(�5m  !   #���  5n  !   $��0,20,0,0q�5o  !   �� 5q     $	��Top=�5r  !   $
��Wrap=�5s  !   $%��/Resources/font/#esamanru Light)�5t  !   $��10(�5u  !   #���  5v  !   $��0,40,0,0q�5w  !   �� 5z  "   $���Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:��6&   �� 5{  "   �� 5|  "   �� 5}  "   �� 5~  "   $��~The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.��6&   �� 5  "   �� 5�  "   �� 5�  "   �� 5�  "   $���THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.��6&   5�     5�     �� 5�     $J��D/UtaitePlayer;component/Resources/drawable/img_rhya_network_x200.png��5�     $	��200��5�     $��Right=�5�     $	��Top=�5�     $��	0,40,5,15q�5�     5�     5�     5�     5�     5�     5�     