using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UtaitePlayer.Classes.DataVO
{
    public class AnimAirInfoDataVO
    {
        // 애니메이션 UUID
        public string uuid { get; set; }
        // 애니메이션 이미지 BitmapImage 형식
        public BitmapImage bitmapImage { get; private set; }
        // 애니메이션 이미지
        public string _image;
        public string image
        {
            get
            {
                return _image;
            }
            set
            {
                if (!value.Equals("[null]"))
                {
                    _image = value;

                    bitmapImage = null;

                    // BitmapImage 변환
                    try
                    {
                        bitmapImage = Utils.URLImageLoadManager.ImageURLToBitmapImage(_image, uuid, 110, 110, Utils.URLImageLoadManager.ImageType.IMAGE_ANIMATION);
                    }
                    catch (Exception) { }
                }
                else
                {
                    // BitmapImage 변환
                    try
                    {
                        bitmapImage = Utils.URLImageLoadManager.ResourceToBitmapImage("pack://application:,,,/UtaitePlayer;component/Resources/drawable/img_no_data.png", 110, 110);
                    }
                    catch (Exception) { }
                }
            }
        }
        // 애니메이션 이름
        public string name { get; set; }
        // 애니메이션 방영일
        private string _startDate { get; set; }
        public string startDate 
        {
            get 
            {
                return string.Format("방영일: {0}", DateTime.ParseExact(_startDate, "yyyyMMdd", null).ToString("yyyy-MM-dd")); 
            } 
            set 
            { 
                _startDate = value;
            }
        }
        // 애니메이션 방영일
        private string _endtDate { get; set; }
        public string endDate 
        {
            get
            {
                if (_endtDate.Equals("99999999"))
                    return string.Format("종료일: 미정");
                else
                    return string.Format("종료일: {0}", DateTime.ParseExact(_endtDate, "yyyyMMdd", null).ToString("yyyy-MM-dd"));
            } 
            set
            {
                _endtDate = value;
            }
        }
        // 애니메이션 방영 요일
        public int dayOfTheWeek { get; set; }
        public string dayOfTheWeekStr 
        { 
            get 
            {
                string result = null;
                switch (dayOfTheWeek)
                {
                    case 0:
                        result = "일요일";
                        break;
                    case 1:
                        result = "월요일";
                        break;
                    case 2:
                        result = "화요일";
                        break;
                    case 3:
                        result = "수요일";
                        break;
                    case 4:
                        result = "목요일";
                        break;
                    case 5:
                        result = "금요일";
                        break;
                    case 6:
                        result = "토요일";
                        break;
                    case 7:
                        result = "기타";
                        break;
                }

                return result; 
            } 
        }
        // 애니메이션 방영 시간
        private string _liveTime { get; set; }
        public string liveTime 
        {
            get 
            {
                if (_liveTime.Equals("[null]"))
                    return "시간: 미정";
                else
                    return string.Format("시간: {0}", DateTime.ParseExact(_liveTime, "HHmm", null).ToString("HH:mm"));
            } 
            set
            {
                _liveTime = value;
            }
        }
        // 애니메이션 공식 사이트 주소
        public string officialSite { get; set; }
    }
}
