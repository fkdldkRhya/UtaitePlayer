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
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout.Page
{
    /// <summary>
    /// SubscribeManagePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SubscribeManagePage : System.Windows.Controls.Page
    {
        // 구독 데이터
        private List<UtaitePlayer.Classes.DataVO.SubscribeArtistDataVO> subscribeArtistDataVOs = new List<Classes.DataVO.SubscribeArtistDataVO>();




        /// <summary>
        /// 생성자
        /// </summary>
        public SubscribeManagePage()
        {
            InitializeComponent();
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
                // 데이터 초기화
                subscribeArtistDataVOs.Clear();

                // UI 기본 설정
                subscribeDataGrid.Visibility = Visibility.Collapsed;
                noResult.Visibility = Visibility.Collapsed;

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Initializing...");

                // 데이터 다시 불러오기
                await Task.Run(() => 
                {
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
                });

                await Task.Run(() => 
                {
                    try
                    {
                        for (int i = 0; i < RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userSubscribeInfoVOs.Count; i ++)
                            subscribeArtistDataVOs.Add(new Classes.DataVO.SubscribeArtistDataVO(RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userSubscribeInfoVOs[i].uuid));
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });


                subscribeDataGrid.ItemsSource = subscribeArtistDataVOs;
                subscribeDataGrid.Items.Refresh();

                if (subscribeArtistDataVOs.Count <= 0)
                {
                    subscribeDataGrid.Visibility = Visibility.Collapsed;
                    noResult.Visibility = Visibility.Visible;
                }
                else
                {
                    subscribeDataGrid.Visibility = Visibility.Visible;
                    noResult.Visibility = Visibility.Collapsed;
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
        /// 아티스트 구독 취소 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ArtistSubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = subscribeDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                UtaitePlayer.Classes.DataVO.SubscribeArtistDataVO subscribeArtistDataVO = subscribeArtistDataVOs[selectedIndex];

                subscribeArtistDataVO.artistLoadingVisibility = Visibility.Visible;
                subscribeArtistDataVO.artistSubscribeButtonVisibility = Visibility.Collapsed;
                subscribeDataGrid.Items.Refresh();

                // 비동기 작업
                await Task.Run(() =>
                {
                    try
                    {
                        string serverResponse = utaitePlayerClient.userSubscribeTask(registryManager.getAuthToken().ToString(), RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient.UserSubscribeTaskInfo.UNSUBSCRIBE, subscribeArtistDataVO.uuid);

                        JObject jObject = JObject.Parse(serverResponse);
                        if (jObject.ContainsKey("result"))
                        {
                            if (!(jObject["result"].Equals("success")))
                            {
                                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userSubscribeInfoVOs.RemoveAt(selectedIndex);
                                subscribeArtistDataVOs.RemoveAt(selectedIndex);
                            }
                            else
                            {
                                ExceptionManager.getInstance().showMessageBox("사용자 구독 데이터 변경 작업을 처리하는 도중 알 수 없는 오류가 발생하였습니다.");
                            }
                        }
                        else
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox("JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });


                if (subscribeArtistDataVOs.Count <= 0)
                {
                    subscribeDataGrid.Visibility = Visibility.Collapsed;
                    noResult.Visibility = Visibility.Visible;
                }
                else
                {
                    subscribeDataGrid.Visibility = Visibility.Visible;
                    noResult.Visibility = Visibility.Collapsed;
                }

                subscribeArtistDataVO.artistLoadingVisibility = Visibility.Collapsed;
                subscribeArtistDataVO.artistSubscribeButtonVisibility = Visibility.Visible;
                subscribeDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 구독 아티스트 검색 바로가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchSubscribeArtistGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try 
            {
                Classes.DataVO.SubscribeArtistDataVO dataRowView = (Classes.DataVO.SubscribeArtistDataVO)((Grid)e.Source).DataContext;

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SEARCH_FOR_TEXT, dataRowView.artistName);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
