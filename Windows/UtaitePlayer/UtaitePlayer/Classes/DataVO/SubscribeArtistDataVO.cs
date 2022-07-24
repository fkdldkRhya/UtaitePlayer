using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.DataVO
{
    public class SubscribeArtistDataVO
    {
        // 아티스트 구분 UUID
        public string uuid { get; private set; } = "Null";
        // 아티스트 이름
        public string artistName { get; private set; } = "Null";
        // 아티스트 이미지
        public string artistImage { get; private set; } = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";
        // 아티스트 버튼 표시 여부
        public Visibility artistSubscribeButtonVisibility { get; set; } = Visibility.Visible;
        // 아티스트 로딩 UI 표시 여부
        public Visibility artistLoadingVisibility { get; set; } = Visibility.Collapsed;



        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uuid">아티스트 UUID</param>
        public SubscribeArtistDataVO(string uuid)
        {
            try
            {
                // UUID 확인
                if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.ContainsKey(uuid))
                {
                    // UUID 설정
                    this.uuid = uuid;

                    // 아티스트 정보 설정
                    RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO singerInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[uuid];
                    artistName = singerInfoVO.name;
                    if (!singerInfoVO.image.Equals("-"))
                        artistImage = singerInfoVO.image;
                }
                else
                {
                    // UUID 없음 - 예외 발생
                    NotFoundArtistUUIDException notFoundArtistUUIDException = new NotFoundArtistUUIDException("아티스트 UUID를 찾을 수 없습니다.");
                    notFoundArtistUUIDException.UUID = uuid;

                    // 예외 발생
                    throw notFoundArtistUUIDException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
