using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.RNException;

namespace UtaitePlayer.Classes.Utils
{
    public class EqualizerSettingDataVO
    {
        // Equalizer 설정 구분 ID
        public int eq_id { get; set; }
        // Equalizer 설정 이름
        public string eq_name { get; set; }
        // Equalizer 생성 및 수정 날짜
        public string eq_date { get; set; }
        // Equalizer 설정 데이터
        public double eq_value_1 { get; set; }
        public double eq_value_2 { get; set; }
        public double eq_value_3 { get; set; }
        public double eq_value_4 { get; set; }
        public double eq_value_5 { get; set; }
        public double eq_value_6 { get; set; }
        public double eq_value_7 { get; set; }
        public double eq_value_8 { get; set; }
        public double eq_value_9 { get; set; }
        public double eq_value_10 { get; set; }
        // 사용자 정보
        public string account { get; set; }



        /// <summary>
        /// 생성자
        /// </summary>
        public EqualizerSettingDataVO(int eq_id, string eq_name, string eq_date)
        {
            try
            {
                // ID 예외 검사
                if (eq_id == -1)
                {
                    EqualizerUnknownDataVlaueException equalizerUnknownDataVlaueException = new EqualizerUnknownDataVlaueException("Equalizer id is '-1', ID cannot be negative.");
                    throw equalizerUnknownDataVlaueException;
                }
                else
                {
                    this.eq_name = eq_name;
                    this.eq_date = eq_date;
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
    }
}
