using CefSharp;
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
    /// SongAddPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SongAddPage : System.Windows.Controls.Page
    {
        // 노래 신청 URL
        private string SONG_ADD_URL = "";




        /// <summary>
        /// 생성자
        /// </summary>
        public SongAddPage()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 로딩 시작 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chromiumWebBrowser_FrameLoadStart(object sender, CefSharp.FrameLoadStartEventArgs e)
        {
            try
            {
                // UI 상태 변경
                Application.Current.Dispatcher.Invoke(() =>
                {
                    loadingProgressbar.Visibility = Visibility.Visible;
                    chromiumWebBrowser.Visibility = Visibility.Hidden;
                });

                // URL 이동 가능 여부 확인
                if (!e.Url.Equals(SONG_ADD_URL))
                {
                    chromiumWebBrowser.Stop();
                    chromiumWebBrowser.LoadUrl(SONG_ADD_URL);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
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
                // 캐시 제거
                await Cef.GetGlobalCookieManager().DeleteCookiesAsync("", "");
                // ChromiumWebBrowser 설정
                chromiumWebBrowser.MenuHandler = new CefSharpContextMenu();
                chromiumWebBrowser.LifeSpanHandler = new MyCustomLifeSpanHandler();
                // URL 설정
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                SONG_ADD_URL = string.Format("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/service/utaite_player_song_add.jsp?auth={0}", registryManager.getAuthToken());
                // URL 로딩
                await chromiumWebBrowser.LoadUrlAsync(SONG_ADD_URL);
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 로딩 종료 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            try
            {
                // UI 상태 변경
                Application.Current.Dispatcher.Invoke(() =>
                {
                    loadingProgressbar.Visibility = Visibility.Hidden;
                    chromiumWebBrowser.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
