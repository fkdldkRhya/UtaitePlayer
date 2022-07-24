using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.CrashHandler
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
                Console.WriteLine("RHYA.Network CrashHandler");
                Console.WriteLine("This program is a user access verification service for Utaite Player only.");
                Console.WriteLine("By default, this program runs together when you run the Utaite Player.");
                Console.WriteLine("");
                Console.WriteLine("Copyright (c) RHYA.Network 2022, All rights reserved.");
                Console.WriteLine("");
                Console.WriteLine("Warning! Do not run this program arbitrarily. The following errors may occur:");
                Console.WriteLine("Unnecessary computer resources can be wasted.");
                Console.WriteLine("In addition, the behavior of the UtaitePlayer service can be catastrophic.");

                // Mutex 확인
                bool createNew;
                new Mutex(true, "kro.kr.rhya-network.utaiteplayer.crashhandler", out createNew);
                if (!createNew)
                    Environment.Exit(0);

                Registry.RegistryManager registryManager = new Registry.RegistryManager();
                int rootPID = registryManager.getRootProgramPID();

                Process[] processes = Process.GetProcesses();
                Process process = null;
                for (int i = 0; i < processes.Length; i ++)
                {
                    if (rootPID == processes[i].Id)
                    {
                        process = processes[i];
                    }
                }

                string path = ExceptionHandler.ExceptionManager.getInstance().getNoErrorFilePath();
                if (new System.IO.FileInfo(path).Exists)
                    System.IO.File.Delete(path);
                

                if (process != null && process.ProcessName.Equals("UtaitePlayer"))
                {
                    while (true)
                    {
                        try
                        {
                            // 종료 확인
                            if (process.HasExited)
                            {
                                // 파일 확인
                                if (new System.IO.FileInfo(path).Exists && int.Parse(System.IO.File.ReadAllText(path)) == rootPID)
                                {
                                    System.IO.File.Delete(path);
                                }
                                else
                                {
                                    ProcessStartInfo psi = new ProcessStartInfo();
                                    psi.WorkingDirectory = registryManager.getInstallPath().ToString();
                                    psi.UseShellExecute = false;
                                    psi.CreateNoWindow = false;
                                    psi.FileName = "UtaitePlayer.exe";
                                    process = Process.Start(psi);
                                }

                                Environment.Exit(0);
                            }

                            Thread.Sleep(1000);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionHandler.ExceptionManager.getInstance().showMessageBox(ex);
                Environment.Exit(0);
            }
        }
    }
}
