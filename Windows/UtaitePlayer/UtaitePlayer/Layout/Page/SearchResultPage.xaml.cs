using HandyControl.Data;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// SearchResultPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SearchResultPage : System.Windows.Controls.Page
    {
        // 숫자 정규식
        private readonly Regex ONLY_NUMBER_REGEX = new Regex("[^0-9.-]+");
        // DataGrid 데이터 바인딩 - 노래
        private List<UtaitePlayer.Classes.DataVO.SearchResultDataVO> searchResultDataVOSForMusic = null;
        // DataGrid 데이터 바인딩 - 아티스트
        private List<UtaitePlayer.Classes.DataVO.SearchResultDataVO> searchResultDataVOSForArtist = null;
        // DataGrid 데이터 바인딩 - 노래
        private List<UtaitePlayer.Classes.DataVO.SearchResultDataVO> searchResultDataVOSForMusicTemp = null;
        // DataGrid 데이터 바인딩 - 아티스트
        private List<UtaitePlayer.Classes.DataVO.SearchResultDataVO> searchResultDataVOSForArtistTemp = null;
        // 페이지 인덱스 - 노래
        private int pageNowIndexForMusic = 0;
        private int pageMaxIndexForMusic = 0;
        // 페이지 인덱스 - 아티스트
        private int pageNowIndexForArtist = 0;
        private int pageMaxIndexForArtist = 0;
        // 이벤트 실행 여부 제어
        private bool pageIndexTextboxForMusicTextChangedNoCheck = false;
        private bool pageIndexTextboxForArtistTextChangedNoCheck = false;



        /// <summary>
        /// 생성자
        /// </summary>
        public SearchResultPage()
        {
            InitializeComponent();

            // 데이터 바인딩
            searchResultDataGridForMusic.ItemsSource = searchResultDataVOSForMusic;
            searchResultDataGridForArtist.ItemsSource = searchResultDataVOSForArtist;
        }



        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 데이터 바인딩
            searchResultDataGridForMusic.ItemsSource = searchResultDataVOSForMusic;
            searchResultDataGridForArtist.ItemsSource = searchResultDataVOSForArtist;
        }



        /// <summary>
        /// Music search result scroll event sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchResultDataGridForMusic_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //first, we make the object with the event arguments (using the values from the current event)
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            //then we need to set the event that we're invoking.
            //the ScrollViewer control internally does the scrolling on MouseWheelEvent, so that's what we're going to use:
            args.RoutedEvent = ScrollViewer.MouseWheelEvent;

            //and finally, we raise the event on the parent ScrollViewer.
            rootSearchResultScrollViewer.RaiseEvent(args);
        }



        /// <summary>
        /// Artist search result scroll event sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchResultDataGridForArtist_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //first, we make the object with the event arguments (using the values from the current event)
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            //then we need to set the event that we're invoking.
            //the ScrollViewer control internally does the scrolling on MouseWheelEvent, so that's what we're going to use:
            args.RoutedEvent = ScrollViewer.MouseWheelEvent;

            //and finally, we raise the event on the parent ScrollViewer.
            rootSearchResultScrollViewer.RaiseEvent(args);
        }



        /// <summary>
        /// 문자열 검색
        /// </summary>
        /// <param name="searchText">검색 대상 문자열</param>
        public async void searchForText(string searchText)
        {
            // 변수 초기화
            if (searchResultDataVOSForMusic == null)
                searchResultDataVOSForMusic = new List<Classes.DataVO.SearchResultDataVO>();
            if (searchResultDataVOSForArtist == null)
                searchResultDataVOSForArtist = new List<Classes.DataVO.SearchResultDataVO>();
            if (searchResultDataVOSForMusicTemp == null)
                searchResultDataVOSForMusicTemp = new List<Classes.DataVO.SearchResultDataVO>();
            if (searchResultDataVOSForArtistTemp == null)
                searchResultDataVOSForArtistTemp = new List<Classes.DataVO.SearchResultDataVO>();

            try
            {
                RHYASearchHelper mRHYASearchHelper = new RHYASearchHelper();

                loadingPanel.Visibility = Visibility.Visible;

                // 비동기 작업
                await Task.Run(() => 
                {
                    try
                    {
                        // 변수 초기화
                        searchResultDataVOSForMusicTemp.Clear();
                        searchResultDataVOSForArtistTemp.Clear();
                        searchResultDataVOSForMusic.Clear();
                        searchResultDataVOSForArtist.Clear();

                        // 노래 제목, 아티스트, 작곡가, 태그 검색
                        foreach (string musicUUID in RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.Keys)
                        {
                            try
                            {
                                // 검색 진행 여부 확인
                                if (!mRHYASearchHelper.searchResultForMusic.Contains(musicUUID))
                                {
                                    // 노래 데이터
                                    RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[musicUUID];
                                    RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO singerInfoVO = null;
                                    if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.ContainsKey(musicInfoVO.singerUUID))
                                        singerInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[musicInfoVO.singerUUID];

                                    // 노래 데이터 비교 - 제목
                                    if (RHYAStringExtensions.Contains(musicInfoVO.name, searchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mRHYASearchHelper.searchResultForMusic.Add(musicUUID);
                                        continue;
                                    }

                                    // 노래 데이터 비교 - 아티스트
                                    if (singerInfoVO != null &&
                                        (RHYAStringExtensions.Contains(singerInfoVO.name, searchText, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        mRHYASearchHelper.searchResultForMusic.Add(musicUUID);
                                        continue;
                                    }

                                    // 노래 데이터 비교 - 작곡가
                                    if (RHYAStringExtensions.Contains(musicInfoVO.songWriter, searchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mRHYASearchHelper.searchResultForMusic.Add(musicUUID);
                                        continue;
                                    }

                                    // 노래 데이터 비교 - 태그
                                    if (RHYAStringExtensions.Contains(musicInfoVO.type, searchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mRHYASearchHelper.searchResultForMusic.Add(musicUUID);
                                        continue;
                                    }
                                }
                            }
                            catch (Exception) {}
                        }

                        // 아티스트 이름 검색
                        foreach (string singerUUID in RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.Keys)
                        {
                            try
                            {
                                // 검색 진행 여부 확인
                                if (!mRHYASearchHelper.searchResultForArtist.Contains(singerUUID))
                                {
                                    // 아티스트 데이터
                                    RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO singerInfo = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[singerUUID];

                                    // 아티스트 데이터 비교 - 이름
                                    if (RHYAStringExtensions.Contains(singerInfo.name, searchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mRHYASearchHelper.searchResultForArtist.Add(singerUUID);
                                        continue;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // 리스트 추가
                await Task.Run(() =>
                {
                    try
                    {
                        // 노래 데이터
                        foreach (string uuid in mRHYASearchHelper.searchResultForMusic)
                            searchResultDataVOSForMusicTemp.Add(
                                new Classes.DataVO.SearchResultDataVO(
                                    Classes.DataVO.SearchResultDataVO.SearchResultDataType.MUSIC_RESULT,
                                    uuid));

                        // 아티스트 데이터
                        foreach (string uuid in mRHYASearchHelper.searchResultForArtist)
                            searchResultDataVOSForArtistTemp.Add(
                                new Classes.DataVO.SearchResultDataVO(
                                    Classes.DataVO.SearchResultDataVO.SearchResultDataType.ARTIST_RESULT,
                                    uuid));

                        pageNowIndexForMusic = searchResultDataVOSForMusicTemp.Count > 0 ? 1 : 0;
                        pageMaxIndexForMusic = searchResultDataVOSForMusicTemp.Count % 10 == 0 ? searchResultDataVOSForMusicTemp.Count / 10 : (searchResultDataVOSForMusicTemp.Count / 10) + 1;
                        pageNowIndexForArtist = searchResultDataVOSForArtistTemp.Count > 0 ? 1 : 0;
                        pageMaxIndexForArtist = searchResultDataVOSForArtistTemp.Count % 10 == 0 ? searchResultDataVOSForArtistTemp.Count / 10 : (searchResultDataVOSForArtistTemp.Count / 10) + 1;

                        if (searchResultDataVOSForMusicTemp.Count > 0)
                        {
                            if (searchResultDataVOSForMusicTemp.Count >= 10)
                            {
                                searchResultDataVOSForMusic.AddRange(searchResultDataVOSForMusicTemp.GetRange(0, 10));
                            }
                            else
                            {
                                searchResultDataVOSForMusic.AddRange(searchResultDataVOSForMusicTemp.GetRange(0, searchResultDataVOSForMusicTemp.Count));
                            }
                        }

                        if (searchResultDataVOSForArtistTemp.Count > 0)
                        {
                            if (searchResultDataVOSForArtistTemp.Count >= 10)
                            {
                                searchResultDataVOSForArtist.AddRange(searchResultDataVOSForArtistTemp.GetRange(0, 10));
                            }
                            else
                            {
                                searchResultDataVOSForArtist.AddRange(searchResultDataVOSForArtistTemp.GetRange(0, searchResultDataVOSForArtistTemp.Count));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                // UI 변경 사항  적용
                searchResultDataGridForMusic.Items.Refresh();
                searchResultDataGridForArtist.Items.Refresh();

                loadingPanel.Visibility = Visibility.Collapsed;

                // UI 초기화
                mainUIInit();
            }
            catch (Exception ex)
            {
                loadingPanel.Visibility = Visibility.Collapsed;
                // 변수 초기화
                searchResultDataVOSForMusicTemp.Clear();
                searchResultDataVOSForArtistTemp.Clear();
                searchResultDataVOSForMusic.Clear();
                searchResultDataVOSForArtist.Clear();
                // UI 초기화
                mainUIInit();

                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// UI 로딩 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // UI 초기화
            mainUIInit();
        }


        /// <summary>
        /// UI 초기화
        /// </summary>
        private void mainUIInit()
        {
            try
            {
                // UI 초기화 - 노래 기본 정보
                if (searchResultDataVOSForMusicTemp.Count <= 0)
                {
                    // 검색 결과 없음 Panel 설정
                    noSearchResultForMusic.Visibility = Visibility.Visible;
                    searchResultDataGridForMusic.Visibility = Visibility.Collapsed;
                    // 인덱스 Textbox 설정
                    pageIndexTextboxForMusic.Text = "0";
                    pageIndexTextboxForMusic.IsEnabled = false;
                    // 인덱스 컨트롤 버튼 설정
                    pageIndexNextButtonForMusic.IsEnabled = false;
                    pageIndexPerviousButtonForMusic.IsEnabled = false;
                    // 인덱스 최대 개수 설정
                    pageIndexTextBlockForMusic.Text = "/0";
                }
                else
                {
                    // 검색 결과 없음 Panel 설정
                    noSearchResultForMusic.Visibility = Visibility.Collapsed;
                    searchResultDataGridForMusic.Visibility = Visibility.Visible;
                    // 인덱스 Textbox 설정
                    pageIndexTextboxForMusic.Text = "1";
                    pageIndexTextboxForMusic.IsEnabled = true;
                    // 인덱스 컨트롤 버튼 설정
                    if (pageMaxIndexForMusic <= 1) pageIndexNextButtonForMusic.IsEnabled = false;
                    else pageIndexNextButtonForMusic.IsEnabled = true;
                    pageIndexPerviousButtonForMusic.IsEnabled = false;
                    // 인덱스 최대 개수 설정
                    pageIndexTextBlockForMusic.Text = string.Format("/{0}", pageMaxIndexForMusic);
                }


                // UI 초기화 - 아티스트 기본 정보
                if (searchResultDataVOSForArtistTemp.Count <= 0)
                {
                    // 검색 결과 없음 Panel 설정
                    noSearchResultForArtist.Visibility = Visibility.Visible;
                    searchResultDataGridForArtist.Visibility = Visibility.Collapsed;
                    // 인덱스 Textbox 설정
                    pageIndexTextboxForArtist.Text = "0";
                    pageIndexTextboxForArtist.IsEnabled = false;
                    // 인덱스 컨트롤 버튼 설정
                    pageIndexNextButtonForArtist.IsEnabled = false;
                    pageIndexPerviousButtonForArtist.IsEnabled = false;
                    // 인덱스 최대 개수 설정
                    pageIndexTextBlockForArtist.Text = "/0";
                }
                else
                {
                    // 검색 결과 없음 Panel 설정
                    noSearchResultForArtist.Visibility = Visibility.Collapsed;
                    searchResultDataGridForArtist.Visibility = Visibility.Visible;
                    // 인덱스 Textbox 설정
                    pageIndexTextboxForArtist.Text = "1";
                    pageIndexTextboxForArtist.IsEnabled = true;
                    // 인덱스 컨트롤 버튼 설정
                    if (pageMaxIndexForArtist <= 1) pageIndexNextButtonForArtist.IsEnabled = false;
                    else pageIndexNextButtonForArtist.IsEnabled = true;
                    pageIndexPerviousButtonForArtist.IsEnabled = false;
                    // 인덱스 최대 개수 설정
                    pageIndexTextBlockForArtist.Text = string.Format("/{0}", pageMaxIndexForArtist);
                }


                // UI 초기화 - 노래 타이틀
                searchResultTitleForMusic.Text = string.Format("노래({0})", searchResultDataVOSForMusicTemp.Count);
                // UI 초기화 - 아티스트 타이틀
                searchResultTitleForArtist.Text = string.Format("아티스트({0})", searchResultDataVOSForArtistTemp.Count);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 인덱스 텍스트 박스 텍스트 입력 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForMusic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ONLY_NUMBER_REGEX.IsMatch(e.Text);
        }



        /// <summary>
        /// 인덱스 텍스트 박스 텍스트 변경 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForMusic_TextChanged(object sender, TextChangedEventArgs e)
        {
            string inputText = ((HandyControl.Controls.TextBox)sender).Text;

            if (pageIndexTextboxForMusicTextChangedNoCheck) return;

            try
            {
                // 최소 길이 확인
                if (inputText.Length <= 0)
                {
                    pageIndexTextboxForMusicTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = "1";
                    ((HandyControl.Controls.TextBox)sender).SelectAll();
                    pageIndexTextboxForMusicTextChangedNoCheck = false;
                    return;
                }

                // 숫자 확인
                if (!ONLY_NUMBER_REGEX.IsMatch(inputText))
                {
                    pageIndexTextboxForMusicTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = Regex.Replace(inputText, @"\D", "");
                    pageIndexTextboxForMusicTextChangedNoCheck = false;
                }

                inputText = ((HandyControl.Controls.TextBox)sender).Text;

                // 최대 길이 확인
                if (int.Parse(inputText) > pageMaxIndexForMusic)
                {
                    pageIndexTextboxForMusicTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = pageMaxIndexForMusic.ToString();
                    ((HandyControl.Controls.TextBox)sender).SelectAll();
                    pageIndexTextboxForMusicTextChangedNoCheck = false;
                }
            }
            catch (Exception)
            {
                if (ONLY_NUMBER_REGEX.IsMatch(((HandyControl.Controls.TextBox)sender).Text))
                {
                    pageIndexTextboxForMusicTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = Regex.Replace(inputText, @"\D", "");
                    pageIndexTextboxForMusicTextChangedNoCheck = false;
                }
            }
        }



        /// <summary>
        /// 인덱스 텍스트 박스 선택 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForMusic_GotFocus(object sender, RoutedEventArgs e)
        {
            ((HandyControl.Controls.TextBox)sender).SelectAll();
        }



        /// <summary>
        /// 인덱스 텍스트 박스 선택 해제 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForMusic_LostFocus(object sender, RoutedEventArgs e)
        {
            ((HandyControl.Controls.TextBox)sender).Text = pageNowIndexForMusic.ToString();
        }



        /// <summary>
        /// 인덱스 텍스트 박스 텍스트 입력 이벤트 - 아티스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForArtist_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ONLY_NUMBER_REGEX.IsMatch(e.Text);
        }



        /// <summary>
        /// 인덱스 텍스트 박스 텍스트 변경 이벤트 - 아티스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForArtist_TextChanged(object sender, TextChangedEventArgs e)
        {
            string inputText = ((HandyControl.Controls.TextBox)sender).Text;

            if (pageIndexTextboxForArtistTextChangedNoCheck) return;

            try
            {
                // 최소 길이 확인
                if (inputText.Length <= 0)
                {
                    pageIndexTextboxForArtistTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = "1";
                    ((HandyControl.Controls.TextBox)sender).SelectAll();
                    pageIndexTextboxForArtistTextChangedNoCheck = false;
                    return;
                }

                // 숫자 확인
                if (!ONLY_NUMBER_REGEX.IsMatch(inputText))
                {
                    ((HandyControl.Controls.TextBox)sender).Text = Regex.Replace(inputText, @"\D", "");
                }

                inputText = ((HandyControl.Controls.TextBox)sender).Text;

                // 최대 길이 확인
                if (int.Parse(inputText) > pageMaxIndexForArtist)
                {
                    pageIndexTextboxForArtistTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = pageMaxIndexForArtist.ToString();
                    ((HandyControl.Controls.TextBox)sender).SelectAll();
                    pageIndexTextboxForArtistTextChangedNoCheck = false;
                }
                else
                {
                    // 최소 길이 확인
                    if (int.Parse(inputText) < pageNowIndexForArtist)
                    {
                        pageIndexTextboxForArtistTextChangedNoCheck = true;
                        ((HandyControl.Controls.TextBox)sender).Text = pageNowIndexForArtist.ToString();
                        ((HandyControl.Controls.TextBox)sender).SelectAll();
                        pageIndexTextboxForArtistTextChangedNoCheck = false;
                    }
                }
            }
            catch (Exception)
            {
                if (ONLY_NUMBER_REGEX.IsMatch(((HandyControl.Controls.TextBox)sender).Text))
                {
                    pageIndexTextboxForArtistTextChangedNoCheck = true;
                    ((HandyControl.Controls.TextBox)sender).Text = Regex.Replace(inputText, @"\D", "");
                    pageIndexTextboxForArtistTextChangedNoCheck = false;
                }
            }
        }



        /// <summary>
        /// 인덱스 텍스트 박스 선택 이벤트 - 아티스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForArtist_GotFocus(object sender, RoutedEventArgs e)
        {
            ((HandyControl.Controls.TextBox)sender).SelectAll();
        }



        /// <summary>
        /// 인덱스 텍스트 박스 선택 해제 이벤트 - 아티스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForArtist_LostFocus(object sender, RoutedEventArgs e)
        {
            ((HandyControl.Controls.TextBox)sender).Text = pageNowIndexForArtist.ToString();
        }



        /// <summary>
        ///  인덱스 텍스트 박스 키 입력 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForMusic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Enter 감지
                if (e.Key == Key.Enter)
                    movePageIndexForMusic(int.Parse(Regex.Replace(((HandyControl.Controls.TextBox)sender).Text, @"\D", "")));
                
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 인덱스 텍스트 박스 키 입력 이벤트 - 아티스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexTextboxForArtist_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Enter 감지
                if (e.Key == Key.Enter)
                    movePageIndexForArtist(int.Parse(Regex.Replace(((HandyControl.Controls.TextBox)sender).Text, @"\D", "")));
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 페이지 이전 인덱스로 이동 버튼 클릭 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexPerviousButtonForMusic_Click(object sender, RoutedEventArgs e)
        {
            movePageIndexForMusic(pageNowIndexForMusic - 1);
        }



        /// <summary>
        /// 페이지 다음 인덱스로 이동 버튼 클릭 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexNextButtonForMusic_Click(object sender, RoutedEventArgs e)
        {
            movePageIndexForMusic(pageNowIndexForMusic + 1);
        }



        /// <summary>
        /// 페이지 인덱스 이동 - 노래
        /// </summary>
        /// <param name="index">인덱스</param>
        private void movePageIndexForMusic(int index)
        {
            try
            {
                if (index <= pageMaxIndexForMusic && index > 0)
                {
                    if (searchResultDataVOSForMusicTemp.Count > 0)
                    {
                        if (searchResultDataVOSForMusicTemp.Count - ((index - 1) * 10) >= 10)
                        {
                            searchResultDataVOSForMusic.Clear();
                            searchResultDataVOSForMusic.AddRange(searchResultDataVOSForMusicTemp.GetRange((index - 1) * 10, 10));
                            searchResultDataGridForMusic.Items.Refresh();
                        }
                        else
                        {
                            searchResultDataVOSForMusic.Clear();
                            searchResultDataVOSForMusic.AddRange(searchResultDataVOSForMusicTemp.GetRange((index - 1) * 10, searchResultDataVOSForMusicTemp.Count - ((index - 1) * 10)));
                            searchResultDataGridForMusic.Items.Refresh();
                        }

                        pageIndexTextboxForMusicTextChangedNoCheck = true;
                        pageIndexTextboxForMusic.Text = index.ToString();
                        pageIndexTextboxForMusicTextChangedNoCheck = false;

                        pageNowIndexForMusic = index;

                        if (pageMaxIndexForMusic == 1) return;

                        // 인덱스 컨트롤 버튼 설정
                        if (index == 0 || index == 1)
                        {
                            pageIndexNextButtonForMusic.IsEnabled = true;
                            pageIndexPerviousButtonForMusic.IsEnabled = false;
                        }
                        else if (index == pageMaxIndexForMusic)
                        {
                            pageIndexNextButtonForMusic.IsEnabled = false;
                            pageIndexPerviousButtonForMusic.IsEnabled = true;
                        }
                        else
                        {
                            pageIndexNextButtonForMusic.IsEnabled = true;
                            pageIndexPerviousButtonForMusic.IsEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 페이지 인덱스 이동 - 아티스트
        /// </summary>
        /// <param name="index">인덱스</param>
        private void movePageIndexForArtist(int index)
        {
            try
            {
                if (index <= pageMaxIndexForArtist && index > 0)
                {
                    if (searchResultDataVOSForArtistTemp.Count > 0)
                    {
                        if (searchResultDataVOSForArtistTemp.Count - ((index - 1) * 10) >= 10)
                        {
                            searchResultDataVOSForArtist.Clear();
                            searchResultDataVOSForArtist.AddRange(searchResultDataVOSForArtistTemp.GetRange((index - 1) * 10, 10));
                            searchResultDataGridForArtist.Items.Refresh();
                        }
                        else
                        {
                            searchResultDataVOSForArtist.Clear();
                            searchResultDataVOSForArtist.AddRange(searchResultDataVOSForArtistTemp.GetRange((index - 1) * 10, searchResultDataVOSForArtistTemp.Count - ((index - 1) * 10)));
                            searchResultDataGridForArtist.Items.Refresh();
                        }

                        pageIndexTextboxForArtistTextChangedNoCheck = true;
                        pageIndexTextboxForArtist.Text = index.ToString();
                        pageIndexTextboxForArtistTextChangedNoCheck = false;

                        pageNowIndexForArtist = index;

                        if (pageMaxIndexForArtist == 1) return;

                        // 인덱스 컨트롤 버튼 설정
                        if (index == 0 || index == 1)
                        {
                            pageIndexNextButtonForArtist.IsEnabled = true;
                            pageIndexPerviousButtonForArtist.IsEnabled = false;
                        }
                        else if (index == pageMaxIndexForArtist)
                        {
                            pageIndexNextButtonForArtist.IsEnabled = false;
                            pageIndexPerviousButtonForArtist.IsEnabled = true;
                        }
                        else
                        {
                            pageIndexNextButtonForArtist.IsEnabled = true;
                            pageIndexPerviousButtonForArtist.IsEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 페이지 이전 인덱스로 이동 버튼 클릭 이벤트 - 아티스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexPerviousButtonForArtist_Click(object sender, RoutedEventArgs e)
        {
            movePageIndexForArtist(pageNowIndexForArtist - 1);
        }



        /// <summary>
        /// 페이지 다음 인덱스로 이동 버튼 클릭 이벤트 - 노래
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageIndexNextButtonForArtist_Click(object sender, RoutedEventArgs e)
        {
            movePageIndexForArtist(pageNowIndexForArtist + 1);
        }



        /// <summary>
        /// 노래 재생 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Classes.DataVO.SearchResultDataVO dataRowView = (Classes.DataVO.SearchResultDataVO)((Path)e.Source).DataContext;
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_PLAY_MUSIC, dataRowView.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트에 담기 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicMyPlayListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = searchResultDataGridForMusic.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.SearchResultDataVO searchResultDataVO = searchResultDataVOSForMusic[selectedIndex];

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_MUSIC_ADD_PLAYLIST, searchResultDataVO.uuid);
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
        private void ShowMusicInfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = searchResultDataGridForMusic.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.SearchResultDataVO searchResultDataVO = searchResultDataVOSForMusic[selectedIndex];
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER, searchResultDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 아티스트 구독 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ArtistSubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = searchResultDataGridForArtist.SelectedIndex;
                if (selectedIndex < 0) return;

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                UtaitePlayer.Classes.DataVO.SearchResultDataVO searchResultDataVO = searchResultDataVOSForArtist[selectedIndex];

                searchResultDataVO.artistLoadingVisibility = Visibility.Visible;
                searchResultDataVO.artistSubscribeButtonVisibility = Visibility.Collapsed;
                searchResultDataGridForArtist.Items.Refresh();

                // 비동기 작업
                await Task.Run(() =>
                {
                    try
                    {
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient.UserSubscribeTaskInfo taskType = RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient.UserSubscribeTaskInfo.SUBSCRIBE;
                        if (searchResultDataVO.artistSubscribeData.Equals("구독 취소"))
                            taskType = RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient.UserSubscribeTaskInfo.UNSUBSCRIBE;
                        

                        string serverResponse = utaitePlayerClient.userSubscribeTask(registryManager.getAuthToken().ToString(), taskType, searchResultDataVO.uuid);
                        
                        JObject jObject = JObject.Parse(serverResponse);
                        if (jObject.ContainsKey("result"))
                        {
                            if (!(jObject["result"].Equals("success")))
                            {
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

                                // 구독 작업 처리 확인
                                searchResultDataVO.artistSubscribeChecker();
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

                searchResultDataVO.artistLoadingVisibility = Visibility.Collapsed;
                searchResultDataVO.artistSubscribeButtonVisibility = Visibility.Visible;
                searchResultDataGridForArtist.Items.Refresh();
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 커스텀 플레이리스트에 노래 추가 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMusicCustomMyPlayListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = searchResultDataGridForMusic.SelectedIndex;
                if (selectedIndex < 0) return;

                UtaitePlayer.Classes.DataVO.SearchResultDataVO searchResultDataVO = searchResultDataVOSForMusic[selectedIndex];

                // 전역 함수 호출
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER, searchResultDataVO.uuid);
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
