using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtaitePlayer.Classes.Utils;

namespace RHYANetwork.UtaitePlayer.AuthCheckManager
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

                // 인자 정보
                string ARG_NAME_SERVER_ADDRESS = "-server=";
                string ARG_NAME_MAIN_PROCESS_NAME = "-process_name=";
                // IPC 서버 주소
                string ipcServerAddress = null;
                string rootProcessName = null;

                // 프로그램 정보 출력
                Console.WriteLine("RHYA.Network AuthCheckManager");
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
                Mutex mutex = new Mutex(true, "kro.kr.rhya-network.utaiteplayer.authcheckermanager", out createNew);
                if (!createNew)
                    Environment.Exit(0);

                // 인자 확인
                if (args.Length > 0)
                {
                    // 인자 설정
                    foreach (string arg in  args)
                    {
                        // 서버 주소 인자 확인
                        if (arg.Contains(ARG_NAME_SERVER_ADDRESS))
                            ipcServerAddress = arg.Replace(ARG_NAME_SERVER_ADDRESS, "");

                        // 우타이테 플레이어 프로세스 이름 인자 확인
                        if (arg.Contains(ARG_NAME_MAIN_PROCESS_NAME))
                            rootProcessName = arg.Replace(ARG_NAME_MAIN_PROCESS_NAME, "");
                    }

                    // 인자 입력 확인
                    if (ipcServerAddress == null) Environment.Exit(0);

                    // Registry 관리자
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                    // UtaitePlayer Web Client
                    UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new Client.UtaitePlayerClient();

                    // IPC Server 연결
                    IpcClientChannel chan = new IpcClientChannel();
                    ChannelServices.RegisterChannel(chan, true);
                    IPCRemoteObject remObject = (IPCRemoteObject)Activator.GetObject(
                                typeof(IPCRemoteObject),
                                ipcServerAddress);

                    // PID 등록
                    registryManager.setAuthCheckManagerPID(Process.GetCurrentProcess().Id);

                    // While 제어 변수
                    bool isLoop = true;

                    // 연결 확인
                    while (isLoop)
                    {
                        try
                        {
                            // 서버 접속 실패 확인
                            if (remObject == null) isLoop = true;

                            // PID 설정 확인
                            if (registryManager.isSetRootProgramPID())
                            {
                                try
                                {
                                    int mainPID = registryManager.getRootProgramPID();

                                    // 우타이테 플레이어 프로세스 확인
                                    bool isNext = false;
                                    Process[] processes = Process.GetProcessesByName(rootProcessName);
                                    if (processes.Length <= 0)
                                    {
                                        // 종료 - PID 등록 안됨
                                        isLoop = false;
                                        continue;
                                    }
                                    // PID 확인 - 1
                                    foreach (Process process in processes)
                                    {
                                        if (process.Id == mainPID)
                                        {
                                            isNext = true;
                                            break;
                                        }
                                    }
                                    // PID 확인 - 2
                                    if (!isNext)
                                    {
                                        // 종료 - PID 등록 안됨
                                        isLoop = false;
                                        continue;
                                    }
                                }
                                catch (Exception) {}
                            }
                            else
                            {
                                // 종료 - PID 등록 안됨
                                isLoop = false;
                                continue;
                            }

                            // 접근 허용 여부 확인
                            if (registryManager.isSetAuthToken())
                            {
                                string jsonValue = utaitePlayerClient.userAccessVerify(registryManager.getAuthToken().ToString());
                                JObject jObject = JObject.Parse(jsonValue);
                                if (jObject.ContainsKey("result"))
                                {
                                    StringBuilder stringBuilder = new StringBuilder();
                                    stringBuilder.Append("-RHYANetwork.AuthCheckManager.Value=");
                                    stringBuilder.Append(jObject["result"].ToString());

                                    // 메시지 전송
                                    remObject.SendMessage(stringBuilder.ToString());
                                }
                            }

                            // 일정 시간 대기 [ 45초 ]
                            Thread.Sleep(45000);
                        }
                        catch (Exception) {}
                    }

                    // 종료
                    Environment.Exit(0);
                }
                else
                {
                    // 종료
                    Environment.Exit(0);
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
