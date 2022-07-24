using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RHYANetwork.UtaitePlayer.DataManager
{
    public class MusicResourcesVO
    {
        // Instance
        private static MusicResourcesVO musicResourcesVO = null;

        // 노래 데이터
        // -------------------------------------------------------------------------- //
        private Dictionary<string, MusicInfoVO> mMusicResources = new Dictionary<string, MusicInfoVO>();
        public Dictionary<string, MusicInfoVO> musicResources {
            get { return mMusicResources; }
        }
        // -------------------------------------------------------------------------- //

        // 아티스트 데이터
        // -------------------------------------------------------------------------- //
        private Dictionary<string, SingerInfoVO> mSingerResources = new Dictionary<string, SingerInfoVO>();
        public Dictionary<string, SingerInfoVO> singerResources
        {
            get { return mSingerResources; }
        }
        // -------------------------------------------------------------------------- //




        /// <summary>
        /// Instance 가져오기
        /// </summary>
        /// <returns>MusicResourcesVO Instance</returns>
        public static MusicResourcesVO getInstance()
        {
            if (musicResourcesVO == null)
                musicResourcesVO = new MusicResourcesVO();

            return musicResourcesVO;
        }



        /// <summary>
        /// 노래 데이터 추가
        /// </summary>
        /// <param name="musicInfoVO">노래 데이터</param>
        public void addMusicResources(MusicInfoVO musicInfoVO)
        {
            if (mMusicResources != null && !mMusicResources.ContainsKey(musicInfoVO.uuid))
                mMusicResources.Add(musicInfoVO.uuid, musicInfoVO);
        }



        /// <summary>
        /// 아티스트 데이터 추가
        /// </summary>
        /// <param name="musicInfoVO">아티스트 데이터</param>
        public void addSingerResources(SingerInfoVO singerInfoVO)
        {
            if (mSingerResources != null && !mSingerResources.ContainsKey(singerInfoVO.uuid))
                mSingerResources.Add(singerInfoVO.uuid, singerInfoVO);
        }



        /// <summary>
        /// 노래 데이터 삭제
        /// </summary>
        /// <param name="musicInfoVO">노래 UUID</param>
        public void removeMusicResources(string uuid)
        {
            if (mMusicResources != null && mMusicResources.ContainsKey(uuid))
                mMusicResources.Remove(uuid);
        }



        /// <summary>
        /// 아티스트 데이터 삭제
        /// </summary>
        /// <param name="musicInfoVO">아티스트 UUID</param>
        public void removeSingerResources(string uuid)
        {
            if (mSingerResources != null && mSingerResources.ContainsKey(uuid))
                mSingerResources.Remove(uuid);
        }



        /// <summary>
        /// 노래 데이터 초기화
        /// </summary>
        public void clearMusicResources()
        {
            if (mMusicResources != null)
                mMusicResources.Clear();
        }


        /// <summary>
        /// 아티스트 데이터 초기화
        /// </summary>
        public void clearSingerResources()
        {
            if (mSingerResources != null)
                mSingerResources.Clear();
        }
    }





    /// <summary>
    /// 노래 데이터 VO
    /// </summary>
    public class MusicInfoVO
    {
        // 노래 데이터 변경 - 노래 구분 UUID
        public string uuid { get; private set; } = null;
        // 노래 데이터 변경 - 노래 이름
        public string name { get; private set; } = null;
        // 노래 데이터 변경 - 노래 아티스트 UUID
        public string singerUUID { get; private set; } = null;
        // 노래 데이터 변경 - 노래 작곡가
        public string songWriter { get; private set; } = null;
        // 노래 데이터 변경 - 노래 이미지
        public string image { get; private set; } = null;
        // 노래 데이터 변경 - 노래 MP3 파일
        public string mp3 { get; private set; } = null;
        // 노래 데이터 변경 - 노래 Type
        public string type { get; private set; } = null;
        // 노래 데이터 변경 - 노래 업로드 날짜
        public string date { get; private set; } = null;
        // 노래 데이터 변경 - 노래 버전
        public int version { get; private set; } = -1;
        // 노래 데이터 변경 - 기타 데이터 1
        public int temp1 { get; set; } = -1;
        // 노래 데이터 변경 - 기타 데이터 2
        public int temp2 { get; set; } = -1;



        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mUUID">노래 UUID</param>
        /// <param name="mName">노래 이름</param>
        /// <param name="mSingerUuid">노래 아티스트 UUID</param>
        /// <param name="mSongWriter">노래 작곡가</param>
        /// <param name="mImage">노래 이미지</param>
        /// <param name="mMp3">노래 MP3 URL</param>
        /// <param name="mType">노래 타입</param>
        /// <param name="mDate">노래 업로드 날짜</param>
        /// <param name="mVersion">노래 버전</param>
        /// <param name="mTemp1">기타 데이터 1</param>
        /// <param name="mTemp2">기타 데이터 2</param>
        public MusicInfoVO(string mUUID, string mName, string mSingerUuid, string mSongWriter, string mImage, string mMp3, string mType, string mDate, int mVersion, int mTemp1, int mTemp2)
        {
            this.uuid = HttpUtility.UrlDecode(mUUID, Encoding.UTF8);
            this.name = HttpUtility.UrlDecode(mName, Encoding.UTF8);
            this.singerUUID = HttpUtility.UrlDecode(mSingerUuid, Encoding.UTF8);
            this.songWriter = HttpUtility.UrlDecode(mSongWriter, Encoding.UTF8);
            this.image = HttpUtility.UrlDecode(mImage, Encoding.UTF8);
            this.mp3 = HttpUtility.UrlDecode(mMp3, Encoding.UTF8);
            this.type = HttpUtility.UrlDecode(mType, Encoding.UTF8);
            this.date = HttpUtility.UrlDecode(mDate, Encoding.UTF8);
            this.version = mVersion;
            this.temp1 = mTemp1;
            this.temp2 = mTemp2;
        }
    }





    /// <summary>
    /// 아티스트 데이터 VO
    /// </summary>
    public class SingerInfoVO
    {
        // 아티스트 데이터 변경 - 아티스트 구분 UUID
        public string uuid { get; private set; } = null;
        // 아티스트 데이터 변경 - 아티스트 이름
        public string name { get; private set; } = null;
        // 아티스트 데이터 변경 - 아티스트 이미지
        public string image { get; private set; } = null;



        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uuid">아티스트 UUID</param>
        /// <param name="name">아티스트 이름</param>
        /// <param name="image">아티스트 이미지</param>
        public SingerInfoVO(string uuid, string name, string image)
        {
            this.uuid = HttpUtility.UrlDecode(uuid, Encoding.UTF8);
            this.name = HttpUtility.UrlDecode(name, Encoding.UTF8);
            this.image = HttpUtility.UrlDecode(image, Encoding.UTF8);
        }
    }
}
