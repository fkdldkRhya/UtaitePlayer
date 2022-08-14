using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.DataVO
{
    public class SearchResultDataVO
    {
        // 검색 결과 타입
        public enum SearchResultDataType
        {
            MUSIC_RESULT,
            ARTIST_RESULT
        }


        // UUID 데이터
        public string uuid { get; private set; }

        // 노래 이미지 BitmapImage 형식
        public BitmapImage bitmapImage { get; private set; }

        // 노래 이름
        public string musicName { get; private set; } = "Null";
        // 노래 이미지
        public string musicImage { get; private set; } = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";
        // 노래 태그
        public string musicTag { get; private set; } = "Null";
        // 노래 작곡가
        public string musicWriter { get; private set; } = "Null";
        // 아티스트 이름
        public string artistName { get; private set; } = "Null";
        // 아티스트 이미지
        public string artistImage { get; private set; } = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";
        // 아티스트 구독 여부
        public string artistSubscribeData { get; private set; } = "구독";
        // 아티스트 버튼 표시 여부
        public Visibility artistSubscribeButtonVisibility { get; set; } = Visibility.Visible;
        // 아티스트 로딩 UI 표시 여부
        public Visibility artistLoadingVisibility { get; set; } = Visibility.Collapsed;




        /// <summary>
        /// 생성자
        /// </summary>
        public SearchResultDataVO(SearchResultDataType searchResultDataType, string uuid)
        {
            try
            {
                if (searchResultDataType == SearchResultDataType.MUSIC_RESULT) // 형식 - 노래 데이터
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
                        musicTag = musicInfoVO.type;
                        musicWriter = musicInfoVO.songWriter;

                        // BitmapImage 변환
                        try
                        {
                            bitmapImage = Utils.URLImageLoadManager.ImageURLToBitmapImage(musicInfoVO.image, musicInfoVO.uuid, 30, 30, Utils.URLImageLoadManager.ImageType.IMAGE_MUSIC);
                        }
                        catch (Exception) { }

                        // 아티스트 정보 설정
                        RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO singerInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[musicInfoVO.singerUUID];
                        artistName = singerInfoVO.name;
                        artistImage = singerInfoVO.image;
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
                else if (searchResultDataType == SearchResultDataType.ARTIST_RESULT)
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

                        // 아티스트 구독 확인
                        bool subscribe = false;
                        foreach (RHYANetwork.UtaitePlayer.DataManager.UserSubscribeInfoVO userSubscribeInfoVO in RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userSubscribeInfoVOs)
                        {
                            if (userSubscribeInfoVO.uuid.Equals(uuid))
                            {
                                subscribe = true;
                                break;
                            }
                        }
                        if (subscribe) artistSubscribeData = "구독 취소";
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 아티스트 구독 확인 함수
        /// </summary>
        public void artistSubscribeChecker()
        {
            try
            {
                // 아티스트 구독 확인
                bool subscribe = false;
                foreach (RHYANetwork.UtaitePlayer.DataManager.UserSubscribeInfoVO userSubscribeInfoVO in RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userSubscribeInfoVOs)
                {
                    if (userSubscribeInfoVO.uuid.Equals(uuid))
                    {
                        subscribe = true;
                        break;
                    }
                }
                if (subscribe) artistSubscribeData = "구독 취소";
                else artistSubscribeData = "구독";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
