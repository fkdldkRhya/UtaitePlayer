using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.Utils
{
    public class PixivImageDownloadForParallel
    {
        // 다운로드 로직
        public Action downloadAction1 = null;
        public Action downloadAction2 = null;
        public Action downloadAction3 = null;
        public Action downloadAction4 = null;
        public Action downloadAction5 = null;
        public Action downloadAction6 = null;
        public Action downloadAction7 = null;
        public Action downloadAction8 = null;
        public Action downloadAction9 = null;
        public Action downloadAction10 = null;
        // 병렬 처리 전용 Thread
        private Thread th1;
        private Thread th2;
        private Thread th3;
        private Thread th4;
        private Thread th5;
        private Thread th6;
        private Thread th7;
        private Thread th8;
        private Thread th9;
        private Thread th10;



        /// <summary>
        /// 다운로드 진행
        /// </summary>
        public void startDownload()
        {
            try
            {
                th1 = new Thread(() =>
                {
                    try
                    {
                        downloadAction1();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th1.Start();

                th2 = new Thread(() =>
                {
                    try
                    {
                        downloadAction2();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th2.Start();

                th3 = new Thread(() =>
                {
                    try
                    {
                        downloadAction3();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th3.Start();

                th4 = new Thread(() =>
                {
                    try
                    {
                        downloadAction4();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th4.Start();

                th5 = new Thread(() =>
                {
                    try
                    {
                        downloadAction5();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th5.Start();

                th6 = new Thread(() =>
                {
                    try
                    {
                        downloadAction6();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th6.Start();

                th7 = new Thread(() =>
                {
                    try
                    {
                        downloadAction7();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th7.Start();

                th8 = new Thread(() =>
                {
                    try
                    {
                        downloadAction8();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th8.Start();

                th9 = new Thread(() =>
                {
                    try
                    {
                        downloadAction9();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th9.Start();

                th10 = new Thread(() =>
                {
                    try
                    {
                        downloadAction10();
                    }
                    catch (Exception ex)
                    {
                        // 예외 처리
                        ExceptionManager.getInstance().showMessageBox(ex);
                    }
                });
                th10.Start();
            }
            catch (Exception ex)
            {
                // 예외 처리
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 다운로드 종료 대기
        /// </summary>
        public void waitDownloadEnd()
        {
            try
            {
                th1.Join();
                th2.Join();
                th3.Join();
                th4.Join();
                th5.Join();
                th6.Join();
                th7.Join();
                th8.Join();
                th9.Join();
                th10.Join();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 다운로드 종료 확인
        /// </summary>
        public bool checkDownloadEnd()
        {
            try
            {
                return th1.IsAlive && th2.IsAlive && th3.IsAlive && th4.IsAlive && th5.IsAlive
                    && th6.IsAlive && th7.IsAlive && th8.IsAlive && th9.IsAlive && th10.IsAlive;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
