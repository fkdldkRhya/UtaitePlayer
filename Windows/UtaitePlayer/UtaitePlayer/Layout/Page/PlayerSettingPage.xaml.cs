using HandyControl.Data;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UtaitePlayer.Classes.Core;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout.Page
{
    /// <summary>
    /// PlayerSettingPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlayerSettingPage : System.Windows.Controls.Page
    {
        // 초기화 상태
        private bool isInit = false;
        // 예약 정지 타이머
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        // 예약 정지 타이머 데이터
        private int reservationStopTime = 0;
        private int reservationStopTimeForNow = 0;
        // 출력 장치 변경 데이터
        private int backSelectedIndexForAudioDevice = -1;




        /// <summary>
        /// 생성자
        /// </summary>
        public PlayerSettingPage()
        {
            InitializeComponent();

            // 변수 초기화
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
        }



        /// <summary>
        /// 페이지 로딩 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // UI 설정
                rootResultScrollViewer.Visibility = Visibility.Collapsed;

                isInit = true;

                // 계정 정보 설정
                accountIDTextBlock.Text = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id;
                accountEmailTextBlock.Text = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.email;

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Initializing...");

                // 설정 파일 읽기
                SettingManager settingManager = new SettingManager();
                SettingManager.SettingData settingData = null;

                await Task.Run(() => 
                {
                    try
                    {
                        settingData = settingManager.readSettingData();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                if (settingData != null)
                {
                    BootStartUtaitePlayerToggleButton.IsChecked = settingData.gs_boot_start;
                    CrashHandlerRunToggleButton.IsChecked = settingData.gs_use_crash_handler;
                    CloseButtonSettingToggleButton.IsChecked = settingData.gs_close_button_event == 0 ? false : true;
                    TopMostToggleButton.IsChecked = settingData.gs_window_top_most;

                    if (settingData.gs_start_mod == 0)
                    {
                        StartModForWindowMODRadioButton.IsChecked = true;
                        StartModForBackgroundMODRadioButton.IsChecked = false;
                    }
                    else
                    {
                        StartModForWindowMODRadioButton.IsChecked = false;
                        StartModForBackgroundMODRadioButton.IsChecked = true;
                    }

                    deviceComboBox.Items.Clear();

                    await Task.Run(() =>
                    {
                        try
                        {
                            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                            string deviceName = null;

                            int defaultID = -1;
                            if (registryManager.isSetAudioDeviceID())
                                defaultID = registryManager.getAudioDeviceID();

                            for (int n = 0; n < WaveOut.DeviceCount; n++)
                            {
                                var caps = WaveOut.GetCapabilities(n);

                                if (n == defaultID) deviceName = caps.ProductName;

                                Application.Current.Dispatcher.Invoke(() => deviceComboBox.Items.Add(caps.ProductName));
                            }

                            backSelectedIndexForAudioDevice = (deviceName != null && defaultID != -1) ? defaultID : -1;
                            Application.Current.Dispatcher.Invoke(() => deviceComboBox.SelectedIndex = backSelectedIndexForAudioDevice);
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    programVersionTextBlock.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    await Task.Run(() =>
                    {
                        try
                        {
                            RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                            string jsonValue = utaitePlayerClient.getProgramInfo();
                            if (jsonValue != null)
                            {
                                JObject jObject = JObject.Parse(HttpUtility.UrlDecode(jsonValue));
                                if (jObject.ContainsKey("update_description_for_windows"))
                                {
                                    string text = jObject["update_description_for_windows"].ToString();
                                    Application.Current.Dispatcher.Invoke(() => 
                                    {
                                        FlowDocument flowDocument = new FlowDocument();
                                        flowDocument.Blocks.Add(new Paragraph(new Run(text)));
                                        updateTextRichTextBox.Document = flowDocument;
                                    });

                                    return;
                                }
                            }

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                FlowDocument flowDocument = new FlowDocument();
                                flowDocument.Blocks.Add(new Paragraph(new Run("데이터 불러오는 중 오류 발생")));
                                updateTextRichTextBox.Document = flowDocument;
                            });
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex.ToString());
                        }
                    });

                    // UI 설정
                    rootResultScrollViewer.Visibility = Visibility.Visible;

                    isInit = false;
                }
                else
                {
                    ExceptionManager.getInstance().showMessageBox("설정 데이터 로딩 중 알 수 없는 오류가 발생하였습니다. 다시 시도해 주십시오.");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
        }



        /// <summary>
        /// 로그아웃 Hyperlink 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogoutHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YesOrNoDialogInfo yesOrNoDialogInfo = new YesOrNoDialogInfo();
                yesOrNoDialogInfo.title = "로그아웃";
                yesOrNoDialogInfo.message = "로그아웃을 진행하시겠습니까?";
                yesOrNoDialogInfo.button1Title = "취소";
                yesOrNoDialogInfo.button2Title = "로그아웃";
                yesOrNoDialogInfo.button1Event = new Action(() =>
                {
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_DIALOG_YES_OR_NO, null);
                });
                yesOrNoDialogInfo.button2Event = new Action(async () =>
                {
                    try
                    {
                        // 로그아웃
                        try
                        {
                            // 노래 중지
                            PlayerService.getInstance().stopMusic();

                            // 데이터 제거
                            await Task.Run(() =>
                            {
                                // 플레이리스트 제거
                                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();
                                musicDataManager.deletePlaylistFile();

                                // 레지스트리 제거
                                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                registryManager.deleteAuthToken();
                            });
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }

                        // 전역 Dialog 설정
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);

                        // 로그아웃
                        RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().hide();
                        ExceptionManager.getInstance().exitProgram();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_DIALOG_YES_OR_NO, yesOrNoDialogInfo);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 계정 설정 Hyperlink 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountSettingHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                System.Diagnostics.Process.Start(string.Format("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/edit_my_account.jsp?isNoRed=1&backurl=1&auth={0}", registryManager.getAuthToken()));
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    ExceptionManager.getInstance().showMessageBox(noBrowser);
            }
            catch (System.Exception other)
            {
                ExceptionManager.getInstance().showMessageBox(other);
            }
        }



        /// <summary>
        /// 컴퓨터 부팅시 실행 여부 ToggleButton 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BootStartUtaitePlayerToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");

                bool value = (bool)((ToggleButton)sender).IsChecked;

                // 관리자 권한 확인
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        StartAdministratorProcess startAdministratorProcess = new StartAdministratorProcess();
                        startAdministratorProcess.Run();

                        return;
                    }

                    try
                    {
                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                        if (value)
                            registryManager.setRunRegistry();
                        else
                            registryManager.deleteRunRegistry();
                    }
                    catch (Exception) { }

                    await Task.Run(() =>
                    {
                        try
                        {
                            SettingManager settingManager = new SettingManager();
                            SettingManager.SettingData settingData = settingManager.readSettingData();
                            settingData.gs_boot_start = value;
                            settingManager.writeSettingData(settingData);
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }
                else
                {
                    StartAdministratorProcess startAdministratorProcess = new StartAdministratorProcess();
                    startAdministratorProcess.Run();

                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }

            RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
        }



        /// <summary>
        /// CrashHandler 실행 여부 ToggleButton 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CrashHandlerRunToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS, new GrowlInfo()
                {
                    Message = "해당 설정은 재시작 후 적용됩니다."
                });

                bool value = (bool)((ToggleButton)sender).IsChecked;

                await Task.Run(() =>
                {
                    SettingManager settingManager = new SettingManager();
                    SettingManager.SettingData settingData = settingManager.readSettingData();
                    settingData.gs_use_crash_handler = value;
                    settingManager.writeSettingData(settingData);
                });

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 리소스 초기화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ResourcesClear_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Removing all resources...");

                // 데이터 파일 제거
                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();
                await Task.Run(() => musicDataManager.deleteAllResourceFile());

                ExceptionManager.getInstance().exitProgram();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 예약 정지 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReservationStopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int hour = (int)ReservationStopForHourNumericUpDown.Value;
                int minute = (int)ReservationStopForMinuteNumericUpDown.Value;

                if (hour == 0 && minute == 0)
                {
                    ReservationStopTime.Text = "00:00";
                    IsEnableTimerTextBlock.Text = "타이머 멉춤";

                    reservationStopTime = 0;
                    reservationStopTimeForNow = 0;

                    if (dispatcherTimer.IsEnabled)
                        dispatcherTimer.Stop();
                }
                else
                {
                    IsEnableTimerTextBlock.Text = "타이머 작동 중";

                    reservationStopTime = (hour * 60 * 60) + (minute * 60);
                    reservationStopTimeForNow = 0;

                    ReservationStopTime.Text = string.Format("{0:D2}:{1:D2}", hour, minute);

                    if (!dispatcherTimer.IsEnabled)
                        dispatcherTimer.Start();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 타이머 Tick 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (reservationStopTime == 0)
                            return;

                        reservationStopTimeForNow = reservationStopTimeForNow + 1;

                        if (reservationStopTime <= reservationStopTimeForNow)
                        {
                            reservationStopTime = 0;
                            reservationStopTimeForNow = 0;
                            dispatcherTimer.Stop();

                            Application.Current.Dispatcher.Invoke(() => IsEnableTimerTextBlock.Text = "타이머 멉춤");
                            Application.Current.Dispatcher.Invoke(() => 
                            {
                                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS, new GrowlInfo()
                                {
                                    Message = "예약 정지 타이머가 작동하여 노래가 일시 정지되었습니다."
                                });

                                if (PlayerService.getInstance().getPlaybackState() == PlaybackState.Playing)
                                    PlayerService.getInstance().pauseMusic();
                                
                            });

                            return;
                        }

                        int temp = (reservationStopTime - reservationStopTimeForNow) / 60;

                        Application.Current.Dispatcher.Invoke(() => ReservationStopTime.Text = string.Format("{0:D2}:{1:D2}", temp / 60, temp % 60));
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 출력 장치 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void deviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (isInit) return;

                if (((ComboBox)sender).SelectedIndex == backSelectedIndexForAudioDevice) return;

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                registryManager.setAudioDeviceID(deviceComboBox.SelectedIndex);

                await Task.Run(() => 
                {
                    try
                    {
                        PlayerService.getInstance().changeAudioDevice();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                backSelectedIndexForAudioDevice = ((ComboBox)sender).SelectedIndex;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
        }




        /// <summary>
        /// 닫기 버튼으로 바로 종료하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CloseButtonSettingToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");

                bool value = (bool)((ToggleButton)sender).IsChecked;

                await Task.Run(() =>
                {
                    SettingManager settingManager = new SettingManager();
                    SettingManager.SettingData settingData = settingManager.readSettingData();

                    int inputValue = 0;

                    if (value) inputValue = 1;

                    settingData.gs_close_button_event = inputValue;
                    settingManager.writeSettingData(settingData);
                });

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
        }



        /// <summary>
        /// 우타이테 플레이어를 가장 위에 있게 하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TopMostToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");

                bool value = (bool)((ToggleButton)sender).IsChecked;

                await Task.Run(() =>
                {
                    SettingManager settingManager = new SettingManager();
                    SettingManager.SettingData settingData = settingManager.readSettingData();

                    settingData.gs_window_top_most = value;
                    settingManager.writeSettingData(settingData);
                });

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MAIN_WINDOW_TOP_MOST_SETTING, value);
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
        }



        /// <summary>
        /// 프로그램 시작 모드 설정 - Window MOD 라디오 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartModForWindowMODRadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");
                
                bool value = (bool)((RadioButton)sender).IsChecked;

                await Task.Run(() =>
                {
                    SettingManager settingManager = new SettingManager();
                    SettingManager.SettingData settingData = settingManager.readSettingData();

                    if (value)
                    {
                        settingData.gs_start_mod = 0;
                    }
                    else
                    {
                        settingData.gs_start_mod = 1;
                    }

                    settingManager.writeSettingData(settingData);
                });

                if (value)
                {
                    StartModForWindowMODRadioButton.IsChecked = true;
                    StartModForBackgroundMODRadioButton.IsChecked = false;
                }
                else
                {
                    StartModForWindowMODRadioButton.IsChecked = false;
                    StartModForBackgroundMODRadioButton.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
        }



        /// <summary>
        /// 프로그램 시작 모드 설정 - Background MOD 라디오 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartModForBackgroundMODRadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving settings...");

                bool value = (bool)((RadioButton)sender).IsChecked;

                await Task.Run(() =>
                {
                    SettingManager settingManager = new SettingManager();
                    SettingManager.SettingData settingData = settingManager.readSettingData();

                    if (value)
                    {
                        settingData.gs_start_mod = 1;
                    }
                    else
                    {
                        settingData.gs_start_mod = 0;
                    }

                    settingManager.writeSettingData(settingData);
                });

                if (value)
                {
                    StartModForWindowMODRadioButton.IsChecked = false;
                    StartModForBackgroundMODRadioButton.IsChecked = true;
                }
                else
                {
                    StartModForWindowMODRadioButton.IsChecked = true;
                    StartModForBackgroundMODRadioButton.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
        }
    }
}
