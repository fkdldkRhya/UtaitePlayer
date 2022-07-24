using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.DataVO;

namespace UtaitePlayer.Classes.Utils
{
    public class MyPlaylistImageManager
    {
        // 이미지 리스트 - MDPI
        public List<OnlyImageDataVO> onlyImageDataVOs_mdpi = new List<OnlyImageDataVO>()
        {
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_1.png", imageID = "0" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_2.png", imageID = "1" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_3.png", imageID = "2" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_4.png", imageID = "3" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_5.png", imageID = "4" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_6.png", imageID = "5" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_7.png", imageID = "6" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_8.png", imageID = "7" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_9.png", imageID = "8" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_10.png", imageID = "9" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-mdpi/ic_playlist_custom_11.png", imageID = "10" }
        };
        // 이미지 리스트 - XXXHDPI
        public List<OnlyImageDataVO> onlyImageDataVOs_xxxhdpi = new List<OnlyImageDataVO>()
        {
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_1.png", imageID = "0" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_2.png", imageID = "1" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_3.png", imageID = "2" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_4.png", imageID = "3" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_5.png", imageID = "4" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_6.png", imageID = "5" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_7.png", imageID = "6" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_8.png", imageID = "7" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_9.png", imageID = "8" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_10.png", imageID = "9" },
            new OnlyImageDataVO() { image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_11.png", imageID = "10" }
        };




        /// <summary>
        /// 플레이리스트 이미지 경로 구하기
        /// </summary>
        /// <param name="input">이미지 번호</param>
        /// <returns></returns>
        public string getImageURL(string input)
        {
            try
            {
                string image = "/UtaitePlayer;component/Resources/drawable/img_no_data.png";

                switch (int.Parse(input))
                {
                    case 0: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_1.png"; break;
                    case 1: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_2.png"; break;
                    case 2: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_3.png"; break;
                    case 3: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_4.png"; break;
                    case 4: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_5.png"; break;
                    case 5: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_6.png"; break;
                    case 6: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_7.png"; break;
                    case 7: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_8.png"; break;
                    case 8: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_9.png"; break;
                    case 9: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_10.png"; break;
                    case 10: image = "/UtaitePlayer;component/Resources/drawable/playlist-xxxhdpi/ic_playlist_custom_11.png"; break;
                }

                return image;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
