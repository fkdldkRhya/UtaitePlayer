﻿using HandyControl.Controls;
using HandyControl.Data;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UtaitePlayer.Classes.Core;
using UtaitePlayer.Classes.DataVO;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // ----------------------------------------------
        // 기타 리소스 데이터
        // ----------------------------------------------
        // ** 셔플 버튼 [ Checked ]
        private readonly BitmapImage SHFFULBUTTON_CHECKED_BITMAPIMAGE = new BitmapImage(new Uri("pack://application:,,,/Resources/drawable/ic_shuffle_checked.png", UriKind.RelativeOrAbsolute));
        // ** 셔플 버튼 [ UnChecked ]
        private readonly BitmapImage SHFFULBUTTON_UNCHECKED_BITMAPIMAGE = new BitmapImage(new Uri("pack://application:,,,/Resources/drawable/ic_shuffle_unchecked.png", UriKind.RelativeOrAbsolute));

        // ** 반복 재생 버튼 [ Selected Color ]
        private readonly System.Windows.Media.Brush REPEATBUTTON_SELECTED_COLOR = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FFFFFF");
        // ** 반복 재생 버튼 [ UnSelected color ]
        private readonly System.Windows.Media.Brush REPEATBUTTON_UNSELECTED_COLOR = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#797979");

        // ** 데이터 없음 이미지
        private readonly BitmapImage NO_DATA_BITMAPIMAGE = new BitmapImage(new Uri("pack://application:,,,/Resources/drawable/img_no_data.png", UriKind.RelativeOrAbsolute));

        // ** 우타이테 플레이어 로고 백터 이미지 V1
        private readonly Geometry UTAITE_PLAYER_LOGO_VECTOR_V1 = Geometry.Parse("M158.4,476.2c-5.6,2.2-10.3,7.5-15,1.3c-3.4-4.4,0.9-9.9-1-14.9c-2.8,0.5-3.7,2.9-5.2,4.6c-3.8,4.4-7.9,8.4-12.8,11.5c-2.2,1.4-4.7,2.4-7.1,0.5c-2.2-1.7-1.9-4.1-1.3-6.4c7.9-32.3,4.7-65.3,5.3-98c0.1-2.8-0.7-5.3-3.9-5.7c-5.2-0.7-9-4.8-14.3-5.3c-0.8-0.1-2.1-0.6-2.2-1.1c-1.4-7-8.4-12.2-5.8-20.7c8.8-29,10.7-58.9,10.8-89.1c0-25.8-0.5-51.6,1.4-77.3c0.8-10.8,2.4-21.6,4.1-32.3c0.6-3.7,0.1-6-3.2-7.9c-3.3-1.9-6.1-4.7-8.5-7.7c-3.8-4.9-5.5-10.3-2.7-16.1c2.9-6,2.3-12,0.7-18c-4.4-16.3-5.8-33.1-8.2-49.8c-1.4-9.6,2.6-13.1,12.3-11.2c28.6,5.6,57.3,11,84.7,21.5c5,1.9,10,1.4,15.1,0.4c22.9-4.6,46-6.8,69.1-1.2c4.4,1.1,5.6-1.4,6.9-4.7c4.1-10.6,6.8-21.5,9.8-32.4c1-3.5,2.3-6.9,4.1-10c3.9-7,8.7-7.9,14.3-2.2c33.7,34.8,60.8,73.6,73.3,121.2c4,15.3,10.4,29.8,12.9,45.4c3.7,23,4.4,46.2,5,69.4c1,41.3,1.7,82.7,1.1,124c0,0.9-0.1,1.7,0,2.6c0.5,2.9-2.4,7.3,0.1,8.3c3.2,1.3,5.4-3.2,7.7-5.5c12.9-13.4,28.1-24.2,41.5-37c2-1.9,4.2-4.1,7.3-2.5c3.1,1.6,3.4,4.8,3.2,7.8c-1.2,12.9-2.9,25.7-7.4,38.1c-5.5,15.2-9.3,30.9-13.1,46.7c-2.8,12-9.2,22.9-15.2,33.7c-0.8,1.5-1.6,2.9-2.8,5.2c7.8-0.8,14-4,20.6-5.7c3.4-0.9,7.3-2.8,10.1,0.7c2.8,3.4,0.6,7.1-1,10.2c-11.3,21.4-29,34-53.2,35.5c-24.7,1.6-49.4-0.2-74-3.3c-11.4-1.4-22.5,1.4-33.7,2.8c-28.1,3.5-56.3,6.1-84.5,8.4c-8.1,0.7-16.1,1.4-24.2,2.1c-7,0.7-13.9-2.6-15-9.1C163,494,157.2,486.4,158.4,476.2z");
        // ----------------------------------------------
        // ----------------------------------------------

        // 종료 확인 변수
        private bool isNoExit = true;

        // 노래 정보 동기화 전용 Thread
        private Thread syncThread;
        private bool syncThreadExit = true;
        // 노래 정보 동기화 전용 Thread 컨트롤 변수
        private bool syncThreadPause = false;

        // 버튼 비활성화 변수
        private bool isButtonEnable = true;

        // ----------------------------------------------
        // 페이지 데이터
        // ----------------------------------------------
        private System.Windows.Controls.Page searchResultPage = null; // 검색 결과
        private System.Windows.Controls.Page utaitePlayerHomePage = null; // 메인 화면
        private System.Windows.Controls.Page subscribeManagePage = null; // 구독 관리 화면
        private System.Windows.Controls.Page musicPlayCountPage = null; // 노래 재생 횟수 화면
        private System.Windows.Controls.Page myPlaylistPage = null; // 나의 플레이리스트 화면
        private System.Windows.Controls.Page songAddPage = null; // 노래 신청 화면
        private System.Windows.Controls.Page announcementPage = null; // 공지사항 화면
        private System.Windows.Controls.Page settingPage = null; // 설정 화면
        // ----------------------------------------------
        // ----------------------------------------------

        // 현재 페이지 정보
        private string nowPage = null;

        // 플레이리스트 이미지
        private List<OnlyImageDataVO> MyPlaylist_ImageList = null;

        // 노래 정보 Drawer 변수 - 선택한 노래 UUID
        private string x_DrawerRightForMusicInfo_MusicUUID = null;

        // 플레이리스트 편집 Drawer 변수 - 선택한 플레이리스트 UUID
        private string x_DrawerRightForEditMyPlaylist_PlaylistUUID = null;
        // 플레이리스트 편집 Drawer 변수 - 선택한 이미지
        private OnlyImageDataVO x_DrawerRightForEditMyPlaylist_OnlyImageDataVO = new OnlyImageDataVO();

        // 플레이리스트 생성 Drawer 변수 - 선택한 이미지
        private OnlyImageDataVO x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO = new OnlyImageDataVO();

        // 플레이리스트 노래 추가 Drawer 변수 - 플레이리스트
        private List<SelectMyPlaylistDataVO> x_DrawerBottomForAddMusicToMyPlaylist_Playlist = new List<SelectMyPlaylistDataVO>();
        // 플레이리스트 노래 추가 Drawer 변수 - 요청한 노래 UUID
        private string x_DrawerBottomForAddMusicToMyPlaylist_MusicUUID = null;
        // 플레이리스트 노래 추가 Drawer 변수 - 요청한 노래 리스트
        private List<string> x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDs = new List<string>();
        // 플레이리스트 노래 추가 Drawer 변수 - 요청한 노래 리스트 타입
        private int x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDType = 0;





        /// <summary>
        /// 생성자
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // 전역 함수 등록
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_REFRESH_MY_PLAYLIST, refreshMyPlaylist);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_RESET_MUSIC_PANEL_INFO, resetMusicInfoPanel);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, showMusicInfoDrawer);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_EDIT_PLAYLIST_DRAWER, x_DrawerRightForEditMyPlaylist);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_CREATE_PLAYLIST_DRAWER, x_DrawerRightForCreateMyPlaylist);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER, showAddMusicToMyPlaylistDrawer);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS, showGrowlMessageForSuccess);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, musicAddPlaylist);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY, musicAddPlaylistForArray);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, musicPlay);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, showLoadingDialog);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, hideLoadingDialog);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SEARCH_FOR_TEXT, searchForText);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_MAIN_WINDOW_TOP_MOST_SETTING, thisWindowTopmostSetting);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_DIALOG_YES_OR_NO, x_YesOrNoDailogSettingForShow);
            RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_DIALOG_YES_OR_NO, x_YesOrNoDailogSettingForHide);

            // 이벤트 설정
            ((INotifyCollectionChanged) currentPlaylist.Items).CollectionChanged += CurrentPlayListView_CollectionChanged;
        }



        /// <summary>
        /// Windows loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 초기화 UI 설정
                showLoadingDialog("Initializing...");

                // 시작 위치 설정
                this.Left = (SystemParameters.WorkArea.Width) / 2 + SystemParameters.WorkArea.Left - Width / 2;
                this.Top = (SystemParameters.WorkArea.Height) / 2 + SystemParameters.WorkArea.Left - Height / 2;

                // 설정 반영
                try
                {
                    SettingManager.SettingData settingData = null;
                    await Task.Run(() => 
                    {
                        try
                        {
                            SettingManager settingManager = new SettingManager();

                            settingData = settingManager.readSettingData();
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    if (settingData != null)
                    {
                        // Topmost
                        thisWindowTopmostSetting(settingData.gs_window_top_most);
                        // isBackground
                        if (settingData.gs_start_mod == 1) Hide();
                    }
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox(ex);
                }

                // TrayIcon 설정
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().show();
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().actionForDoubleClick = new Action(() => // 더블 클릭 이벤트
                {
                    this.Show();
                });
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().actionForExitMenu = new Action(() => // 종료 메뉴 버튼 클릭 이벤트
                {
                    // 노래 중지
                    PlayerService.getInstance().stopMusic();

                    // 프로그램 종료
                    isNoExit = false;
                    RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().hide();
                    ExceptionManager.getInstance().exitProgram();
                });
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().actionForLogoutMenu = new Action(async () => // 로그아웃 메뉴 버튼 클릭 이벤트
                {
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

                    // 로그아웃
                    isNoExit = false;
                    RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().hide();
                    ExceptionManager.getInstance().exitProgram();
                });
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().actionForMutetMenu = new Action(() => // 음소거 메뉴 버튼 클릭 이벤트
                {
                    Application.Current.Dispatcher.Invoke(() => soundValueSlider.Value = 0);
                });
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().actionForUnMutetMenu = new Action(() => // 음소거 해제 메뉴 버튼 클릭 이벤트
                {
                    Application.Current.Dispatcher.Invoke(() => soundValueSlider.Value = 10);
                });

                // Window state 상태 반영
                if (this.WindowState == WindowState.Maximized)
                {
                    maximizeButton.Visibility = Visibility.Collapsed;
                    restoreButton.Visibility = Visibility.Visible;
                }
                else
                {
                    maximizeButton.Visibility = Visibility.Visible;
                    restoreButton.Visibility = Visibility.Collapsed;
                }

                // 전역함수 등록
                RHYAGlobalFunctionManager.Register(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_AUTH_CHECK_MANAGER_DIALOG, setStateAuthCheckManagerDialog);

                // ChromiumWebBrowser 설정
                musicInfoChromiumWebBrowser.MenuHandler = new CefSharpContextMenu();
                musicInfoChromiumWebBrowser.LifeSpanHandler = new MyCustomLifeSpanHandler();

                // 라이브러리 선언
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();

                // 노래 정보 설정 이벤트 리스너
                PlayerService.getInstance().setMusicInfoSettingListener(async () =>
                {
                    // 비동기 작업
                    await Task.Run(() =>
                    {
                        try
                        {
                            // Null 확인
                            if (PlayerService.getInstance().getMusicInfo() != null)
                            {
                                // 노래 데이터 설정 - 이미지
                                using (WebClient wc = new WebClient())
                                {
                                    byte[] buffer = wc.DownloadData(new Uri(PlayerService.getInstance().getMusicInfo().image, UriKind.Absolute));
                                    wc.Dispose();

                                    using (MemoryStream ms = new MemoryStream(buffer))
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            try
                                            {
                                                // Bitmap 이미지
                                                BitmapImage img = new BitmapImage();

                                                // Stream 데이터 설정
                                                img.BeginInit();
                                                img.CacheOption = BitmapCacheOption.OnLoad;
                                                img.StreamSource = ms;
                                                img.EndInit();

                                                // 이미지 설정
                                                musicImage.Source = img;
                                            }
                                            catch (Exception ex)
                                            {
                                                ExceptionManager.getInstance().showMessageBox(ex);
                                            }
                                        });

                                        ms.Dispose();
                                    }
                                }

                                // 노래 데이터 설정 - 기본 정보
                                if (PlayerService.getInstance().getMusicInfo().name != null)
                                {
                                    musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(string.Format("changeMarqueeText('{0}')",
                                        PlayerService.getInstance().getMusicInfo().name.Replace("'", "\\'")));
                                    musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(string.Format("changeSubMarqueeText('{0}')",
                                        RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.ContainsKey(PlayerService.getInstance().getMusicInfo().singerUUID) ?
                                        RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[PlayerService.getInstance().getMusicInfo().singerUUID].name.Replace("'", "\\'") :
                                        "아티스트 정보 없음"));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                });

                // 노래 종료 이벤트 리스너 설정
                PlayerService.getInstance().setPlaybackStoppedListener(async () =>
                {
                    try
                    {
                        // 플레이 리스트 개수 확인
                        if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count > 0)
                        {
                            // 현재 노래 반복 재생 확인
                            if (PlayStateManager.getInstance().isRepeat == PlayStateManager.RepeatState.ONLY_ONE_REPEAT)
                            {
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        string musicUUID = PlayerService.getInstance().getMusicInfo().uuid;
                                        PlayerService.getInstance().stopMusic();
                                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                        PlayerService.getInstance().putMusicForURL(musicUUID, (string)registryManager.getAuthToken());
                                        PlayerService.getInstance().playMusic();
                                    }
                                    catch (Exception ex)
                                    {
                                        // 예외 처리
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                });

                                return;
                            }

                            // 셔플 기능 확인
                            if (PlayStateManager.getInstance().isShfful)
                            {
                                int randomIndex = new Random().Next(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1);
                                RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[randomIndex];

                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                                            Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                                    }
                                    catch (Exception ex)
                                    {
                                        // 예외 처리
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                });

                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        Application.Current.Dispatcher.Invoke(() => userPlaylistInfoVO.isPlay = true);
                                        Application.Current.Dispatcher.Invoke(() => currentPlaylist.Items.Refresh());

                                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = randomIndex;
                                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(randomIndex);

                                        PlayerService.getInstance().stopMusic();
                                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                        PlayerService.getInstance().putMusicForURL(
                                            userPlaylistInfoVO.uuid,
                                            (string)registryManager.getAuthToken());
                                        PlayerService.getInstance().playMusic();
                                    }
                                    catch (Exception ex)
                                    {
                                        // 예외 처리
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                });

                                return;
                            }

                            // 플레이리스트 인덱스 확인
                            if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1 <= RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex)
                            {
                                // 노래 전체 반복 재생 확인
                                if (PlayStateManager.getInstance().isRepeat == PlayStateManager.RepeatState.ALL_REPEAT)
                                {
                                    // 처음 노래 재생
                                    RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[0];

                                    await Task.Run(() =>
                                    {
                                        try
                                        {
                                            for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                                                Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                                        }
                                        catch (Exception ex)
                                        {
                                            // 예외 처리
                                            ExceptionManager.getInstance().showMessageBox(ex);
                                        }
                                    });

                                    await Task.Run(() =>
                                    {
                                        try
                                        {
                                            Application.Current.Dispatcher.Invoke(() => userPlaylistInfoVO.isPlay = true);
                                            Application.Current.Dispatcher.Invoke(() => currentPlaylist.Items.Refresh());

                                            RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = 0;
                                            new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(0);

                                            PlayerService.getInstance().stopMusic();
                                            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                            PlayerService.getInstance().putMusicForURL(
                                                userPlaylistInfoVO.uuid,
                                                (string)registryManager.getAuthToken());
                                            PlayerService.getInstance().playMusic();
                                        }
                                        catch (Exception ex)
                                        {
                                            // 예외 처리
                                            ExceptionManager.getInstance().showMessageBox(ex);
                                        }
                                    });

                                    return;
                                }

                                // 노래 종료
                                PlayerService.getInstance().stopMusic();
                            }
                            else
                            {
                                // 다음 노래 재생
                                int nextMusicIndex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex + 1;
                                RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[nextMusicIndex];

                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                                            Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                                    }
                                    catch (Exception ex)
                                    {
                                        // 예외 처리
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                });

                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        Application.Current.Dispatcher.Invoke(() => userPlaylistInfoVO.isPlay = true);
                                        Application.Current.Dispatcher.Invoke(() => currentPlaylist.Items.Refresh());

                                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = nextMusicIndex;
                                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(nextMusicIndex);

                                        PlayerService.getInstance().stopMusic();
                                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                        PlayerService.getInstance().putMusicForURL(
                                            userPlaylistInfoVO.uuid,
                                            (string)registryManager.getAuthToken());
                                        PlayerService.getInstance().playMusic();
                                    }
                                    catch (Exception ex)
                                    {
                                        // 예외 처리
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                });
                            }
                        }
                        else
                        {
                            // 노래 데이터 설정 - 기본 정보
                            musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(string.Format("changeMarqueeText('{0}')",
                                PlayerService.getInstance().getMusicInfo().name.Replace("'", "\\'")));
                            musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(string.Format("changeSubMarqueeText('{0}')",
                                RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.ContainsKey(PlayerService.getInstance().getMusicInfo().singerUUID) ?
                                RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[PlayerService.getInstance().getMusicInfo().singerUUID].name.Replace("'", "\\'") :
                                "아티스트 정보 없음"));
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // Music default info loading
                await musicInfoChromiumWebBrowser.LoadUrlAsync(utaitePlayerClient.RHYA_NETWORK_MARQUEE_TEXT_URL);
                musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("changeMarqueeText('현재 재생 중인 노래 없음')");
                musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("changeSubMarqueeText('현재 재생 중인 노래 없음')");

                // Current playlist item setting
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1 >= RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex
                    && RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex != -1
                    && RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count >= 0)
                {
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex].isPlay = true;
                    await Task.Run(() =>
                    {
                        try
                        {
                            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                            PlayerService.getInstance().putMusicForURL(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex].uuid, (string)registryManager.getAuthToken());
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }

                currentPlaylist.ItemsSource = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs;
                musicCountTextblock.Text = string.Format("노래({0})", RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count);

                // Event 설정 - Current playlist all select checkbox
                currentPlaylistAllSelectCheckbox.Click += async delegate (object s, RoutedEventArgs a)
                {
                    try
                    {
                        bool currentPlaylistAllSelectCheckboxValue = (bool)currentPlaylistAllSelectCheckbox.IsChecked;

                        if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count <= 0)
                        {
                            currentPlaylistItemCountTextblock.Text = "선택 0";
                            currentPlaylistAllSelectCheckbox.IsChecked = false;
                            return;
                        }

                        await Task.Run(() =>
                        {
                            try
                            {
                                for (int index = 0; index < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; index++)
                                {
                                    RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index];
                                    Application.Current.Dispatcher.Invoke(() => userPlaylistInfoVO.isChecked = currentPlaylistAllSelectCheckboxValue);
                                }

                                if (currentPlaylistAllSelectCheckboxValue)
                                {
                                    Application.Current.Dispatcher.Invoke(() => currentPlaylistItemCountTextblock.Text = string.Format("선택 {0}", RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count));
                                    Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Visible);
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() => currentPlaylistItemCountTextblock.Text = "선택 0");
                                    Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Collapsed);
                                }
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });


                        // ListView upadate
                        currentPlaylist.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                };

                // 시간 및 상태 동기화 Thread
                syncThread = new Thread(delegate ()
                {
                    while (syncThreadExit)
                    {
                        try
                        {
                            // 0.2초 대기
                            Thread.Sleep(200);

                            // Thread 일시정지 확인
                            if (syncThreadPause) return;

                            // 시간 설정
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                currentTimeTextBlock.Text = string.Format("{0:hh\\:mm\\:ss}", PlayerService.getInstance().getCurrentTime());
                                endTimeTextBlock.Text = string.Format("{0:hh\\:mm\\:ss}", PlayerService.getInstance().getTotalTime());
                            });

                            // 플레이 상태 동기화
                            PlaybackState playbackState = PlayerService.getInstance().getPlaybackState();
                            switch (playbackState)
                            {
                                // 일시 정지
                                case PlaybackState.Stopped:
                                case PlaybackState.Paused:
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            playButton.Visibility = Visibility.Visible;
                                            pauseButton.Visibility = Visibility.Collapsed;
                                        });

                                        break;
                                    }

                                // 재생
                                case PlaybackState.Playing:
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            playButton.Visibility = Visibility.Collapsed;
                                            pauseButton.Visibility = Visibility.Visible;
                                        });

                                        break;
                                    }
                            }

                            // Slider 동기화
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                musicTimeSlider.Maximum = PlayerService.getInstance().getLength();
                                musicTimeSlider.Value = PlayerService.getInstance().getPosition();
                            });

                            // 노래 볼륨 동기화
                            PlayerService.getInstance().setVolume(PlayStateManager.getInstance().musicVolume);
                        }
                        catch (Exception) { }
                    }
                });
                syncThread.Start();

                // 페이지 초기화
                searchResultPage = new Page.SearchResultPage();
                utaitePlayerHomePage = new Page.UtaitePlayerHomePage();
                subscribeManagePage = new Page.SubscribeManagePage();
                musicPlayCountPage = new Page.MusicPlayCountPage();
                myPlaylistPage = new Page.MyPlaylistPage();
                songAddPage = new Page.SongAddPage();
                announcementPage = new Page.AnnouncementPage();
                settingPage = new Page.PlayerSettingPage();

                // 초기화 UI 설정
                hideLoadingDialog();

                // 페이지 로딩
                mainFrame.Navigate(utaitePlayerHomePage);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Windows loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 변수 초기화
                MyPlaylistImageManager myPlaylistImageManager = new MyPlaylistImageManager();
                MyPlaylist_ImageList = myPlaylistImageManager.onlyImageDataVOs_xxxhdpi;
            }
            catch (Exception ex)
            {
                // 프로그램 종료
                isNoExit = false;
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().hide();
                ExceptionManager.getInstance().showMessageBox(ex);
                ExceptionManager.getInstance().exitProgram();
            }
        }



        /// <summary>
        /// Window state change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                maximizeButton.Visibility = Visibility.Collapsed;
                restoreButton.Visibility = Visibility.Visible;

                this.BorderThickness = new Thickness(
                    SystemParameters.WindowResizeBorderThickness.Left + SystemParameters.FixedFrameVerticalBorderWidth + 1,
                    SystemParameters.WindowResizeBorderThickness.Top + SystemParameters.FixedFrameHorizontalBorderHeight + 1,
                    SystemParameters.WindowResizeBorderThickness.Right + SystemParameters.FixedFrameVerticalBorderWidth + 1,
                    SystemParameters.WindowResizeBorderThickness.Bottom + SystemParameters.FixedFrameHorizontalBorderHeight + 1);
            }
            else
            {
                maximizeButton.Visibility = Visibility.Visible;
                restoreButton.Visibility = Visibility.Collapsed;

                this.BorderThickness = new Thickness();
            }
        }



        /// <summary>
        /// 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void closeButton_Click(object sender, RoutedEventArgs e)
        {
            int buttonEvent = -1;

            await Task.Run(() => 
            {
                try
                {
                    SettingManager settingManager = new SettingManager();
                    buttonEvent = settingManager.readSettingData().gs_close_button_event;
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox(ex);
                }
            });

            // 이벤트 분리
            switch (buttonEvent)
            {
                default:
                    {
                        // 숨기기
                        this.Hide();

                        break;
                    }

                case 0:
                    {
                        // 숨기기
                        this.Hide();

                        break;
                    }

                case 1:
                    {
                        // 숨기기 및 종료
                        this.Hide();
                        ExceptionManager.getInstance().exitProgram();

                        break;
                    }
            }
        }



        /// <summary>
        /// 최소화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }



        /// <summary>
        /// 되돌리기 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }



        /// <summary>
        /// 최대화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }



        /// <summary>
        /// Window 종료 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 숨기기
            this.Hide();

            // 프로그램 종료 확인
            if (isNoExit)
            {
                e.Cancel = true;
            }
            else
            {
                RHYANetwork.UtaitePlayer.TrayIcon.RhyaTrayIcon.getInstance().hide();
                ExceptionManager.getInstance().exitProgram();
            }
        }



        /// <summary>
        /// 현재 플레이리스트 노래 Checkbox Click 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CurrentPlaylistMusicCheckbox_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    int count = 0;

                    bool isAllChecked = true;

                    for (int index = 0; index < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; index++)
                    {
                        RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index];

                        if (!userPlaylistInfoVO.isChecked)
                            isAllChecked = false;
                        else
                            count++;
                    }

                    Application.Current.Dispatcher.Invoke(() => currentPlaylistItemCountTextblock.Text = string.Format("선택 {0}", count));

                    if (count > 0)
                        Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Visible);
                    else
                        Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Collapsed);

                    if (isAllChecked)
                    {
                        for (int index = 0; index < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; index ++)
                        {
                            RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index];
                            Application.Current.Dispatcher.Invoke(() => userPlaylistInfoVO.isChecked = true);
                        }
                    }

                    Application.Current.Dispatcher.Invoke(() => currentPlaylistAllSelectCheckbox.IsChecked = isAllChecked);


                    // ListView upadate
                    Application.Current.Dispatcher.Invoke(() => currentPlaylist.Items.Refresh());
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox(ex);
                }
            });
        }



        /// <summary>
        /// 현재 플레이리스트 제거 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void currentPlaylistDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // 선택된 아이템 개수
            int selectedIndex = 0;

            await Task.Run(() =>
            {
                try
                {
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                    for (int index = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1; index >= 0; index--)
                    {
                        if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index].isChecked)
                        {
                            if (index == RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex)
                            {
                                // 노래 재생 중지
                                PlayerService.getInstance().stopMusic();
                                Application.Current.Dispatcher.Invoke(() => musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("changeMarqueeText('현재 재생 중인 노래 없음')"));
                                Application.Current.Dispatcher.Invoke(() => musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("changeSubMarqueeText('현재 재생 중인 노래 없음')"));
                                Application.Current.Dispatcher.Invoke(() => musicImage.Source = NO_DATA_BITMAPIMAGE);
                                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = -1;
                                registryManager.setMyPlaylistIndex(-1);
                            }
                            else if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex > 0 && index < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex)
                            {
                                int setMyPlaylistIndex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex - 1;
                                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = setMyPlaylistIndex;
                                registryManager.setMyPlaylistIndex(setMyPlaylistIndex);
                            }

                            selectedIndex = selectedIndex + 1;

                            Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.RemoveAt(index));
                        }
                    }

                    Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Collapsed);
                    Application.Current.Dispatcher.Invoke(() => musicCountTextblock.Text = string.Format("노래({0})", RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count));
                    Application.Current.Dispatcher.Invoke(() => currentPlaylistItemCountTextblock.Text = "선택 0");
                    Application.Current.Dispatcher.Invoke(() => currentPlaylistAllSelectCheckbox.IsChecked = false);

                    // 메시지 출력
                    showGrowlMessageForSuccess(new GrowlInfo()
                    {
                        Message = string.Format("{0}개의 항목이 플레이리스트에서 삭제되었습니다.", selectedIndex)
                    });

                    // 플레이리스트 저장
                    new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().writePlaylistResourcesFile();

                    // ListView upadate
                    Application.Current.Dispatcher.Invoke(() => currentPlaylist.Items.Refresh());
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox(ex);
                }
            });
        }



        /// <summary>
        /// 소리 크기 조절 Panel 숨기기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoundControlPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            soundControlPanel.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// 소리 크기 조절 Panel 보여주기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoundControlPanelSettingButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            soundControlPanel.Visibility = Visibility.Visible;
        }



        /// <summary>
        /// 셔플 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shffulButton_Click(object sender, RoutedEventArgs e)
        {
            shffulButton.Background = PlayStateManager.getInstance().isShfful ?
                new ImageBrush() { ImageSource = SHFFULBUTTON_UNCHECKED_BITMAPIMAGE } :
                new ImageBrush() { ImageSource = SHFFULBUTTON_CHECKED_BITMAPIMAGE };

            // 변수 설정
            PlayStateManager.getInstance().isShfful = !PlayStateManager.getInstance().isShfful;
        }



        /// <summary>
        /// 반복 재생 버튼 1 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repeatButton1_Click(object sender, RoutedEventArgs e)
        {
            if (PlayStateManager.getInstance().isRepeat == PlayStateManager.RepeatState.NO_REPEAT)
            {
                repeatButton1Path.Fill = REPEATBUTTON_SELECTED_COLOR;
                PlayStateManager.getInstance().isRepeat = PlayStateManager.RepeatState.ALL_REPEAT;
            }
            else if (PlayStateManager.getInstance().isRepeat == PlayStateManager.RepeatState.ALL_REPEAT)
            {
                repeatButton1Path.Fill = REPEATBUTTON_UNSELECTED_COLOR;
                repeatButton1.Visibility = Visibility.Collapsed;
                repeatButton2.Visibility = Visibility.Visible;
                PlayStateManager.getInstance().isRepeat = PlayStateManager.RepeatState.ONLY_ONE_REPEAT;
            }
        }



        /// <summary>
        /// 반복 재생 버튼 2 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repeatButton2_Click(object sender, RoutedEventArgs e)
        {
            if (PlayStateManager.getInstance().isRepeat == PlayStateManager.RepeatState.ONLY_ONE_REPEAT)
            {
                repeatButton1.Visibility = Visibility.Visible;
                repeatButton2.Visibility = Visibility.Collapsed;
                PlayStateManager.getInstance().isRepeat = PlayStateManager.RepeatState.NO_REPEAT;
            }
        }



        /// <summary>
        /// 재생 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PlayerService.getInstance().getPlaybackState() == PlaybackState.Stopped &&
                    PlayerService.getInstance().getMusicInfo() == null &&
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count > 0 &&
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex != -1)
                {
                    int nowPlayindex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex;
                    // 설정 변경
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[nowPlayindex].isPlay = false;
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[0].isPlay = true;
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = 0;
                    new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(0);

                    // 노래 재생
                    await Task.Run(() =>
                    {
                        try
                        {
                            // 노래 재생
                            UtaitePlayer.Classes.Core.PlayerService.getInstance().putMusicForURL(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[0].uuid, new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getAuthToken().ToString());
                            UtaitePlayer.Classes.Core.PlayerService.getInstance().playMusic();
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    // 새로고침
                    refreshMyPlaylist(null);

                    // 종료
                    return;
                }

                // 재생 상태 확인 
                if (PlayerService.getInstance().getPlaybackState() == PlaybackState.Paused || PlayerService.getInstance().getPlaybackState() == PlaybackState.Stopped && isButtonEnable)
                {
                    if (PlayerService.getInstance().getMusicInfo() != null)
                    {
                        // 노래 재생
                        PlayerService.getInstance().playMusic();
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 일시정지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PlayerService.getInstance().getPlaybackState() == PlaybackState.Playing && isButtonEnable)
                {
                    if (PlayerService.getInstance().getMusicInfo() != null)
                    {
                        // 노래 재생
                        PlayerService.getInstance().pauseMusic();
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 볼륨 설정 Slider 값 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayStateManager.getInstance().musicVolume = Convert.ToSingle(e.NewValue / 10);
        }



        /// <summary>
        /// 노래 시간 Slider 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicTimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Sync Thread 일시정지
            syncThreadPause = true;
            // 버튼 비활성화
            isButtonEnable = false;

            try
            {
                // 기본 설정 확인
                if ((PlayerService.getInstance().getPlaybackState() != PlaybackState.Stopped) && PlayerService.getInstance().getMusicInfo() != null)
                {
                    // 현재 플레이 상태 저장
                    PlaybackState playbackState = PlayerService.getInstance().getPlaybackState();

                    // 노래 일시정지
                    PlayerService.getInstance().pauseMusic();

                    // 노래 시간 변경
                    PlayerService.getInstance().setPosition((long)musicTimeSlider.Maximum / (long)musicTimeSlider.ActualWidth * (long)e.GetPosition((Slider)sender).X);

                    if (playbackState != PlaybackState.Paused)
                    {
                        // 노래 재생
                        PlayerService.getInstance().playMusic();
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }

            // Sync Thread 일시정지 해제
            syncThreadPause = false;
            // 버튼 활성화
            isButtonEnable = true;
        }



        /// <summary>
        /// 현재 플레이리스트 ListView 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void currentPlaylist_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((FrameworkElement)e.OriginalSource).DataContext as RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO != null)
                {
                    int index = currentPlaylist.SelectedIndex;
                    if (!(index == -1) && RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1 >= index)
                    {
                        RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index];

                        await Task.Run(() =>
                        {
                            try
                            {
                                for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                                    if (checkerIndex != index)
                                        Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });

                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = index;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(index);

                        userPlaylistInfoVO.isPlay = true;

                        currentPlaylist.Items.Refresh();

                        currentPlaylistBlockRect.Visibility = Visibility.Visible;

                        await Task.Run(() =>
                        {
                            try
                            {
                                PlayerService.getInstance().stopMusic();
                                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                PlayerService.getInstance().putMusicForURL(userPlaylistInfoVO.uuid, (string)registryManager.getAuthToken());
                                PlayerService.getInstance().playMusic();
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });

                        currentPlaylistBlockRect.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 이전 노래 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PerviousButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 인덱스 확인
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex != -1 &&
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count > 0 &&
                    PlayerService.getInstance().getMusicInfo() != null)
                {
                    // 시간 확인
                    if (PlayerService.getInstance().getCurrentTime() > TimeSpan.FromSeconds(3))
                    {
                        // 다시 재생
                        await Task.Run(() => PlayerService.getInstance().setPosition(0));
                        // 종료
                        return;
                    } 

                    int index = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex == 0 ?
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1 : RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex - 1;

                    RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index];

                    await Task.Run(() =>
                    {
                        try
                        {
                            for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                                if (checkerIndex != index)
                                    Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = index;
                    new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(index);

                    userPlaylistInfoVO.isPlay = true;

                    currentPlaylist.Items.Refresh();

                    currentPlaylistBlockRect.Visibility = Visibility.Visible;

                    await Task.Run(() =>
                    {
                        try
                        {
                            PlayerService.getInstance().stopMusic();
                            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                            PlayerService.getInstance().putMusicForURL(userPlaylistInfoVO.uuid, (string)registryManager.getAuthToken());
                            PlayerService.getInstance().playMusic();
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    currentPlaylistBlockRect.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 다음 노래 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 인덱스 확인
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex != -1 &&
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count > 0 &&
                    PlayerService.getInstance().getMusicInfo() != null)
                {
                    int index = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex == RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1 ?
                        0 : RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex + 1;

                    RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[index];

                    await Task.Run(() =>
                    {
                        try
                        {
                            for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                                if (checkerIndex != index)
                                    Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = index;
                    new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(index);

                    userPlaylistInfoVO.isPlay = true;

                    currentPlaylist.Items.Refresh();

                    currentPlaylistBlockRect.Visibility = Visibility.Visible;

                    await Task.Run(() =>
                    {
                        try
                        {
                            PlayerService.getInstance().stopMusic();
                            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                            PlayerService.getInstance().putMusicForURL(userPlaylistInfoVO.uuid, (string)registryManager.getAuthToken());
                            PlayerService.getInstance().playMusic();
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    currentPlaylistBlockRect.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }




        /// <summary>
        /// AuthCheckManager Dialog 관리
        /// </summary>
        /// <param name="obj">True or False</param>
        private void setStateAuthCheckManagerDialog(object obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if ((bool)obj)
                    {
                        clickBlockPanelForAuthCheckManager.Visibility = Visibility.Collapsed;
                        x_AuthCheckManagerDialog.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        clickBlockPanelForAuthCheckManager.Visibility = Visibility.Visible;
                        x_AuthCheckManagerDialog.Visibility = Visibility.Visible;

                        // 노래 정지
                        if (PlayerService.getInstance().getPlaybackState() == PlaybackState.Playing)
                        {
                            if (PlayerService.getInstance().getMusicInfo() != null)
                            {
                                // 노래 재생
                                PlayerService.getInstance().pauseMusic();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.getInstance().showMessageBox(ex);
                }
            });
        }



        /// <summary>
        /// AuthCheckManager Dialog 프로그램 종료 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_AuthCheckManagerDialog_ExitButtonClick(object sender, RoutedEventArgs e)
        {
            ExceptionManager.getInstance().exitProgram();
        }



        /// <summary>
        /// AuthCheckManager Dialog 이용권 신청 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_AuthCheckManagerDialog_LicensesApplicationButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient().RHYA_NETWORK_LICENSES_APPLICATION_URL);
            }
            catch (Win32Exception noBrowser)
            {
                ExceptionManager.getInstance().showMessageBox(noBrowser);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Loading dialog 보여주기
        /// </summary>
        /// <param name="title">제목</param>
        private void showLoadingDialog(string title)
        {
            clickBlockPanelForLoadingDialog.Visibility = Visibility.Visible;
            x_LoadingDialog.Visibility = Visibility.Visible;
            x_LoadingDialogTitle.Content = title;
        }



        /// <summary>
        /// Loading dialog 보여주기
        /// </summary>
        /// <param name="title">Loading dialog 보여주기</param>
        private void showLoadingDialog(object title)
        {
            clickBlockPanelForLoadingDialog.Visibility = Visibility.Visible;
            x_LoadingDialog.Visibility = Visibility.Visible;
            x_LoadingDialogTitle.Content = (string)title;
        }



        /// <summary>
        /// Loading dialog 숨기기
        /// </summary>
        private void hideLoadingDialog()
        {
            clickBlockPanelForLoadingDialog.Visibility = Visibility.Collapsed;
            x_LoadingDialog.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// Loading dialog 숨기기
        /// </summary>
        private void hideLoadingDialog(object noUse)
        {
            clickBlockPanelForLoadingDialog.Visibility = Visibility.Collapsed;
            x_LoadingDialog.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// Side Menu 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SideMenu_SelectionChanged(object sender, HandyControl.Data.FunctionEventArgs<object> e)
        {
            try
            {
                // Null 확인
                if (nowPage != null)
                {
                    string inputPage = (string)((HandyControl.Controls.SideMenuItem)e.Info).Header;

                    inputPage = inputPage.Trim();

                    if (!inputPage.Equals(nowPage))
                    {
                        switch (inputPage)
                        {
                            // 우타이테 플레이어 홈 화면
                            case "홈":
                                {
                                    nowPage = "홈";
                                    mainFrame.NavigationService.Navigate(utaitePlayerHomePage);
                                    break;
                                }

                            // 우타이테 플레이어 구독 관리 화면
                            case "구독 관리":
                                {
                                    nowPage = "구독 관리";
                                    mainFrame.NavigationService.Navigate(subscribeManagePage);
                                    break;
                                }

                            // 우타이테 플레이어 노래 재생 횟수 화면
                            case "노래 재생 횟수":
                                {
                                    nowPage = "노래 재생 횟수";
                                    mainFrame.NavigationService.Navigate(musicPlayCountPage);
                                    break;
                                }

                            // 우타이테 플레이어 플레이리스트 화면
                            case "플레이리스트":
                                {
                                    nowPage = "플레이리스트";
                                    mainFrame.NavigationService.Navigate(myPlaylistPage);
                                    break;
                                }

                            // 우타이테 플레이어 노래 신청 화면
                            case "노래 신청":
                                {
                                    nowPage = "노래 신청";
                                    mainFrame.NavigationService.Navigate(songAddPage);
                                    break;
                                }

                            // 우타이테 플레이어 공지사항 화면
                            case "공지사항":
                                {
                                    nowPage = "공지사항";
                                    mainFrame.NavigationService.Navigate(announcementPage);
                                    break;
                                }

                            // 우타이테 플레이어 설정 화면
                            case "설정":
                                {
                                    nowPage = "설정";
                                    mainFrame.NavigationService.Navigate(settingPage);
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Search TextBox 키 입력 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Enter 감지
                if (e.Key == Key.Enter)
                {
                    string inputText = ((HandyControl.Controls.TextBox)sender).Text;

                    // 길이 확인
                    if (!(inputText.Replace(" ", "").Length <= 0))
                    {
                        mainSideMenuNowSelectDisable();
   
                        nowPage = "검색";
                        mainFrame.NavigationService.Navigate(searchResultPage);
                        ((Page.SearchResultPage)searchResultPage).searchForText(inputText);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            
        }



        /// <summary>
        /// 현재 플레이리스트 새로고침
        /// </summary>
        /// <param name="obj">No used</param>
        public async void refreshMyPlaylist(object obj)
        {
            try
            {
                // 선탣 항목 정보 재설정
                await Task.Run(() =>
                {
                    try
                    {
                        int count = 0;

                        bool isAllChecked = true;
                        foreach (RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO in RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs)
                        {
                            if (!userPlaylistInfoVO.isChecked)
                                isAllChecked = false;
                            else
                                count++;

                        }

                        Application.Current.Dispatcher.Invoke(() => currentPlaylistItemCountTextblock.Text = string.Format("선택 {0}", count));

                        if (count > 0)
                            Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Visible);
                        else
                            Application.Current.Dispatcher.Invoke(() => currentPlaylistDeleteButton.Visibility = Visibility.Collapsed);

                        if (isAllChecked)
                        {
                            foreach (RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO in RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs)
                                Application.Current.Dispatcher.Invoke(() => userPlaylistInfoVO.isChecked = true);
                        }

                        Application.Current.Dispatcher.Invoke(() => currentPlaylistAllSelectCheckbox.IsChecked = isAllChecked);


                        // ListView upadate
                        Application.Current.Dispatcher.Invoke(() => currentPlaylist.Items.Refresh());
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // 노래 정보 재설정
                musicCountTextblock.Text = string.Format("노래({0})", RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count);

                // 현재 플레이리스트 UI 다시 로딩
                currentPlaylist.Items.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 데이터 다시 설정
        /// </summary>
        /// <param name="obj">No used</param>
        public void resetMusicInfoPanel(object obj)
        {
            try
            {
                // 노래 데이터 설정 - 기본 정보
                musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("changeMarqueeText('현재 재생 중인 노래 없음')");
                musicInfoChromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("changeSubMarqueeText('현재 재생 중인 노래 없음')");
                musicImage.Source = NO_DATA_BITMAPIMAGE;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 정보 Drawer 표시
        /// </summary>
        /// <param name="uuid">노래 UUID</param>
        public async void showMusicInfoDrawer(object uuid)
        {
            try
            {
                // Loading ProgressBar 설정
                x_DrawerRightForMusicInfo_InfoPanel.Visibility = Visibility.Collapsed;
                x_DrawerRightForMusicInfo_LoadingProgressBar.Visibility = Visibility.Visible;

                clickBlockPanelForDrawer.Visibility = Visibility.Visible;

                // 노래 정보
                string musicUUID = uuid.ToString();
                RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[musicUUID];

                // 노래 아티스트 이름 설정
                if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.ContainsKey(musicInfoVO.singerUUID))
                {
                    x_DrawerRightForMusicInfo_CreditData_Artist.Text = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[musicInfoVO.singerUUID].name;
                    x_DrawerRightForMusicInfo_MainMusicArtist.Text = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[musicInfoVO.singerUUID].name;
                }
                else 
                {
                    x_DrawerRightForMusicInfo_MainMusicArtist.Text = "데이터 없음";
                    x_DrawerRightForMusicInfo_CreditData_Artist.Text = "데이터 없음";
                }

                // 노래 이름 설정
                x_DrawerRightForMusicInfo_MainMusicTitle.Text = musicInfoVO.name;

                // 크레딧 설정
                x_DrawerRightForMusicInfo_CreditData_SongWriter.Text = musicInfoVO.songWriter;
                x_DrawerRightForMusicInfo_CreditData_Type.Text = musicInfoVO.type;
                x_DrawerRightForMusicInfo_CreditData_UploadDate.Text = musicInfoVO.date;

                // 노래 가사 설정 초기회
                x_DrawerRightForMusicInfo_MoreLyricsTextBox.Text = "더보기";
                x_DrawerRightForMusicInfo_LyricsData.MaxHeight = 350;

                // Drawer 표시
                DrawerRightForMusicInfo.IsOpen = true;

                // 노래 이미지 설정
                await Task.Run(() =>
                {
                    try
                    {
                        // 노래 데이터 설정 - 이미지
                        using (WebClient wc = new WebClient())
                        {
                            byte[] buffer = wc.DownloadData(new Uri(musicInfoVO.image, UriKind.Absolute));
                            wc.Dispose();

                            using (MemoryStream ms = new MemoryStream(buffer))
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        // Bitmap 이미지
                                        BitmapImage img = new BitmapImage();

                                        // Stream 데이터 설정
                                        img.BeginInit();
                                        img.CacheOption = BitmapCacheOption.OnLoad;
                                        img.StreamSource = ms;
                                        img.EndInit();

                                        // 이미지 설정
                                        x_DrawerRightForMusicInfo_MainMusicImage.Source = img;
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                });

                                ms.Dispose();
                            }
                        }

                        // 노래 가사 설정
                        try
                        {
                            // 파일 경로 구하기
                            string lyricsFileName = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().getLyricsFileName(musicUUID);
                            // 파일 읽기
                            string readData = File.ReadAllText(lyricsFileName);
                            // 데이터 복호화
                            string encryptKey = new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getCryptoKey().ToString();
                            RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto mAESCrypto = new RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto();
                            readData = mAESCrypto.decryptAES(readData, encryptKey, mAESCrypto.MAIN_ENCRYPT_DECRYPT_IV, RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto.AESKeySize.SIZE_256);
                            // 가사 설정
                            Application.Current.Dispatcher.Invoke(() => x_DrawerRightForMusicInfo_LyricsData.Text = readData);
                        }
                        catch (Exception)
                        {
                            // 예외 처리
                            Application.Current.Dispatcher.Invoke(() => x_DrawerRightForMusicInfo_LyricsData.Text = "오류! - 가사 파일을 읽는 도중 오류가 발생함");
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // 노래 UUID 설정
                x_DrawerRightForMusicInfo_MusicUUID = musicUUID;

                // Loading ProgressBar 설정
                x_DrawerRightForMusicInfo_InfoPanel.Visibility = Visibility.Visible;
                x_DrawerRightForMusicInfo_LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 정보 Drawer 노래 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_DrawerRightForMusicInfo_MusicPlayButtonClick(object sender, RoutedEventArgs e)
        {
            x_DrawerRightForMusicInfo_MusicPlayButton.IsEnabled = false;

            try
            {
                // 노래 UUID Null 확인
                if (x_DrawerRightForMusicInfo_MusicUUID != null)
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, x_DrawerRightForMusicInfo_MusicUUID);
                
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }

            x_DrawerRightForMusicInfo_MusicPlayButton.IsEnabled = true;
        }



        /// <summary>
        /// 노래 정보 Drawer 노래 가사 더보기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_DrawerRightForMusicInfo_MoreLyricsTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 상태 확인
            if (x_DrawerRightForMusicInfo_MoreLyricsTextBox.Text.Equals("더보기"))
            {
                x_DrawerRightForMusicInfo_MoreLyricsTextBox.Text = "접기";
                x_DrawerRightForMusicInfo_LyricsData.MaxHeight = double.MaxValue;
            }
            else
            {
                x_DrawerRightForMusicInfo_MoreLyricsTextBox.Text = "더보기";
                x_DrawerRightForMusicInfo_LyricsData.MaxHeight = 250;
            }
        }



        /// <summary>
        /// Frame Navigated 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                try
                {
                    mainFrame.RemoveBackEntry();
                }
                catch (Exception) { }

                // 초기 UI 로딩 작업 완료 이벤트
                if (nowPage == null)
                {
                    // 변수 초기화
                    nowPage = "홈";

                    // 초기화 UI 설정
                    hideLoadingDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Main SideMenu 현재 선택된 아이템 선택 비활성화
        /// </summary>
        private void mainSideMenuNowSelectDisable()
        {
            try
            {
                ((HandyControl.Controls.SideMenuItem)mainSideMenu.Items[0]).IsSelected = false;
                ((HandyControl.Controls.SideMenuItem)mainSideMenu.Items[1]).IsSelected = false;
                ((HandyControl.Controls.SideMenuItem)mainSideMenu.Items[2]).IsSelected = false;
                ((HandyControl.Controls.SideMenuItem)mainSideMenu.Items[3]).IsSelected = false;
                ((HandyControl.Controls.SideMenuItem)mainSideMenu.Items[4]).IsSelected = false;
                ((HandyControl.Controls.SideMenuItem)mainSideMenu.Items[5]).IsSelected = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Growl Success 메시지 보여주기
        /// </summary>
        /// <param name="messageInfo">메시지 정보</param>
        public void showGrowlMessageForSuccess(object messageInfo)
        {
            try
            {
                ((GrowlInfo)messageInfo).Token = "RHYANETWORK_UP_MAINWINDOWS";
                ((GrowlInfo)messageInfo).Icon = UTAITE_PLAYER_LOGO_VECTOR_V1;
                ((GrowlInfo)messageInfo).IconBrush = REPEATBUTTON_SELECTED_COLOR;
                Application.Current.Dispatcher.Invoke(() => Growl.Success((GrowlInfo)messageInfo));
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트에 노래 추가
        /// </summary>
        /// <param name="musicUUID">노래 UUID</param>
        public async void musicAddPlaylist(object musicUUID)
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                showLoadingDialog("Loading...");

                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count + 1 > 100)
                {
                    // 제거 대상 노래 재생 감지
                    if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex == 0)
                    {
                        // 노래 중지
                        await Task.Run(() =>
                        {
                            try
                            {
                                // 상태 비교
                                if (UtaitePlayer.Classes.Core.PlayerService.getInstance().getPlaybackState() == PlaybackState.Playing
                                || UtaitePlayer.Classes.Core.PlayerService.getInstance().getPlaybackState() == PlaybackState.Paused)
                                    UtaitePlayer.Classes.Core.PlayerService.getInstance().stopMusic();
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });
                        // 인덱스 정보 변경
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = -1;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(-1);
                        // 노래 정보 재설정
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_RESET_MUSIC_PANEL_INFO, null);
                    }
                    else
                    {
                        int setMyPlaylistIndex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex - 1;
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = setMyPlaylistIndex;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(setMyPlaylistIndex);
                    }

                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.RemoveAt(0);
                }

                // 메시지 출력
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                    new GrowlInfo()
                    {
                        Message = "1개의 항목이 플레이리스트에 추가되었습니다."
                    });

                // 플레이리스트에 추가
                RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = new RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO() { uuid = (string)musicUUID, isPlay = false };
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().addUserPlaylistInfoVO(userPlaylistInfoVO);
                // 플레이리스트 저장
                new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().writePlaylistResourcesFile();

                // 노래 재생
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count == 1)
                {
                    userPlaylistInfoVO.isPlay = true;
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = 0;

                    // 노래 재생
                    await Task.Run(() =>
                    {
                        try
                        {
                            // 노래 재생
                            UtaitePlayer.Classes.Core.PlayerService.getInstance().putMusicForURL((string)musicUUID, registryManager.getAuthToken().ToString());
                            UtaitePlayer.Classes.Core.PlayerService.getInstance().playMusic();
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }

                refreshMyPlaylist(null);

                hideLoadingDialog();

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_REFRESH_MY_PLAYLIST, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 재생
        /// </summary>
        /// <param name="musicUUID">노래 UUID</param>
        public async void musicPlay(object musicUUID)
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                showLoadingDialog("Loading...");

                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count + 1 > 100)
                {
                    // 제거 대상 노래 재생 감지
                    if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex == 0)
                    {
                        // 노래 중지
                        await Task.Run(() =>
                        {
                            try
                            {
                                // 상태 비교
                                if (UtaitePlayer.Classes.Core.PlayerService.getInstance().getPlaybackState() == PlaybackState.Playing
                                || UtaitePlayer.Classes.Core.PlayerService.getInstance().getPlaybackState() == PlaybackState.Paused)
                                    UtaitePlayer.Classes.Core.PlayerService.getInstance().stopMusic();
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });
                        // 인덱스 정보 변경
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = -1;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(-1);
                        // 노래 정보 재설정
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_RESET_MUSIC_PANEL_INFO, null);
                    }
                    else
                    {
                        int setMyPlaylistIndex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex - 1;
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = setMyPlaylistIndex;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(setMyPlaylistIndex);
                    }

                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.RemoveAt(0);
                }

                // 플레이리스트 설정 변경
                await Task.Run(() =>
                {
                    try
                    {
                        for (int checkerIndex = 0; checkerIndex < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; checkerIndex++)
                            Application.Current.Dispatcher.Invoke(() => RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[checkerIndex].isPlay = false);
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // 플레이리스트에 추가
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().addUserPlaylistInfoVO(new RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO() { uuid = (string)musicUUID, isPlay = true });

                // 메시지 출력
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                    new GrowlInfo()
                    {
                        Message = "1개의 항목이 플레이리스트에 추가되었습니다."
                    });

                // 플레이리스트 상태 저장
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1;
                new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count - 1);
                // 플레이리스트 저장
                new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().writePlaylistResourcesFile();

                refreshMyPlaylist(null);

                // 노래 재생
                await Task.Run(() =>
                {
                    try
                    {
                        // 노래 재생
                        UtaitePlayer.Classes.Core.PlayerService.getInstance().putMusicForURL((string) musicUUID, registryManager.getAuthToken().ToString());
                        UtaitePlayer.Classes.Core.PlayerService.getInstance().playMusic();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                hideLoadingDialog();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트에 노래 추가 [배열]
        /// </summary>
        /// <param name="musicUUID">노래 UUID List</param>
        public async void musicAddPlaylistForArray(object musicUUIDList)
        {
            try
            {
                List<string> musicUUIDs = (List<string>)musicUUIDList;

                if (musicUUIDs.Count <= 0)
                {
                    hideLoadingDialog();
                    return;
                }

                showLoadingDialog("Loading...");

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count + musicUUIDs.Count > 100)
                {
                    int stopIndex = (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count + musicUUIDs.Count) - 100;
                    if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex <= stopIndex)
                    {
                        // 노래 중지
                        await Task.Run(() =>
                        {
                            try
                            {
                                // 상태 비교
                                if (UtaitePlayer.Classes.Core.PlayerService.getInstance().getPlaybackState() == PlaybackState.Playing
                                || UtaitePlayer.Classes.Core.PlayerService.getInstance().getPlaybackState() == PlaybackState.Paused)
                                    UtaitePlayer.Classes.Core.PlayerService.getInstance().stopMusic();
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });
                        // 인덱스 정보 변경
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = -1;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(-1);
                        // 노래 정보 재설정
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_RESET_MUSIC_PANEL_INFO, null);
                        // 제거
                        for (int i = 0; i < stopIndex; i++)
                            RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.RemoveAt(i);
                        
                    }
                    else
                    {
                        // 인덱스 정보 변경
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex - stopIndex;
                        new RHYANetwork.UtaitePlayer.Registry.RegistryManager().setMyPlaylistIndex(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex);
                        // 제거
                        for (int i = 0; i < stopIndex; i++)
                            RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.RemoveAt(i);
                    }
                }

                // 메시지 출력
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                    new GrowlInfo()
                    {
                        Message = string.Format("{0}개의 항목이 플레이리스트에 추가되었습니다.", musicUUIDs.Count)
                    });

                int orgCount = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count;
                int index = 0;

                foreach (string uuid in musicUUIDs)
                {
                    // 플레이리스트에 추가
                    RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = new RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO() { uuid = uuid, isPlay = false };
                    if (orgCount == 0 && index == 0) userPlaylistInfoVO.isPlay = true;

                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().addUserPlaylistInfoVO(userPlaylistInfoVO);

                    index++;
                }

                // 플레이리스트 저장
                new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().writePlaylistResourcesFile();

                // 노래 재생
                if (orgCount == 0)
                {
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = 0;

                    // 노래 재생
                    await Task.Run(() =>
                    {
                        try
                        {
                            // 노래 재생
                            UtaitePlayer.Classes.Core.PlayerService.getInstance().putMusicForURL(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[0].uuid, registryManager.getAuthToken().ToString());
                            UtaitePlayer.Classes.Core.PlayerService.getInstance().playMusic();
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }

                refreshMyPlaylist(null);

                hideLoadingDialog();

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_REFRESH_MY_PLAYLIST, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 검색 함수 [ 전역 함수 전용 ]
        /// </summary>
        /// <param name="text">검색 Text</param>
        public void searchForText(object text)
        {
            try
            {
                mainSideMenuNowSelectDisable();

                nowPage = "검색";
                mainFrame.NavigationService.Navigate(searchResultPage);
                ((Page.SearchResultPage)searchResultPage).searchForText((string)text);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 현재 플레이리스트 아이템 개수 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentPlayListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (currentPlaylist.Items.Count > 0)
            {
                noPlaylistDataImage.Visibility = Visibility.Collapsed;
                currentPlaylist.Visibility = Visibility.Visible;
            }
            else 
            {
                noPlaylistDataImage.Visibility = Visibility.Visible;
                currentPlaylist.Visibility = Visibility.Collapsed;
            }
        }



        /// <summary>
        /// Topmost 설정
        /// </summary>
        /// <param name="value">설정 값</param>
        private void thisWindowTopmostSetting(object value)
        {
            try
            {
                this.Topmost = (bool)value;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Yes or No dialog 설정 - 보여주기
        /// </summary>
        /// <param name="data">Dialog 데이터</param>
        private void x_YesOrNoDailogSettingForShow(object data)
        {
            try
            {
                clickBlockPanelForYesOrNoDialog.Visibility = Visibility.Visible;
                x_YesOrNoDialog.Visibility = Visibility.Visible; 

                YesOrNoDialogInfo yesOrNoDialogInfo = (YesOrNoDialogInfo)data;
                x_YesOrNoDialogTitle.Content = yesOrNoDialogInfo.title;
                x_YesOrNoDialogMessage.Text = yesOrNoDialogInfo.message;
                x_YesOrNoDialogButton1.Content = yesOrNoDialogInfo.button1Title;
                x_YesOrNoDialogButton2.Content = yesOrNoDialogInfo.button2Title;
                x_YesOrNoDialogButton1.Click += (sender, e) => yesOrNoDialogInfo.button1Event();
                x_YesOrNoDialogButton2.Click += (sender, e) => yesOrNoDialogInfo.button2Event();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Yes or No dialog 설정 - 숨기기
        /// </summary>
        /// <param name="value">사용 안함</param>
        private void x_YesOrNoDailogSettingForHide(object value)
        {
            try
            {
                clickBlockPanelForYesOrNoDialog.Visibility = Visibility.Collapsed;
                x_YesOrNoDialog.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 편집 Drawer 리스트 박스 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_DrawerRightForEditMyPlaylist_ImageSelectListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as UtaitePlayer.Classes.DataVO.OnlyImageDataVO != null))
                {
                    int selectedIndex = x_DrawerRightForEditMyPlaylist_ImageSelectListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    UtaitePlayer.Classes.DataVO.OnlyImageDataVO onlyImageDataVO = MyPlaylist_ImageList[selectedIndex];
                    x_DrawerRightForEditMyPlaylist_OnlyImageDataVO.image = onlyImageDataVO.image;
                    x_DrawerRightForEditMyPlaylist_OnlyImageDataVO.imageID = onlyImageDataVO.imageID;

                    x_DrawerRightForEditMyPlaylist_PlaylistImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,{0}", x_DrawerRightForEditMyPlaylist_OnlyImageDataVO.image), UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 편집 Drawer 보여주기
        /// </summary>
        /// <param name="dataVO">Playlist data VO</param>
        private void x_DrawerRightForEditMyPlaylist(object dataVO)
        {
            try
            {
                MyPlaylistDataVO myPlaylistDataVO = (MyPlaylistDataVO) dataVO;

                x_DrawerRightForEditMyPlaylist_RootGrid.Visibility = Visibility.Collapsed;
                x_DrawerRightForEditMyPlaylist_LoadingProgressBar.Visibility = Visibility.Visible;

                clickBlockPanelForDrawer.Visibility = Visibility.Visible;

                DrawerRightForEditMyPlaylist.IsOpen = true;

                x_DrawerRightForEditMyPlaylist_PlaylistUUID = myPlaylistDataVO.uuid;
                x_DrawerRightForEditMyPlaylist_PlaylistImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,{0}", myPlaylistDataVO.image), UriKind.RelativeOrAbsolute));
                x_DrawerRightForEditMyPlaylist_PlaylistNameTextbox.Text = myPlaylistDataVO.name;
                if (x_DrawerRightForEditMyPlaylist_ImageSelectListBox.ItemsSource == null) x_DrawerRightForEditMyPlaylist_ImageSelectListBox.ItemsSource = MyPlaylist_ImageList;

                x_DrawerRightForEditMyPlaylist_OnlyImageDataVO.image = myPlaylistDataVO.image;
                x_DrawerRightForEditMyPlaylist_OnlyImageDataVO.imageID = myPlaylistDataVO.imageID;

                x_DrawerRightForEditMyPlaylist_RootGrid.Visibility = Visibility.Visible;
                x_DrawerRightForEditMyPlaylist_LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 편집 Drawer 플레이리스트 이름 길이 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_DrawerRightForEditMyPlaylist_PlaylistNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                x_DrawerRightForEditMyPlaylist_PlaylistNameCount.Text = string.Format("{0}/40 글자", x_DrawerRightForEditMyPlaylist_PlaylistNameTextbox.Text.Length);
                x_DrawerRightForEditMyPlaylist_SaveButton.IsEnabled = x_DrawerRightForEditMyPlaylist_PlaylistNameTextbox.Text.Length <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 편집 Drawer 데이터 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void x_DrawerRightForEditMyPlaylist_SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Saving...");

                string name = x_DrawerRightForEditMyPlaylist_PlaylistNameTextbox.Text;
                bool isSuccess = false;

                await Task.Run(() => 
                {
                    try
                    {
                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                        string result1 = utaitePlayerClient.saveMyPlaylistName(registryManager.getAuthToken().ToString(), x_DrawerRightForEditMyPlaylist_PlaylistUUID, name);
                        string result2 = utaitePlayerClient.saveMyPlaylistImage(registryManager.getAuthToken().ToString(), x_DrawerRightForEditMyPlaylist_PlaylistUUID, x_DrawerRightForEditMyPlaylist_OnlyImageDataVO.imageID);

                        JObject object1 = JObject.Parse(result1);
                        JObject object2 = JObject.Parse(result2);

                        if (!(object1.ContainsKey("result") && object2.ContainsKey("result") && object1["result"].ToString().Equals("success") && object2["result"].ToString().Equals("success")))
                            ExceptionManager.getInstance().showMessageBox("플레이리스트 이름 또는 이미지 데이터를 설정하는 도중 오류가 발생하였습니다. 다시 시도하여 주십시오.");
                        else
                            isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                x_DrawerRightForEditMyPlaylist_ImageSelectListBox.SelectedItem = -1;

                if (isSuccess)
                    if (nowPage.Equals("플레이리스트"))
                        await Task.Run(() => ((Layout.Page.MyPlaylistPage)myPlaylistPage).Refresh());

                DrawerRightForEditMyPlaylist.IsOpen = false;

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 생성 Drawer 보여주기
        /// </summary>
        /// <param name="noUse">사용 </param>
        private void x_DrawerRightForCreateMyPlaylist(object noUse)
        {
            try
            {
                x_DrawerRightForCreateMyPlaylist_RootGrid.Visibility = Visibility.Collapsed;
                x_DrawerRightForCreateMyPlaylist_LoadingProgressBar.Visibility = Visibility.Visible;

                x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.image = MyPlaylist_ImageList[0].image;
                x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.imageID = MyPlaylist_ImageList[0].imageID;
                x_DrawerRightForCreateMyPlaylist_PlaylistImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,{0}", x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.image), UriKind.RelativeOrAbsolute));

                DrawerRightForCreateMyPlaylist.IsOpen = true;

                clickBlockPanelForDrawer.Visibility = Visibility.Visible;

                if (x_DrawerRightForCreateMyPlaylist_ImageSelectListBox.ItemsSource == null) x_DrawerRightForCreateMyPlaylist_ImageSelectListBox.ItemsSource = MyPlaylist_ImageList;

                x_DrawerRightForCreateMyPlaylist_RootGrid.Visibility = Visibility.Visible;
                x_DrawerRightForCreateMyPlaylist_LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 생성 Drawer 플레이리스트 이름 길이 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_DrawerRightForCreateMyPlaylist_PlaylistNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                x_DrawerRightForCreateMyPlaylist_PlaylistNameCount.Text = string.Format("{0}/40 글자", x_DrawerRightForCreateMyPlaylist_PlaylistNameTextbox.Text.Length);
                x_DrawerRightForCreateMyPlaylist_SaveButton.IsEnabled = x_DrawerRightForCreateMyPlaylist_PlaylistNameTextbox.Text.Length <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 생성 Drawer 리스트 박스 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_DrawerRightForCreateMyPlaylist_ImageSelectListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as UtaitePlayer.Classes.DataVO.OnlyImageDataVO != null))
                {
                    int selectedIndex = x_DrawerRightForCreateMyPlaylist_ImageSelectListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    UtaitePlayer.Classes.DataVO.OnlyImageDataVO onlyImageDataVO = MyPlaylist_ImageList[selectedIndex];
                    x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.image = onlyImageDataVO.image;
                    x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.imageID = onlyImageDataVO.imageID;

                    x_DrawerRightForCreateMyPlaylist_PlaylistImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,{0}", x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.image), UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 생성 Drawer 데이터 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void x_DrawerRightForCreateMyPlaylist_SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (x_DrawerRightForCreateMyPlaylist_PlaylistNameTextbox.Text.Length <= 0) return;

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                string name = x_DrawerRightForCreateMyPlaylist_PlaylistNameTextbox.Text;
                bool isSuccess = false;

                await Task.Run(() =>
                {
                    try
                    {
                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                        
                        JObject jObject = JObject.Parse(utaitePlayerClient.createMyPlaylist(registryManager.getAuthToken().ToString(), name, x_DrawerRightForCreateMyPlaylist_OnlyImageDataVO.imageID));

                        if (!(jObject.ContainsKey("result") && jObject.ContainsKey("result") && jObject["result"].ToString().Equals("success")))
                        {
                            ExceptionManager.getInstance().showMessageBox("플레이리스트 생성하는 도중 오류가 발생하였습니다. 다시 시도하여 주십시오.");
                        }
                        else
                        {
                            isSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                x_DrawerRightForCreateMyPlaylist_PlaylistNameTextbox.Text = "";
                x_DrawerRightForCreateMyPlaylist_ImageSelectListBox.SelectedItem = -1;

                if (isSuccess)
                    if (nowPage.Equals("플레이리스트"))
                        await Task.Run(() => ((Layout.Page.MyPlaylistPage)myPlaylistPage).Refresh());

                DrawerRightForCreateMyPlaylist.IsOpen = false;

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Drawer 닫기 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Drawer_Closed(object sender, RoutedEventArgs e)
        {
            try
            {
                clickBlockPanelForDrawer.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Drawer Block Panel 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickBlockPanelForDrawer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                clickBlockPanelForDrawer.Visibility = Visibility.Collapsed;

                DrawerRightForMusicInfo.IsOpen = false;
                DrawerBottomForAddMusicPlaylist.IsOpen = false;
                DrawerRightForCreateMyPlaylist.IsOpen = false;
                DrawerRightForEditMyPlaylist.IsOpen = false;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 정보 화면에서 플레이리스트에 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMyPlaylist_MenuitemRootPanel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PlayerService.getInstance().getMusicInfo() != null)
                    showAddMusicToMyPlaylistDrawer(PlayerService.getInstance().getMusicInfo().uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스에 노래 추가
        /// </summary>
        /// <param name="playlistUUID">플레이리스트 UUID</param>
        /// <param name="musicUUID">노래 UUID</param>
        private bool addMusicToMyPlaylist(string playlistUUID, string musicUUID)
        {
            try
            {
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.ContainsKey(playlistUUID))
                {
                    if (!RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.ContainsKey(musicUUID))
                    {
                        // 메시지 출력
                        Application.Current.Dispatcher.Invoke(() =>
                            RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                            new GrowlInfo()
                            {
                                Message = "해당 노래를 찾을 수 없습니다."
                            })
                        );

                        return false;
                    }

                    RHYANetwork.UtaitePlayer.DataManager.UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[playlistUUID];
                    if (userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count + 1 > 100)
                    {
                        // 메시지 출력
                        Application.Current.Dispatcher.Invoke(() =>
                            RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                            new GrowlInfo()
                            {
                                Message = "해당 플레이리스트에는 노래를 더 이상 추가할 수 없습니다."
                            })
                        );

                        return false;
                    }

                    JArray jArray = new JArray();
                    for (int i = 0; i < userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count; i++)
                        jArray.Add(userCustomPlaylistInfoVO.userPlaylistInfoVOs[i].uuid);

                    jArray.Add(musicUUID);

                    string data = jArray.ToString();

                    RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                    bool isSuccess = false;

                    JObject jObject = JObject.Parse(utaitePlayerClient.saveMyPlaylist(registryManager.getAuthToken().ToString(), playlistUUID, data));
                    isSuccess = jObject.ContainsKey("result") && jObject["result"].ToString().Equals("success") ? true : false;

                    return isSuccess;
                }
                else
                {
                    // 메시지 출력
                    Application.Current.Dispatcher.Invoke(() => 
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                        new GrowlInfo()
                        {
                            Message = "해당 플레이리스트를 찾을 수 없습니다."
                        })
                    );

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스에 노래 추가 [배열]
        /// </summary>
        /// <param name="playlistUUID">플레이리스트 UUID</param>
        /// <param name="musicUUIDs">노래 UUID 리스트</param>
        private bool addMusicToMyPlaylistForArray(string playlistUUID, List<string> musicUUIDs)
        {
            try
            {
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.ContainsKey(playlistUUID))
                {
                    for (int index = 0; index < musicUUIDs.Count; index++)
                    {
                        if (!RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.ContainsKey(musicUUIDs[index]))
                        {
                            // 메시지 출력
                            Application.Current.Dispatcher.Invoke(() =>
                                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                                new GrowlInfo()
                                {
                                    Message = "해당 노래를 찾을 수 없습니다."
                                })
                            );

                            return false;
                        }
                    }

                    RHYANetwork.UtaitePlayer.DataManager.UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[playlistUUID];
                    if (userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count + musicUUIDs.Count > 100)
                    {
                        // 메시지 출력
                        Application.Current.Dispatcher.Invoke(() =>
                            RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                            new GrowlInfo()
                            {
                                Message = "해당 플레이리스트에는 노래를 더 이상 추가할 수 없습니다."
                            })
                        );

                        return false;
                    }

                    JArray jArray = new JArray();
                    for (int i = 0; i < userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count; i++)
                        jArray.Add(userCustomPlaylistInfoVO.userPlaylistInfoVOs[i].uuid);

                    for (int index = 0; index < musicUUIDs.Count; index++)
                        jArray.Add(musicUUIDs[index]);

                    string data = jArray.ToString();

                    RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                    bool isSuccess = false;

                    JObject jObject = JObject.Parse(utaitePlayerClient.saveMyPlaylist(registryManager.getAuthToken().ToString(), playlistUUID, data));
                    isSuccess = jObject.ContainsKey("result") && jObject["result"].ToString().Equals("success") ? true : false;

                    return isSuccess;
                }
                else
                {
                    // 메시지 출력
                    Application.Current.Dispatcher.Invoke(() =>
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                        new GrowlInfo()
                        {
                            Message = "해당 플레이리스트를 찾을 수 없습니다."
                        })
                    );

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트에 노래 추가 Drawer 보여주기
        /// </summary>
        /// <param name="data">데이터 (노래 UUID)</param>
        private async void showAddMusicToMyPlaylistDrawer(object data)
        {
            try
            {
                x_DrawerBottomForAddMusicToMyPlaylist_MusicUUID = data.ToString();

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                if (selectPlaylistForAddMusicToPlaylistListView.ItemsSource == null)
                    selectPlaylistForAddMusicToPlaylistListView.ItemsSource = x_DrawerBottomForAddMusicToMyPlaylist_Playlist;

                x_DrawerBottomForAddMusicToMyPlaylist_Playlist.Clear();

                await Task.Run(() =>
                {
                    try
                    {
                        List<string> playlistKeysList = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.Keys.ToList();
                        for (int i = 0; i < playlistKeysList.Count; i++)
                            x_DrawerBottomForAddMusicToMyPlaylist_Playlist.Add(new SelectMyPlaylistDataVO(playlistKeysList[i]));
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                selectPlaylistForAddMusicToPlaylistListView.Items.Refresh();

                x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDType = 0;

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);

                clickBlockPanelForDrawer.Visibility = Visibility.Visible;

                DrawerBottomForAddMusicPlaylist.IsOpen = true;
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
        /// 노래 추가 Drawer 플레이리스트 선택 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void selectPlaylistForAddMusicToPlaylistListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((FrameworkElement)e.OriginalSource).DataContext as Classes.DataVO.SelectMyPlaylistDataVO != null)
                {
                    if (x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDType == 0)
                    {
                        DrawerBottomForAddMusicPlaylist.IsOpen = false;

                        int index = selectPlaylistForAddMusicToPlaylistListView.SelectedIndex;
                        if (!(index == -1))
                        {
                            // 전역 Dialog 설정
                            RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                            bool isSuccess = false;
                            await Task.Run(() =>
                            {
                                try
                                {
                                    isSuccess = addMusicToMyPlaylist(x_DrawerBottomForAddMusicToMyPlaylist_Playlist[index].uuid, x_DrawerBottomForAddMusicToMyPlaylist_MusicUUID);
                                }
                                catch (Exception)
                                {
                                    isSuccess = false;
                                }
                                
                            });

                            if (isSuccess)
                            {
                                await Task.Run(() => ((Layout.Page.MyPlaylistPage)myPlaylistPage).Refresh());

                                // 메시지 출력
                                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                                    new GrowlInfo()
                                    {
                                        Message = string.Format("'{0}' 플레이리스트에 1곡이 추가되었습니다.", x_DrawerBottomForAddMusicToMyPlaylist_Playlist[index].name)
                                    });
                            }
                            else
                            {
                                // 메시지 출력
                                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                                    new GrowlInfo()
                                    {
                                        Message = "플레이리스트에 노래를 추가하는 도중 오류가 발생하였습니다."
                                    });
                            }
                        }
                    }
                    else if (x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDType == 1)
                    {
                        DrawerBottomForAddMusicPlaylist.IsOpen = false;

                        int index = selectPlaylistForAddMusicToPlaylistListView.SelectedIndex;
                        if (!(index == -1))
                        {
                            // 전역 Dialog 설정
                            RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                            bool isSuccess = false;
                            await Task.Run(() =>
                            {
                                try
                                {
                                    isSuccess = addMusicToMyPlaylistForArray(x_DrawerBottomForAddMusicToMyPlaylist_Playlist[index].uuid, x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDs);
                                }
                                catch (Exception)
                                {
                                    isSuccess = false;
                                }

                            });

                            if (isSuccess)
                            {
                                await Task.Run(() => ((Layout.Page.MyPlaylistPage)myPlaylistPage).Refresh());

                                // 메시지 출력
                                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                                    new GrowlInfo()
                                    {
                                        Message = string.Format("'{0}' 플레이리스트에 {1}곡이 추가되었습니다.", x_DrawerBottomForAddMusicToMyPlaylist_Playlist[index].name, x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDs.Count)
                                    });
                            }
                            else
                            {
                                // 메시지 출력
                                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                                    new GrowlInfo()
                                    {
                                        Message = "플레이리스트에 노래를 추가하는 도중 오류가 발생하였습니다."
                                    });
                            }
                        }
                    }
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
        /// 현재 플레이리스트에서 노래 커스텀 플레이리스트에 담기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void currentPlaylist_AddCustomMyPlaylist(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count > 0)
                {
                    // 전역 Dialog 설정
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                    x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDs.Clear();

                    await Task.Run(() => 
                    {
                        try
                        {
                            for (int i = 0; i < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; i++)
                            {
                                RHYANetwork.UtaitePlayer.DataManager.UserPlaylistInfoVO userPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userPlaylistInfoVOs[i];
                                if (userPlaylistInfoVO.isChecked)
                                    x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDs.Add(userPlaylistInfoVO.uuid);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    x_DrawerBottomForAddMusicToMyPlaylist_MusicUUIDType = 1;

                    // 전역 Dialog 설정
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                    if (selectPlaylistForAddMusicToPlaylistListView.ItemsSource == null)
                        selectPlaylistForAddMusicToPlaylistListView.ItemsSource = x_DrawerBottomForAddMusicToMyPlaylist_Playlist;

                    x_DrawerBottomForAddMusicToMyPlaylist_Playlist.Clear();

                    await Task.Run(() =>
                    {
                        try
                        {
                            List<string> playlistKeysList = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.Keys.ToList();
                            for (int i = 0; i < playlistKeysList.Count; i++)
                                x_DrawerBottomForAddMusicToMyPlaylist_Playlist.Add(new SelectMyPlaylistDataVO(playlistKeysList[i]));
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    selectPlaylistForAddMusicToPlaylistListView.Items.Refresh();

                    // 전역 Dialog 설정
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);

                    clickBlockPanelForDrawer.Visibility = Visibility.Visible;

                    DrawerBottomForAddMusicPlaylist.IsOpen = true;
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
    }
}

