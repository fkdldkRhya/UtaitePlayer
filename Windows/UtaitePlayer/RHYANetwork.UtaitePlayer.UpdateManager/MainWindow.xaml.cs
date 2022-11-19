using System;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Ionic.Zip;
using System.Diagnostics;
using System.Threading;

namespace RHYANetwork.UtaitePlayer.UpdateManager
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // 창 닫기 버튼 제거
        // -------------------------------------------------------------------------- //
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        // -------------------------------------------------------------------------- //

        // 업데이트 데이터 변수
        // -------------------------------------------------------------------------- //
        private string TargetPath = null;
        private int MaximumValue = 0;
        // -------------------------------------------------------------------------- //




        public MainWindow()
        {
            InitializeComponent();
        }



        /// <summary>
        /// Window Activityed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            // 창 닫기 버튼 제거
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
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
        }



        /// <summary>
        /// Main task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 업데이트 명령 확인
                string fileName = "update.upuc";
                string argKeyName1 = "-key=";
                string inputKey = null;
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length >= 1)
                {
                    foreach (string arg in args)
                    {
                        if (arg.Contains(argKeyName1))
                            inputKey = arg.Replace(argKeyName1, "");
                    }


                    // Target Path 설정
                    TargetPath = (string)new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getInstallPath();

                    string upcPath = Path.Combine(TargetPath, "update", fileName);

                    if (TargetPath != null && TargetPath != null && new System.IO.DirectoryInfo(TargetPath).Exists && new System.IO.FileInfo(upcPath).Exists && System.IO.File.ReadAllText(upcPath).Equals(inputKey))
                    {
                        // 파일 제거
                        System.IO.File.Delete(upcPath);

                        // UI 변경
                        taskLog.Visibility = Visibility.Visible;
                        taskProgressBar.Visibility = Visibility.Visible;

                        // 업데이트 작업 진행
                        updateTask();

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                showMessageBox(ex);
            }

            Environment.Exit(0);
        }



        /// <summary>
        /// 업데이트 작업 진행
        /// </summary>
        private async void updateTask()
        {
            try
            {
                taskLog.Text = "업데이트 준비 중...";
                taskProgressBar.IsIndeterminate = true;
                // 2.5초 대기
                await Task.Run(() => Thread.Sleep(2500));

                // 파일 이름
                const string FILE_NAME = "update.zip";

                // 파일 다운로드
                taskLog.Text = "업데이트 파일 다운로드 중...";
                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new Client.UtaitePlayerClient();
                await Task.Run(() => utaitePlayerClient.updateFileDownload(System.IO.Path.Combine(TargetPath, FILE_NAME)));
                taskProgressBar.IsIndeterminate = false;

                // 파일 압축 해제
                taskLog.Text = "업데이트 파일 압축 해제 중...";
                await Task.Run(() =>
                {
                    try
                    {
                        using (ZipFile zip = ZipFile.Read(System.IO.Path.Combine(TargetPath, FILE_NAME)))
                        {
                            FileInfo fi = new FileInfo(System.IO.Path.Combine(TargetPath, FILE_NAME));
                            zip.ExtractProgress += Extract_Progress;
                            MaximumValue = zip.Entries.Count;

                            for (int i = 0; i < zip.Entries.Count; i++)
                            {
                                try
                                {
                                    ZipEntry entry = zip[i];
                                    byte[] byteIbm437 = Encoding.GetEncoding("IBM437").GetBytes(zip[i].FileName);
                                    string euckrFileName = Encoding.GetEncoding("utf-8").GetString(byteIbm437);
                                    entry.FileName = euckrFileName;
                                    entry.Extract(TargetPath, ExtractExistingFileAction.OverwriteSilently);
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        showMessageBox(ex);
                    }
                });

                // 작업 마무리
                taskLog.Text = "파일 제거 중...";
                await Task.Run(() => File.Delete(System.IO.Path.Combine(TargetPath, FILE_NAME)));

                // 작업 마무리
                taskLog.Text = "Starting UtaitePlayer service...";

                // 2.5초 대기
                await Task.Run(() => Thread.Sleep(2500));

                new Thread(() =>
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.WorkingDirectory = TargetPath;
                        process.StartInfo.FileName = System.IO.Path.Combine(TargetPath, "UtaitePlayer.exe");
                        process.StartInfo.UseShellExecute = false;

                        process.Start();

                        // 프로그램 종료
                        Environment.Exit(0);
                    }
                    catch (Exception) {}
                }).Start();
            }
            catch (Exception ex)
            {
                // 예외 처리
                showMessageBox(ex);
            }
        }



        /// <summary>
        /// Progressbar event 등록
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Extract_Progress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    taskLog.Text = e.CurrentEntry.FileName;

                    taskProgressBar.Maximum = MaximumValue;

                    double value = 0;
                    checked { value = taskProgressBar.Value + 1; }
                    taskProgressBar.Value = value;
                });
            }
        }



        /// <summary>
        /// Exception message box 출력
        /// </summary>
        /// <param name="exception">Exception</param>
        public void showMessageBox(Exception exception)
        {
            const string DEFAULT_TITLE = "RHYA.Network ExceptionManager";
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("우타이테 플레이어 (Utaite Player) 클라이언트에서 예외가 발생했습니다. 자세한 정보는 다음 내용을 참조하십시오.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(exception.Message);

            System.Windows.MessageBox.Show(stringBuilder.ToString(), DEFAULT_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
