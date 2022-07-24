using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.DataVO
{
    public class SelectMyPlaylistDataVO
    {
        // 플레이리스트 UUID
        public string uuid { get; private set; }
        // 플레이리스트 이름
        public string name { get; private set; }




        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uuid">플레이리스트 UUID</param>
        public SelectMyPlaylistDataVO(string uuid)
        {
            try
            {
                if (!RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.ContainsKey(uuid))
                {
                    // UUID 없음 - 예외 발생
                    NotFoundMyPlaylistUUIDException notFoundMyPlaylistUUIDException = new NotFoundMyPlaylistUUIDException("커스텀 플레이리스트 UUID를 찾을 수 없습니다.");
                    notFoundMyPlaylistUUIDException.UUID = uuid;

                    // 예외 발생
                    throw notFoundMyPlaylistUUIDException;
                }

                // UUID 설정
                this.uuid = uuid;

                RHYANetwork.UtaitePlayer.DataManager.UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs[uuid];
                name = userCustomPlaylistInfoVO.name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
