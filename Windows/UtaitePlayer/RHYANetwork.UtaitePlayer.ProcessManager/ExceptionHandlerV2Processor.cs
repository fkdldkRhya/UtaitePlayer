using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.ProcessManager
{
    public class ExceptionHandlerV2Processor
    {
        // 인자 정보
        public readonly string ARG_NAME_SERVER_ADDRESS = "-server=";
        public readonly string ARG_NAME_MAIN_PROCESS_NAME = "-process_name=";
        public readonly string ARG_NAME_EXCEPTION_TEXT = "-exception_text=";
        public readonly string ARG_SPLIT_EXCEPTION_TEXT = "<>";
        // 프로세스 정보
        public readonly string EXCEPTION_HANDLER_V2_FILE_NAME = "RHYANetwork.UtaitePlayer.ExceptionHandlerV2.exe";




        /// <summary>
        /// 프로세스 실행
        /// </summary>
        public void startProcess(string message)
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
                stringBuilder.Append(" ");
                stringBuilder.Append(ARG_NAME_EXCEPTION_TEXT);
                stringBuilder.Append(message.Replace(" ", ARG_SPLIT_EXCEPTION_TEXT));

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WorkingDirectory = new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getInstallPath().ToString();
                psi.UseShellExecute = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.FileName = EXCEPTION_HANDLER_V2_FILE_NAME;
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
    }
}
