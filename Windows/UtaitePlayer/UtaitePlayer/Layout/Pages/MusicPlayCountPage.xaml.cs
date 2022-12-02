using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout.Pages
{
    /// <summary>
    /// MusicPlayCountPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MusicPlayCountPage : System.Windows.Controls.Page
    {
        // 데이터 로딩 감지
        public bool isLoaded = false;
        // 구독 데이터
        private List<UtaitePlayer.Classes.DataVO.MusicPlayCountDataVO> musicPlayCountDataVOs = new List<Classes.DataVO.MusicPlayCountDataVO>();




        /// <summary>
        /// 생성자
        /// </summary>
        public MusicPlayCountPage()
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

                isLoaded = true;

                // 데이터 초기화
                musicPlayCountDataVOs.Clear();

                // UI 기본 설정
                musicCountDataGrid.Visibility = Visibility.Collapsed;
                noResult.Visibility = Visibility.Collapsed;
                LoadingProgressBar.Visibility = Visibility.Visible;

                // 데이터 설정
                if (musicCountDataGrid.ItemsSource == null)
                    musicCountDataGrid.ItemsSource = musicPlayCountDataVOs;

                // 분위기별 우타이테 선호도
                double musicTag1 = 0;
                double musicTag2 = 0;
                double musicTag3 = 0;
                double musicTag4 = 0;
                double musicTag5 = 0;

                // 데이터 불러오기
                await Task.Run(() =>
                {
                    try
                    {
                        RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                        string clientResult = utaitePlayerClient.getMusicPlayCount(registryManager.getAuthToken().ToString());

                        JObject jObject = JObject.Parse(clientResult);
                        if (jObject.ContainsKey("result") && jObject.ContainsKey("message") && jObject["result"].ToString().Equals("success"))
                        {
                            JArray jArray = JArray.Parse(HttpUtility.UrlDecode(jObject["message"].ToString()));
                            for (int i = 0; i < jArray.Count; i++)
                            {
                                JObject musicCountJObject = JObject.Parse(jArray[i].ToString());
                                if (musicCountJObject.ContainsKey("uuid") && musicCountJObject.ContainsKey("count"))
                                {
                                    Classes.DataVO.MusicPlayCountDataVO musicPlayCountDataVO = new Classes.DataVO.MusicPlayCountDataVO(musicCountJObject["uuid"].ToString(), ((int)musicCountJObject["count"]) / 2);

                                    if (musicPlayCountDataVO.musicTag.Contains("#신남"))
                                        musicTag1 = musicTag1 + 1;
                                    if (musicPlayCountDataVO.musicTag.Contains("#슬픔"))
                                        musicTag2 = musicTag2 + 1;
                                    if (musicPlayCountDataVO.musicTag.Contains("#즐거움"))
                                        musicTag3 = musicTag3 + 1;
                                    if (musicPlayCountDataVO.musicTag.Contains("#잔잔"))
                                        musicTag4 = musicTag4 + 1;
                                    if (musicPlayCountDataVO.musicTag.Contains("#사랑"))
                                        musicTag5 = musicTag5 + 1;

                                    musicPlayCountDataVOs.Add(musicPlayCountDataVO);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // 분위기별 우타이테 선호도
                SeriesCollection seriesCollectionFoMusicTag = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "#신남",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(musicTag1) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "#슬픔",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(musicTag2) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "#즐거움",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(musicTag3) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "#잔잔",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(musicTag4) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "#사랑",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(musicTag5) },
                        DataLabels = true
                    }
                };
                pieChartForMusicTag.Series = seriesCollectionFoMusicTag;

                if (musicPlayCountDataVOs.Count <= 0)
                {
                    noResult.Visibility = Visibility.Visible;
                    musicCountDataGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    noResult.Visibility = Visibility.Collapsed;
                    musicCountDataGrid.Visibility = Visibility.Visible;
                }

                LoadingProgressBar.Visibility = Visibility.Collapsed;

                // 데이터 새로고침
                musicCountDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButtonForMusicCountDataGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Classes.DataVO.MusicPlayCountDataVO dataRowView = (Classes.DataVO.MusicPlayCountDataVO)((System.Windows.Shapes.Path)e.Source).DataContext;

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, dataRowView.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 정보 보기 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMusicInfoMenuItemForMusicCountDataGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = musicCountDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MusicPlayCountDataVO topUtaiteDataVO = musicPlayCountDataVOs[selectedIndex];
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, topUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 사용자 플레이리스트에 담기 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicCustomMyPlayListMenuItemForMusicCountDataGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = musicCountDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MusicPlayCountDataVO topUtaiteDataVO = musicPlayCountDataVOs[selectedIndex];

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER, topUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }




        /// <summary>
        /// 현재 플레이리스트에 담기 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicMyPlayListMenuItemForMusicCountDataGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = musicCountDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.MusicPlayCountDataVO topUtaiteDataVO = musicPlayCountDataVOs[selectedIndex];
                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, topUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
