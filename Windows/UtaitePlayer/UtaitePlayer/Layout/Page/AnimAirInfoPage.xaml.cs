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

namespace UtaitePlayer.Layout.Page
{
    /// <summary>
    /// AnimAirInfoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AnimAirInfoPage : System.Windows.Controls.Page
    {
        // 데이터 로딩 감지
        public bool isLoaded = false;
        // 애니메이션 데이터
        private List<Classes.DataVO.AnimAirInfoDataVO> animAirInfoDataVOs = new List<Classes.DataVO.AnimAirInfoDataVO>();
        // 애니메이션 출력 데이터
        private List<Classes.DataVO.AnimAirInfoDataVO> animAirInfoDataVOsForSelected = new List<Classes.DataVO.AnimAirInfoDataVO>();
        // 초기화 성공 여부
        public bool isInitSuccess = false;
        // 실행 여부 감지
        private bool isNoRun = true;




        /// <summary>
        /// 생성자
        /// </summary>
        public AnimAirInfoPage()
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

                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Data loading...");

                // 데이터 초기화
                animAirInfoDataVOs.Clear();

                // UI 기본 설정
                animInfolistListBox.Visibility = Visibility.Collapsed;
                noResult.Visibility = Visibility.Collapsed;

                // 데이터 설정
                if (animInfolistListBox.ItemsSource == null)
                    animInfolistListBox.ItemsSource = animAirInfoDataVOsForSelected;

                JArray jArray = null;

                await Task.Run(() =>
                {
                    try
                    {
                        string serverResponse = utaitePlayerClient.getOHLIAnimAirInfo(registryManager.getAuthToken().ToString());
                        JObject jObject = JObject.Parse(serverResponse);

                        if (jObject.ContainsKey("result") && jObject.ContainsKey("message") && jObject["result"].ToString().Equals("success"))
                        {
                            jArray = JArray.Parse(HttpUtility.UrlDecode(jObject["message"].ToString()));
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
                                animAirInfoDataVOs.Add(new Classes.DataVO.AnimAirInfoDataVO()
                                {
                                    uuid = temp["uuid"].ToString(),
                                    image = temp["image"].ToString(),
                                    name = temp["name"].ToString(),
                                    startDate = temp["start_day"].ToString(),
                                    endDate = temp["end_day"].ToString(),
                                    liveTime = temp["live_time"].ToString(),
                                    officialSite = temp["official_site"].ToString(),
                                    dayOfTheWeek = (int)temp["day_of_the_week"]
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

                if (animAirInfoDataVOs.Count <= 0)
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
                            animAirInfoDataLoad(0);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (animAirInfoDataVOsForSelected.Count <= 0)
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
                isNoRun = false;
                isLoaded = true;
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
            finally
            {
                // 전역 Dialog 설정
                RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);
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
                case "일요일":
                    arg = 0;
                    break;
                case "월요일":
                    arg = 1;
                    break;
                case "화요일":
                    arg = 2;
                    break;
                case "수요일":
                    arg = 3;
                    break;
                case "목요일":
                    arg = 4;
                    break;
                case "금요일":
                    arg = 5;
                    break;
                case "토요일":
                    arg = 6;
                    break;
                case "기타":
                    arg = 7;
                    break;
            }

            await Task.Run(() =>
            {
                try
                {
                    animAirInfoDataLoad(arg);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (animAirInfoDataVOsForSelected.Count <= 0)
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
        /// 특정 요일 데이터 로딩
        /// </summary>
        /// <param name="data">요일</param>
        private void animAirInfoDataLoad(int data)
        {
            try
            {
                animAirInfoDataVOsForSelected.Clear();

                for (int i = 0; i < animAirInfoDataVOs.Count; i++)
                    if (animAirInfoDataVOs[i].dayOfTheWeek == data)
                        animAirInfoDataVOsForSelected.Add(animAirInfoDataVOs[i]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 공식 사이트 방문 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoOfficialSiteAnimInfolistListBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = animInfolistListBox.SelectedIndex;
                if (selectedIndex < 0) return;

                Classes.DataVO.AnimAirInfoDataVO animAirInfoDataVO = animAirInfoDataVOsForSelected[selectedIndex];

                if (!animAirInfoDataVO.Equals("[null]"))
                    System.Diagnostics.Process.Start(animAirInfoDataVO.officialSite);
                else
                    // 메시지 출력
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS,
                        new GrowlInfo()
                        {
                            Message = "해당 애니메이션은 공식 사이트 주소 데이터가 없습니다."
                        });
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
