using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UtaitePlayer.Classes.Utils
{
    public class SettingManager
    {
        /// <summary>
        /// 설정 데이터
        /// </summary>
        public class SettingData
        {
            // 컴퓨터 부팅시 실행
            public bool gs_boot_start { get; set; }
            // 비정상적인 종료 감지 사용 여부
            public bool gs_use_crash_handler { get; set; }
            // 닫는 버튼 이벤트 변경
            public int gs_close_button_event { get; set; }
            // 새로고침 버튼 활성화 여부
            public bool gs_enable_reload_btn { get; set; }
            // Topmost 설정
            public bool gs_window_top_most { get; set; }
            // 프로그램 시작 모드
            public int gs_start_mod { get; set; }
            // Equalizer Gain 60 Value
            public float ps_equalizer_gain_60 { get; set; }
            // Equalizer Gain 170 Value
            public float ps_equalizer_gain_170 { get; set; }
            // Equalizer Gain 310 Value
            public float ps_equalizer_gain_310 { get; set; }
            // Equalizer Gain 600 Value
            public float ps_equalizer_gain_600 { get; set; }
            // Equalizer Gain 1K Value
            public float ps_equalizer_gain_1000 { get; set; }
            // Equalizer Gain 3K Value
            public float ps_equalizer_gain_3000 { get; set; }
            // Equalizer Gain 6K Value
            public float ps_equalizer_gain_6000 { get; set; }
            // Equalizer Gain 120 Value
            public float ps_equalizer_gain_12000 { get; set; }
            // Equalizer Gain 140 Value
            public float ps_equalizer_gain_14000 { get; set; }
            // Equalizer Gain 16K Value
            public float ps_equalizer_gain_16000 { get; set; }
        }





        /// <summary>
        /// 설정 파일 읽기
        /// </summary>
        /// <returns>설정 파일 데이터</returns>
        public SettingData readSettingData()
        {
            try
            {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().
                    WithNamingConvention(UnderscoredNamingConvention.Instance).
                    Build();

                checkSettingFileExists();

                return deserializer.Deserialize<SettingData>(File.ReadAllText(getSettingFilePath()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 설정 파일 경로 구하기
        /// </summary>
        /// <returns>설정 파일 경로</returns>
        public string getSettingFilePath()
        {
            try
            {
                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();
                RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();

                return Path.Combine(Path.Combine(registryManager.getInstallPath().ToString(), musicDataManager.DATA_FILE_SAVE_DIRECTORY_NAME), "rnPlayerSetting.yml");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 파일 생성 확인
        /// </summary>
        public void checkSettingFileExists()
        {
            try
            {

                if (!new FileInfo(getSettingFilePath()).Exists)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(getSettingYAMLComment());
                    stringBuilder.AppendLine("gs_boot_start: false");
                    stringBuilder.AppendLine("gs_use_crash_handler: true");
                    stringBuilder.AppendLine("gs_close_button_event: 0");
                    stringBuilder.AppendLine("gs_enable_reload_btn: true");
                    stringBuilder.AppendLine("gs_window_top_most: false");
                    stringBuilder.AppendLine("gs_start_mod: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_60: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_170: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_310: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_600: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_1000: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_3000: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_6000: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_12000: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_14000: 0");
                    stringBuilder.AppendLine("ps_equalizer_gain_16000: 0");

                    File.WriteAllText(getSettingFilePath(), stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 설정 파일 내용 쓰기
        /// </summary>
        /// <param name="settingData">설정 데이터</param>
        public void writeSettingData(SettingData settingData)
        {
            try
            {
                var serializer = new SerializerBuilder().
                    WithNamingConvention(UnderscoredNamingConvention.Instance).
                    Build();
                string yaml = serializer.Serialize(settingData);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(getSettingYAMLComment());
                stringBuilder.AppendLine(yaml);

                File.WriteAllText(getSettingFilePath(), stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// YAML 설정 파일 주석 내용
        /// </summary>
        /// <returns>YAML 설정 파일 주석</returns>
        public string getSettingYAMLComment()
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("#");
                stringBuilder.AppendLine("# < Utaite Player Setting File >");
                stringBuilder.AppendLine("#");
                stringBuilder.AppendLine("# Copyright (c) RHYA.Network 2022, All rights reserved.");
                stringBuilder.AppendLine("#");
                stringBuilder.AppendLine("# This is the Utaite Player settings file. You can change the settings manually by changing the contents of the file.");
                stringBuilder.AppendLine("# CAUTION! Changing the file incorrectly may cause settings to initialize or cause errors during program execution.");
                stringBuilder.AppendLine("#");
                stringBuilder.AppendLine("");

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
