using HandyControl.Controls;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    /// PixivTopImagePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PixivTopImagePage : System.Windows.Controls.Page
    {
        // 리스트 설정
        private List<Classes.DataVO.PixivTopImageDataVO> pixivTopImageDatVOs = new List<Classes.DataVO.PixivTopImageDataVO>();
        // Load checker
        private bool isLoaded = false;




        /// <summary>
        /// 생성자
        /// </summary>
        public PixivTopImagePage()
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
                if (pixivTopImageDatVOs.Count <= 0 && !isLoaded)
                {
                    isLoaded = true;

                    LoadingProgressBar.Visibility = Visibility.Visible;

                    // 데이터 파일 경로
                    RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();
                    string imageFilePath = System.IO.Path.Combine(musicDataManager.DATA_FILE_SAVE_DIRECTORY_NAME, "images_pixivtop");

                    DirectoryInfo di = new DirectoryInfo(imageFilePath);
                    if (!di.Exists) di.Create();

                    // 데이터 초기화
                    pixivTopImageDatVOs.Clear();

                    // UI 기본 설정
                    pixivTopImageListBox.Visibility = Visibility.Collapsed;
                    noResult.Visibility = Visibility.Collapsed;

                    // 데이터 설정
                    if (pixivTopImageListBox.ItemsSource == null)
                        pixivTopImageListBox.ItemsSource = pixivTopImageDatVOs;

                    RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto();
                    RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                    JArray jArray = null;

                    await Task.Run(() =>
                    {
                        try
                        {
                            JObject jObject = JObject.Parse(utaitePlayerClient.getPixivTopImageList(registryManager.getAuthToken().ToString()));

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

                    PixivImageDownloadForParallel pixivImageDownloadForParallel = new PixivImageDownloadForParallel();
                    pixivImageDownloadForParallel.downloadAction1 = new Action(() => imageDownloader(imageFilePath, 0, 5, jArray));
                    pixivImageDownloadForParallel.downloadAction2 = new Action(() => imageDownloader(imageFilePath, 5, 10, jArray));
                    pixivImageDownloadForParallel.downloadAction3 = new Action(() => imageDownloader(imageFilePath, 10, 15, jArray));
                    pixivImageDownloadForParallel.downloadAction4 = new Action(() => imageDownloader(imageFilePath, 15, 20, jArray));
                    pixivImageDownloadForParallel.downloadAction5 = new Action(() => imageDownloader(imageFilePath, 20, 25, jArray));
                    pixivImageDownloadForParallel.downloadAction6 = new Action(() => imageDownloader(imageFilePath, 25, 30, jArray));
                    pixivImageDownloadForParallel.downloadAction7 = new Action(() => imageDownloader(imageFilePath, 30, 35, jArray));
                    pixivImageDownloadForParallel.downloadAction8 = new Action(() => imageDownloader(imageFilePath, 35, 40, jArray));
                    pixivImageDownloadForParallel.downloadAction9 = new Action(() => imageDownloader(imageFilePath, 40, 45, jArray));
                    pixivImageDownloadForParallel.downloadAction10 = new Action(() => imageDownloader(imageFilePath, 45, 50, jArray));

                    pixivImageDownloadForParallel.startDownload();

                    if (jArray != null)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                pixivImageDownloadForParallel.waitDownloadEnd();

                                List<string> usedImages = new List<string>();

                                for (int i = 0; i < jArray.Count; i++)
                                {
                                    string imageName = jArray[i].ToString();
                                    string imagePath = System.IO.Path.Combine(imageFilePath, imageName);
                                    string imageURL = utaitePlayerClient.getPixivTopImageURL(registryManager.getAuthToken().ToString(), imageName);

                                    Classes.DataVO.PixivTopImageDataVO pixivTopImageDataVO = new Classes.DataVO.PixivTopImageDataVO();

                                    usedImages.Add(imageName);

                                    using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                                    {
                                        BitmapImage bitmapImage = new BitmapImage();
                                        bitmapImage.BeginInit();
                                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                        bitmapImage.CreateOptions = BitmapCreateOptions.None;
                                        bitmapImage.DecodePixelWidth = 110;
                                        bitmapImage.StreamSource = fs;
                                        bitmapImage.EndInit();
                                        bitmapImage.Freeze();

                                        pixivTopImageDataVO.image = bitmapImage;
                                    }

                                    pixivTopImageDataVO.url = imageURL;

                                    pixivTopImageDatVOs.Add(pixivTopImageDataVO);
                                }

                                FileInfo[] files = di.GetFiles();
                                foreach (FileInfo file in files)
                                    if (!usedImages.Contains(file.Name))
                                        file.Delete();
                            }
                            catch (Exception ex)
                            {
                                // 예외 처리
                                ExceptionManager.getInstance().showMessageBox(ex);
                            }
                        });
                    }

                    if (pixivTopImageDatVOs.Count <= 0)
                    {
                        noResult.Visibility = Visibility.Visible;
                        pixivTopImageListBox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        noResult.Visibility = Visibility.Collapsed;
                        pixivTopImageListBox.Visibility = Visibility.Visible;
                    }

                    LoadingProgressBar.Visibility = Visibility.Collapsed;

                    // 데이터 새로고침
                    pixivTopImageListBox.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 이미지 더블 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pixivTopImageListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control src = e.Source as Control;

                // 좌클릭 확인
                if (src != null && e.ChangedButton == MouseButton.Left && (((FrameworkElement)e.OriginalSource).DataContext as Classes.DataVO.PixivTopImageDataVO != null))
                {
                    int selectedIndex = pixivTopImageListBox.SelectedIndex;
                    if (selectedIndex < 0) return;

                    Classes.DataVO.PixivTopImageDataVO pixivTopImageDataVO = pixivTopImageDatVOs[selectedIndex];

                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_IMAGE_VIEWER_DRAWER, pixivTopImageDataVO.url);
                }
            }
            catch (Exception ex)
            {
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
                    string imageName = sources[i].ToString();
                    string imagePath = System.IO.Path.Combine(rootPath, imageName);
                    string imageURL = utaitePlayerClient.getPixivTopImageURL(registryManager.getAuthToken().ToString(), imageName);

                    if (!new System.IO.FileInfo(imagePath).Exists)
                    {
                        using (WebClient client = new WebClient())
                            client.DownloadFile(new Uri(imageURL), imagePath);
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
