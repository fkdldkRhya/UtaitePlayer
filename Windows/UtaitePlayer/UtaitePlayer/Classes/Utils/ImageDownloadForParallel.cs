using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.Utils
{
    public class ImageDownloadForParallel
    {
        // 다운로드 로직
        public Action downloadAction1 = null;
        public Action downloadAction2 = null;
        public Action downloadAction3 = null;
        public Action downloadAction4 = null;
        public Action downloadAction5 = null;
        // 병렬 처리 전용 Thread
        private Thread th1;
        private Thread th2;
        private Thread th3;
        private Thread th4;
        private Thread th5;



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
                return th1.IsAlive && th2.IsAlive && th3.IsAlive && th4.IsAlive && th5.IsAlive;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
