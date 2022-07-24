using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.DataVO
{
    public class NewUtaiteDataVO
    {
        // UUID 데이터
        public string uuid { get; private set; }
        // 노래 이름
        public string musicName { get; private set; } = "Null";
        // 노래 이미지
        public string musicImage { get; private set; } = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";
        // 아티스트 이름
        public string artistName { get; private set; } = "Null";




        /// <summary>
        /// 생성자
        /// </summary>
        public NewUtaiteDataVO(string uuid)
        {
            try
            {
                // UUID 확인
                if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.ContainsKey(uuid))
                {
                    // UUID 설정
                    this.uuid = uuid;

                    // 노래 정보 설정
                    RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[uuid];
                    musicName = musicInfoVO.name;
                    musicImage = musicInfoVO.image;

                    // 아티스트 정보 설정
                    RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO singerInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[musicInfoVO.singerUUID];
                    artistName = singerInfoVO.name;
                }
                else
                {
                    // UUID 없음 - 예외 발생
                    NotFoundMusicUUIDException notFoundMusicUUIDException = new NotFoundMusicUUIDException("노래 UUID를 찾을 수 없습니다.");
                    notFoundMusicUUIDException.UUID = uuid;

                    // 예외 발생
                    throw notFoundMusicUUIDException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
