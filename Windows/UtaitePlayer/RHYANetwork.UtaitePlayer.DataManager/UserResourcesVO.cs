using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.DataManager
{
    public class UserResourcesVO
    {
        // Instance
        private static UserResourcesVO userResourcesVO = null;

        // 사용자 데이터
        // -------------------------------------------------------------------------- //
        private UserInfoVO mUserInfoVO = new UserInfoVO();
        public UserInfoVO userInfoVO
        {
            get { return mUserInfoVO; }
        }
        // -------------------------------------------------------------------------- //

        // 사용자 플레이리스트 데이터
        // -------------------------------------------------------------------------- //
        private List<UserPlaylistInfoVO> mUserPlaylistInfoVOs = new List<UserPlaylistInfoVO>();
        public List<UserPlaylistInfoVO> userPlaylistInfoVOs
        {
            get { return mUserPlaylistInfoVOs; }
        }
        // -------------------------------------------------------------------------- //

        // 사용자 커스텀 플레이리스트 데이터
        // -------------------------------------------------------------------------- //
        private Dictionary<string, UserCustomPlaylistInfoVO> mUserCustomPlaylistInfoVOs = new Dictionary<string, UserCustomPlaylistInfoVO>();
        public Dictionary<string, UserCustomPlaylistInfoVO> userCustomPlaylistInfoVOs
        {
            get { return mUserCustomPlaylistInfoVOs; }
        }
        // -------------------------------------------------------------------------- //

        // 사용자 구독 데이터
        // -------------------------------------------------------------------------- //
        private List<UserSubscribeInfoVO> mUserSubscribeInfoVOs = new List<UserSubscribeInfoVO>();
        public List<UserSubscribeInfoVO> userSubscribeInfoVOs
        {
            get { return mUserSubscribeInfoVOs; }
        }
        // -------------------------------------------------------------------------- //

        // 플레이리스트 인덱스
        private int _MyPlaylistIndex = -1;
        public int myPlaylistIndex
        {
            get { return _MyPlaylistIndex; }
            set
            {
                if (mUserPlaylistInfoVOs.Count <= 0) return;

                if (value == -1 || value <= mUserPlaylistInfoVOs.Count - 1)
                    _MyPlaylistIndex = value;
                else
                    throw new Exception("사용자 플레이리스트 인덱스 설정 불가, 범위를 벗어났습니다.");
            }
        }




        /// <summary>
        /// Instance 가져오기
        /// </summary>
        /// <returns>MusicResourcesVO Instance</returns>
        public static UserResourcesVO getInstance()
        {
            if (userResourcesVO == null)
                userResourcesVO = new UserResourcesVO();

            return userResourcesVO;
        }



        /// <summary>
        /// 플레이리스트 데이터 삽입
        /// </summary>
        /// <param name="userPlaylistInfoVO">노래 데이터</param>
        public void addUserPlaylistInfoVO(UserPlaylistInfoVO userPlaylistInfoVO)
        {
            try
            {
                if (!(mUserPlaylistInfoVOs.Count > 100))
                    mUserPlaylistInfoVOs.Add(userPlaylistInfoVO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        
        /// <summary>
        /// 플레이리스트 데이터 제거
        /// </summary>
        /// <param name="index">인덱스</param>
        public void removeUserPlaylistInfoVO(int index)
        {
            try
            {
                if (mUserPlaylistInfoVOs.Count - 1 >= index)
                    mUserPlaylistInfoVOs.RemoveAt(index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 구간 데이터 제거
        /// </summary>
        /// <param name="index1">인덱스 1</param>
        /// <param name="index2">인덱스 2</param>
        public void removeRangeUserPlaylistInfoVO(int index1, int index2)
        {
            try
            {
                if (index1 <= index2 && mUserPlaylistInfoVOs.Count - 1 >= index1 && mUserPlaylistInfoVOs.Count - 1 >= index2)
                {
                    for (int i = index2; i >= 0; i--)
                    {
                        mUserPlaylistInfoVOs.RemoveAt(i);

                        if (index1 == i) break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 데이터 초기화
        /// </summary>
        public void clearUserPlaylistInfoVO()
        {
            try
            {
                mUserPlaylistInfoVOs.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }





    /// <summary>
    /// 사용자 데이터 VO
    /// </summary>
    public class UserInfoVO
    {
        // 사용자 데이터 변경 - 사용자 구분 UUID
        public string uuid { get; set; } = null;
        // 사용자 데이터 변경 - 사용자 이름
        public string name { get; set; } = null;
        // 사용자 데이터 변경 - 사용자 생일
        public string birthday { get; set; } = null;
        // 사용자 데이터 변경 - 사용자 가입 날짜
        public string date { get; set; } = null;
        // 사용자 데이터 변경 - 사용자 아이디
        public string id { get; set; } = null;
        // 사용자 데이터 변경 - 사용자 이메일
        public string email { get; set; } = null;



        public bool nullChecker()
        {
            return uuid == null || name == null || birthday == null || date == null || id == null || email == null;
        }
    }





    /// <summary>
    /// 사용자 플레이리스트 데이터 VO
    /// </summary>
    public class UserPlaylistInfoVO
    {
        // 사용자 플레이리스트 데이터 변경 - 노래 구분 UUID
        private string mUUID = null;
        public string uuid 
        {
            get 
            {
                return mUUID;
            }

            set
            {
                try
                {
                    // 노래 UUID 확인
                    if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.ContainsKey(value))
                    {
                        RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[value];

                        // 데이터 설정
                        mUUID = value;
                        singerUUID = musicInfoVO.singerUUID;
                    }
                    else
                    {
                        // 예외 발생
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }
        }

        // 노래 선택 여부
        public bool isChecked { get; set; }
        // 노래 재생 여부
        private bool _isPlay = false;
        public bool isPlay
        {
            get { return isPlay; }
            set
            {
                _isPlay = value;
            }
        }
        // 글자색
        public string color
        {
            get
            {
                if (_isPlay)
                {
                    return "#FFFFFF";
                }
                else
                {
                    return "#FF646464";
                }
            }
        }




        // 노래 기본 정보
        // ==============================================


        // ----------------------------------------------
        // 노래 이름
        // ----------------------------------------------
        public string musicName
        {
            get
            {
                try
                {
                    if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.ContainsKey(mUUID))
                    {
                        return RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[mUUID].name;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        // ----------------------------------------------
        // ----------------------------------------------


        // ----------------------------------------------
        // 아티스트 이름
        // ----------------------------------------------
        private string _singerUUID = null;
        public string singerName
        {
            get
            {
                try
                {
                    if (RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources.ContainsKey(_singerUUID))
                    {
                        return RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().singerResources[_singerUUID].name;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public string singerUUID
        {
            get { return _singerUUID; }
            private set
            {
                _singerUUID = value;
            }
        }
        // ----------------------------------------------
        // ----------------------------------------------


        // ==============================================
    }





    /// <summary>
    /// 사용자 커스텀 플레이리스트 데이터 VO
    /// </summary>
    public class UserCustomPlaylistInfoVO
    {
        // 사용자 커스텀 플레이리스트 데이터 변경 - 커스텀 플레이리스트 구분 UUID
        public string uuid { get; private set; } = null;
        // 사용자 커스텀 플레이리스트 데이터 변경 - 커스텀 플레이리스트 이름
        public string name { get; set; } = null;
        // 사용자 커스텀 플레이리스트 데이터 변경 - 커스텀 플레이리스트 이미지
        public string image { get; set; } = null;

        // 사용자 플레이리스트 데이터
        // -------------------------------------------------------------------------- //
        private List<UserPlaylistInfoVO> mUserPlaylistInfoVOs = new List<UserPlaylistInfoVO>();
        public List<UserPlaylistInfoVO> userPlaylistInfoVOs
        {
            get { return mUserPlaylistInfoVOs; }
        }
        // -------------------------------------------------------------------------- //



        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="uuid">커스텀 플레이리스트 구분 UUID</param>
        public UserCustomPlaylistInfoVO(string uuid)
        {
            this.uuid = uuid;
        }



        /// <summary>
        /// 플레이리스트 데이터 삽입
        /// </summary>
        /// <param name="userPlaylistInfoVO">노래 데이터</param>
        public void addUserPlaylistInfoVO(UserPlaylistInfoVO userPlaylistInfoVO)
        {
            try
            {
                if (!(mUserPlaylistInfoVOs.Count > 100))
                    mUserPlaylistInfoVOs.Add(userPlaylistInfoVO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 데이터 제거
        /// </summary>
        /// <param name="index">인덱스</param>
        public void removeUserPlaylistInfoVO(int index)
        {
            try
            {
                if (mUserPlaylistInfoVOs.Count - 1 >= index)
                    mUserPlaylistInfoVOs.RemoveAt(index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 구간 데이터 제거
        /// </summary>
        /// <param name="index1">인덱스 1</param>
        /// <param name="index2">인덱스 2</param>
        public void removeRangeUserPlaylistInfoVO(int index1, int index2)
        {
            try
            {
                if (index1 <= index2 && mUserPlaylistInfoVOs.Count - 1 >= index1 && mUserPlaylistInfoVOs.Count - 1 >= index2)
                {
                    for (int i = index2; i >= 0; i--)
                    {
                        mUserPlaylistInfoVOs.RemoveAt(i);

                        if (index1 == i) break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 데이터 초기화
        /// </summary>
        public void clearUserPlaylistInfoVO()
        {
            try
            {
                mUserPlaylistInfoVOs.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }





    /// <summary>
    /// 사용자 구독 데이터 VO
    /// </summary>
    public class UserSubscribeInfoVO
    {
        // 사용자 구독 데이터 변경 - 아티스트 구분 UUID
        private string mUUID = null;
        public string uuid
        {
            get
            {
                return mUUID;
            }
            set
            {
                if (MusicResourcesVO.getInstance().singerResources.ContainsKey(value))
                    mUUID = value;
            }
        }
    }
}
