using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UtaitePlayer.Classes.DataVO
{
    public class AnimUploadInfoDataVO
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
                        bitmapImage = Utils.URLImageLoadManager.ImageURLToBitmapImage(_image, uuid, 300, 169, Utils.URLImageLoadManager.ImageType.IMAGE_ANIMATION);
                    }
                    catch (Exception) { }
                }
                else
                {
                    // BitmapImage 변환
                    try
                    {
                        bitmapImage = Utils.URLImageLoadManager.ResourceToBitmapImage("pack://application:,,,/UtaitePlayer;component/Resources/drawable/img_no_data.png", 300, 169);
                    }
                    catch (Exception) { }
                }
            }
        }
        // 애니메이션 이름
        public string name { get; set; }
        // 애니메이션 Episode
        private string _episode { get; set; }
        public string episode
        {
            get
            {
                return string.Format("Episode: {0}", _episode.Equals("[null]") ? "정보 없음" : _episode);
            }
            set
            {
                _episode = value;
            }
        }
        // 애니메이션 사이트 주소
        private string _url { get; set; }
        public string url
        {
            get
            {
                if (_url.Equals("[null]"))
                    return null;
                else
                    return _url;
            }
            set
            {
                _url = value;
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
                        result = "월요일";
                        break;
                    case 1:
                        result = "화요일";
                        break;
                    case 2:
                        result = "수요일";
                        break;
                    case 3:
                        result = "목요일";
                        break;
                    case 4:
                        result = "금요일";
                        break;
                    case 5:
                        result = "토요일";
                        break;
                    case 6:
                        result = "일요일";
                        break;
                }

                return result;
            }
        }
        // 애니메이션 데이터 갱신 날짜
        public string date { get; set; }
    }
}
