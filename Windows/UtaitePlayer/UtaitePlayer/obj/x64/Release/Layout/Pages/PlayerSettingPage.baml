   M S B A M L   `   `   `  ���� S  OUtaitePlayer, Version=1.1.0.7, Culture=neutral, PublicKeyToken=1db8b1a767e76951E'clr-namespace:UtaitePlayer.Layout.PagesUtaitePlayer.Layout.Pages  1    +UtaitePlayer.Layout.Pages.PlayerSettingPage   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     b ^SharpVectors.Converters.Wpf, Version=1.7.7.0, Culture=neutral, PublicKeyToken=b532964b8548be771svgc&http://sharpvectors.codeplex.com/svgc/  S OHandyControl, Version=3.3.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  3local'clr-namespace:UtaitePlayer.Layout.Pages  -     N� Title$  PlayerSettingPage��.��� .+�� -    �Name$ LoadingProgressBar��$��0��$y�	Collapsed=�   HandyControlThemes � 	Resources# [    ProgressBarFlat#��[  � IsIndeterminate . $	��Top=�$��5����Z� ��Z� (  "HandyControl.Controls.ScrollViewer  -   $��rootResultScrollViewer��# [  .���  � RowDefinitions �� $
��auto ��� $
��auto �.+�� .+��� $<��6/UtaitePlayer;component/Resources/drawable/ic_user.png��$��40��$��40��$
��Left=�$	��Top=�$��	10,20,0,0q��� -    ��Name$ accountIDTextBlock��$
��Null��$	��Top=�$��NoWrap=�$��CharacterEllipsis=�$&�� /Resources/font/#esamanru Medium)�$��20(�  Color_White1#���  $��60,20,130,0q��� -   $ accountEmailTextBlock��$
��Null��$��Bottom=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��14(�#���  $��
60,0,130,0q��� $��Bottom=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��12(�  Color_White3#���  $��	60,0,10,0q�$��Right=��� -     Color_Blue1#���  .�로그아웃�� $��Bottom=�$��NoWrap=�$��CharacterEllipsis=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��	60,0,65,0q�$��Right=��� -   #���  .�계정 설정� $��1��$��0,20,0,0q�.+���  �� Orientation$ Vertical=�$��	10,0,10,0q�.+�#  HandyControl.Controls.Divider     LineStrokeThickness$ 1Q�$��Right=�# [     Content$ 일반 설정��$	��Top=�$%��/Resources/font/#esamanru Light)�$��15(�#���  $��
10,10,10,0q�� $��0,10,0,0q�.+�d� -   	 d�Name$'	 !BootStartUtaitePlayerToggleButton��  ToggleButtonSwitch#���  $	��Top=�$
��Left=�$��40��$��0q��� $ ��컴퓨터 부팅시 실행��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��50,0,0,0q�$
��Left=��� $U��O(우타이테 플레이어를 Windows가 시작될 때 같이 실행합니다.)��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	155,0,0,0q�$
��Left=�� $��0,10,0,0q�.+�d� -	   $!	 CrashHandlerRunToggleButton��#���  $��Stretch=�$
��Left=�$��40��$��0q��� $#��비정상적인 종료 감지��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��50,0,0,0q�$
��Left=��� $Q��K(우타이테 플레이어 비정상 종료 감시 서비스 작동 여부)��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	168,0,0,0q�$
��Left=�� $��0,10,0,0q�.+�d� -
   $$	 CloseButtonSettingToggleButton��#���  $��Center=�$
��Left=�$��40��$��0q��� $-��'닫기 버튼으로 바로 종료하기��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��50,0,0,0q�$
��Left=��� $B��<(창 닫기 버튼으로 바로 종료 기능 사용 여부)��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	202,9,0,9q�$
��Left=�� $��0,10,0,0q�.+�d� -   $$	 ReloadButtonEnableToggleButton��#���  $��Center=�$
��Left=�$��40��$��0q��� $#��새로고침 버튼 활성화��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��50,0,0,0q�$
��Left=�� $��0,10,0,0q�.+�d� -   $	 TopMostToggleButton��#���  $��Center=�$
��Left=�$��40��$��0q��� $>��8우타이테 플레이어를 가장 위에 있게 하기��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��50,0,0,0q�$
��Left=�� $��0,10,0,0q�.+��� $ ��프로그램 시작 모드��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��0q�$
��Left=��� $��0,20,0,0q�$ 
Horizontal=�.+�	� -   
 	�Name$%
 StartModForWindowMODRadioButton��$%��/Resources/font/#esamanru Light)�$��11(�$��
Window MOD��$
��Left=�	� -   $)
 #StartModForBackgroundMODRadioButton��$%��/Resources/font/#esamanru Light)�$��11(�$��Background MOD��$
��Left=�$��15,0,0,0q�� $��0,10,0,0q�.+��� -   # [  $	��120��$��30��$
��Left=�$	��Top=�$��0,3,10,0q�.���� $ 
Horizontal=�.+�F� $��10��$��10������p !������0@S�gfffff/@@���� ��     ,@S �     (@gfffffJ@`*� �B������#@S ^     #@ �vhfffff%@��|hfffff)@S R�gfffff0@�y�     4@���43333�7@�����YN@43333�9@�l������L;@`am�$�S@�4433333<@��� t����`"��`�@��� �`����� v��433333E@������������7@��� b�@� m] l��_Z`O� �U�����|C ͷ �8��� -1�fffff&@@ �`H�Pt@���433333/@���gfffff7@�؈� ]�S �������O@�9d�[��`F�Ӏl��� �����y� Ϛ�����N@43333�D@������N@`m���43333sD@Ӏ����t������P@�����LE@fffff�P@    @F@ffffffP@�����H@�����P@    �I@�����?O@�����LK@������L@43333�N@������I@43333�P@����̌E@ 	������;@gffff�Q@������+@ �033333'@`��������"@@�������2@gfffff1@������?@������+@�����LC@433333(@������F@������)@������I@�����0@� u�� @� ��  !������0@	
������� �� Stretch$
 Fill=��� $��Bottom=�$%��/Resources/font/#esamanru Light)�$��9(�#���  $��리소스 새로고침��$��5,0,0,0q��� $j��d(다운로드 받은 노래 데이터를 제거하고 우타이테 플레이어가 종료됩니다.)��$��Bottom=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	134,0,0,5q�$
��Left=�  $ 1Q�$��Right=�# [  $ 재생 설정��$	��Top=�$%��/Resources/font/#esamanru Light)�$��15(�#���  $��
10,10,10,0q�� $��0,10,0,0q�.+��� $��출력 장치��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��0q�$
��Left=�$  HandyControl.Controls.ComboBox  -     Name$ deviceComboBox��$��0,20,0,0q�$	��260��$%��/Resources/font/#esamanru Light)�$��12(�$
��Left=�� $��0,10,0,0q�.+��� $��예약 정지��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��0q�$
��Left=��� $	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��59,2,0,0q�$
��Left=�$	��447��$|��v(지정된 시간에 노래 재생을 멈춥니다. 0시간 0분으로 설정할 경우 타이머가 종료됩니다.)��)  #HandyControl.Controls.NumericUpDown  -     Name$) #ReservationStopForHourNumericUpDown��   Minimum$ 0Q�   Maximum$ 12Q�   Value$ 00Q�$��80��$
��Left=�$	��Top=�$��0,25,0,0q�   ValueFormat Z� �� $��시간��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��14(�#���  $��	90,32,0,0q�$
��Left=�  -   $+ %ReservationStopForMinuteNumericUpDown��$ 0Q�$ 60Q�$ 00Q�$��80��$
��Left=�$	��Top=�$��
135,25,0,0q� Z� �� $	��분��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��14(�#���  $��
228,32,0,0q�$
��Left=��� -   # [  $��98��$��30��$
��Left=�$	��Top=�$��0,70,0,0q�.���� $ 
Horizontal=�.+��� $��10��$��10��$<��6/UtaitePlayer;component/Resources/drawable/ic_save.png���� $��Bottom=�$%��/Resources/font/#esamanru Light)�$��9(�#���  $��적용��$��5,0,0,0q��� $��남은 시간: ��$��Bottom=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	112,0,0,5q�$
��Left=��� -   $ ReservationStopTime��$��00:00��$��Bottom=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	160,0,0,5q�$
��Left=��� -   $ IsEnableTimerTextBlock��$��타이머 멈춤��$��Bottom=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	221,0,0,5q�$
��Left=�  $ 1Q�$��Right=�# [  $ 프로그램 정보��$	��Top=�$%��/Resources/font/#esamanru Light)�$��15(�#���  $��
10,10,10,0q��� $E��?/UtaitePlayer;component/Resources/drawable/img_up_logo_main.png��$	��150��$
��Left=�$	��Top=�$��0,0,0,15q�� $
��Left=�.+��� $��프로그램 버전��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��0q�$
��Left=��� -   $ programVersionTextBlock��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��	120,0,0,0q�$
��Left=�� $
��Left=�.+��� $��개발자 정보��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��0q�$
��Left=��� $��CHOI SI-HUN (최시훈)��$��Center=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $
��Left=�����  �� Path$ Margin� �� ElementName$ programVersionTextBlock���� $ Vertical=�.+��� $��업데이트 내역��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��12(�#���  $��0q�$
��Left=��� -    ��Name$ updateTextRichTextBox��$%��/Resources/font/#esamanru Light)�$��10(� �� 
IsReadOnly . $
��Left=�$	��Top=�$��0,10,0,0q�$	��300��$	��130��  $ 1Q�$��Right=�# [  $ 라이센스 정보��$	��Top=�$%��/Resources/font/#esamanru Light)�$��15(�#���  $��
10,10,10,0q�� $��0,10,0,0q�.+��� $��MIT License��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��0q��� $$��Copyright (c) 2022 CHOI SI-HUN��$	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��0,20,0,0q��� $	��Top=�$
��Wrap=�$%��/Resources/font/#esamanru Light)�$��10(�#���  $��0,40,0,0q��� $���Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:���� �� �� �� $��~The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.���� �� �� �� $���THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.���� $J��D/UtaitePlayer;component/Resources/drawable/img_rhya_network_x200.png��$	��200��$��Right=�$	��Top=�$��	0,40,5,15q�