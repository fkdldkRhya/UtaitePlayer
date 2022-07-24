using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Classes.DataVO
{
    public class MyPlaylistDataVO
    {
        // 플레이리스트 UUID
        public string uuid { get; private set; } = "Null";
        // 플레이리스트 이름
        public string name { get; private set; } = "Null";
        // 플레이리스트 이미지
        public string image { get; private set; } = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";
        // 플레이리스트 이미지 ID
        public string imageID { get; private set; } = "-1";
        // 플레이리스트 곡 개수
        public string count { get; private set; } = "0곡";
        // 플레이리스트 소유 계정
        public string account { get; private set; } = "Null";




        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uuid">플레이리스트 UUID</param>
        public MyPlaylistDataVO(string uuid)
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
                count = string.Format("{0}곡", userCustomPlaylistInfoVO.userPlaylistInfoVOs.Count());
                account = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id;
                imageID = userCustomPlaylistInfoVO.image;

                MyPlaylistImageManager myPlaylistImageManager = new MyPlaylistImageManager();
                image = myPlaylistImageManager.getImageURL(userCustomPlaylistInfoVO.image);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
