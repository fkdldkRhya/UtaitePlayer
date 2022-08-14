using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.ProcessManager
{
    public class AuthCheckManagerProcessor
    {
        // 인자 정보
        public readonly string ARG_NAME_SERVER_ADDRESS = "-server=";
        public readonly string ARG_NAME_MAIN_PROCESS_NAME = "-process_name=";
        // 프로세스 정보
        public readonly string AUTH_CHECK_MANAGER_PROCESS_NAME = "RHYANetwork.UtaitePlayer.AuthCheckManager";
        public readonly string AUTH_CHECK_MANAGER_FILE_NAME = "RHYANetwork.UtaitePlayer.AuthCheckManager.exe";




        /// <summary>
        /// 프로세스 실행 여부 확인
        /// </summary>
        /// <returns>프로세스 실행 여부</returns>
        public bool processStartCheck()
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                Process[] processes = Process.GetProcessesByName(AUTH_CHECK_MANAGER_PROCESS_NAME);
                if (processes.Length > 0)
                {
                    int pid = registryManager.getAuthCheckManagerPID();
                    foreach (Process process in processes)
                        if (process.Id == pid)
                            return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 프로세스 실행
        /// </summary>
        public void startProcess()
        {
            try
            {
                IPCServerInfoManager iPCServerInfoManager = new IPCServerInfoManager();

                // 서버 정보 조합
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(ARG_NAME_SERVER_ADDRESS);
                stringBuilder.Append("ipc://");
                stringBuilder.Append(iPCServerInfoManager.IPC_SERVER_CHANNEL_NAME);
                stringBuilder.Append("/");
                stringBuilder.Append(iPCServerInfoManager.IPC_SERVER_NAME);
                stringBuilder.Append(" ");
                stringBuilder.Append(ARG_NAME_MAIN_PROCESS_NAME);
                stringBuilder.Append(iPCServerInfoManager.ROOT_PROCESS_NAME);

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WorkingDirectory = new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getInstallPath().ToString();
                psi.UseShellExecute = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = AUTH_CHECK_MANAGER_FILE_NAME;
                psi.Arguments = stringBuilder.ToString();

                Process rootProcess = new Process();
                rootProcess = Process.Start(psi);
                rootProcess.WaitForExit();
                rootProcess.Close();
                rootProcess.Dispose();
                rootProcess = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 프로세스 종료
        /// </summary>
        public void killProcess()
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                Process[] processes = Process.GetProcessesByName(AUTH_CHECK_MANAGER_PROCESS_NAME);
                if (processes.Length > 0)
                {
                    int pid = registryManager.getAuthCheckManagerPID();
                    foreach (Process process in processes)
                        if (process.Id == pid)
                            process.Kill();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
