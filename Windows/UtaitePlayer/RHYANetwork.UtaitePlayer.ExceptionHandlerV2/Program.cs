using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtaitePlayer.Classes.Utils;

namespace RHYANetwork.UtaitePlayer.ExceptionHandlerV2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try 
            {
                IPCRemoteObject getVar = new IPCRemoteObject();
                RHYANetwork.UtaitePlayer.ProcessManager.ExceptionHandlerV2Processor exceptionHandlerV2Processor = new ProcessManager.ExceptionHandlerV2Processor();

                // 인자 정보
                string ARG_NAME_SERVER_ADDRESS = exceptionHandlerV2Processor.ARG_NAME_SERVER_ADDRESS;
                string ARG_NAME_MAIN_PROCESS_NAME = exceptionHandlerV2Processor.ARG_NAME_MAIN_PROCESS_NAME;
                string ARG_NAME_EXCEPTION_TEXT = exceptionHandlerV2Processor.ARG_NAME_EXCEPTION_TEXT;
                string ARG_SPLIT_EXCEPTION_TEXT = exceptionHandlerV2Processor.ARG_SPLIT_EXCEPTION_TEXT;
                // IPC 서버 주소
                string ipcServerAddress = null;
                string rootProcessName = null;
                // 메시지 내용
                string messages = null;

                // 프로그램 정보 출력
                Console.WriteLine("RHYA.Network ExceptionHandlerV2");
                Console.WriteLine("This program is a user access verification service for Utaite Player only.");
                Console.WriteLine("By default, this program runs together when you run the Utaite Player.");
                Console.WriteLine("");
                Console.WriteLine("Copyright (c) RHYA.Network 2022, All rights reserved.");
                Console.WriteLine("");
                Console.WriteLine("Warning! Do not run this program arbitrarily. The following errors may occur:");
                Console.WriteLine("Unnecessary computer resources can be wasted.");
                Console.WriteLine("In addition, the behavior of the UtaitePlayer service can be catastrophic.");

                // 인자 확인
                if (args.Length > 0)
                {
                    // 인자 설정
                    foreach (string arg in args)
                    {
                        // 서버 주소 인자 확인
                        if (arg.Contains(ARG_NAME_SERVER_ADDRESS))
                            ipcServerAddress = arg.Replace(ARG_NAME_SERVER_ADDRESS, "");

                        // 우타이테 플레이어 프로세스 이름 인자 확인
                        if (arg.Contains(ARG_NAME_MAIN_PROCESS_NAME))
                            rootProcessName = arg.Replace(ARG_NAME_MAIN_PROCESS_NAME, "");

                        // 오류 메시지
                        if (arg.Contains(ARG_NAME_EXCEPTION_TEXT))
                            messages = arg.Replace(ARG_NAME_EXCEPTION_TEXT, "");
                    }

                    // 인자 입력 확인
                    if (ipcServerAddress == null && messages != null) Environment.Exit(0);

                    // Registry 관리자
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();

                    int rootPID = registryManager.getRootProgramPID();

                    Process[] processes = Process.GetProcesses();
                    Process process = null;
                    for (int i = 0; i < processes.Length; i++)
                    {
                        if (rootPID == processes[i].Id)
                        {
                            process = processes[i];
                        }
                    }

                    if (process != null && process.ProcessName.Equals("UtaitePlayer"))
                    {
                        // 종료 확인
                        if (!process.HasExited)
                        {
                            // IPC Server 연결
                            IpcClientChannel chan = new IpcClientChannel();
                            ChannelServices.RegisterChannel(chan, true);
                            IPCRemoteObject remObject = (IPCRemoteObject)Activator.GetObject(
                                        typeof(IPCRemoteObject),
                                        ipcServerAddress);

                            // 메시지 전송
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append(getVar.MESSAGE_KEY_EXCEPTION_HANDLER_V2);
                            stringBuilder.Append(messages.Replace(ARG_SPLIT_EXCEPTION_TEXT, " "));

                            // 메시지 전송
                            remObject.SendMessage(stringBuilder.ToString());

                            // 프로그램 종료
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("RHYANetwork.UtaitePlayer.ExceptionHandlerV2.Error.log", ex.Message);

                // 예외 처리
                Environment.Exit(0);
            }
        }
    }
}
