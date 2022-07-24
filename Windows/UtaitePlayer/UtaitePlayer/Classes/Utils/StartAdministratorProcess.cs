using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UtaitePlayer.Classes.Utils
{
    public class StartAdministratorProcess
    {
        // VBS 파일 이름
        public readonly string PRIVILEGESEXEC_FILE_NAME = "PrivilegesEXEC.vbs";




        /// <summary>
        /// 관리자 권한으로 실행
        /// </summary>
        public void Run()
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Set objShell = CreateObject(\"Shell.Application\")");
                stringBuilder.AppendLine(string.Format("objShell.ShellExecute \"UtaitePlayer.exe\", \"\" , \"{0}\", \"runas\", 0", registryManager.getInstallPath()));

                string path = getPath();

                System.IO.File.WriteAllText(path, stringBuilder.ToString());

                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();

                new Thread(() =>
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "cscript";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.Arguments = string.Format("//B //Nologo {0}", PRIVILEGESEXEC_FILE_NAME);
                        process.StartInfo.WorkingDirectory = System.IO.Path.Combine(registryManager.getInstallPath().ToString(), musicDataManager.DATA_FILE_SAVE_DIRECTORY_NAME);
                        process.Start();

                        Application.Current.Dispatcher.Invoke(() => ExceptionManager.getInstance().exitProgram());
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 파일 경로 반환
        /// </summary>
        /// <returns>VBS 파일 경로</returns>
        public string getPath()
        {
            try
            {
                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();
                return System.IO.Path.Combine(System.IO.Path.Combine(new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getInstallPath().ToString(), musicDataManager.DATA_FILE_SAVE_DIRECTORY_NAME), PRIVILEGESEXEC_FILE_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
