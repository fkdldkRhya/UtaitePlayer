using CefSharp;
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
using System.Windows.Shapes;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        // 로그인 URL
        private readonly string LOGIN_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/sign_in.jsp?rpid=10&ctoken=1";
        // CallBack URL
        private readonly string CALLBACK_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/callback.jsp";
        // 로그인 성공 여부 변수
        private bool mIsSuccessUser = false;
        public bool isSuccessUser
        {
            get { return mIsSuccessUser; }
        }




        public LoginWindow()
        {
            InitializeComponent();
        }



        /// <summary>
        /// Windows loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 시작 위치 설정
            this.Left = (SystemParameters.WorkArea.Width) / 2 + SystemParameters.WorkArea.Left - Width / 2;
            this.Top = (SystemParameters.WorkArea.Height) / 2 + SystemParameters.WorkArea.Left - Height / 2;

            // 변수 초기화
            mIsSuccessUser = false;
        }



        /// <summary>
        /// Main task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // 변수 초기화
            mIsSuccessUser = false;

            // 캐시 제거
            await Cef.GetGlobalCookieManager().DeleteCookiesAsync("", "");
            // ChromiumWebBrowser 설정
            chromiumWebBrowser.MenuHandler = new CefSharpContextMenu();
            chromiumWebBrowser.LifeSpanHandler = new MyCustomLifeSpanHandler();
            // URL 로딩
            await chromiumWebBrowser.LoadUrlAsync(LOGIN_URL);
        }



        /// <summary>
        /// 브라우저 로딩 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chromiumWebBrowser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            try
            {
                // UI 상태 변경
                Application.Current.Dispatcher.Invoke(() =>
                {
                    taskProgressBar.IsIndeterminate = true;
                    taskProgressBar.Visibility = Visibility.Visible;
                    taskLabel.Visibility = Visibility.Visible;
                    chromiumWebBrowser.Visibility = Visibility.Hidden;
                });

                // URL 이동 가능 여부 확인
                if (e.Url.Contains("https://policies.google.com/"))
                {
                    chromiumWebBrowser.Stop();
                    chromiumWebBrowser.LoadUrl(LOGIN_URL);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
                // 프로그램 종료
                ExceptionManager.getInstance().exitProgram();
            }
        }



        /// <summary>
        /// 브라우저 로딩 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void chromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            try
            {
                // UI 상태 변경
                Application.Current.Dispatcher.Invoke(() =>
                {
                    taskProgressBar.Visibility = Visibility.Hidden;
                    chromiumWebBrowser.Visibility = Visibility.Visible;
                    taskLabel.Visibility = Visibility.Hidden;
                    taskProgressBar.IsIndeterminate = false;
                });

                // 로그인 성공 확인
                if (e.Url != null && e.Url.Equals(CALLBACK_URL))
                {
                    // Cookie 데이터 읽기
                    string userUUID = null;
                    string autoLoginToken = null;
                    var cookies = await Cef.GetGlobalCookieManager().VisitAllCookiesAsync();
                    foreach (Cookie cookie in cookies)
                    {
                        switch (cookie.Name)
                        {
                            case "AutoLogin_UserUUID": // 사용자 UUID
                                userUUID = cookie.Value;
                                break;

                            case "AutoLogin_TokenUUID": // 사용자 자동 로그인 Token
                                autoLoginToken = cookie.Value;
                                break;
                        }
                    }
                    // 쿠키 데이터 확인
                    if (userUUID != null && autoLoginToken != null)
                    {
                        // Auth token 발급
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                        JObject jObject = JObject.Parse(utaitePlayerClient.authTokenCreate(userUUID, autoLoginToken));
                        
                        // Json 분석
                        if (jObject.ContainsKey("result") && jObject.ContainsKey("message"))
                        {
                            // 발급 결과 확인
                            if (jObject["result"].ToString().Equals("success"))
                            {
                                // 레지스트리 등록
                                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                                if (registryManager.isSetAuthToken()) registryManager.deleteAuthToken();
                                registryManager.setAuthToken((string) jObject["message"]);

                                // 변수 변경
                                mIsSuccessUser = true;

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    // 창 닫기
                                    this.Close();
                                });

                                // 종료
                                return;
                            }
                        }
                    }

                    // 캐시 제거
                    await Cef.GetGlobalCookieManager().DeleteCookiesAsync("", "");

                    // 로그인 화면 로딩
                    chromiumWebBrowser.LoadUrl(LOGIN_URL);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
                // 프로그램 종료
                ExceptionManager.getInstance().exitProgram();
            }
        }
    }
}
