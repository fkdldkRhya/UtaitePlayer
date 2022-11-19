using HandyControl.Data;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtaitePlayer.Classes.DataVO;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout.Pages
{
    /// <summary>
    /// MyPlaylistPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MyPlaylistPage : System.Windows.Controls.Page
    {
        // 데이터 로딩 감지
        public bool isLoaded = false;
        // 플레이리스트 데이터
        private List<MyPlaylistDataVO> myPlaylistDataVOs = new List<MyPlaylistDataVO>();
        // 현재 보여주고 있는 플레이리스트 데이터
        private UtaitePlayer.Classes.DataVO.MyPlaylistDataVO x_PlaylistInfoLayout_SelectedPlaylist = null;
        private List<MyPlaylistItemDataVO> x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs = new List<MyPlaylistItemDataVO>();





        /// <summary>
        /// 생성자
        /// </summary>
        public MyPlaylistPage()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 페이지 로딩 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            reload();
        }



        /// <summary>
        /// 데이터 로딩
        /// </summary>
        public async void reload()
        {
            try
            {
                if (isLoaded) return;

                // 창 닫기
                x_PlaylistInfoLayout_HideLayout();
                // 새로고침
                await Task.Run(() => Refresh());

                isLoaded = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 편집 메뉴 아이템 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditMyPlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = myPlaylistListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MyPlaylistDataVO myPlaylistDataVO = myPlaylistDataVOs[selectedIndex];
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_EDIT_PLAYLIST_DRAWER, myPlaylistDataVO);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 데이터 다시 로딩
        /// </summary>
        public void Refresh()
        {
            try
            {
                // UI 설정
                Application.Current.Dispatcher.Invoke(() => 
                {
                    myPlaylistRootGrid.Visibility = Visibility.Collapsed;
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");
                    x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox.IsChecked = false;
                });

                // 데이터 다시 불러오기
                try
                {
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                    RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();

                    string getUserMoreInfoJsonValue = utaitePlayerClient.getUserMoreData((string)registryManager.getAuthToken());
                    // 서버 응답 확인
                    JObject jObjectForGetUserMoreInfoJsonValue = JObject.Parse(getUserMoreInfoJsonValue);
                    if (jObjectForGetUserMoreInfoJsonValue.ContainsKey("result"))
                    {
                        if (((string)jObjectForGetUserMoreInfoJsonValue["result"]).Equals("fail"))
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox("사용자 커스텀 플레이리스트, 구독 정보 로딩 중 오류 발생, JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");

                            return;
                        }
                    }
                    // 데이터 설정
                    new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().userSubscribeAndPlaylistResourcesParser(getUserMoreInfoJsonValue);
                }
                catch (Exception ex)
                {
                    ExceptionManager.getInstance().showMessageBox(ex);
                }

                // 플레이리스트 초기화
                Application.Current.Dispatcher.Invoke(() => 
                {
                    myPlaylistDataVOs.Clear();
                    myPlaylistListBox.ItemsSource = myPlaylistDataVOs;
                });

                x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Clear();

                bool checker = false;

                try
                {
                    List<string> playlistKeysList = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.Keys.ToList();
                    for (int i = 0; i < playlistKeysList.Count; i++)
                    {
                        MyPlaylistDataVO myPlaylistDataVO = new MyPlaylistDataVO(playlistKeysList[i]);
                        myPlaylistDataVOs.Add(myPlaylistDataVO);

                        if (x_PlaylistInfoLayout_SelectedPlaylist != null && x_PlaylistInfoLayout_SelectedPlaylist.uuid.Equals(playlistKeysList[i]) && !checker)
                        {
                            for (int index = 0; index < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[myPlaylistDataVO.uuid].userPlaylistInfoVOs.Count; index++)
                                x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Add(new MyPlaylistItemDataVO(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[myPlaylistDataVO.uuid].userPlaylistInfoVOs[index].uuid));
                            
                            x_PlaylistInfoLayout_SelectedPlaylist = null;
                            x_PlaylistInfoLayout_SelectedPlaylist = myPlaylistDataVO;

                            checker = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionManager.getInstance().showMessageBox(ex);
                }

                Application.Current.Dispatcher.Invoke(() => 
                {
                    if (myPlaylistDataVOs.Count <= 0)
                    {
                        noResult.Visibility = Visibility.Visible;
                        myPlaylistListBox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        noResult.Visibility = Visibility.Collapsed;
                        myPlaylistListBox.Visibility = Visibility.Visible;
                    }

                    myPlaylistListBox.Items.Refresh();

                    // 전역 Dialog 설정
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);

                    // UI 설정
                    myPlaylistRootGrid.Visibility = Visibility.Visible;
                });

                if (checker && x_PlaylistInfoLayout_SelectedPlaylist != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count <= 0)
                        {
                            noResultPanelForPlaylist.Visibility = Visibility.Visible;
                            x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            noResultPanelForPlaylist.Visibility = Visibility.Collapsed;
                            x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Visible;
                        }

                        x_PlaylistInfoLayout_SettingDefaultInfo(x_PlaylistInfoLayout_SelectedPlaylist);
                        x_PlaylistInfoLayout_MusicCount.Text = string.Format("{0}곡", x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count);
                        x_PlaylistInfoLayout_MyPlaylistDataGrid.Items.Refresh();
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 제거 메뉴 아이템 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMyPlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = myPlaylistListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MyPlaylistDataVO myPlaylistDataVO = myPlaylistDataVOs[selectedIndex];

                YesOrNoDialogInfo yesOrNoDialogInfo = new YesOrNoDialogInfo();
                yesOrNoDialogInfo.title = "플레이리스트 제거";
                yesOrNoDialogInfo.message = string.Format("정말로 '{0}' 플레이리스트를 제거하시겠습니까?", myPlaylistDataVO.name);
                yesOrNoDialogInfo.button1Title = "취소";
                yesOrNoDialogInfo.button2Title = "제거";
                yesOrNoDialogInfo.button1Event = new Action(() =>
                {
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_DIALOG_YES_OR_NO, null);
                });
                yesOrNoDialogInfo.button2Event = new Action(async () =>
                {
                    try
                    {
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_DIALOG_YES_OR_NO, null);
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                        await Task.Run(() => 
                        {
                            try
                            {
                                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                                string result = utaitePlayerClient.removeMyPlaylist(registryManager.getAuthToken().ToString(), myPlaylistDataVO.uuid);
                                JObject object1 = JObject.Parse(result);

                                if (!(object1.ContainsKey("result") && object1["result"].ToString().Equals("success")))
                                {
                                    ExceptionManager.getInstance().showMessageBox("플레이리스트 제거 도중 오류가 발생하였습니다. 다시 시도하여 주십시오.");
                                }
                            }
                            catch (Exception ex)
                            {
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });

                        await Task.Run(() => Refresh());
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                    finally
                    {
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
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
        /// 플레이리스트 전체 재생 메뉴 아이템 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AllPlayMyPlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = myPlaylistListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MyPlaylistDataVO myPlaylistDataVO = myPlaylistDataVOs[selectedIndex];

                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.ContainsKey(myPlaylistDataVO.uuid))
                {
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                    RHYANetwork.UtaitePlayer.DataManager.UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[myPlaylistDataVO.uuid];

                    List<string> uuidList = new List<string>();

                    await Task.Run(() =>
                    {
                        for (int index = 0; index < userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count; index++)
                            uuidList.Add(userCustomPlaylistInfoVO.userPlaylistInfoVOs[index].uuid);
                    });

                    // 전역 함수 호출
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY, uuidList);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 생성 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateMyPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_CREATE_PLAYLIST_DRAWER, null);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 더블 클릭 감지 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void myPlaylistListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as MyPlaylistDataVO != null))
                {
                    int selectedIndex = myPlaylistListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox.IsChecked = false;

                    MyPlaylistDataVO myPlaylistDataVO = myPlaylistDataVOs[selectedIndex];
                    if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.ContainsKey(myPlaylistDataVO.uuid))
                    {
                        x_PlaylistInfoLayout_SelectedPlaylist = myPlaylistDataVO;
                        x_PlaylistInfoLayout_SettingDefaultInfo(x_PlaylistInfoLayout_SelectedPlaylist);

                        loadingProgressbar.Visibility = Visibility.Visible;
                        x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Collapsed;

                        await Task.Run(() =>
                        {
                            try
                            {
                                RHYANetwork.UtaitePlayer.DataManager.UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[myPlaylistDataVO.uuid];
                                for (int i = 0; i < userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count; i++)
                                    x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Add(new MyPlaylistItemDataVO(userCustomPlaylistInfoVO.userPlaylistInfoVOs[i].uuid));
                            }
                            catch (Exception ex)
                            {
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });

                        x_PlaylistInfoLayout_MyPlaylistDataGrid.Items.Refresh();

                        if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count <= 0)
                        {
                            noResultPanelForPlaylist.Visibility = Visibility.Visible;
                            x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            noResultPanelForPlaylist.Visibility = Visibility.Collapsed;
                            x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Visible;
                        }

                        loadingProgressbar.Visibility = Visibility.Collapsed;
                        x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 닫기 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPlaylistMoreInfoPanelCloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                x_PlaylistInfoLayout_HideLayout();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 닫기
        /// </summary>
        private void x_PlaylistInfoLayout_HideLayout()
        {
            try
            {
                x_PlaylistInfoLayout_RootPanel.Visibility = Visibility.Collapsed;

                // 변수 초기화
                if (x_PlaylistInfoLayout_MyPlaylistDataGrid.ItemsSource == null)
                    x_PlaylistInfoLayout_MyPlaylistDataGrid.ItemsSource = x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs;

                x_PlaylistInfoLayout_SelectedPlaylist = null;

                x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 설정 - 기본 정보
        /// </summary>
        /// <param name="myPlaylistDataVO">플레이리스트 정보</param>
        private void x_PlaylistInfoLayout_SettingDefaultInfo(MyPlaylistDataVO myPlaylistDataVO)
        {
            try
            {
                x_PlaylistInfoLayout_RootPanel.Visibility = Visibility.Visible;
                x_PlaylistInfoLayout_MusicCount.Text = myPlaylistDataVO.count;
                x_PlaylistInfoLayout_PlaylistTitle.Text = myPlaylistDataVO.name;
                x_PlaylistInfoLayout_AccountName.Text = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.name;
                x_PlaylistInfoLayout_AccountID.Text = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id;
                x_PlaylistInfoLayout_PlaylistImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,{0}", myPlaylistDataVO.image), UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 플레이리스트 편집 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_PlaylistInfoLayout_EditMyPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (x_PlaylistInfoLayout_SelectedPlaylist == null) return;
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_EDIT_PLAYLIST_DRAWER, x_PlaylistInfoLayout_SelectedPlaylist);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 플레이리스트에 담기 아이템 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_PlaylistInfoLayout_MyPlaylistDataGrid_AddMusicMyPlayListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = x_PlaylistInfoLayout_MyPlaylistDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MyPlaylistItemDataVO myPlaylistItemDataVO = x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[selectedIndex];

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, myPlaylistItemDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 노래 정보 보기 아이템 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_PlaylistInfoLayout_MyPlaylistDataGrid_ShowMusicInfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = x_PlaylistInfoLayout_MyPlaylistDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MyPlaylistItemDataVO myPlaylistItemDataVO = x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[selectedIndex];
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, myPlaylistItemDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 플레이리스트서 삭제 아이템 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void x_PlaylistInfoLayout_MyPlaylistDataGrid_DeleteMusicMyPlayListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check selected item
                bool isSelectedItem = false;
                await Task.Run(() => 
                {
                    try
                    {
                        for (int i = 0; i < x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count; i++)
                            if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[i].isCheck) { isSelectedItem = true; break; }

                        if (isSelectedItem)
                        {
                            for (int index = x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count - 1; index >= 0; index--)
                                if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[index].isCheck)
                                    x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.RemoveAt(index);

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                if (isSelectedItem)
                {
                    x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox.IsChecked = false;
                    x_PlaylistInfoLayout_MusicCount.Text = string.Format("{0}곡", x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count);
                    x_PlaylistInfoLayout_MyPlaylistDataGrid.Items.Refresh();
                    if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count <= 0)
                    {
                        noResultPanelForPlaylist.Visibility = Visibility.Visible;
                        x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        noResultPanelForPlaylist.Visibility = Visibility.Collapsed;
                        x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Visible;
                    }
                    return;
                }


                int selectedIndex = x_PlaylistInfoLayout_MyPlaylistDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.RemoveAt(selectedIndex);
                x_PlaylistInfoLayout_MusicCount.Text = string.Format("{0}곡", x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count);
                x_PlaylistInfoLayout_MyPlaylistDataGrid.Items.Refresh();

                if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count <= 0)
                {
                    noResultPanelForPlaylist.Visibility = Visibility.Visible;
                    x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    noResultPanelForPlaylist.Visibility = Visibility.Collapsed;
                    x_PlaylistInfoLayout_MyPlaylistDataGrid.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 플레이리스트 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void x_PlaylistInfoLayout_SaveMyPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (x_PlaylistInfoLayout_SelectedPlaylist == null) return;

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                bool isSuccess = false;

                await Task.Run(() =>
                {
                    try
                    {
                        JArray jArray = new JArray();
                        for (int index = 0; index  < x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count; index++)
                            jArray.Add(x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[index].uuid);

                        string data = jArray.ToString();

                        JObject jObject = JObject.Parse(utaitePlayerClient.saveMyPlaylist(registryManager.getAuthToken().ToString(), x_PlaylistInfoLayout_SelectedPlaylist.uuid, data));
                        isSuccess = jObject.ContainsKey("result") && jObject["result"].ToString().Equals("success") ? true : false;
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // 새로고침
                await Task.Run(() => Refresh());

                if (isSuccess)
                    // 메시지 출력
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                        new GrowlInfo()
                        {
                            Message = "플레이리스트의 변경 사항을 저장하였습니다."
                        });
                else
                    // 메시지 출력
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                        new GrowlInfo()
                        {
                            Message = "플레이리스트의 변경 사항을 저장하는 도중 오류가 발생하였습니다. 다시 시도하여 주십시오."
                        });
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
        /// 플레이리스트 정보 화면 - 플레이리스트 전체 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void x_PlaylistInfoLayout_PlayMyPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UtaitePlayer.Classes.DataVO.MyPlaylistDataVO myPlaylistDataVO = x_PlaylistInfoLayout_SelectedPlaylist;

                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.ContainsKey(myPlaylistDataVO.uuid))
                {
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                    RHYANetwork.UtaitePlayer.DataManager.UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[myPlaylistDataVO.uuid];

                    List<string> uuidList = new List<string>();

                    await Task.Run(() =>
                    {
                        for (int index = 0; index < userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count; index++)
                            uuidList.Add(userCustomPlaylistInfoVO.userPlaylistInfoVOs[index].uuid);
                    });

                    // 전역 함수 호출
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY, uuidList);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void x_PlaylistInfoLayout_PlayButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Classes.DataVO.MyPlaylistItemDataVO dataRowView = (Classes.DataVO.MyPlaylistItemDataVO)((System.Windows.Shapes.Path)e.Source).DataContext;

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, dataRowView.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 컬럼 Checkbox 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyPlaylistInfoPanel_ColumnCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Classes.DataVO.MyPlaylistItemDataVO dataRowView = (Classes.DataVO.MyPlaylistItemDataVO)((CheckBox)e.Source).DataContext;

                if (dataRowView != null)
                {
                    bool value = (bool)(((CheckBox)sender).IsChecked);
                    bool allChecked = true;

                    dataRowView.isCheck = value;

                    await Task.Run(() =>
                    {
                        try
                        {
                            for (int i = 0; i < x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count; i++)
                            {
                                MyPlaylistItemDataVO myPlaylistItemDataVO = x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[i];


                                if (!myPlaylistItemDataVO.isCheck) { allChecked = false; break; };
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });

                    x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox.IsChecked = allChecked;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 정보 화면 - 컬럼 Header Checkbox 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count <= 0)
                {
                    ((CheckBox)sender).IsChecked = false;
                    return;
                }

                bool value = (bool)((CheckBox)sender).IsChecked;

                await Task.Run(() =>
                {
                    try
                    {
                        for (int i = 0; i < x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs.Count; i++)
                            x_PlaylistInfoLayout_SelectedPlaylist_ItemDataVOs[i].isCheck = value;
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                x_PlaylistInfoLayout_MyPlaylistDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
