using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.MutexManager
{
    public class MutexList
    {
        // Mutex 지원 서비스 이름
        public enum ServiceList
        {
            ROOT_SERVICE_UTAITE_PLAYER,
            SERVICE_AUTH_CHECK_MANAGER,
            SERVICE_CRASH_HANDLER,
            SERVICE_RHYA_MESSAGE_RECEIVER
        }



        /// <summary>
        /// 지정 서비스 Mutex 이름 가져오기
        /// </summary>
        /// <param name="serviceList">서비스</param>
        /// <returns></returns>
        public string GetMutexName(ServiceList serviceList)
        {
            try
            {
                string name = null;

                switch (serviceList)
                {
                    case ServiceList.ROOT_SERVICE_UTAITE_PLAYER:
                        name = "kro.kr.rhya-network.utaiteplayer";
                        break;

                    case ServiceList.SERVICE_AUTH_CHECK_MANAGER:
                        name = "kro.kr.rhya-network.utaiteplayer.crashhandler";
                        break;

                    case ServiceList.SERVICE_CRASH_HANDLER:
                        name = "kro.kr.rhya-network.utaiteplayer.authcheckermanager";
                        break;
                }

                return name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
