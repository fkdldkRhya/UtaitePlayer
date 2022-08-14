using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.ProcessKILLER
{
    class Program
    {
        // Console 숨기기 Win API
        // =========================================================
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // =========================================================




        static void Main(string[] args)
        {
            try
            {
                // Console 숨기기
                ShowWindow(GetConsoleWindow(), 0);

                // 프로그램 정보 출력
                Console.WriteLine("RHYA.Network ProcessKILLER");
                Console.WriteLine("This program is a user access verification service for Utaite Player only.");
                Console.WriteLine("By default, this program runs together when you run the Utaite Player.");
                Console.WriteLine("");
                Console.WriteLine("Copyright (c) RHYA.Network 2022, All rights reserved.");
                Console.WriteLine("");
                Console.WriteLine("Warning! Do not run this program arbitrarily. The following errors may occur:");
                Console.WriteLine("Unnecessary computer resources can be wasted.");
                Console.WriteLine("In addition, the behavior of the UtaitePlayer service can be catastrophic.");

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                try
                {
                    Process[] processes = Process.GetProcessesByName("RHYANetwork.UtaitePlayer.CrashHandler");
                    if (processes != null && processes.Length > 0)
                        for (int i = 0; i < processes.Length; i++)
                            processes[i].Kill();
                }
                catch (Exception) { }

                try
                {
                    RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor authCheckManagerProcessor = new RHYANetwork.UtaitePlayer.ProcessManager.AuthCheckManagerProcessor();
                    authCheckManagerProcessor.killProcess();
                }
                catch (Exception) { }

                try
                {
                    Process[] processes = Process.GetProcessesByName("UtaitePlayer");
                    if (processes.Length > 0)
                    {
                        int pid = registryManager.getRootProgramPID();
                        foreach (Process process in processes)
                            if (process.Id == pid)
                                process.Kill();
                    }
                }
                catch (Exception) { }

                // 레지스트리 초기화
                if (registryManager.isSetAuthToken())
                    registryManager.deleteAuthToken();
                if (registryManager.isSetCryptoKey())
                    registryManager.deleteCryptoKey();
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionHandler.ExceptionManager.getInstance().showMessageBox(ex);
            }

            Environment.Exit(0);
        }
    }
}
