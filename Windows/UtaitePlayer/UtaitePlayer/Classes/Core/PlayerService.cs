using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;
using UtaitePlayer.Classes.NAudioModule;
using System.ComponentModel;

namespace UtaitePlayer.Classes.Core
{
    public class PlayerService
    {
        // Instance
        private static PlayerService playerService;

        // Media Player
        private AudioFileReader audioFileReader = null;
        private WaveOutEvent wasapiOut = null;
        private Equalizer equalizer;
        private readonly EqualizerBand[] EQUALIZERBANDS = new EqualizerBand[]
        {
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 60, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 170, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 310, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 600, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 1000, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 3000, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 3000, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 12000, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 14000, Gain = 0},
            new EqualizerBand {Bandwidth = 0.8f, Frequency = 16000, Gain = 0}
        };

        // Event call back delegate
        public delegate void PlaybackStoppedListener();
        public delegate void MusicInfoSettingListener();

        // Event call back listener
        private PlaybackStoppedListener playbackStoppedListener = null;
        private MusicInfoSettingListener musicInfoSettingListener = null;

        // Music info
        private RHYANetwork.UtaitePlayer.DataManager.MusicInfoVO musicInfoVO = null;

        /// <summary>
        /// EqualizerBand 선택 Enum
        /// </summary>
        public enum EqualizerBandSelect
        {
            Band1, Band2, Band3, Band4, Band5, Band6, Band7, Band8, Band9, Band10
        }




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

                if (audioFileReader == null)
                {
                    audioFileReader = new AudioFileReader(url);
                }
                else
                {
                    audioFileReader.Close();
                    audioFileReader = null;
                    audioFileReader = new AudioFileReader(url);
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

                if (equalizer == null)
                {
                    equalizer = new Equalizer(audioFileReader, EQUALIZERBANDS);
                }
                else
                {
                    equalizer = null;
                    equalizer = new Equalizer(audioFileReader, EQUALIZERBANDS);
                }

                musicInfoVO = null;
                musicInfoVO = RHYANetwork.UtaitePlayer.DataManager.MusicResourcesVO.getInstance().musicResources[uuid];
                if (musicInfoSettingListener != null)
                    musicInfoSettingListener();

                wasapiOut.PlaybackStopped += WasapiOut_PlaybackStopped;
                wasapiOut.Init(equalizer);
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
            if (audioFileReader == null) return;

            audioFileReader.Volume = volume;
        }



        /// <summary>
        /// 현재 재생 시간 반환
        /// </summary>
        /// <returns>현재 재생 시간</returns>
        public TimeSpan getCurrentTime()
        {
            if (audioFileReader == null)
                return TimeSpan.Zero;

            return audioFileReader.CurrentTime;
        }



        /// <summary>
        /// 전체 재생 시간 반환
        /// </summary>
        /// <returns>전체 재생 시간</returns>
        public TimeSpan getTotalTime()
        {
            if (audioFileReader == null)
                return TimeSpan.Zero;

            return audioFileReader.TotalTime;
        }



        /// <summary>
        /// 최대 길이 반환
        /// </summary>
        /// <returns>최대 길이</returns>
        public long getLength()
        {
            if (audioFileReader == null)
                return 0;

            return audioFileReader.Length;
        }



        /// <summary>
        /// 현재 길이 반환
        /// </summary>
        /// <returns>현재 길이</returns>
        public long getPosition()
        {
            if (audioFileReader == null)
                return 0;

            return audioFileReader.Position;
        }



        /// <summary>
        /// 현재 길이 설정
        /// </summary>
        /// <param name="position">길이</param>
        public void setPosition(long position)
        {
            try
            {
                audioFileReader.Position = position;
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

                if (audioFileReader == null)
                {
                    audioFileReader = new AudioFileReader(url);
                }
                else
                {
                    audioFileReader.Close();
                    audioFileReader = null;
                    audioFileReader = new AudioFileReader(url);
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

                if (musicInfoSettingListener != null)
                    musicInfoSettingListener();

                if (equalizer == null)
                {
                    equalizer = new Equalizer(audioFileReader, EQUALIZERBANDS);
                }
                else
                {
                    equalizer = null;
                    equalizer = new Equalizer(audioFileReader, EQUALIZERBANDS);
                }

                wasapiOut.PlaybackStopped += WasapiOut_PlaybackStopped;
                wasapiOut.Init(equalizer);

                this.musicInfoVO = musicInfoVO;

                setPosition(pos);

                if (playbackState == PlaybackState.Playing)
                    playMusic();
                else if (playbackState == PlaybackState.Paused)
                    pauseMusic();
            }
            catch (Exception){ }
        }



        // =====================================================================
        // =====================================================================
        // ======================== Equalizer 제어 변수 ========================
        // =====================================================================
        // =====================================================================
        // Equalizer 설정 최솟 값
        public const float MinimumGain = -30;
        // Equalizer 설정 최대 값
        public const float MaximumGain = 30;
        /// <summary>
        /// Equalizer PropertyChanged Event
        /// </summary>
        private void OnPropertyChanged()
        {
            if (equalizer != null)
                equalizer?.Update();
        }
        /// <summary>
        /// Set equalizer band gain vlaue
        /// </summary>
        public void SetEqualizerBandGainValue(EqualizerBandSelect equalizerBandSelect, float gain)
        {
            try
            {
                int bandsIndex = (int)equalizerBandSelect;

                if (bandsIndex >= 0 && bandsIndex <= EQUALIZERBANDS.Length - 1 && EQUALIZERBANDS[bandsIndex].Gain != gain)
                {
                    EQUALIZERBANDS[bandsIndex].Gain = gain;
                    OnPropertyChanged();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get equalizer band gain vlaue
        /// </summary>
        public float GetEqualizerBandGainValue(EqualizerBandSelect equalizerBandSelect)
        {
            try
            {
                int bandsIndex = (int)equalizerBandSelect;

                if (bandsIndex >= 0 && bandsIndex <= EQUALIZERBANDS.Length - 1)
                {
                    return EQUALIZERBANDS[bandsIndex].Gain;
                }
                else
                {
                    // 예외 발생
                    EqualizerBandOutOfIndexException equalizerBandOutOfIndexException = new EqualizerBandOutOfIndexException("Equalizer를 설정하는 과정에서 배열의 길이를 넘어선 참조 및 설정이 발생하였습니다.");
                    equalizerBandOutOfIndexException.Index = bandsIndex;
                    throw equalizerBandOutOfIndexException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // =====================================================================
        // =====================================================================
        // =====================================================================
        // =====================================================================
    }
}
