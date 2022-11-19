using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RHYANetwork.UtaitePlayer.DataManager
{
    public class MusicDataManager
    {
        // 폴더 이름
        // -------------------------------------------------------------------------- //
        public readonly string DATA_FILE_SAVE_DIRECTORY_NAME = "resources";
        public readonly string LYRICS_FILE_SAVE_DIRECTORY_NAME = "lyrics";
        // -------------------------------------------------------------------------- //

        // 파일 이름
        // -------------------------------------------------------------------------- //
        public readonly string MUSIC_LIST_FILE_NAME = "rnMusicResources.dat";
        public readonly string SINGER_LIST_FILE_NAME = "rnSingerResources.dat";
        public readonly string PLAYLIST_FILE_NAME = "rnPlaylistResources.dat";
        // -------------------------------------------------------------------------- //

        // Json key 이름
        // -------------------------------------------------------------------------- //
        private readonly string JSON_KEY_MUSIC_UUID = "uuid";
        private readonly string JSON_KEY_MUSIC_NAME = "name";
        private readonly string JSON_KEY_MUSIC_SINGER_UUID = "singeruuid";
        private readonly string JSON_KEY_MUSIC_SINGER_IMAGE = "singerimage";
        private readonly string JSON_KEY_MUSIC_SINGER_NAME = "singer";
        private readonly string JSON_KEY_MUSIC_SONG_WRITER = "songwriter";
        private readonly string JSON_KEY_MUSIC_LYRICS = "lyrics";
        private readonly string JSON_KEY_MUSIC_IMAGE = "image";
        private readonly string JSON_KEY_MUSIC_MP3 = "mp3";
        private readonly string JSON_KEY_MUSIC_TYPE = "type";
        private readonly string JSON_KEY_MUSIC_DATE = "date";
        private readonly string JSON_KEY_MUSIC_VERSION = "version";
        private readonly string JSON_KEY_MUSIC_TEMP_1 = "temp1";
        private readonly string JSON_KEY_MUSIC_TEMP_2 = "temp2";
        private readonly string JSON_KEY_USER_UUID = "uuid";
        private readonly string JSON_KEY_USER_NAME = "name";
        private readonly string JSON_KEY_USER_ID = "id";
        private readonly string JSON_KEY_USER_EMAIL = "email";
        private readonly string JSON_KEY_USER_REGDATE = "regdate";
        private readonly string JSON_KEY_USER_BIRTHDAY = "birthday";
        private readonly string JSON_KEY_USER_PLAYLIST = "play_list";
        private readonly string JSON_KEY_USER_SUBSCRIBE = "subscribe_list";
        // -------------------------------------------------------------------------- //

        // 기타 데이터
        // -------------------------------------------------------------------------- //
        private readonly string MY_PLAYLIST_NAME_KEY = "_NAME_";
        private readonly string MY_PLAYLIST_NAME_IMAGE = "_IMAGE_TYPE_";
        private readonly string MY_SUBSCRIBE_LIST_KEY = "list";
        // -------------------------------------------------------------------------- //




        /// <summary>
        /// 생성자
        /// </summary>
        public MusicDataManager()
        {
            // Resources 폴더 확인
            System.IO.DirectoryInfo directoryInfo1 = new System.IO.DirectoryInfo(DATA_FILE_SAVE_DIRECTORY_NAME);
            if (!directoryInfo1.Exists)
                directoryInfo1.Create();

            // Lyrics 폴더 확인
            System.IO.DirectoryInfo directoryInfo2 = new System.IO.DirectoryInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, LYRICS_FILE_SAVE_DIRECTORY_NAME));
            if (!directoryInfo2.Exists)
                directoryInfo2.Create();
        }



        /// <summary>
        /// 데이터 파일 존재 유무 확인
        /// </summary>
        /// <returns>존재 또는 존재하지 않음</returns>
        public bool isExistsResourceFile()
        {
            return new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, MUSIC_LIST_FILE_NAME)).Exists 
                && new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, SINGER_LIST_FILE_NAME)).Exists;
        }



        /// <summary>
        /// 노래 데이터 -> JSON 변환 및 저장 [노래 데이터]
        /// </summary>
        /// <param name="musicResources">노래 데이터</param>
        public void writeMusicResourcesFile(Dictionary<string, MusicInfoVO> musicResources)
        {
            try
            {
                // 파일 데이터 생성
                JArray rootObject = new JArray();
                // 노래 데이터 읽기
                foreach (string songUUID in musicResources.Keys)
                {
                    // 노래 데이터 생성
                    MusicInfoVO musicInfoVO = musicResources[songUUID];
                    JObject musicObject = new JObject();
                    musicObject.Add(JSON_KEY_MUSIC_UUID, musicInfoVO.uuid);
                    musicObject.Add(JSON_KEY_MUSIC_NAME, musicInfoVO.name);
                    musicObject.Add(JSON_KEY_MUSIC_SINGER_UUID, musicInfoVO.singerUUID);
                    musicObject.Add(JSON_KEY_MUSIC_SONG_WRITER, musicInfoVO.songWriter);
                    musicObject.Add(JSON_KEY_MUSIC_IMAGE, musicInfoVO.image);
                    musicObject.Add(JSON_KEY_MUSIC_MP3, musicInfoVO.mp3);
                    musicObject.Add(JSON_KEY_MUSIC_TYPE, musicInfoVO.type);
                    musicObject.Add(JSON_KEY_MUSIC_DATE, musicInfoVO.date);
                    musicObject.Add(JSON_KEY_MUSIC_VERSION, musicInfoVO.version);
                    musicObject.Add(JSON_KEY_MUSIC_TEMP_1, musicInfoVO.temp1);
                    musicObject.Add(JSON_KEY_MUSIC_TEMP_2, musicInfoVO.temp2);

                    // 데이터 추가
                    rootObject.Add(musicObject);
                }

                // 데이터 암호화
                string writeData = rootObject.ToString();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto(); 
                if (registryManager.isSetCryptoKey())
                {
                    writeData = aesCrypto.encryptAES(writeData, (string) registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, MUSIC_LIST_FILE_NAME), writeData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// JSON 데이터 -> 노래 데이터 [노래 데이터]
        /// </summary>
        /// <returns>노래 데이터</returns>
        public Dictionary<string, MusicInfoVO> readMusicResourcesFile()
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();

                Dictionary<string, MusicInfoVO> musicResources = new Dictionary<string, MusicInfoVO>();

                // 노래 데이터 읽기
                string readValue = System.IO.File.ReadAllText(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, MUSIC_LIST_FILE_NAME));
                // 노래 데이터 복호화
                if (registryManager.isSetCryptoKey())
                {
                    readValue = aesCrypto.decryptAES(readValue, (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                    // JSON 변환
                    JArray rootObject = JArray.Parse(readValue);
                    foreach (JObject valueObject in rootObject)
                    {
                        // 키 확인
                        if (valueObject.ContainsKey(JSON_KEY_MUSIC_UUID) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_NAME) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_SINGER_UUID) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_SONG_WRITER) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_IMAGE) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_MP3) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_TYPE) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_DATE) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_VERSION) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_TEMP_1) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_TEMP_2)) {

                            MusicInfoVO musicInfoVO = new MusicInfoVO(
                                (string)valueObject[JSON_KEY_MUSIC_UUID],
                                (string)valueObject[JSON_KEY_MUSIC_NAME],
                                (string)valueObject[JSON_KEY_MUSIC_SINGER_UUID],
                                (string)valueObject[JSON_KEY_MUSIC_SONG_WRITER],
                                (string)valueObject[JSON_KEY_MUSIC_IMAGE],
                                (string)valueObject[JSON_KEY_MUSIC_MP3],
                                (string)valueObject[JSON_KEY_MUSIC_TYPE],
                                (string)valueObject[JSON_KEY_MUSIC_DATE],
                                (int)valueObject[JSON_KEY_MUSIC_VERSION],
                                (int)valueObject[JSON_KEY_MUSIC_TEMP_1],
                                (int)valueObject[JSON_KEY_MUSIC_TEMP_2]);


                            musicResources.Add(musicInfoVO.uuid, musicInfoVO);
                        }
                    }

                    return musicResources;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 데이터 -> JSON 변환 및 저장 [아티스트 데이터]
        /// </summary>
        /// <param name="singerResources">아티스트 데이터</param>
        public void writeSingerResourcesFile(Dictionary<string, SingerInfoVO> singerResources)
        {
            try
            {
                // 파일 데이터 생성
                JArray rootObject = new JArray();
                // 노래 데이터 읽기
                foreach (string songUUID in singerResources.Keys)
                {
                    // 노래 데이터 생성
                    SingerInfoVO singerInfoVO = singerResources[songUUID];
                    JObject singerObject = new JObject();
                    singerObject.Add(JSON_KEY_MUSIC_SINGER_UUID, singerInfoVO.uuid);
                    singerObject.Add(JSON_KEY_MUSIC_SINGER_NAME, singerInfoVO.name);
                    singerObject.Add(JSON_KEY_MUSIC_SINGER_IMAGE, singerInfoVO.image);

                    // 데이터 추가
                    rootObject.Add(singerObject);
                }

                // 데이터 암호화
                string writeData = rootObject.ToString();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();
                if (registryManager.isSetCryptoKey())
                {
                    writeData = aesCrypto.encryptAES(writeData, (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, SINGER_LIST_FILE_NAME), writeData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// JSON 데이터 -> 아티스트 데이터 [아티스트 데이터]
        /// </summary>
        /// <returns>아티스트 데이터</returns>
        public Dictionary<string, SingerInfoVO> readSingerResourcesFile()
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();

                Dictionary<string, SingerInfoVO> singerResources = new Dictionary<string, SingerInfoVO>();

                // 노래 데이터 읽기
                string readValue = System.IO.File.ReadAllText(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, SINGER_LIST_FILE_NAME));
                // 노래 데이터 복호화
                if (registryManager.isSetCryptoKey())
                {
                    readValue = aesCrypto.decryptAES(readValue, (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                    // JSON 변환
                    JArray rootObject = JArray.Parse(readValue);
                    foreach (JObject valueObject in rootObject)
                    {
                        // 키 확인
                        if (valueObject.ContainsKey(JSON_KEY_MUSIC_SINGER_UUID) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_SINGER_NAME) &&
                            valueObject.ContainsKey(JSON_KEY_MUSIC_SINGER_IMAGE))
                        {

                            SingerInfoVO singerInfoVO = new SingerInfoVO(
                                (string)valueObject[JSON_KEY_MUSIC_SINGER_UUID],
                                (string)valueObject[JSON_KEY_MUSIC_SINGER_NAME],
                                (string)valueObject[JSON_KEY_MUSIC_SINGER_IMAGE]);


                            singerResources.Add(singerInfoVO.uuid, singerInfoVO);
                        }
                    }

                    return singerResources;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 서버 응답 JSON -> 노래, 가수 데이터 설정
        /// </summary>
        /// <param name="jsonValue">서버 응답 JSON 데이터</param>
        public void musicAndSingerResourcesParser(string jsonValue)
        {
            try
            {
                // 초기화
                MusicResourcesVO.getInstance().clearMusicResources();
                MusicResourcesVO.getInstance().clearSingerResources();

                // 모듈 선언
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();

                // 데이터 변수 선언
                Dictionary<string, MusicInfoVO> musicResources = new Dictionary<string, MusicInfoVO>();
                Dictionary<string, SingerInfoVO> singerResources = new Dictionary<string, SingerInfoVO>();
                JObject rootObject = JObject.Parse(jsonValue);
                IList<string> keys = rootObject.Properties().Select(p => p.Name).ToList();
                foreach (string key in keys)
                {
                    JObject subOject = JObject.Parse(rootObject[key].ToString());

                    // 노래 데이터
                    // ---------------------------------------- //
                    // 키 확인
                    if (subOject.ContainsKey(JSON_KEY_MUSIC_UUID) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_NAME) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_SINGER_UUID) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_SONG_WRITER) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_IMAGE) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_LYRICS) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_MP3) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_TYPE) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_DATE) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_VERSION))
                    {
                        MusicInfoVO musicInfoVO = new MusicInfoVO(
                            (string)subOject[JSON_KEY_MUSIC_UUID],
                            (string)subOject[JSON_KEY_MUSIC_NAME],
                            (string)subOject[JSON_KEY_MUSIC_SINGER_UUID],
                            (string)subOject[JSON_KEY_MUSIC_SONG_WRITER],
                            (string)subOject[JSON_KEY_MUSIC_IMAGE],
                            (string)subOject[JSON_KEY_MUSIC_MP3],
                            (string)subOject[JSON_KEY_MUSIC_TYPE],
                            (string)subOject[JSON_KEY_MUSIC_DATE],
                            (int)subOject[JSON_KEY_MUSIC_VERSION],
                            -1,
                            -1);

                        if (registryManager.isSetCryptoKey())
                        {
                            string writeData = aesCrypto.encryptAES(HttpUtility.UrlDecode((string)subOject[JSON_KEY_MUSIC_LYRICS], Encoding.UTF8), (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                            System.IO.File.WriteAllText(getLyricsFileName(musicInfoVO.uuid), writeData);
                        }

                        MusicResourcesVO.getInstance().addMusicResources(musicInfoVO);
                    }
                    // ---------------------------------------- //


                    // 아티스트 데이터
                    // ---------------------------------------- //
                    if (subOject.ContainsKey(JSON_KEY_MUSIC_SINGER_UUID) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_SINGER_NAME) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_SINGER_IMAGE))
                    {
                        SingerInfoVO singerInfoVO = new SingerInfoVO(
                            (string)subOject[JSON_KEY_MUSIC_SINGER_UUID],
                            (string)subOject[JSON_KEY_MUSIC_SINGER_NAME],
                            (string)subOject[JSON_KEY_MUSIC_SINGER_IMAGE]);

                        MusicResourcesVO.getInstance().addSingerResources(singerInfoVO);
                    }
                    // ---------------------------------------- //
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 가사 파일 경로 구하는 함수
        /// </summary>
        /// <param name="uuid">노래 UUID</param>
        /// <returns>가사 파일 경로 구하기</returns>
        public string getLyricsFileName(string uuid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("rnLyrics-");
            stringBuilder.Append(uuid);
            stringBuilder.Append(".dat");

            return System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, System.IO.Path.Combine(LYRICS_FILE_SAVE_DIRECTORY_NAME, stringBuilder.ToString()));
        }



        /// <summary>
        /// 노래 정보 동기화 진행
        /// </summary>
        /// <param name="versionJsonValue"></param>
        public void checkMusicResourceVersion(string versionJsonValue)
        {
            try
            {
                // 데이터 변수 선언
                JObject rootObject = JObject.Parse(versionJsonValue);
                IList<string> keys = rootObject.Properties().Select(p => p.Name).ToList();
                foreach (string key in keys)
                {
                    JObject subOject = JObject.Parse(rootObject[key].ToString());

                    // 키 확인
                    if (subOject.ContainsKey(JSON_KEY_MUSIC_UUID) &&
                        subOject.ContainsKey(JSON_KEY_MUSIC_VERSION))
                    {
                        bool isReload = false;
                        

                        string uuid = (string)subOject[JSON_KEY_MUSIC_UUID];
                        string version = (string)subOject[JSON_KEY_MUSIC_VERSION];

                        if (MusicResourcesVO.getInstance().musicResources.ContainsKey(uuid))
                        {
                            try
                            {
                                // 데이터 버전 확인
                                int versionToInt = int.Parse(version);
                                if (MusicResourcesVO.getInstance().musicResources[uuid].version != versionToInt)
                                    isReload = true;
                                
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            isReload = true;
                        }


                        // 동기화 여부 확인
                        if (isReload)
                        {
                            // 노래 데이터 동기화
                            RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();
                            RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                            RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new Client.UtaitePlayerClient();
                            string responseValue = utaitePlayerClient.getMusicResources((string)registryManager.getAuthToken(), uuid);

                            JObject musicResource = JObject.Parse(responseValue);
                            // 노래 데이터
                            // ---------------------------------------- //
                            // 키 확인
                            if (musicResource.ContainsKey(JSON_KEY_MUSIC_UUID) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_NAME) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_SINGER_UUID) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_SONG_WRITER) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_IMAGE) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_LYRICS) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_MP3) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_TYPE) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_DATE) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_VERSION))
                            {
                                MusicInfoVO musicInfoVO = new MusicInfoVO(
                                    (string)musicResource[JSON_KEY_MUSIC_UUID],
                                    (string)musicResource[JSON_KEY_MUSIC_NAME],
                                    (string)musicResource[JSON_KEY_MUSIC_SINGER_UUID],
                                    (string)musicResource[JSON_KEY_MUSIC_SONG_WRITER],
                                    (string)musicResource[JSON_KEY_MUSIC_IMAGE],
                                    (string)musicResource[JSON_KEY_MUSIC_MP3],
                                    (string)musicResource[JSON_KEY_MUSIC_TYPE],
                                    (string)musicResource[JSON_KEY_MUSIC_DATE],
                                    (int)musicResource[JSON_KEY_MUSIC_VERSION],
                                    -1,
                                    -1);

                                if (registryManager.isSetCryptoKey())
                                {
                                    string writeData = aesCrypto.encryptAES(HttpUtility.UrlDecode((string)musicResource[JSON_KEY_MUSIC_LYRICS], Encoding.UTF8), (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                                    System.IO.File.WriteAllText(getLyricsFileName(musicInfoVO.uuid), writeData);
                                }

                                MusicResourcesVO.getInstance().removeMusicResources(uuid);
                                MusicResourcesVO.getInstance().addMusicResources(musicInfoVO);
                            }
                            // ---------------------------------------- //

                            // 아티스트 데이터
                            // ---------------------------------------- //
                            if (musicResource.ContainsKey(JSON_KEY_MUSIC_SINGER_UUID) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_SINGER_NAME) &&
                                musicResource.ContainsKey(JSON_KEY_MUSIC_SINGER_IMAGE))
                            {
                                SingerInfoVO singerInfoVO = new SingerInfoVO(
                                    (string)musicResource[JSON_KEY_MUSIC_SINGER_UUID],
                                    (string)musicResource[JSON_KEY_MUSIC_SINGER_NAME],
                                    (string)musicResource[JSON_KEY_MUSIC_SINGER_IMAGE]);

                                MusicResourcesVO.getInstance().removeSingerResources(uuid);
                                MusicResourcesVO.getInstance().addSingerResources(singerInfoVO);
                            }
                            // ---------------------------------------- //
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// JSON 데이터 -> 플레이리스트 데이터 [플레이리스트 데이터]
        /// </summary>
        public void readPlaylistResourcesFile()
        {
            try
            {
                // 파일 확인
                if (!new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, PLAYLIST_FILE_NAME)).Exists) return;

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();

                // 데이터 초기화
                UserResourcesVO.getInstance().clearUserPlaylistInfoVO();

                // 플레이리스트 데이터 읽기
                string readValue = System.IO.File.ReadAllText(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, PLAYLIST_FILE_NAME));
                // 플레이리스트 데이터 복호화
                if (registryManager.isSetCryptoKey())
                {
                    readValue = aesCrypto.decryptAES(readValue, (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                    // JSON 변환
                    JArray rootObject = JArray.Parse(readValue);
                    foreach (string musicUUID in rootObject)
                    {
                        UserPlaylistInfoVO userPlaylistInfoVO = new UserPlaylistInfoVO();
                        userPlaylistInfoVO.uuid = musicUUID;

                        if (userPlaylistInfoVO.uuid != null)
                            UserResourcesVO.getInstance().addUserPlaylistInfoVO(userPlaylistInfoVO);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 데이터 -> JSON 변환 및 저장 [플레이리스트 데이터]
        /// </summary>
        public void writePlaylistResourcesFile()
        {
            try
            {
                // 파일 데이터 생성
                JArray rootObject = new JArray();
                // 플레이리스트 데이터 읽기
                for (int index = 0; index < UserResourcesVO.getInstance().userPlaylistInfoVOs.Count; index ++)
                {
                    rootObject.Add(UserResourcesVO.getInstance().userPlaylistInfoVOs[index].uuid);
                }

                // 데이터 암호화
                string writeData = rootObject.ToString();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new Registry.RegistryManager();
                RHYANetwork.UtaitePlayer.CryptoModule.AESCrypto aesCrypto = new CryptoModule.AESCrypto();
                if (registryManager.isSetCryptoKey())
                {
                    writeData = aesCrypto.encryptAES(writeData, (string)registryManager.getCryptoKey(), aesCrypto.MAIN_ENCRYPT_DECRYPT_IV, CryptoModule.AESCrypto.AESKeySize.SIZE_128);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, PLAYLIST_FILE_NAME), writeData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 서버 응답 JSON -> 사용자 데이터 설정 
        /// </summary>
        /// <param name="jsonValue">서버 응답 JSON</param>
        public void userDataResourcesParser(string jsonValue)
        {
            try
            {
                // 데이터 초기화
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.uuid = null;
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.name = null;
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id = null;
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.birthday = null;
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.email = null;
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.date = null;

                JObject rootObject = JObject.Parse(jsonValue);
                // 키 확인
                if (rootObject.ContainsKey(JSON_KEY_USER_UUID) &&
                    rootObject.ContainsKey(JSON_KEY_USER_NAME) &&
                    rootObject.ContainsKey(JSON_KEY_USER_ID) &&
                    rootObject.ContainsKey(JSON_KEY_USER_EMAIL) &&
                    rootObject.ContainsKey(JSON_KEY_USER_REGDATE) &&
                    rootObject.ContainsKey(JSON_KEY_USER_BIRTHDAY))
                {
                    // 데이터 추출
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.uuid = HttpUtility.UrlDecode((string)rootObject[JSON_KEY_USER_UUID], Encoding.UTF8);
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.name = HttpUtility.UrlDecode((string)rootObject[JSON_KEY_USER_NAME], Encoding.UTF8);
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id = HttpUtility.UrlDecode((string)rootObject[JSON_KEY_USER_ID], Encoding.UTF8);
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.birthday = HttpUtility.UrlDecode((string)rootObject[JSON_KEY_USER_BIRTHDAY], Encoding.UTF8);
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.email = HttpUtility.UrlDecode((string)rootObject[JSON_KEY_USER_EMAIL], Encoding.UTF8);
                    RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.date = HttpUtility.UrlDecode((string)rootObject[JSON_KEY_USER_REGDATE], Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 서버 응답 JSON -> 사용자 구독, 커스텀 플레이리스트 데이터 설정 
        /// </summary>
        /// <param name="jsonValue">서버 응답 JSON</param>
        public void userSubscribeAndPlaylistResourcesParser(string jsonValue)
        {
            try
            {
                // 데이터 초기화
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userSubscribeInfoVOs.Clear();
                RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.Clear();

                // 데이터 읽기
                JObject rootObject = JObject.Parse(jsonValue);
                // 키 확인
                if (rootObject.ContainsKey(JSON_KEY_USER_PLAYLIST) && rootObject.ContainsKey(JSON_KEY_USER_SUBSCRIBE))
                {
                    // 데이터 추출
                    string playlistJsonValue = (string)rootObject[JSON_KEY_USER_PLAYLIST];
                    string subscribeListJsonValue = (string)rootObject[JSON_KEY_USER_SUBSCRIBE];

                    // 데이터 적용 - 사용자 플레이리스트
                    JObject playListObject = JObject.Parse(HttpUtility.UrlDecode(playlistJsonValue, Encoding.UTF8));
                    IList<string> keysForPlaylist = playListObject.Properties().Select(p => p.Name).ToList();
                    foreach (string uuid in keysForPlaylist)
                    {
                        UserCustomPlaylistInfoVO userCustomPlaylistInfoVO = new UserCustomPlaylistInfoVO(uuid);

                        string playlistName = null;
                        string playlistImage = null;

                        JArray jArray = JArray.Parse(HttpUtility.UrlDecode(playListObject[uuid].ToString(), Encoding.UTF8));
                        foreach (string arrayValue in jArray)
                        {
                            if (arrayValue.Contains(MY_PLAYLIST_NAME_KEY)) // 플레이리스트 이름 설정
                            {
                                playlistName = arrayValue.Replace(MY_PLAYLIST_NAME_KEY, "");
                            }
                            else if (arrayValue.Contains(MY_PLAYLIST_NAME_IMAGE)) // 플레이리스트 이미지 설정
                            {
                                playlistImage = arrayValue.Replace(MY_PLAYLIST_NAME_IMAGE, "");
                            }
                            else // 노래 추가
                            {
                                UserPlaylistInfoVO userPlaylistInfoVO = new UserPlaylistInfoVO();
                                userPlaylistInfoVO.uuid = arrayValue;

                                if (userPlaylistInfoVO.uuid != null)
                                    userCustomPlaylistInfoVO.addUserPlaylistInfoVO(userPlaylistInfoVO);
                            }
                        }

                        userCustomPlaylistInfoVO.name = playlistName;
                        userCustomPlaylistInfoVO.image = playlistImage;

                        // 설정 확인
                        if (playlistName != null && playlistImage != null)
                        {
                            UserResourcesVO.getInstance().userCustomPlaylistInfoVOs.Add(uuid, userCustomPlaylistInfoVO);
                        }
                    }

                    // 데이터 적용 - 구독 리스트
                    JObject subscribeObject = JObject.Parse(HttpUtility.UrlDecode(subscribeListJsonValue, Encoding.UTF8));
                    if (subscribeObject.ContainsKey(MY_SUBSCRIBE_LIST_KEY))
                    {
                        JArray subscribeArrayMain = JArray.Parse(subscribeObject[MY_SUBSCRIBE_LIST_KEY].ToString());
                        foreach (string uuid in subscribeArrayMain)
                        {
                            UserSubscribeInfoVO userSubscribeInfoVO = new UserSubscribeInfoVO();
                            userSubscribeInfoVO.uuid = uuid;

                            // 설정 확인
                            if (userSubscribeInfoVO.uuid != null)
                            {
                                UserResourcesVO.getInstance().userSubscribeInfoVOs.Add(userSubscribeInfoVO);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 플레이리스트 파일 제거
        /// </summary>
        public void deletePlaylistFile()
        {
            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, PLAYLIST_FILE_NAME));

                // 파일 확인
                if (!fileInfo.Exists) return;

                // 파일 제거
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 모든 리소스 파일 제거
        /// </summary>
        public void deleteAllResourceFile()
        {
            try
            {
                System.IO.FileInfo fileInfo1 = new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, PLAYLIST_FILE_NAME));
                System.IO.FileInfo fileInfo2 = new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, MUSIC_LIST_FILE_NAME));
                System.IO.FileInfo fileInfo3 = new System.IO.FileInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, SINGER_LIST_FILE_NAME));

                if (fileInfo1.Exists) fileInfo1.Delete();
                if (fileInfo2.Exists) fileInfo2.Delete();
                if (fileInfo3.Exists) fileInfo3.Delete();

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.IO.Path.Combine(DATA_FILE_SAVE_DIRECTORY_NAME, LYRICS_FILE_SAVE_DIRECTORY_NAME));
                foreach (System.IO.FileInfo File in di.GetFiles())
                    File.Delete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
