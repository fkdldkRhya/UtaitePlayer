using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.DataVO
{
    public class RecommendUtaiteDataVO
    {
        // UUID 데이터
        public string uuid { get; private set; }
        // 노래 이름
        public string musicName { get; private set; } = "Null";
        // 노래 이미지
        public string musicImage { get; private set; } = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";
        // 노래 태그
        public string musicTag { get; private set; } = "Null";
        // 노래 작곡가
        public string musicWriter { get; private set; } = "Null";
        // 노래 등록 날짜
        public string musicDate { get; private set; } = "Null";
        // 노래 가사
        public string musicLyrics { get; private set; } = "Null";
        // 아티스트 이름
        public string artistName { get; private set; } = "Null";




        /// <summary>
        /// 생성자
        /// </summary>
        public RecommendUtaiteDataVO(string uuid)
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
                    musicTag = musicInfoVO.type;
                    musicWriter = musicInfoVO.songWriter;
                    musicDate = musicInfoVO.date;

                    // 아티스트 정보 설정
                    RHYANetwork.UtaitePlayer.DataManager.SingerInfoVO singerInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[musicInfoVO.singerUUID];
                    artistName = singerInfoVO.name;

                    // 가사 데이터 설정
                    // =============================================
                    // 파일 경로 구하기
                    string lyricsFileName = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager().getLyricsFileName(uuid);
                    // 파일 읽기
                    string readData = File.ReadAllText(lyricsFileName);
                    // 데이터 복호화
                    string encryptKey = new RHYANetwork.UtaitePlayer.Registry.RegistryManager().getCryptoKey().ToString();
                    RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto mAESCrypto = new RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto();
                    readData = mAESCrypto.decryptAES(readData, encryptKey, mAESCrypto.MAIN_ENCRYPT_DECRYPT_IV, RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto.AESKeySize.SIZE_256);
                    // 가사 설정
                    musicLyrics = readData;
                    // =============================================
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
