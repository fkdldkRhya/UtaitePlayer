using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.Registry
{
    public class RegistryManager
    {
        // Registry Key 이름
        private readonly string REGISTRY_ROOT_SUB_KEY_NAME = "Software";
        private readonly string REGISTRY_ROOT_KEY_NAME = "UtaitePlayer";
        // Registry key List
        private readonly string REGISTRY_INSTALL_PATH_KEY_NAME = "InstallPath";
        private readonly string REGISTRY_AUTH_TOKEN_KEY_NAME = "authToken";
        private readonly string REGISTRY_DATA_ENCRYPT_DECRYPT_KEY_NAME = "cryptoKey";
        private readonly string REGISTRY_MY_PLAYLIST_INDEX_KEY_NAME = "myPlaylistIndex";
        private readonly string REGISTRY_ROOT_PROGRAM_PID_KEY_NAME = "mainPID";
        private readonly string REGISTRY_AUTH_CHECK_MANAGER_PID_KEY_NAME = "acmPID";
        private readonly string REGISTRY_AUDIO_DEVICE_ID_KEY_NAME = "audioDevice";
        private readonly string REGISTRY_START_PROGRAM_KEY_NAME = "UtaitePlayer";




        /// <summary>
        /// 프로그램 설치 경로 설정
        /// </summary>
        /// <param name="path">설치 경로</param>
        public void setInstallPath(string path)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_INSTALL_PATH_KEY_NAME, path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 설치 경로 불러오기
        /// </summary>
        /// <returns>설치 경로</returns>
        public object getInstallPath()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValue(REGISTRY_INSTALL_PATH_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 설치 경로 KEY 데이터 존재 여부 확인
        /// </summary>
        /// <returns>KEY 데이터 존재 여부 확인</returns>
        public bool isSetInstallPath()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_INSTALL_PATH_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Auth Token 설정
        /// </summary>
        /// <param name="token">Auth Token</param>
        public void setAuthToken(string token)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_AUTH_TOKEN_KEY_NAME, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Auth Token 제거
        /// </summary>
        public void deleteAuthToken()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.DeleteValue(REGISTRY_AUTH_TOKEN_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Auth Token 불러오기
        /// </summary>
        /// <returns>Auth token</returns>
        public object getAuthToken()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValue(REGISTRY_AUTH_TOKEN_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Auth Token KEY 데이터 존재 여부 확인
        /// </summary>
        /// <returns>KEY 데이터 존재 여부 확인</returns>
        public bool isSetAuthToken()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_AUTH_TOKEN_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Crypto key 설정
        /// </summary>
        /// <param name="key">Crypto key</param>
        public void setCryptoKey(string key)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_DATA_ENCRYPT_DECRYPT_KEY_NAME, key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Auth Token 제거
        /// </summary>
        public void deleteCryptoKey()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.DeleteValue(REGISTRY_DATA_ENCRYPT_DECRYPT_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Crypto key 불러오기
        /// </summary>
        /// <returns>설치 경로</returns>
        public object getCryptoKey()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValue(REGISTRY_DATA_ENCRYPT_DECRYPT_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Crypto key KEY 데이터 존재 여부 확인
        /// </summary>
        /// <returns>KEY 데이터 존재 여부 확인</returns>
        public bool isSetCryptoKey()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_DATA_ENCRYPT_DECRYPT_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 현재 플레이리스트 인덱스 불러오기
        /// </summary>
        /// <returns>현재 플레이리스트 인덱스</returns>
        public int getMyPlaylistIndex()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return (int)registryKey.GetValue(REGISTRY_MY_PLAYLIST_INDEX_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 현재 플레이리스트 인덱스 데이터 존재 여부 확인
        /// </summary>
        /// <returns>현재 플레이리스트 인덱스 존재 여부 확인</returns>
        public bool isSetMyPlaylistIndex()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_MY_PLAYLIST_INDEX_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 현재 플레이리스트 인덱스 데이터 설정
        /// </summary>
        /// <param name="index">현재 플레이리스트 인덱스</param>
        public void setMyPlaylistIndex(int index)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_MY_PLAYLIST_INDEX_KEY_NAME, index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 우타이테 플레이어 PID 불러오기
        /// </summary>
        /// <returns>우타이테 플레이어 PID</returns>
        public int getRootProgramPID()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return (int)registryKey.GetValue(REGISTRY_ROOT_PROGRAM_PID_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 우타이테 플레이어 PID 데이터 존재 여부 확인
        /// </summary>
        /// <returns>우타이테 플레이어 PID 존재 여부 확인</returns>
        public bool isSetRootProgramPID()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_ROOT_PROGRAM_PID_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 우타이테 플레이어 PID 데이터 설정
        /// </summary>
        /// <param name="pid">우타이테 플레이어 PID</param>
        public void setRootProgramPID(int pid)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_ROOT_PROGRAM_PID_KEY_NAME, pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AuthCheckManager PID 불러오기
        /// </summary>
        /// <returns>AuthCheckManager PID</returns>
        public int getAuthCheckManagerPID()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return (int)registryKey.GetValue(REGISTRY_AUTH_CHECK_MANAGER_PID_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AuthCheckManager PID 데이터 존재 여부 확인
        /// </summary>
        /// <returns>AuthCheckManager PID 존재 여부 확인</returns>
        public bool isSetAuthCheckManagerPID()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_AUTH_CHECK_MANAGER_PID_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AuthCheckManager PID 데이터 설정
        /// </summary>
        /// <param name="pid">AuthCheckManager PID</param>
        public void setAuthCheckManagerPID(int pid)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_AUTH_CHECK_MANAGER_PID_KEY_NAME, pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 시작 프로그램 등록
        /// </summary>
        public void setRunRegistry()
        {
            try
            {
                string name = "-?RhyaName:UtaitePlayer.exe";
                string path = string.Concat("-?RhyaPath:", getInstallPath().ToString().Replace(" ", "<SPC>"));

                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                registryKey.SetValue(REGISTRY_START_PROGRAM_KEY_NAME, string.Format("\"{0}\" {1} {2}", System.IO.Path.Combine(getInstallPath().ToString(), "RHYA.Network.UtaitePlayer.Launcher.exe"), name, path));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 시작 프로그램 제거
        /// </summary>
        public void deleteRunRegistry()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                registryKey.DeleteValue(REGISTRY_START_PROGRAM_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// AudioDevice ID 데이터 존재 여부 확인
        /// </summary>
        /// <returns>AudioDevice ID 존재 여부</returns>
        public bool isSetAudioDeviceID()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return registryKey.GetValueNames().Contains(REGISTRY_AUDIO_DEVICE_ID_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// AudioDevice ID 등록
        /// </summary>
        /// <param name="id">AudioDevice ID</param>
        public void setAudioDeviceID(int id)
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                registryKey.SetValue(REGISTRY_AUDIO_DEVICE_ID_KEY_NAME, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AudioDevice ID 가져오기
        /// </summary>
        /// <returns>AudioDevice ID</param>
        public int getAudioDeviceID()
        {
            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT_SUB_KEY_NAME).CreateSubKey(REGISTRY_ROOT_KEY_NAME);
                return (int)registryKey.GetValue(REGISTRY_AUDIO_DEVICE_ID_KEY_NAME);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
