using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Classes.Core
{
    public class PlayerService
    {
        // Instance
        private static PlayerService playerService;

        // Media Player
        private MediaFoundationReader mediaFoundationReader = null;
        private VolumeWaveProvider16 volumeWaveProvider16 = null;
        private WaveOutEvent wasapiOut = null;

        // Event call back delegate
        public delegate void PlaybackStoppedListener();
        public delegate void MusicInfoSettingListener();

        // Event call back listener
        private PlaybackStoppedListener playbackStoppedListener = null;
        private MusicInfoSettingListener musicInfoSettingListener = null;

        // Music info
        private RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = null;




        /// <summary>
        /// Instance 가져오기
        /// </summary>
        /// <returns>MusicResourcesVO Instance</returns>
        public static PlayerService getInstance()
        {
            if (playerService == null)
                playerService = new PlayerService();

            return playerService;
        }



        /// <summary>
        /// 노래 재생 중지 또는 노래 끝 이벤트 리스너 설정
        /// </summary>
        /// <param name="musicInfoSettingListener">MusicInfoSettingListener</param>
        public void setMusicInfoSettingListener(MusicInfoSettingListener musicInfoSettingListener)
        {
            this.musicInfoSettingListener = null;
            this.musicInfoSettingListener = musicInfoSettingListener;
        }



        /// <summary>
        /// 노래 정보 설정 이벤트 리스너 설정
        /// </summary>
        /// <param name="playbackStoppedListener">PlaybackStoppedListener</param>
        public void setPlaybackStoppedListener(PlaybackStoppedListener playbackStoppedListener)
        {
            this.playbackStoppedListener = null;
            this.playbackStoppedListener = playbackStoppedListener;
        }



        /// <summary>
        /// 노래 재생 중지
        /// </summary>
        public void stopMusic()
        {
            try
            {
                if (wasapiOut == null) return;
                
                wasapiOut.PlaybackStopped -= WasapiOut_PlaybackStopped;
                wasapiOut.Stop();
                wasapiOut.Dispose();

                musicInfoVO = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 재생
        /// </summary>
        public void playMusic()
        {
            try
            {
                if (wasapiOut == null) return;

                wasapiOut.Play();

                if (musicInfoSettingListener != null)
                    musicInfoSettingListener();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 일시정지
        /// </summary>
        public void pauseMusic()
        {
            try
            {
                if (wasapiOut == null) return;

                wasapiOut.Pause();

                if (musicInfoSettingListener != null)
                    musicInfoSettingListener();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 삽입
        /// </summary>
        /// <param name="uuid">노래 UUID</param>
        /// <param name="authToken">사용자 인증 토큰</param>
        public void putMusicForURL(string uuid, string authToken)
        {
            try
            {
                // 노래 UUID 확인
                if (!RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources.ContainsKey(uuid))
                {
                    NotFoundMusicUUIDException notFoundMusicUUIDException = new NotFoundMusicUUIDException("노래 UUID를 찾을 수 없습니다.");
                    notFoundMusicUUIDException.UUID = uuid;

                    // 예외 발생
                    throw notFoundMusicUUIDException;
                }

                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                string url = utaitePlayerClient.getFullServerUrl(8, new Dictionary<string, string>() {{ "uuid", uuid } , { "auth", authToken } });
                utaitePlayerClient.applyUtaitePlayCount(authToken, uuid);

                if (mediaFoundationReader == null)
                {
                    mediaFoundationReader = new MediaFoundationReader(url);
                }
                else
                {
                    mediaFoundationReader.Close();
                    mediaFoundationReader = null;
                    mediaFoundationReader = new MediaFoundationReader(url);
                }

                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                // Audio ID 설정
                if (!registryManager.isSetAudioDeviceID())
                {
                    for (int n = 0; n < WaveOut.DeviceCount;)
                    {
                        registryManager.setAudioDeviceID(n);
                        break;
                    }
                }

                if (wasapiOut == null)
                {
                    wasapiOut = new WaveOutEvent() { DeviceNumber = registryManager.getAudioDeviceID() };
                }
                else
                {
                    if (wasapiOut.PlaybackState == PlaybackState.Playing || wasapiOut.PlaybackState == PlaybackState.Paused)
                        stopMusic();

                    wasapiOut = null;
                    wasapiOut = new WaveOutEvent() { DeviceNumber = registryManager.getAudioDeviceID() };
                }

                if (volumeWaveProvider16 == null)
                {
                    volumeWaveProvider16 = new VolumeWaveProvider16(mediaFoundationReader);
                }
                else
                {
                    volumeWaveProvider16 = null;
                    volumeWaveProvider16 = new VolumeWaveProvider16(mediaFoundationReader);
                }

                musicInfoVO = null;
                musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[uuid];
                if (musicInfoSettingListener != null)
                    musicInfoSettingListener();

                wasapiOut.PlaybackStopped += WasapiOut_PlaybackStopped;
                wasapiOut.Init(volumeWaveProvider16);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 노래 재생 중지 또는 노래 끝 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WasapiOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // 콜백 함수 호출
            if (playbackStoppedListener != null)
                playbackStoppedListener();
        }



        /// <summary>
        /// 볼륨 설정
        /// </summary>
        /// <param name="volume">볼륨 설정 값</param>
        public void setVolume(float volume)
        {
            if (volumeWaveProvider16 == null) return;

            volumeWaveProvider16.Volume = volume;
        }



        /// <summary>
        /// 현재 재생 시간 반환
        /// </summary>
        /// <returns>현재 재생 시간</returns>
        public TimeSpan getCurrentTime()
        {
            if (mediaFoundationReader == null)
                return TimeSpan.Zero;

            return mediaFoundationReader.CurrentTime;
        }



        /// <summary>
        /// 전체 재생 시간 반환
        /// </summary>
        /// <returns>전체 재생 시간</returns>
        public TimeSpan getTotalTime()
        {
            if (mediaFoundationReader == null)
                return TimeSpan.Zero;

            return mediaFoundationReader.TotalTime;
        }



        /// <summary>
        /// 최대 길이 반환
        /// </summary>
        /// <returns>최대 길이</returns>
        public long getLength()
        {
            if (mediaFoundationReader == null)
                return 0;

            return mediaFoundationReader.Length;
        }



        /// <summary>
        /// 현재 길이 반환
        /// </summary>
        /// <returns>현재 길이</returns>
        public long getPosition()
        {
            if (mediaFoundationReader == null)
                return 0;

            return mediaFoundationReader.Position;
        }



        /// <summary>
        /// 현재 길이 설정
        /// </summary>
        /// <param name="position">길이</param>
        public void setPosition(long position)
        {
            try
            {
                mediaFoundationReader.Position = position;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 현재 플레이어 상태 불러오기
        /// </summary>
        /// <returns>플레이어 상태</returns>
        public PlaybackState getPlaybackState()
        {
            if (wasapiOut == null)
                return PlaybackState.Stopped;

            return wasapiOut.PlaybackState;
        }



        /// <summary>
        /// 현재 노래 정보 가져오기
        /// </summary>
        /// <returns>노래 정보 VO</returns>
        public RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO getMusicInfo()
        {
            return musicInfoVO;
        }



        /// <summary>
        /// 출력 장치 변경
        /// </summary>
        public void changeAudioDevice()
        {
            try
            {
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = this.musicInfoVO;

                PlaybackState playbackState = getPlaybackState();
                long pos = getPosition();

                RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();
                string url = utaitePlayerClient.getFullServerUrl(8, new Dictionary<string, string>() { { "uuid", musicInfoVO.uuid }, { "auth", registryManager.getAuthToken().ToString() } });

                if (mediaFoundationReader == null)
                {
                    mediaFoundationReader = new MediaFoundationReader(url);
                }
                else
                {
                    mediaFoundationReader.Close();
                    mediaFoundationReader = null;
                    mediaFoundationReader = new MediaFoundationReader(url);
                }

                // Audio ID 설정
                if (!registryManager.isSetAudioDeviceID())
                {
                    for (int n = 0; n < WaveOut.DeviceCount;)
                    {
                        registryManager.setAudioDeviceID(n);
                        break;
                    }
                }

                if (wasapiOut == null)
                {
                    wasapiOut = new WaveOutEvent() { DeviceNumber = registryManager.getAudioDeviceID() };
                }
                else
                {
                    if (wasapiOut.PlaybackState == PlaybackState.Playing || wasapiOut.PlaybackState == PlaybackState.Paused)
                        stopMusic();

                    wasapiOut = null;
                    wasapiOut = new WaveOutEvent() { DeviceNumber = registryManager.getAudioDeviceID() };
                }

                if (volumeWaveProvider16 == null)
                {
                    volumeWaveProvider16 = new VolumeWaveProvider16(mediaFoundationReader);
                }
                else
                {
                    volumeWaveProvider16 = null;
                    volumeWaveProvider16 = new VolumeWaveProvider16(mediaFoundationReader);
                }

                if (musicInfoSettingListener != null)
                    musicInfoSettingListener();

                wasapiOut.PlaybackStopped += WasapiOut_PlaybackStopped;
                wasapiOut.Init(volumeWaveProvider16);

                this.musicInfoVO = musicInfoVO;

                setPosition(pos);

                if (playbackState == PlaybackState.Playing)
                    playMusic();
                else if (playbackState == PlaybackState.Paused)
                    pauseMusic();
            }
            catch (Exception){ }
        }
    }
}
