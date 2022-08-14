using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RHYANetwork.UtaitePlayer.Client
{
    public class UtaitePlayerClient
    {
        // Service name
        private readonly string RHYA_NETWORK_SERVICE_NAME = "kro_kr_rhya__network_jp__player";
        // RHYA.Network base URL
        private readonly string RHYA_NETWORK_UTAITE_PLAYER_URL = "https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager";
        private readonly string RHYA_NETWORK_DATA_BUFFER_URL = "https://rhya-network.kro.kr/RhyaNetwork/data_buffer_user_manager";
        private readonly string RHYA_NETWORK_AUTH_TOKEN_CHECK_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/auth_token_checker.jsp";
        private readonly string RHYA_NETWORK_AUTH_TOKEN_CREATE_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/auth_token.jsp";
        private readonly string RHYA_NETWORK_GET_USER_DATA_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/auth_info.jsp";
        // Public url
        public readonly string RHYA_NETWORK_MARQUEE_TEXT_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/utils/utaite_player_msuic_info_marquee_text.jsp";
        public readonly string RHYA_NETWORK_LICENSES_APPLICATION_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/service/utaite_player_licenses_application.jsp";

        // 구독 관리 Enum
        public enum UserSubscribeTaskInfo
        {
            SUBSCRIBE = 0,
            UNSUBSCRIBE = 1
        }
        // 애니메이션 업로드 정보 Enum
        public enum AnimeUploadInfo
        {
            GET_ALL_INFO = 0,
            GET_DATE_CHECK_INFO = 1
        }




        /// <summary>
        /// 프로그램 정보 반환
        /// </summary>
        /// <returns>프로그램 정보 JSON</returns>
        public string getProgramInfo()
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(6, null));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 업데이트 파일 다운로드
        /// </summary>
        /// <param name="path">파일 저장 경로</param>
        public void updateFileDownload(string path)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(getFullServerUrl(15, null), path);
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Auth Token 유효성 검사
        /// </summary>
        /// <param name="token">Auth Token</param>
        /// <returns>검사 결과 JSON</returns>
        public string authTokenVerify(string token)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(-1, new Dictionary<string, string> { { "token", token } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Auth Token 발급
        /// </summary>
        /// <param name="userUUID">사용자 UUID</param>
        /// <param name="autoLoginToken">사용자 자동 로그인 Tokoen</param>
        /// <returns>결과 JSON</returns>
        public string authTokenCreate(string userUUID, string autoLoginToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(-2, new Dictionary<string, string> { { "token", autoLoginToken }, { "user", userUUID } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 모든 노래, 아티스트 데이터 출력
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns>모든 노래 데이터</returns>
        public string getMusicAndSingerAllResources(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(1, new Dictionary<string, string> { { "auth", authToken }, { "all", "1" } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 모든 노래 버전 데이터 출력
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns>모든 노래 버전 데이터</returns>
        public string getMusicAllVersionResources(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(1, new Dictionary<string, string> { { "auth", authToken }, { "version", "1" } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 특정 노래 데이터 출력
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns>특정 노래 데이터</returns>
        public string getMusicResources(string authToken, string uuid)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(1, new Dictionary<string, string> { { "auth", authToken }, { "suuid", uuid } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 사용자 데이터 출력
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns>사용자 데이터</returns>
        public string getUserData(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(-3, new Dictionary<string, string> { { "token", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 사용자 부과 데이터 출력
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns>사용자 부과 데이터</returns>
        public string getUserMoreData(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(0, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 사용자 접근 확인
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns>사용자 접근 확인 결과</returns>
        public string userAccessVerify(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(5, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 사용자 구독 관리
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="userSubscribeTaskInfo">구독/구독 취소</param>
        /// <param name="artistUUID">아티스트 UUID</param>
        /// <returns></returns>
        public string userSubscribeTask(string authToken, UserSubscribeTaskInfo userSubscribeTaskInfo, string artistUUID)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(13, new Dictionary<string, string> { { "auth", authToken }, { "index", UserSubscribeTaskInfo.SUBSCRIBE == userSubscribeTaskInfo ? "0" : "1" }, { "value", artistUUID } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 최신 우타이테 정보 불러오기
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getNewUtaiteList(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(1, new Dictionary<string, string> { { "auth", authToken }, { "new", "1" } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 인기 우타이테 정보 불러오기
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getTopUtaiteList(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(12, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 사용자별 가장 많이 듣는 노래 가사별 유사도 Top30 우타이테 정보 불러오기
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getUserManyPlayMusicNgramForLyrics(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(20, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// OHLI 애니메이션 방영 정보 불러오기
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getOHLIAnimAirInfo(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(21, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 니코니코 동화 순위 데이터 불러오기
        /// </summary>
        /// <returns></returns>
        public string getNicoNicoDougaRankList()
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(16, null));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 니코니코 동화 순위 데이터 불러오기
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getIsAccessUtaiteService(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(5, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 우타이테 재생 횟수 반영
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="musicUUID">노래 UUID</param>
        public void applyUtaitePlayCount(string authToken, string musicUUID)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadString(getFullServerUrl(11, new Dictionary<string, string> { { "auth", authToken }, { "music", musicUUID } }));
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 이름 데이터 저장
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="playlistUUID">플레이리스트 UUID</param>
        /// <param name="playlistName">플레이리스트 이름</param>
        /// <returns></returns>
        public string saveMyPlaylistName(string authToken, string playlistUUID, string playlistName)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(14, new Dictionary<string, string> { { "auth", authToken }, { "index", "2" }, { "value1", "name" }, { "value2", playlistUUID }, { "value3", HttpUtility.UrlEncode(HttpUtility.UrlEncode(playlistName, Encoding.UTF8), Encoding.UTF8) } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 이미지 데이터 저장
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="playlistUUID">플레이리스트 UUID</param>
        /// <param name="playlistImage">플레이리스트 이미지</param>
        /// <returns></returns>
        public string saveMyPlaylistImage(string authToken, string playlistUUID, string playlistImage)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(14, new Dictionary<string, string> { { "auth", authToken }, { "index", "2" }, { "value1", "image" }, { "value2", playlistUUID }, { "value3", HttpUtility.UrlEncode(string.Format("_IMAGE_TYPE_{0}", playlistImage), Encoding.UTF8) } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 제거
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="playlistUUID">플레이리스트 UUID</param>
        /// <returns></returns>
        public string removeMyPlaylist(string authToken, string playlistUUID)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(14, new Dictionary<string, string> { { "auth", authToken }, { "index", "0" }, { "value1", playlistUUID }, { "value2", "" }, { "value3", "" } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 생성
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="playlistName">플레이리스트 이름</param>
        /// <param name="playlistImage">플레이리스트 이미지</param>
        /// <returns></returns>
        public string createMyPlaylist(string authToken, string playlistName, string playlistImage)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(14, new Dictionary<string, string> { { "auth", authToken }, { "index", "1" }, { "value1", HttpUtility.UrlEncode(HttpUtility.UrlEncode(playlistName, Encoding.UTF8), Encoding.UTF8) }, { "value2", HttpUtility.UrlEncode(string.Format("_IMAGE_TYPE_{0}", playlistImage), Encoding.UTF8) }, { "value3", "" } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 애니메이션 업로드 데이터 불러오기
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="animeUploadInfo">작업 저리 인자</param>
        /// <param name="date">날짜 정보</param>
        /// <returns></returns>
        public string getAnimUploadInfo(string authToken, AnimeUploadInfo animeUploadInfo, string date)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(22, new Dictionary<string, string> { { "auth", authToken }, { "smode", (animeUploadInfo == AnimeUploadInfo.GET_ALL_INFO ? 0 : 1).ToString() }, { "date", HttpUtility.UrlEncode(date, Encoding.UTF8)  } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 노래 저장
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="uuid">플레이리스트 UUID</param>
        /// <param name="data">플레이리스트 데이터</param>
        /// <returns></returns>
        public string saveMyPlaylist(string authToken, string uuid, string data)
        {
            try
            {
                WebClient webClient = new WebClient();

                const int DIV_DATA_SIZE = 1500;

                if (data.Length > DIV_DATA_SIZE)
                {
                    string requestKey = null;

                    for (int i = 0; i < (data.Length / DIV_DATA_SIZE) + 1; i++)
                    {
                        string splitStr;
                        int subStringLenForStart = DIV_DATA_SIZE * i;
                        int subStringLenForEnd = subStringLenForStart + DIV_DATA_SIZE;

                        if (data.Length >= subStringLenForEnd)
                            splitStr = data.Substring(subStringLenForStart, DIV_DATA_SIZE);
                        else
                            splitStr = data.Substring(subStringLenForStart);

                        splitStr = HttpUtility.UrlEncode(splitStr, Encoding.UTF8);
                        splitStr = HttpUtility.UrlEncode(splitStr, Encoding.UTF8);

                        if (i == 0)
                        {
                            Stream stream = webClient.OpenRead(getFullServerUrl(-4, new Dictionary<string, string> { { "mode", "0" }, { "default", splitStr } }));
                            StreamReader streamReader = new StreamReader(stream);
                            string resultForGetRequestKey = streamReader.ReadToEnd();
                            streamReader.Dispose();
                            stream.Dispose();
  

                            JObject jObject = JObject.Parse(resultForGetRequestKey);

                            if (jObject.ContainsKey("result") && jObject["result"].ToString().Equals("success") && jObject.ContainsKey("message"))
                                requestKey = jObject["message"].ToString();
                            else
                                return null;
                        }
                        else
                        {
                            int index = i - 1;

                            Stream stream = webClient.OpenRead(getFullServerUrl(-4, new Dictionary<string, string> { { "mode", "1" }, { "index", index.ToString() }, { "request", requestKey }, { "input", splitStr } }));
                            StreamReader streamReader = new StreamReader(stream);
                            string resultForGetRequestKey = streamReader.ReadToEnd();
                            streamReader.Dispose();
                            stream.Dispose();

                            JObject jObject = JObject.Parse(resultForGetRequestKey);

                            if (!(jObject.ContainsKey("result") && jObject["result"].ToString().Equals("success")))
                                return null;
                        }
                    }

                    Stream streamRoot = webClient.OpenRead(getFullServerUrl(14, new Dictionary<string, string> { { "auth", authToken }, { "index", "4" }, { "value1", HttpUtility.UrlEncode(requestKey, Encoding.UTF8) }, { "value2", HttpUtility.UrlEncode(uuid, Encoding.UTF8) }, { "value3", "" } }));

                    StreamReader streamReaderRoot = new StreamReader(streamRoot);

                    string result = streamReaderRoot.ReadToEnd();

                    streamReaderRoot.Dispose();
                    streamRoot.Dispose();
                    webClient.Dispose();

                    return result;
                }
                else
                {
                    Stream stream = webClient.OpenRead(getFullServerUrl(14, new Dictionary<string, string> { { "auth", authToken }, { "index", "3" }, { "value1", HttpUtility.UrlEncode(uuid, Encoding.UTF8) }, { "value2", HttpUtility.UrlEncode(HttpUtility.UrlEncode(data, Encoding.UTF8), Encoding.UTF8) }, { "value3", "" } }));

                    StreamReader streamReader = new StreamReader(stream);

                    string result = streamReader.ReadToEnd();

                    streamReader.Dispose();
                    stream.Dispose();
                    webClient.Dispose();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }



        /// <summary>
        /// 사용자 노래 재생 횟수 구하는 함수
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getMusicPlayCount(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(18, new Dictionary<string, string> { { "auth", authToken } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Pixiv Top 이미지 정보 구하는 함수
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <returns></returns>
        public string getPixivTopImageList(string authToken)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(getFullServerUrl(19, new Dictionary<string, string> { { "auth", authToken }, { "smode", "1" } }));
                StreamReader streamReader = new StreamReader(stream);

                string result = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                webClient.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Pixiv Top 이미지를 출력하는 함수
        /// </summary>
        /// <param name="authToken">사용자 Auth Token</param>
        /// <param name="name">이미지 이름</param>
        /// <returns></returns>
        public string getPixivTopImageURL(string authToken, string name)
        {
            try
            {
                return getFullServerUrl(19, new Dictionary<string, string> { { "auth", authToken }, { "smode", "0" }, { "name", name } });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 접속 URL 생성 함수
        /// </summary>
        /// <param name="mode">명령어 종류</param>
        /// <param name="parms">명령어 인자</param>
        /// <returns>RHYA.Network Service Utaite Player UURL</returns>
        public string getFullServerUrl(int mode, Dictionary<string, string> parms)
        {
            try
            {
                int index = 0;
                StringBuilder stringBuilder = new StringBuilder();


                switch (mode)
                {
                    // 우타이테 플레이어 관리자 URL
                    // ------------------------------------------------------
                    default:
                        stringBuilder.Append(RHYA_NETWORK_UTAITE_PLAYER_URL);
                        // 파라미터 설정
                        stringBuilder.Append("?");
                        stringBuilder.Append("mode=");
                        stringBuilder.Append(mode);

                        break;
                    // ------------------------------------------------------


                    // Auth Token 유효성 검사 URL
                    // ------------------------------------------------------
                    case -1:
                        stringBuilder.Append(RHYA_NETWORK_AUTH_TOKEN_CHECK_URL);
                        // 파라미터 설정
                        stringBuilder.Append("?");
                        stringBuilder.Append("name=");
                        stringBuilder.Append(RHYA_NETWORK_SERVICE_NAME);
                        break;
                    // ------------------------------------------------------


                    // Auth Token 발급 URL
                    // ------------------------------------------------------
                    case -2:
                        stringBuilder.Append(RHYA_NETWORK_AUTH_TOKEN_CREATE_URL);
                        // 파라미터 설정
                        stringBuilder.Append("?");
                        stringBuilder.Append("name=");
                        stringBuilder.Append(RHYA_NETWORK_SERVICE_NAME);
                        break;
                    // ------------------------------------------------------



                    // 사용자 정보 가져오기 URL
                    // ------------------------------------------------------
                    case -3:
                        stringBuilder.Append(RHYA_NETWORK_GET_USER_DATA_URL);
                        // 파라미터 설정
                        stringBuilder.Append("?");
                        stringBuilder.Append("name=");
                        stringBuilder.Append(RHYA_NETWORK_SERVICE_NAME);
                        break;
                    // ------------------------------------------------------

                    // 데이버 버퍼 URL
                    // ------------------------------------------------------
                    case -4:
                        stringBuilder.Append(RHYA_NETWORK_DATA_BUFFER_URL);
                        // 파라미터 설정
                        stringBuilder.Append("?");
                        stringBuilder.Append("nouse=0");
                        break;
                    // ------------------------------------------------------
                }


                // Null 확인
                if (parms != null)
                {
                    // 추가 파라미터 합치기
                    foreach (string key in parms.Keys)
                    {
                        stringBuilder.Append("&");
                        stringBuilder.Append(key);
                        stringBuilder.Append("=");
                        stringBuilder.Append(parms[key]);

                        checked
                        {
                            index += 1;
                        }
                    }
                }


                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
