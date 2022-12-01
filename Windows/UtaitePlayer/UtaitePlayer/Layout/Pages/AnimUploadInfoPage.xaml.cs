using HandyControl.Data;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

namespace UtaitePlayer.Layout.Pages
{
    /// <summary>
    /// AnimUploadInfoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AnimUploadInfoPage : System.Windows.Controls.Page
    {
        // 데이터 로딩 감지
        public bool isLoaded = false;
        // 애니메이션 데이터
        private List<Classes.DataVO.AnimUploadInfoDataVO> animUploadInfoDataVOs = new List<Classes.DataVO.AnimUploadInfoDataVO>();
        // 애니메이션 출력 데이터
        private List<Classes.DataVO.AnimUploadInfoDataVO> animUploadInfoDataVOsForSelected = new List<Classes.DataVO.AnimUploadInfoDataVO>();
        // 초기화 성공 여부
        public bool isInitSuccess = false;
        // 실행 여부 감지
        private bool isNoRun = true;
        // 데이터 갱신
        private bool isReloadData = false;




        /// <summary>
        /// 생성자
        /// </summary>
        public AnimUploadInfoPage()
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
                if (isLoaded) { isInitSuccess = true; return; }

                // 모듈 선언
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                // 데이터 초기화
                animUploadInfoDataVOs.Clear();

                // UI 기본 설정
                animInfolistListBox.Visibility = Visibility.Collapsed;
                noResult.Visibility = Visibility.Collapsed;

                // 데이터 설정
                if (animInfolistListBox.ItemsSource == null)
                    animInfolistListBox.ItemsSource = animUploadInfoDataVOsForSelected;

                JArray jArray = null;

                await Task.Run(() =>
                {
                    try
                    {
                        string serverResponse = utaitePlayerClient.getAnimUploadInfo(registryManager.getAuthToken().ToString(), RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient.AnimeUploadInfo.GET_ALL_INFO, null);
                        JObject jObject = JObject.Parse(serverResponse);

                        if (jObject.ContainsKey("result") && jObject.ContainsKey("message") && jObject["result"].ToString().Equals("success"))
                        {
                            isReloadData = false;

                            jArray = JArray.Parse(HttpUtility.UrlDecode(jObject["message"].ToString()));
                        }
                        else
                        {
                            isReloadData = true;

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    // 메시지 출력
                                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                                        new GrowlInfo()
                                        {
                                            Message = "애니메이션 데이터가 갱신 중입니다. 잠시 후 다시 시도하여 주십시오."
                                        });
                                }
                                catch (Exception ex)
                                {
                                    // 예외 처리
                                    ExceptionManager.getInstance().showMessageBox(ex);
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });

                if (jArray != null)
                {
                    int downloadCount = jArray.Count / 5;

                    string downloadPath = URLImageLoadManager.getImageSavePath(URLImageLoadManager.ImageType.IMAGE_ANIMATION);

                    ImageDownloadForParallel imageDownloadForParallel = new ImageDownloadForParallel();
                    imageDownloadForParallel.downloadAction1 = new Action(() => imageDownloader(downloadPath, 0, downloadCount, jArray));
                    imageDownloadForParallel.downloadAction2 = new Action(() => imageDownloader(downloadPath, downloadCount, downloadCount * 2, jArray));
                    imageDownloadForParallel.downloadAction3 = new Action(() => imageDownloader(downloadPath, downloadCount * 2, downloadCount * 3, jArray));
                    imageDownloadForParallel.downloadAction4 = new Action(() => imageDownloader(downloadPath, downloadCount * 3, downloadCount * 4, jArray));
                    imageDownloadForParallel.downloadAction5 = new Action(() => imageDownloader(downloadPath, downloadCount * 4, jArray.Count, jArray));

                    imageDownloadForParallel.startDownload();

                    await Task.Run(() =>
                    {
                        try
                        {
                            imageDownloadForParallel.waitDownloadEnd();

                            for (int i = 0; i < jArray.Count; i++)
                            {
                                JObject temp = JObject.Parse(jArray[i].ToString());
                                animUploadInfoDataVOs.Add(new Classes.DataVO.AnimUploadInfoDataVO()
                                {
                                    uuid = temp["uuid"].ToString(),
                                    image = temp["image"].ToString(),
                                    name = temp["name"].ToString(),
                                    episode = temp["episode"].ToString(),
                                    url = temp["url"].ToString(),
                                    date = temp["date"].ToString(),
                                    dayOfTheWeek = (int)temp["yoil"]
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }

                if (animUploadInfoDataVOs.Count <= 0)
                {
                    noResult.Visibility = Visibility.Visible;
                    animInfolistListBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    noResult.Visibility = Visibility.Collapsed;
                    animInfolistListBox.Visibility = Visibility.Visible;
                }

                if (isNoRun)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            animAirInfoDataLoad(6);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (animUploadInfoDataVOsForSelected.Count <= 0)
                                {
                                    noResult.Visibility = Visibility.Visible;
                                    animInfolistListBox.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
                                    noResult.Visibility = Visibility.Collapsed;
                                    animInfolistListBox.Visibility = Visibility.Visible;
                                }

                                animInfolistListBox.Items.Refresh();
                            });
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox(ex);
                        }
                    });
                }

                isInitSuccess = true;

                if (!isReloadData)
                    isNoRun = false;

                isLoaded = true;
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 특정 요일 데이터 로딩
        /// </summary>
        /// <param name="data">요일</param>
        private void animAirInfoDataLoad(int data)
        {
            try
            {
                animUploadInfoDataVOsForSelected.Clear();

                for (int i = 0; i < animUploadInfoDataVOs.Count; i++)
                    if (animUploadInfoDataVOs[i].dayOfTheWeek == data)
                        animUploadInfoDataVOsForSelected.Add(animUploadInfoDataVOs[i]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 애니메이션 요일 선택 TabControl 선택 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void yoilSelectedTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitSuccess) return;

            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            int arg = 0;

            switch (tabItem)
            {
                case "월요일":
                    arg = 0;
                    break;
                case "화요일":
                    arg = 1;
                    break;
                case "수요일":
                    arg = 2;
                    break;
                case "목요일":
                    arg = 3;
                    break;
                case "금요일":
                    arg = 4;
                    break;
                case "토요일":
                    arg = 5;
                    break;
                case "일요일":
                    arg = 6;
                    break;
            }

            await Task.Run(() =>
            {
                try
                {
                    animAirInfoDataLoad(arg);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (animUploadInfoDataVOsForSelected.Count <= 0)
                        {
                            noResult.Visibility = Visibility.Visible;
                            animInfolistListBox.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            noResult.Visibility = Visibility.Collapsed;
                            animInfolistListBox.Visibility = Visibility.Visible;
                        }

                        animInfolistListBox.Items.Refresh();
                    });
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox(ex);
                }
            });
        }



        /// <summary>
        /// 애니메이션 업로드 정보 ListBox 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animInfolistListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as UtaitePlayer.Classes.DataVO.AnimUploadInfoDataVO != null))
                {
                    int selectedIndex = animInfolistListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    UtaitePlayer.Classes.DataVO.AnimUploadInfoDataVO animUploadInfoDataVO = animUploadInfoDataVOsForSelected[selectedIndex];

                    if (!animUploadInfoDataVO.url.Equals("[null]"))
                        System.Diagnostics.Process.Start(animUploadInfoDataVO.url);
                    else
                        // 메시지 출력
                        RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                            new GrowlInfo()
                            {
                                Message = "해당 애니메이션은 사이트 주소 데이터가 없습니다."
                            });
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 이미지 다운로더
        /// </summary>
        /// <param name="rootPath">ROOT 경로</param>
        /// <param name="startIndex">시작 인덱스</param>
        /// <param name="endIndex">종료 인덱스</param>
        /// <param name="sources">다운로드 데이터</param>
        private void imageDownloader(string rootPath, int startIndex, int endIndex, JArray sources)
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();

                for (int i = startIndex; i < endIndex; i++)
                {
                    JObject temp = JObject.Parse(sources[i].ToString());

                    string imageName = string.Format("{0}.png", temp["uuid"].ToString());
                    string imagePath = System.IO.Path.Combine(rootPath, imageName);

                    if (!new System.IO.FileInfo(imagePath).Exists && !temp["image"].ToString().Equals("[null]"))
                    {
                        using (WebClient client = new WebClient())
                            client.DownloadFile(new Uri(temp["image"].ToString()), imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
