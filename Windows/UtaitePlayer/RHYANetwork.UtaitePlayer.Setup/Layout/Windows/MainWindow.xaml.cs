using Ionic.Zip;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RHYANetwork.UtaitePlayer.Setup.Layout.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // 압축 해제 데이터 변수
        // -------------------------------------------------------------------------- //
        private int UnZIP_MaximumValue = 0;
        // -------------------------------------------------------------------------- //

        // 프로그램 제거 데이터 변수
        // -------------------------------------------------------------------------- //
        private double Remove_FileCount = 0f;
        // -------------------------------------------------------------------------- //




        /// <summary>
        /// 생성자
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }



        /// <summary>
        /// 최소화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }



        /// <summary>
        /// Window 로딩 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 시작 위치 설정
                this.Left = (SystemParameters.WorkArea.Width) / 2 + SystemParameters.WorkArea.Left - Width / 2;
                this.Top = (SystemParameters.WorkArea.Height) / 2 + SystemParameters.WorkArea.Left - Height / 2;

                // 가장 위에 올리기 설정
                this.Topmost = true;
                // 2.5 초 대기
                await Task.Run(() => Thread.Sleep(2500));

                // Root 폴더 경로
                string INSTALL_PATH = System.IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "RHYANetwork.UtaitePlayer");
                // 설치 파일 경로 리스트
                string OS_STARTUP_PATH = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "UtaitePlayer");
                string UP_DRIVER_VCREDIST_SETUP_PATH = System.IO.Path.Combine(INSTALL_PATH, "up_windows_driver_vcredist.exe");
                string UP_INSTALL_ZIP_FILE_PATH = System.IO.Path.Combine(INSTALL_PATH, "up_install_zip.zip");
                string UP_EXEC_MAIN_FILE_NAME = "UtaitePlayer.exe";
                string UP_EXEC_MAIN_FILE_PATH = System.IO.Path.Combine(INSTALL_PATH, UP_EXEC_MAIN_FILE_NAME);
                string UP_EXEC_PROCESSKILLER_PATH = System.IO.Path.Combine(INSTALL_PATH, "RHYANetwork.UtaitePlayer.ProcessKILLER.exe");
                // 기타 정보
                string RHYANETWORK_WEB_URL = "https://rhya-network.kro.kr";
                string RHYANETWORK_PUBLISHER = "RHYA.Network Service";

                if (new System.IO.FileInfo(UP_EXEC_MAIN_FILE_PATH).Exists) // 제거 관리자
                {
                    SetupProgressBar_Message.Text = "Removeing utaite player...";

                    RunProcess(UP_EXEC_PROCESSKILLER_PATH, INSTALL_PATH, true);

                    // 파일 제거
                    Remove_FileCount = 0;
                    SetupProgressBar.Value = 0;
                    await Task.Run(() => 
                    {
                        try
                        {
                            DeleteInstallFileCount(INSTALL_PATH);
                            Application.Current.Dispatcher.Invoke(() => SetupProgressBar.Maximum = Remove_FileCount);
                            DeleteInstallFile(INSTALL_PATH);
                        }
                        catch (Exception) { }
                    });

                    await Task.Run(() =>
                    {
                        try
                        {
                            // 레지스트리 제거
                            RegistryKey ProudctDirRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths", true);
                            if (ProudctDirRegistry.GetSubKeyNames().Contains(UP_EXEC_MAIN_FILE_NAME))
                                ProudctDirRegistry.DeleteSubKeyTree(UP_EXEC_MAIN_FILE_NAME);
                            RegistryKey ProudctDelRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true);
                            if (ProudctDelRegistry.GetSubKeyNames().Contains(UP_EXEC_MAIN_FILE_NAME))
                                ProudctDelRegistry.DeleteSubKeyTree(UP_EXEC_MAIN_FILE_NAME);

                            // 바로가기 제거
                            System.IO.File.Delete(string.Format("{0}\\Utaite Player.lnk", Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory)));
                            System.IO.File.Delete(string.Format("{0}\\Utaite Player.lnk", OS_STARTUP_PATH));
                        }
                        catch (Exception) { }
                    });
                }
                else // 설치 관리자
                {
                    JObject programInfo = JObject.Parse(getProgramInfo());

                    // 서비스 접근 여부 확인
                    // ---------------------------------------- //
                    // ---------------------------------------- //
                    if (programInfo.ContainsKey("result") && programInfo["result"].ToString().Equals("service_access_block"))
                    {
                        MessageBox.Show("지금은 우타이테 플레이어 (Utaite Player) 서비스를 이용할 수 없습니다. RHYA.Network 홈페이지의 공지사항을 확인하거나 이러한 문제가 지속된다면 관리자께 문의하십시오.", "서비스 접근 거부됨", MessageBoxButton.OK, MessageBoxImage.Information);
                        // 프로그램 종료
                        Environment.Exit(0);
                    }
                    // ---------------------------------------- //
                    // ---------------------------------------- //

                    // 버전 정보 확인
                    // ---------------------------------------- //
                    // ---------------------------------------- //
                    Version nowVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    var newVersionGet = programInfo["version_for_windows"];
                    // ---------------------------------------- //
                    // ---------------------------------------- //

                    // Root 폴더 생성
                    if (!new System.IO.DirectoryInfo(INSTALL_PATH).Exists)
                        System.IO.Directory.CreateDirectory(INSTALL_PATH);

                    // Startup 폴더 생성
                    if (!new System.IO.DirectoryInfo(OS_STARTUP_PATH).Exists)
                        System.IO.Directory.CreateDirectory(OS_STARTUP_PATH);

                    // 설치 관리자 복사
                    string TEMP_PROGRAM_COPY_PATH = System.IO.Path.Combine(INSTALL_PATH, "RHYANetwork.UtaitePlayer.Setup.exe");
                    string TEMP_PROGRAM_PATH = Assembly.GetEntryAssembly().Location;
                    await Task.Run(() =>
                    {
                        try
                        {
                            System.IO.File.Copy(TEMP_PROGRAM_PATH, TEMP_PROGRAM_COPY_PATH, true);
                        }
                        catch (Exception) {}
                    });

                    // 드라이버 다운로드
                    // --------------------------------------------------
                    // --------------------------------------------------
                    SetupProgressBar_Message.Text = "Download driver...";
                    using (WebClient webClient = new WebClient())
                    {
                        // Progressbar 이벤트 등록
                        webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

                        // x64, x86 구분 및 다운로드
                        if (Environment.Is64BitOperatingSystem)
                            await webClient.DownloadFileTaskAsync(new Uri("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=23&bit=x64"),
                                UP_DRIVER_VCREDIST_SETUP_PATH);
                        else
                            await webClient.DownloadFileTaskAsync(new Uri("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=23&bit=x86"),
                                UP_DRIVER_VCREDIST_SETUP_PATH);
                    }
                    // --------------------------------------------------
                    // --------------------------------------------------


                    // 가장 위에 올리기 취소
                    this.Topmost = false;


                    // 드라이버 설치
                    // --------------------------------------------------
                    // --------------------------------------------------
                    RunProcess(UP_DRIVER_VCREDIST_SETUP_PATH, INSTALL_PATH, true);
                    // --------------------------------------------------
                    // --------------------------------------------------


                    // 가장 위에 올리기 설정
                    this.Topmost = true;


                    // ZIP 파일 다운로드
                    // --------------------------------------------------
                    // --------------------------------------------------
                    SetupProgressBar_Message.Text = "Download utaite player zip file...";
                    using (WebClient webClient = new WebClient())
                    {
                        // Progressbar 이벤트 등록
                        webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

                        // ZIP 파일 다운로드
                        await webClient.DownloadFileTaskAsync(new Uri("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=24"),
                            UP_INSTALL_ZIP_FILE_PATH);
                    }
                    // --------------------------------------------------
                    // --------------------------------------------------


                    // Progressbar 초기화
                    SetupProgressBar.Value = 0;


                    // ZIP 파일 압축 해제
                    // --------------------------------------------------
                    // --------------------------------------------------
                    await Task.Run(() =>
                    {
                        try
                        {
                            using (ZipFile zip = ZipFile.Read(UP_INSTALL_ZIP_FILE_PATH))
                            {
                                FileInfo fi = new FileInfo(UP_INSTALL_ZIP_FILE_PATH);
                                zip.ExtractProgress += Extract_Progress;
                                UnZIP_MaximumValue = zip.Entries.Count;

                                for (int i = 0; i < zip.Entries.Count; i++)
                                {
                                    try
                                    {
                                        ZipEntry entry = zip[i];
                                        byte[] byteIbm437 = Encoding.GetEncoding("IBM437").GetBytes(zip[i].FileName);
                                        string euckrFileName = Encoding.GetEncoding("utf-8").GetString(byteIbm437);
                                        entry.FileName = euckrFileName;
                                        entry.Extract(INSTALL_PATH, ExtractExistingFileAction.OverwriteSilently);
                                    }
                                    catch (Exception) { }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // 예외 처리
                            ShowMessageBox(ex);
                        }
                    });
                    // --------------------------------------------------
                    // --------------------------------------------------


                    // Progressbar 설정
                    SetupProgressBar.Maximum = 100;


                    // 바로가기 만들기
                    // --------------------------------------------------
                    // --------------------------------------------------
                    SetupProgressBar_Message.Text = "Create shortcut...";
                    SetupProgressBar.Value = 0;
                    await Task.Run(() =>
                    {
                        try
                        {
                            // 바탕화면
                            LinkFileCreate(Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory), UP_EXEC_MAIN_FILE_PATH, INSTALL_PATH);
                            // 프로그램 등록
                            LinkFileCreate(OS_STARTUP_PATH, UP_EXEC_MAIN_FILE_PATH, INSTALL_PATH);
                        }
                        catch (Exception) { }
                    });
                    SetupProgressBar.Value = 100;
                    // --------------------------------------------------
                    // --------------------------------------------------


                    // 레지스트리 작업 진행
                    // --------------------------------------------------
                    // --------------------------------------------------
                    SetupProgressBar_Message.Text = "Create registry...";
                    SetupProgressBar.Value = 0;
                    await Task.Run(() =>
                    {
                        try
                        {
                            RegistryKey ProudctDirRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths", true).CreateSubKey(UP_EXEC_MAIN_FILE_NAME, true);
                            ProudctDirRegistry.SetValue("", UP_EXEC_MAIN_FILE_PATH);
                            RegistryKey ProudctDelRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true).CreateSubKey(UP_EXEC_MAIN_FILE_NAME, true);
                            ProudctDelRegistry.SetValue("", UP_EXEC_MAIN_FILE_PATH);
                            ProudctDelRegistry.SetValue("DisplayName", "우타이테 플레이어 (Utaite Player)");
                            ProudctDelRegistry.SetValue("UninstallString", System.IO.Path.Combine(INSTALL_PATH, System.IO.Path.GetFileName(Assembly.GetEntryAssembly().Location)));
                            ProudctDelRegistry.SetValue("DisplayIcon", UP_EXEC_MAIN_FILE_PATH);
                            ProudctDelRegistry.SetValue("DisplayVersion", newVersionGet);
                            ProudctDelRegistry.SetValue("URLInfoAbout", RHYANETWORK_WEB_URL);
                            ProudctDelRegistry.SetValue("Publisher", RHYANETWORK_PUBLISHER);
                        }
                        catch (Exception) { }
                    });
                    SetupProgressBar.Value = 100;
                    // --------------------------------------------------
                    // --------------------------------------------------


                    // 프로그램 실행 및 종료
                    SetupProgressBar_Message.Text = "Start utaite player...";
                    // 0.5 초 대기
                    await Task.Run(() => Thread.Sleep(500));
                    RunProcess(UP_EXEC_MAIN_FILE_PATH, INSTALL_PATH, false);
                }

                // 프로그램 종료
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex);
                Environment.Exit(0);
            }
        }



        /// <summary>
        /// Web Client 다운로드 Progressbar 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());

                double percentage = bytesIn / totalBytes * 100;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        SetupProgressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
                    }
                    catch (Exception) { }
                });
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 예외 메시지 박스 출력
        /// </summary>
        /// <param name="exception"></param>
        private void ShowMessageBox(Exception exception)
        {
            // 가장 위에 올리기 취소
            this.Topmost = false;

            const string DEFAULT_TITLE = "RHYA.Network ExceptionManager";
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("우타이테 플레이어 (Utaite Player) 클라이언트 설치 프로그램에서 예외가 발생했습니다. 자세한 정보는 다음 내용을 참조하십시오.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(exception.Message);

            System.Windows.MessageBox.Show(stringBuilder.ToString(), DEFAULT_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /// <summary>
        /// 외부 프로세스 실행
        /// </summary>
        /// <param name="fileName">파일 경로</param>
        /// <param name="workDir">작업 경로</param>
        /// <param name="isWait">대기 여부</param>
        /// <returns></returns>
        private int RunProcess(string fileName, string workDir, bool isWait)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = fileName;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.WorkingDirectory = workDir;

                p.Start();

                if (isWait)
                {
                    p.WaitForExit();
                    return p.ExitCode;
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 압축 해제 Progressbar event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Extract_Progress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetupProgressBar_Message.Text = string.Format("Decompressing...   ({0})", e.CurrentEntry.FileName);

                    SetupProgressBar.Maximum = UnZIP_MaximumValue;

                    double value = 0;
                    checked { value = SetupProgressBar.Value + 1; }
                    SetupProgressBar.Value = value;
                });
            }
        }



        /// <summary>
        /// 바로가기 파일 생성 함수
        /// </summary>
        /// <param name="targetDir">생성 경로</param>
        /// <param name="targetExec">원본 파일 경로</param>
        /// <param name="workDir">작업 경로</param>
        private void LinkFileCreate(string targetDir, string targetExec, string workDir)
        {
            try
            {
                DirectoryInfo DirInfo = new DirectoryInfo(targetDir);
                if (!DirInfo.Exists)
                    Directory.CreateDirectory(targetDir);

                string LinkFileName = string.Format("{0}\\Utaite Player.lnk", targetDir);
                FileInfo LinkFile = new FileInfo(LinkFileName);
                if (LinkFile.Exists)
                    LinkFile.Delete();

                // 바로가기 생성
                WshShell wsh = new WshShell();
                IWshShortcut Link = wsh.CreateShortcut(LinkFile.FullName);

                // 원본 파일의 경로 
                Link.TargetPath = targetExec;
                Link.WorkingDirectory = workDir;
                Link.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 프로그램 제거
        /// </summary>
        /// <param name="path">설치 부모 경로</param>
        public void DeleteInstallFile(string path)
        {
            try
            {
                foreach (string Folder in Directory.GetDirectories(path))
                    DeleteInstallFile(Folder); //재귀함수 호출

                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() => SetupProgressBar.Value = SetupProgressBar.Value + 1);

                        FileInfo fi = new FileInfo(file);
                        fi.Delete();
                    }
                    catch (Exception) { }
                }

                Directory.Delete(path, true);
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 프로그램 제거 정보 가져오기
        /// </summary>
        /// <param name="path">설치 부모 경로</param>
        public void DeleteInstallFileCount(string path)
        {
            try
            {
                foreach (string Folder in Directory.GetDirectories(path))
                    DeleteInstallFileCount(Folder); //재귀함수 호출

                foreach (string file in Directory.GetFiles(path))
                    Remove_FileCount = Remove_FileCount + 1;
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 프로그램 정보 반환
        /// </summary>
        /// <returns>프로그램 정보 JSON</returns>
        public string getProgramInfo()
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=6");
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
