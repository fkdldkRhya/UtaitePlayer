using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RHYANetwork.UtaitePlayer.ExceptionHandler
{
    public class ExceptionManager
    {
        // Instance
        private static ExceptionManager exceptionManager = null;
        // Mutex
        private Mutex mutex;
        // Mutex name
        private string mutexName = string.Empty;



        /// <summary>
        /// Get instance 
        /// </summary>
        /// <returns></returns>
        public static ExceptionManager getInstance()
        {
            if (exceptionManager == null)
                exceptionManager = new ExceptionManager();

            return exceptionManager;
        }



        /// <summary>
        /// Exception message box 출력
        /// 
        /// ** Utaite Player에 관한 오류는 ExceptionManagerV2로 대신 처리
        /// </summary>
        /// <param name="exception">Exception</param>
        public void showMessageBox(Exception exception)
        {
            const string DEFAULT_TITLE = "RHYA.Network ExceptionManager";
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("우타이테 플레이어 (Utaite Player) 클라이언트에서 예외가 발생했습니다. 자세한 정보는 다음 내용을 참조하십시오.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(exception.ToString());

            MutexManager.MutexList mutexManager = new MutexManager.MutexList();

            if (mutexName != string.Empty && mutexName.Equals(mutexManager.GetMutexName(MutexManager.MutexList.ServiceList.ROOT_SERVICE_UTAITE_PLAYER)))
            {   // UtaitePlayer.exe 에서 발생한 오류

            }
            else
            {   // 기타 오류
                System.Windows.MessageBox.Show(stringBuilder.ToString(), DEFAULT_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Exception message box 출력
        /// </summary>
        /// <param name="exception">Exception for string</param>
        public void showMessageBox(string exception)
        {
            const string DEFAULT_TITLE = "RHYA.Network ExceptionManager";
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("우타이테 플레이어 (Utaite Player) 클라이언트에서 예외가 발생했습니다. 자세한 정보는 다음 내용을 참조하십시오.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(exception);


            System.Windows.MessageBox.Show(stringBuilder.ToString(), DEFAULT_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /// <summary>
        /// Exception message box 출력
        /// </summary>
        public void showMessageBoxForUnhandledException()
        {
            const string DEFAULT_TITLE = "RHYA.Network ExceptionManager";
            System.Windows.MessageBox.Show("우타이테 플레이어 (Utaite Player) 클라이언트에서 처리되지 않은 알 수 없는 예외가 발생했습니다. 프로그램을 재시작해주십시오. 만약 이러한 오류가 지속해서 발생할 경우 관리자에게 문의하십시오", DEFAULT_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /// <summary>
        /// 프로그램 종료
        /// </summary>
        public async void exitProgram()
        {
            await Task.Run(() => 
            {
                try
                {
                    mutex.Dispose();
                    mutex.Close();

                    // 파일 생성
                    createNoErrorFile();
                    // 프로세스 종료
                    RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor authCheckManagerProcessor = new RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor();
                    authCheckManagerProcessor.killProcess();
                }
                catch (Exception ex)
                {
                    showMessageBox(ex);
                }
            });
            
            // 프로그램 종료
            Environment.Exit(0);
        }



        /// <summary>
        /// '오류 없음' 파일 생성
        /// </summary>
        /// <returns></returns>
        public void createNoErrorFile()
        {
            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
            System.IO.File.WriteAllText(getNoErrorFilePath(), registryManager.getRootProgramPID().ToString());
        }



        /// <summary>
        /// '오류 없음' 파일 경로 구하기
        /// </summary>
        /// <returns></returns>
        public string getNoErrorFilePath()
        {
            return System.IO.Path.Combine(new Registry.RegistryManager().getInstallPath().ToString(), "RHYANetwork.NoError.Exited");
        }



        /// <summary>
        /// Mutex 설정 확인
        /// </summary>
        /// <param name="name">Mutex 이름</param>
        public void taskMutex(string name)
        {
            // Mutex 확인
            bool createNew = false;

            mutex = new Mutex(true, name, out createNew);

            mutexName = name;

            if (!createNew)
                // 종료
                exitProgram();
        }
    }
}
