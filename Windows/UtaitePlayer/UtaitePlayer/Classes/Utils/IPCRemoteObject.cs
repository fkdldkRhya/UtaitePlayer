using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UtaitePlayer.Classes.Utils
{
    public class IPCRemoteObject : MarshalByRefObject
    {
        /// <summary>
        /// 메시지 전송
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>메시지</returns>
        public void SendMessage(string msg)
        {
            try
            {
                // 메시지 구분 키
                const string MESSAGE_KEY_AUTH_CHECK_MANAGER = "-RHYANetwork.AuthCheckManager.Value=";

                // 메시지 확인
                if (msg.Contains(MESSAGE_KEY_AUTH_CHECK_MANAGER))
                {
                    // 메시지 추출
                    msg = msg.Replace(MESSAGE_KEY_AUTH_CHECK_MANAGER, "");

                    // 전역 함수 실행
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_AUTH_CHECK_MANAGER_DIALOG, msg.Equals("success"));
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }
    }
}
