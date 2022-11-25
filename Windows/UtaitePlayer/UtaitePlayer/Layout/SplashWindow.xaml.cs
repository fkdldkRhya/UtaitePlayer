using HandyControl.Tools;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtaitePlayer.Classes.Utils;
using UtaitePlayer.Layout;

namespace UtaitePlayer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplashWindow : Window
    {
        // 프로그램 종료 확인 변수
        private bool isExit = false;




        /// <summary>
        /// 생성자
        /// </summary>
        public SplashWindow()
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
            try
            {
                // Mutex 작업 진행
                RHYANetwork.UtaitePlayer.MutexManager.MutexList mutexList = new RHYANetwork.UtaitePlayer.MutexManager.MutexList();
                ExceptionManager.getInstance().taskMutex(mutexList.GetMutexName(RHYANetwork.UtaitePlayer.MutexManager.MutexList.ServiceList.ROOT_SERVICE_UTAITE_PLAYER)); 

                // 라이브러리 선언
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                // PID 설정
                registryManager.setRootProgramPID(Process.GetCurrentProcess().Id);

                // 시작 위치 설정
                this.Left = (SystemParameters.WorkArea.Width) / 2 + SystemParameters.WorkArea.Left - Width / 2;
                this.Top = (SystemParameters.WorkArea.Height) / 2 + SystemParameters.WorkArea.Left - Height / 2;
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
        /// Main task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 라이브러리 초기 작업
                ConfigHelper.Instance.SetLang("ko-kr");

                // 1.0초 대기
                await Task.Run(() => Thread.Sleep(1000));

                // 프로그램 정보 확인
                await Task.Run(() => checkProgramInfo());
                // 로그인 확인
                await Task.Run(() => loginCheck());
                // 데이터 불러오기
                await Task.Run(() => loadServiceData());
                // IPC 서버 시작
                // ===========================================
                RHYANetwork.UtaitePlayer.ProcessManager.IPCServerInfoManager iPCServerInfoManager = new RHYANetwork.UtaitePlayer.ProcessManager.IPCServerInfoManager();
                RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor authCheckManagerProcessor = new RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor();

                IpcServerChannel chan = new IpcServerChannel(iPCServerInfoManager.IPC_SERVER_CHANNEL_NAME);
                //register channel
                ChannelServices.RegisterChannel(chan, true);
                //register remote object
                RemotingConfiguration.RegisterWellKnownServiceType(
                       typeof(UtaitePlayer.Classes.Utils.IPCRemoteObject),
                       iPCServerInfoManager.IPC_SERVER_NAME,
                       WellKnownObjectMode.SingleCall);
                // ===========================================

                // AuthCheckManager 실행
                // ===========================================
                new Thread(() =>
                {
                    try
                    {
                        // 프로세스 실행 확인
                        if (!authCheckManagerProcessor.processStartCheck())
                        {
                            // 프로세스 실행
                            authCheckManagerProcessor.startProcess();
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                }).Start();
                // ===========================================

                // CrashHandler 실행
                // ===========================================
                new Thread(() =>
                {
                    try
                    {
                        if (!new SettingManager().readSettingData().gs_use_crash_handler) return;

                        Process process = new Process();
                        process.StartInfo.FileName = "RHYANetwork.UtaitePlayer.CrashHandler.exe";
                        process.StartInfo.WorkingDirectory = new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getInstallPath().ToString();
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                }).Start();
                // ===========================================

                // 메인 화면 전환
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // 창 닫기
                this.Close();
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
                ExceptionManager.getInstance().exitProgram();
            }
        }



        /// <summary>
        /// 프로그램 정보 확인
        /// </summary>
        private void checkProgramInfo()
        {
            // 레지스트리 등록 확인
            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
            try
            {
                registryManager.setInstallPath(System.Windows.Forms.Application.StartupPath);
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }

            // Temp 파일 제거 - PrivilegesEXEC File
            // ===========================================
            StartAdministratorProcess startAdministratorProcess = new StartAdministratorProcess();
            // 예외 처리
            System.IO.FileInfo PrivilegesEXECFile = new FileInfo(startAdministratorProcess.getPath());
            // 예외 처리
            if (PrivilegesEXECFile.Exists) PrivilegesEXECFile.Delete();
            // ===========================================

            // JSON Key 이름
            string resultKeyName = "result";
            string programVersionKeyName = "version_for_windows";

            try
            {
                // 프로그램 정보 확인
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                JObject programInfo = JObject.Parse(utaitePlayerClient.getProgramInfo());


                // 서비스 접근 여부 확인
                // ---------------------------------------- //
                if (programInfo.ContainsKey(resultKeyName) && programInfo[resultKeyName].ToString().Equals("service_access_block"))
                {
                    MessageBox.Show("지금은 우타이테 플레이어 (Utaite Player) 서비스를 이용할 수 없습니다. RHYA.Network 홈페이지의 공지사항을 확인하거나 이러한 문제가 지속된다면 관리자께 문의하십시오.", "서비스 접근 거부됨", MessageBoxButton.OK, MessageBoxImage.Information);
                    // 프로그램 종료
                    ExceptionManager.getInstance().exitProgram();
                }
                // ---------------------------------------- //


                // 버전 정보 확인
                // ---------------------------------------- //
                Version nowVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var newVersionGet = programInfo[programVersionKeyName];
                if (!programInfo.ContainsKey(programVersionKeyName) || newVersionGet == null || newVersionGet.Type == JTokenType.Null)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox("JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                    // 프로그램 종료
                    ExceptionManager.getInstance().exitProgram();
                }
                Version newVersion = new Version(newVersionGet.ToString());
                int compareResult = newVersion.CompareTo((object)nowVersion);

                if (compareResult > 0)
                {
                    // 메시지 출력
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("New version: ");
                    stringBuilder.AppendLine(newVersionGet.ToString());
                    stringBuilder.Append("Current version: ");
                    stringBuilder.AppendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    stringBuilder.AppendLine("");
                    stringBuilder.AppendLine("우타이테 플레이어 (Utaite Player) 서비스를 업데이트 해야 합니다. 업데이트를 진행하지 않으면 서비스를 이용할 수 없습니다. 확인 버튼을 누르면 업데이트가 자동으로 진행됩니다. 만약 업데이트가 진행되지 않거나 업데이트 과정에서 계속 문제가 발생하는 경우 수동으로 최신버전으로 업그레이드하거나 관리자에게 문의하십시오.");
                    if (MessageBox.Show(stringBuilder.ToString(), "업데이트 필요", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        string targetPath = null;
                        targetPath = (string)registryManager.getInstallPath();
                        if (targetPath != null && targetPath != null && new System.IO.DirectoryInfo(targetPath).Exists)
                        {
                            Guid guid = Guid.NewGuid();

                            // 파일 생성
                            System.IO.File.WriteAllText(System.IO.Path.Combine(targetPath, "update", "update.upuc"), guid.ToString());

                            new Thread(() =>
                            {
                                try
                                {
                                    // AuthCheckManager 종료
                                    try
                                    {
                                        RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor authCheckManagerProcessor = new RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor();
                                        authCheckManagerProcessor.killProcess();
                                    }
                                    catch (Exception ex) 
                                    {
                                        // 예외 처리
                                        ExceptionManager.getInstance().showMessageBox(ex);
                                    }
                                    // 업데이트 필요
                                    Process process = new Process();
                                    process.StartInfo.FileName = System.IO.Path.Combine(targetPath, "update", "RHYANetwork.UtaitePlayer.UpdateManager.exe");
                                    process.StartInfo.WorkingDirectory = targetPath;
                                    process.StartInfo.UseShellExecute = false;
                                    process.StartInfo.Arguments = string.Format("-key={0}", guid.ToString());
                                    process.Start();

                                    // 프로그램 종료
                                    ExceptionManager.getInstance().exitProgram();
                                }
                                catch (Exception) { }
                            }).Start();
                        }
                        else
                        {
                            // 프로그램 종료
                            ExceptionManager.getInstance().exitProgram();
                        }
                    }
                    else
                    {
                        // 프로그램 종료
                        ExceptionManager.getInstance().exitProgram();
                    }
                }
                // ---------------------------------------- //
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
        /// 사용자 로그인 확인
        /// </summary>
        private void loginCheck()
        {
            // 레지스트리 등록 확인
            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
            try
            {
                // 다음 작업 진행 여부
                bool isNextTask = false;

                // Auth Token 확인
                if (registryManager.isSetAuthToken())
                {
                    // Auth Token 읽기
                    string authToken = (string)registryManager.getAuthToken();
                    if (authToken != null)
                    {
                        // Auth Token 유효성 검사
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                        // Json 분석
                        string jsonValue = utaitePlayerClient.authTokenVerify(authToken);
                        if (jsonValue != null)
                        {
                            JObject jObject = JObject.Parse(jsonValue);
                            string resultKeyName = "result";
                            if (jObject.ContainsKey(resultKeyName))
                            {
                                string result = (string)jObject[resultKeyName];
                                if (result.Equals("success"))
                                {
                                    isNextTask = true;
                                }
                            }
                        }
                    }
                }

                // 다음 작업 진행 확인
                if (!isNextTask)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 로그인 화면 전환
                        Layout.LoginWindow loginWindow = new Layout.LoginWindow();
                        loginWindow.ShowDialog();

                        // 로그인 성공 확인
                        if (!loginWindow.isSuccessUser)
                        {
                            ExceptionManager.getInstance().showMessageBox("계정 인증 실패! 로그인을 진행하지 않으면 이용하실 수 없습니다.");
                            // 프로그램 종료
                            ExceptionManager.getInstance().exitProgram();
                        }
                    });

                    // 우타이테 플레이어 사용가능 여부 확인
                    RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                    string authToken = (string)registryManager.getAuthToken();
                    string jsonValue = utaitePlayerClient.getIsAccessUtaiteService(authToken);
                    JObject jObject = JObject.Parse(jsonValue);
                    if (!(jObject.ContainsKey("result") && ((string)jObject["result"]).Equals("success")))
                    {
                        utaitePlayerClient.getUserMoreData((string)registryManager.getAuthToken());
                        if (registryManager.isSetAuthToken())
                            registryManager.deleteAuthToken();

                        ExceptionManager.getInstance().showMessageBox("계정 인증 실패! 해당 계정으로는 우타이테 플레이어 서비스를 이용할 수 없습니다.");
                        // 프로그램 종료
                        ExceptionManager.getInstance().exitProgram();
                    }
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
        /// 프로그램 종료 확인
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            // 종료 확인
            if (isExit)
            {
                // 프로그램 완전 종료
                ExceptionManager.getInstance().exitProgram();
            }
        }



        /// <summary>
        /// 데이터 로딩
        /// </summary>
        private void loadServiceData()
        {
            try
            {
                // 데이터 암호화 키 확인
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                if (!registryManager.isSetCryptoKey())
                {
                    // 데이터 암호화 키 생성
                    RHYANetwork.UtaitePlayer.CryptoModule.KeyGen keyGen = new RHYANetwork.UtaitePlayer.CryptoModule.KeyGen();
                    string randomKey = keyGen.getRandomKey();
                    // 데이터 암호화 키 등록
                    registryManager.setCryptoKey(randomKey);
                }

                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();


                // 노래, 아티스트 데이터 관리
                // ---------------------------------------- //
                // 데이터 파일 확인
                if (!musicDataManager.isExistsResourceFile())
                {
                    // 데이터 파일 생성
                    string serverJsonValue = utaitePlayerClient.getMusicAndSingerAllResources((string)registryManager.getAuthToken());
                    JObject jObject = JObject.Parse(serverJsonValue);
                    // 데이터 1차 검사
                    if (jObject.ContainsKey("result"))
                    {
                        if (((string)jObject["result"]).Equals("fail"))
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox("노래 데이터 로딩 중 오류 발생, JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                            // 프로그램 종료
                            ExceptionManager.getInstance().exitProgram();
                        }
                    }

                    // 데이터 변환
                    musicDataManager.musicAndSingerResourcesParser(serverJsonValue);

                    if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources != null 
                        && RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources != null)
                    {
                        musicDataManager.writeMusicResourcesFile(RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources);
                        musicDataManager.writeSingerResourcesFile(RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources);
                    }
                    else
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox("노래 또는 아티스트 데이터를 불러오는 중에 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                        // 프로그램 종료
                        ExceptionManager.getInstance().exitProgram();
                    }
                }
                else
                {
                    // 데이터 초기화
                    RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().clearMusicResources();
                    RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().clearSingerResources();

                    // 파일 읽기 - 노래 데이터
                    Dictionary<string, RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO> readValueForMusicResources = musicDataManager.readMusicResourcesFile();
                    if (readValueForMusicResources != null)
                        foreach (string key in readValueForMusicResources.Keys)
                            RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().addMusicResources(readValueForMusicResources[key]);
                    // 파일 읽기 - 아티스트 데이터
                    Dictionary<string, RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO> readValueForSingerResources = musicDataManager.readSingerResourcesFile();
                    if (readValueForSingerResources != null)
                        foreach (string key in readValueForSingerResources.Keys)
                            RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().addSingerResources(readValueForSingerResources[key]);

                    // 노래 데이터 버전 확인
                    string versionJsonValue = utaitePlayerClient.getMusicAllVersionResources((string)registryManager.getAuthToken());
                    JObject jObjectForVersionJsonValue = JObject.Parse(versionJsonValue);
                    // 데이터 1차 검사
                    if (jObjectForVersionJsonValue.ContainsKey("result"))
                    {
                        if (((string)jObjectForVersionJsonValue["result"]).Equals("fail"))
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox("노래 버전 데이터 로딩 중 오류 발생, JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                            // 프로그램 종료
                            ExceptionManager.getInstance().exitProgram();
                        }
                    }

                    // 데이터 버전 확인
                    musicDataManager.checkMusicResourceVersion(versionJsonValue);
                }
                // ---------------------------------------- //


                // 사용자 정보 관리
                // ---------------------------------------- //
                string getUserInfoJsonValue = utaitePlayerClient.getUserData((string)registryManager.getAuthToken());
                // 서버 응답 확인
                JObject jObjectForGetUserInfoJsonValue = JObject.Parse(getUserInfoJsonValue);
                if (jObjectForGetUserInfoJsonValue.ContainsKey("result"))
                {
                    if (((string)jObjectForGetUserInfoJsonValue["result"]).Equals("fail"))
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox("사용자 정보 로딩 중 오류 발생, JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                        // 프로그램 종료
                        ExceptionManager.getInstance().exitProgram();
                    }
                }
                // 사용자 데이터 설정
                musicDataManager.userDataResourcesParser(getUserInfoJsonValue);
                // Null 확인
                if (RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.nullChecker())
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox("사용자 정보 로딩 중 오류 발생, 데이터는 Null일 수 없습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                    // 프로그램 종료
                    ExceptionManager.getInstance().exitProgram();
                }
                // ---------------------------------------- //


                // 플레이리스트 관리
                // ---------------------------------------- //
                musicDataManager.readPlaylistResourcesFile();

                try
                {
                    // 플레이리스트 인덱스 읽기
                    if (registryManager.isSetMyPlaylistIndex())
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = registryManager.getMyPlaylistIndex();
                    else
                        RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().myPlaylistIndex = -1;
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    ExceptionManager.getInstance().showMessageBox(ex);
                }
                // ---------------------------------------- //


                // 커스텀 플레이리스트, 구독 관리
                // ---------------------------------------- //
                string getUserMoreInfoJsonValue = utaitePlayerClient.getUserMoreData((string)registryManager.getAuthToken());
                // 서버 응답 확인
                JObject jObjectForGetUserMoreInfoJsonValue = JObject.Parse(getUserMoreInfoJsonValue);
                if (jObjectForGetUserMoreInfoJsonValue.ContainsKey("result"))
                {
                    if (((string)jObjectForGetUserMoreInfoJsonValue["result"]).Equals("fail"))
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox("사용자 커스텀 플레이리스트, 구독 정보 로딩 중 오류 발생, JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");
                        // 프로그램 종료
                        ExceptionManager.getInstance().exitProgram();
                    }
                }
                // 데이터 설정
                musicDataManager.userSubscribeAndPlaylistResourcesParser(getUserMoreInfoJsonValue);
                // ---------------------------------------- //
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
