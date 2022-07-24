using HandyControl.Data;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
    /// UtaitePlayerHomePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UtaitePlayerHomePage : System.Windows.Controls.Page
    {
        // 우타이테 제외 리스트
        private List<string> blockUtaiteUUIDs = new List<string>();

        // 최신 우타이테 데이터
        private List<UtaitePlayer.Classes.DataVO.NewUtaiteDataVO> newUtaiteDataVOs = new List<Classes.DataVO.NewUtaiteDataVO>();
        // 노래가 가장 많은 아티스트
        private List<UtaitePlayer.Classes.DataVO.ManyMusicDataVO> manyMusicDataVOs = new List<Classes.DataVO.ManyMusicDataVO>();
        // 인기 우타이테 데이터
        private List<UtaitePlayer.Classes.DataVO.TopUtaiteDataVO> topUtaiteDataVOs = new List<Classes.DataVO.TopUtaiteDataVO>();
        // 분위기별 우타이테 데이터
        private List<UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO> typeUtaiteDataVOs_1 = new List<Classes.DataVO.TypeUtaiteDataVO>();
        private List<UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO> typeUtaiteDataVOs_2 = new List<Classes.DataVO.TypeUtaiteDataVO>();
        private List<UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO> typeUtaiteDataVOs_3 = new List<Classes.DataVO.TypeUtaiteDataVO>();
        private List<UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO> typeUtaiteDataVOs_4 = new List<Classes.DataVO.TypeUtaiteDataVO>();
        private List<UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO> typeUtaiteDataVOs_5 = new List<Classes.DataVO.TypeUtaiteDataVO>();
        // 니코니코동 순위 데이터
        private List<UtaitePlayer.Classes.DataVO.NicoNicoDougaRankDataVO> nicoNicoDougaRankDataVOs = new List<Classes.DataVO.NicoNicoDougaRankDataVO>();




        /// <summary>
        /// 생성자
        /// </summary>
        public UtaitePlayerHomePage()
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
                // 모듈 선언
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                // Loading UI 설정
                loadingPanel.Visibility = Visibility.Visible;

                // 0.5초 대기
                await Task.Run(() => System.Threading.Thread.Sleep(500));

                // UI 데이터 초기화
                if (newUtaiteListBox.ItemsSource == null)
                    newUtaiteListBox.Items.Clear();
                if (manyMusicListDataGrid.ItemsSource == null)
                    manyMusicListDataGrid.Items.Clear();
                if (topUtaiteListDataGrid.ItemsSource == null)
                    topUtaiteListDataGrid.Items.Clear();
                if (nicoNicoDougaRankListDataGrid.ItemsSource == null)
                    nicoNicoDougaRankListDataGrid.Items.Clear();

                // 아이템 소스 할당
                manyMusicListDataGrid.ItemsSource = manyMusicDataVOs;
                newUtaiteListBox.ItemsSource = newUtaiteDataVOs;
                topUtaiteListDataGrid.ItemsSource = topUtaiteDataVOs;
                nicoNicoDougaRankListDataGrid.ItemsSource = nicoNicoDougaRankDataVOs;

                // 최신 우타이테 데이터 초기화
                newUtaiteDataVOs.Clear();
                // 최신 우타이테 데이터 불러오기
                await Task.Run(() => 
                { 
                    try
                    {
                        string serverResponse = utaitePlayerClient.getNewUtaiteList(registryManager.getAuthToken().ToString());
                        JObject jObject = JObject.Parse(serverResponse);
                        IList<string> keys = jObject.Properties().Select(p => p.Name).ToList();
                        foreach (string key in keys)
                        {
                            // 키 확인
                            if (Regex.IsMatch(key, @"^[0-9]+$"))
                            {
                                JObject musicInfo = JObject.Parse(jObject[key].ToString());
                                if (musicInfo.ContainsKey("result") && ((string) musicInfo["result"]).Equals("success") && musicInfo.ContainsKey("uuid"))
                                {
                                    string uuid = musicInfo["uuid"].ToString();

                                    blockUtaiteUUIDs.Add(uuid);
                                    newUtaiteDataVOs.Add(new UtaitePlayer.Classes.DataVO.NewUtaiteDataVO(uuid));
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
                // 노래가 가장 많은 아티스트 데이터 초기화
                manyMusicDataVOs.Clear();
                // 노래가 가장 많은 아티스트 데이터 불러오기
                await Task.Run(() =>
                {
                    try
                    {
                        // 노래 개수 설정
                        List<UtaitePlayer.Classes.DataVO.ManyMusicDataVO> manyMusicDataVOs = new List<Classes.DataVO.ManyMusicDataVO>();
                        List<string> singerKeysList = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.Keys.ToList();
                        for (int index = 0; index < singerKeysList.Count; index++)
                        {
                            int tCount = 0;
                            List<string> keysList = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.Keys.ToList();
                            for (int count = 0; count < keysList.Count; count++)
                                if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[keysList[count]].singerUUID.Equals(singerKeysList[index]))
                                    tCount = tCount + 1;

                            manyMusicDataVOs.Add(new Classes.DataVO.ManyMusicDataVO(singerKeysList[index], tCount));
                        }

                        manyMusicDataVOs.Sort((x, y) => x.musicCount.CompareTo(y.musicCount));
                        if (manyMusicDataVOs.Count > 5)
                            manyMusicDataVOs.RemoveRange(5, manyMusicDataVOs.Count - 5);

                        for (int index = 0; index < manyMusicDataVOs.Count; index ++)
                        {
                            int tempIndex = (manyMusicDataVOs.Count - 1) - index;
                            manyMusicDataVOs[tempIndex].musicCountRank = index + 1;
                            this.manyMusicDataVOs.Add(manyMusicDataVOs[tempIndex]);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex.ToString());
                    }
                });
                // 인기 우타이테 데이터 초기화
                topUtaiteDataVOs.Clear();
                // 인기 우타이테 데이터 불러오기
                await Task.Run(() =>
                {
                    try
                    {
                        string serverResponse = utaitePlayerClient.getTopUtaiteList(registryManager.getAuthToken().ToString());
                        JObject jObject = JObject.Parse(serverResponse);
                        IList<string> keys = jObject.Properties().Select(p => p.Name).ToList();
                        foreach (string key in keys)
                        {
                            // 키 확인
                            if (Regex.IsMatch(key, @"^[0-9]+$"))
                            {
                                JObject musicInfo = JObject.Parse(jObject[key].ToString());
                                if (musicInfo.ContainsKey("result") && ((string)musicInfo["result"]).Equals("success") && musicInfo.ContainsKey("uuid"))
                                {
                                    string uuid = musicInfo["uuid"].ToString();

                                    topUtaiteDataVOs.Add(new UtaitePlayer.Classes.DataVO.TopUtaiteDataVO(uuid, int.Parse(key)));
                                    blockUtaiteUUIDs.Add(uuid);
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
                // 니코니코 동화 순위 데이터 초기화
                nicoNicoDougaRankDataVOs.Clear();
                // 니코니코 동화 순위 데이터 불러오기
                await Task.Run(() =>
                {
                    try
                    {
                        string serverResponse = utaitePlayerClient.getNicoNicoDougaRankList();

                        JObject jObject = JObject.Parse(serverResponse);
                        if (jObject.ContainsKey("result") && jObject.ContainsKey("message") && jObject["result"].ToString().Equals("success"))
                        {
                            JArray jArray = JArray.Parse(HttpUtility.UrlDecode(jObject["message"].ToString(), Encoding.UTF8));
                            for (int index = 0; index < jArray.Count; index++)
                            {
                                JObject rankData = (JObject)jArray[index];

                                if (!(rankData.ContainsKey("rank") &&
                                    rankData.ContainsKey("title") &&
                                    rankData.ContainsKey("viewCounter") &&
                                    rankData.ContainsKey("likeCounter") &&
                                    rankData.ContainsKey("contentId")))
                                    continue;

                                nicoNicoDougaRankDataVOs.Add(new Classes.DataVO.NicoNicoDougaRankDataVO((int)rankData["rank"],
                                    (string)rankData["contentId"],
                                    (string)rankData["title"],
                                    (int)rankData["viewCounter"],
                                    (int)rankData["likeCounter"]));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                // 분위기별 우타이테
                if (typeUtaiteListBox.ItemsSource == null)
                {
                    // 분위기별 우타이테 데이터 초기화
                    typeUtaiteDataVOs_1.Clear();
                    typeUtaiteDataVOs_2.Clear();
                    typeUtaiteDataVOs_3.Clear();
                    typeUtaiteDataVOs_4.Clear();
                    typeUtaiteDataVOs_5.Clear();
                    // 분위기별 우타이테 데이터 불러오기
                    await Task.Run(() =>
                    {
                        try
                        {
                            List<string> keyList = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.Keys.ToList();

                            for (int i = keyList.Count - 1; i > 0; i--)
                            {
                                Random random = new Random(Guid.NewGuid().GetHashCode());
                                int rnd = random.Next(0, i);
                                string temp = keyList[i];
                                keyList[i] = keyList[rnd];
                                keyList[rnd] = temp;
                            }

                            foreach (string musicUUID in keyList)
                            {
                                // 노래 UUID 확인
                                if (!blockUtaiteUUIDs.Contains(musicUUID))
                                {
                                    RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[musicUUID];

                                    bool isAdd = false;

                                    if (musicInfoVO.type.Contains("#신남") && typeUtaiteDataVOs_1.Count < 30 && !musicInfoVO.type.Contains("#모음집"))
                                    {
                                        typeUtaiteDataVOs_1.Add(new Classes.DataVO.TypeUtaiteDataVO(musicUUID));
                                        isAdd = true;
                                    }

                                    if (musicInfoVO.type.Contains("#슬픔") && typeUtaiteDataVOs_2.Count < 30 && !musicInfoVO.type.Contains("#모음집"))
                                    {
                                        typeUtaiteDataVOs_2.Add(new Classes.DataVO.TypeUtaiteDataVO(musicUUID));
                                        isAdd = true;
                                    }

                                    if (musicInfoVO.type.Contains("#즐거움") && typeUtaiteDataVOs_3.Count < 30 && !musicInfoVO.type.Contains("#모음집"))
                                    {
                                        typeUtaiteDataVOs_3.Add(new Classes.DataVO.TypeUtaiteDataVO(musicUUID));
                                        isAdd = true;
                                    }

                                    if (musicInfoVO.type.Contains("#잔잔") && typeUtaiteDataVOs_4.Count < 30 && !musicInfoVO.type.Contains("#모음집"))
                                    {
                                        typeUtaiteDataVOs_4.Add(new Classes.DataVO.TypeUtaiteDataVO(musicUUID));
                                        isAdd = true;
                                    }

                                    if (musicInfoVO.type.Contains("#모음집") && typeUtaiteDataVOs_5.Count < 30)
                                    {
                                        typeUtaiteDataVOs_5.Add(new Classes.DataVO.TypeUtaiteDataVO(musicUUID));
                                        isAdd = true;
                                    }

                                    if (isAdd) blockUtaiteUUIDs.Add(musicUUID);

                                    if (typeUtaiteDataVOs_1.Count >= 30 &&
                                        typeUtaiteDataVOs_2.Count >= 30 &&
                                        typeUtaiteDataVOs_3.Count >= 30 &&
                                        typeUtaiteDataVOs_4.Count >= 30 &&
                                        typeUtaiteDataVOs_5.Count >= 30)
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                    settingTypeUtaiteItemSource();
                }

                // 아이템 새로고침
                newUtaiteListBox.Items.Refresh();
                topUtaiteListDataGrid.Items.Refresh();
                nicoNicoDougaRankListDataGrid.Items.Refresh();
                manyMusicListDataGrid.Items.Refresh();

                // Loading UI 설정
                loadingPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 최신 우타이테 리스트 박스 스크롤 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newUtaiteListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                ListBox listBox = sender as ListBox;
                ScrollViewer scrollviewer = FindVisualChildren<ScrollViewer>(listBox).FirstOrDefault();
                if (e.Delta > 0)
                    scrollviewer.LineLeft();
                else
                    scrollviewer.LineRight();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// Utils Function
        /// </summary>
        /// <typeparam name="T">[NOT DEFINED]</typeparam>
        /// <param name="depObj">[NOT DEFINED]</param>
        /// <returns></returns>
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T) child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }



        /// <summary>
        /// 최신 우타이테 리스트 박스 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newUtaiteListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as UtaitePlayer.Classes.DataVO.NewUtaiteDataVO != null))
                {
                    int selectedIndex = newUtaiteListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    UtaitePlayer.Classes.DataVO.NewUtaiteDataVO newUtaiteDataVO = newUtaiteDataVOs[selectedIndex];

                    // 전역 함수 호출
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, newUtaiteDataVO.uuid);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 인기 우타이테 DataGrid 스크롤 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topUtaiteListDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try 
            {
                //first, we make the object with the event arguments (using the values from the current event)
                var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

                //then we need to set the event that we're invoking.
                //the ScrollViewer control internally does the scrolling on MouseWheelEvent, so that's what we're going to use:
                args.RoutedEvent = ScrollViewer.MouseWheelEvent;

                //and finally, we raise the event on the parent ScrollViewer.
                rootHomeScrollViewer.RaiseEvent(args);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트에 담기 메뉴 클릭 이벤트 - 최신 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicMyPlayListMenuItemForNewUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = newUtaiteListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.NewUtaiteDataVO newUtaiteDataVO  = newUtaiteDataVOs[selectedIndex];
               
                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, newUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 정보 보기 메뉴 클릭 이벤트 - 최신 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMusicInfoMenuItemForNewUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = newUtaiteListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.NewUtaiteDataVO newUtaiteDataVO = newUtaiteDataVOs[selectedIndex];
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, newUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트에 담기 메뉴 클릭 이벤트 - 인기 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicMyPlayListMenuItemForTopUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex =  topUtaiteListDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.TopUtaiteDataVO topUtaiteDataVO = topUtaiteDataVOs[selectedIndex];

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, topUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 정보 보기 메뉴 클릭 이벤트 - 인기 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMusicInfoMenuItemForTopUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = topUtaiteListDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.TopUtaiteDataVO topUtaiteDataVO = topUtaiteDataVOs[selectedIndex];
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, topUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 재생 버튼 클릭 이벤트 - 인기 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButtonForTopUtaite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Classes.DataVO.TopUtaiteDataVO dataRowView = (Classes.DataVO.TopUtaiteDataVO)((System.Windows.Shapes.Path)e.Source).DataContext;

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, dataRowView.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 최신 우타이테 전체 재생 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AllMusicPlayButtonForNewUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> uuidList = new List<string>();

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                await Task.Run(() =>
                {
                    for (int index = 0; index < newUtaiteDataVOs.Count; index++)
                        uuidList.Add(newUtaiteDataVOs[index].uuid);
                });
                
                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY, uuidList);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 인기 우타이테 전체 재생 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AllMusicPlayButtonForTopUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                List<string> uuidList = new List<string>();

                await Task.Run(() =>
                {
                    for (int index = 0; index < topUtaiteDataVOs.Count; index++)
                        uuidList.Add(topUtaiteDataVOs[index].uuid);
                });

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY, uuidList);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 분위기별 우타이테 Tag 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicTag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool state = ((HandyControl.Controls.Tag)sender).IsSelected;

                musicTag_1.IsSelected = false;
                musicTag_2.IsSelected = false;
                musicTag_3.IsSelected = false;
                musicTag_4.IsSelected = false;
                musicTag_5.IsSelected = false;

                ((HandyControl.Controls.Tag)sender).IsSelected = !state;

                if (!(musicTag_1.IsSelected ||
                    musicTag_2.IsSelected ||
                    musicTag_3.IsSelected ||
                    musicTag_4.IsSelected ||
                    musicTag_5.IsSelected))
                    ((HandyControl.Controls.Tag)sender).IsSelected = true;

                settingTypeUtaiteItemSource();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 분위기별 우타이테 리스트 박스 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeUtaiteListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO != null))
                {
                    int selectedIndex = typeUtaiteListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO typeUtaiteDataVO = null;

                    if (musicTag_1.IsSelected)
                    {
                        typeUtaiteDataVO = typeUtaiteDataVOs_1[selectedIndex];
                    }
                    else if (musicTag_2.IsSelected)
                    {
                        typeUtaiteDataVO = typeUtaiteDataVOs_2[selectedIndex];
                    }
                    else if (musicTag_3.IsSelected)
                    {
                        typeUtaiteDataVO = typeUtaiteDataVOs_3[selectedIndex];
                    }
                    else if (musicTag_4.IsSelected)
                    {
                        typeUtaiteDataVO = typeUtaiteDataVOs_4[selectedIndex];
                    }
                    else if (musicTag_5.IsSelected)
                    {
                        typeUtaiteDataVO = typeUtaiteDataVOs_5[selectedIndex];
                    }

                    // 전역 함수 호출
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, typeUtaiteDataVO.uuid);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 분위기별 우타이테 리스트 박스 스크롤 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeUtaiteListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                ListBox listBox = sender as ListBox;
                ScrollViewer scrollviewer = FindVisualChildren<ScrollViewer>(listBox).FirstOrDefault();

                if (scrollviewer == null) return;

                if (e.Delta > 0)
                    scrollviewer.LineLeft();
                else
                    scrollviewer.LineRight();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 분위기별 우타이테 데이터 설정
        /// </summary>
        private void settingTypeUtaiteItemSource()
        {
            try
            {
                if (musicTag_1.IsSelected)
                {
                    typeUtaiteListBox.ItemsSource = typeUtaiteDataVOs_1;
                }
                else if (musicTag_2.IsSelected)
                {
                    typeUtaiteListBox.ItemsSource = typeUtaiteDataVOs_2;
                }
                else if (musicTag_3.IsSelected)
                {
                    typeUtaiteListBox.ItemsSource = typeUtaiteDataVOs_3;
                }
                else if (musicTag_4.IsSelected)
                {
                    typeUtaiteListBox.ItemsSource = typeUtaiteDataVOs_4;
                }
                else if (musicTag_5.IsSelected)
                {
                    typeUtaiteListBox.ItemsSource = typeUtaiteDataVOs_5;
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 분위기별 우타이테 전체 재생 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AllMusicPlayButtonForTypeUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");

                List<string> uuidList = new List<string>();

                if (musicTag_1.IsSelected)
                {
                    await Task.Run(() =>
                    {
                        for (int index = 0; index < typeUtaiteDataVOs_1.Count; index++)
                            uuidList.Add(typeUtaiteDataVOs_1[index].uuid);
                    });
                }
                else if (musicTag_2.IsSelected)
                {
                    await Task.Run(() =>
                    {
                        for (int index = 0; index < typeUtaiteDataVOs_2.Count; index++)
                            uuidList.Add(typeUtaiteDataVOs_2[index].uuid);
                    });
                }
                else if (musicTag_3.IsSelected)
                {
                    await Task.Run(() =>
                    {
                        for (int index = 0; index < typeUtaiteDataVOs_3.Count; index++)
                            uuidList.Add(typeUtaiteDataVOs_3[index].uuid);
                    });
                }
                else if (musicTag_4.IsSelected)
                {
                    await Task.Run(() =>
                    {
                        for (int index = 0; index < typeUtaiteDataVOs_4.Count; index++)
                            uuidList.Add(typeUtaiteDataVOs_4[index].uuid);
                    });
                }
                else if (musicTag_5.IsSelected)
                {
                    await Task.Run(() =>
                    {
                        for (int index = 0; index < typeUtaiteDataVOs_5.Count; index++)
                            uuidList.Add(typeUtaiteDataVOs_5[index].uuid);
                    });
                }

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY, uuidList);
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 노래 정보 보기 메뉴 클릭 이벤트 - 최신 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicMyPlayListMenuItemForTypeUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = typeUtaiteListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO typeUtaiteDataVO = null;

                if (musicTag_1.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_1[selectedIndex];
                }
                else if (musicTag_2.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_2[selectedIndex];
                }
                else if (musicTag_3.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_3[selectedIndex];
                }
                else if (musicTag_4.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_4[selectedIndex];
                }
                else if (musicTag_5.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_5[selectedIndex];
                }

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, typeUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트에 담기 메뉴 클릭 이벤트 - 분위기별 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMusicInfoMenuItemForTypeUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = typeUtaiteListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO typeUtaiteDataVO = null;

                if (musicTag_1.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_1[selectedIndex];
                }
                else if (musicTag_2.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_2[selectedIndex];
                }
                else if (musicTag_3.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_3[selectedIndex];
                }
                else if (musicTag_4.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_4[selectedIndex];
                }
                else if (musicTag_5.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_5[selectedIndex];
                }

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, typeUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 니코니코 동화 순위 DataGrid 스크롤 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nicoNicoDougaRank_ListDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                //first, we make the object with the event arguments (using the values from the current event)
                var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

                //then we need to set the event that we're invoking.
                //the ScrollViewer control internally does the scrolling on MouseWheelEvent, so that's what we're going to use:
                args.RoutedEvent = ScrollViewer.MouseWheelEvent;

                //and finally, we raise the event on the parent ScrollViewer.
                rootHomeScrollViewer.RaiseEvent(args);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 니코니코 동화 바로가기 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NicoNicoDougaLinkGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Classes.DataVO.NicoNicoDougaRankDataVO dataRowView = (Classes.DataVO.NicoNicoDougaRankDataVO)((Grid)e.Source).DataContext;
                System.Diagnostics.Process.Start(string.Format("https://nico.ms/{0}", dataRowView.contentId));
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
        /// 노래가 가장 많은 아티스트 순위 DataGrid 스크롤 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manyMusicListDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                //first, we make the object with the event arguments (using the values from the current event)
                var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

                //then we need to set the event that we're invoking.
                //the ScrollViewer control internally does the scrolling on MouseWheelEvent, so that's what we're going to use:
                args.RoutedEvent = ScrollViewer.MouseWheelEvent;

                //and finally, we raise the event on the parent ScrollViewer.
                rootHomeScrollViewer.RaiseEvent(args);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manyMusicLinkDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Classes.DataVO.ManyMusicDataVO dataRowView = (Classes.DataVO.ManyMusicDataVO)((Grid)e.Source).DataContext;

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SEARCH_FOR_TEXT, dataRowView.artistName);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 커스텀 플레이리스트에 노래 추가 - 최신 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicCustomMyPlayListMenuItemForNewUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = newUtaiteListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.NewUtaiteDataVO newUtaiteDataVO = newUtaiteDataVOs[selectedIndex];

                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER, newUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 커스텀 플레이리스트에 노래 추가 - 인기 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicCustomMyPlayListMenuItemForTopUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = topUtaiteListDataGrid.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.TopUtaiteDataVO topUtaiteDataVO = topUtaiteDataVOs[selectedIndex];

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER, topUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 커스텀 플레이리스트에 노래 추가 - 분위기별 우타이테
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicCustomMyPlayListMenuItemForTypeUtaite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = typeUtaiteListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.TypeUtaiteDataVO typeUtaiteDataVO = null;

                if (musicTag_1.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_1[selectedIndex];
                }
                else if (musicTag_2.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_2[selectedIndex];
                }
                else if (musicTag_3.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_3[selectedIndex];
                }
                else if (musicTag_4.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_4[selectedIndex];
                }
                else if (musicTag_5.IsSelected)
                {
                    typeUtaiteDataVO = typeUtaiteDataVOs_5[selectedIndex];
                }

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER, typeUtaiteDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
