using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.DataVO
{
    public class ManyMusicDataVO
    {
        // 아티스트 구분 UUID
        public string uuid { get; private set; } = "Null";
        // 아티스트 이름
        public string artistName { get; private set; } = "Null";
        // 아티스트 이미지 BitmapImage 형식
        public BitmapImage bitmapImage { get; private set; }
        // 아티스트 이미지
        public string artistImage { get; private set; }
        // 아티스트 노래 개수
        public int musicCountForInt { get; private set; } = -1;
        // 아티스트 노래 개수
        public string musicCount { get; private set; } = "0개";
        // 아티스트 노래 개수 순위
        public int musicCountRank { get; set; } = 0;



        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uuid">아티스트 UUID</param>
        /// <param name="count">노래 개수</param>
        public ManyMusicDataVO(string uuid, int count)
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
                    {
                        artistImage = singerInfoVO.image;

                        // BitmapImage 변환
                        try
                        {
                            bitmapImage = Utils.URLImageLoadManager.ImageURLToBitmapImage(singerInfoVO.image, singerInfoVO.uuid, 30, 30, Utils.URLImageLoadManager.ImageType.IMAGE_SINGER);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        // BitmapImage 변환
                        try
                        {
                            bitmapImage = Utils.URLImageLoadManager.ResourceToBitmapImage("pack://application:,,,/UtaitePlayer;component/Resources/drawable/img_no_data.png", 30, 30);
                        }
                        catch (Exception) { }
                    }

                    musicCountForInt = count;
                    musicCount = string.Format("{0}개", count);
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
