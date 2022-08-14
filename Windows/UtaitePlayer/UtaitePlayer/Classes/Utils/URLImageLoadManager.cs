using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UtaitePlayer.Classes.Utils
{
    public class URLImageLoadManager
    {
        // 디렉토리 이름 변수
        public static string IMAGE_MUSIC_DIRECTORY_NAME = "images_music";
        public static string IMAGE_SINGER_DIRECTORY_NAME = "images_singer";
        public static string IMAGE_ANIMATION_DIRECTORY_NAME = "images_animation";




        // 이미지 종류
        public enum ImageType
        {
            IMAGE_MUSIC,
            IMAGE_SINGER,
            IMAGE_ANIMATION
        }



        /// <summary>
        /// Image URL을 BitmapImage 형식으로 변경
        /// </summary>
        /// <param name="url">이미지 URL</param>
        /// <param name="uuid">이미지 UUID</param>
        /// <param name="wSize">DecodePixelWidth 설정</param>
        /// <param name="hSize">DecodePixelHeight 설정</param>
        /// <returns></returns>
        public static BitmapImage ImageURLToBitmapImage(string url, string uuid, int wSize, int hSize, ImageType imageType)
        {
            try
            {
                // 데이터 파일 경로
                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();

                string directoryName = null;
                switch (imageType)
                {
                    case ImageType.IMAGE_MUSIC:
                        directoryName = IMAGE_MUSIC_DIRECTORY_NAME;
                        break;
                    case ImageType.IMAGE_SINGER:
                        directoryName = IMAGE_SINGER_DIRECTORY_NAME;
                        break;
                    case ImageType.IMAGE_ANIMATION:
                        directoryName = IMAGE_ANIMATION_DIRECTORY_NAME;
                        break;
                }

                string imageFilePath = System.IO.Path.Combine(musicDataManager.DATA_FILE_SAVE_DIRECTORY_NAME, directoryName);
                string imageName = string.Format("{0}.png", uuid);
                string imagePath = System.IO.Path.Combine(imageFilePath, imageName);

                DirectoryInfo di = new DirectoryInfo(imageFilePath);
                if (!di.Exists) di.Create();

                if (!new System.IO.FileInfo(imagePath).Exists)
                {
                    using (WebClient client = new WebClient())
                        client.DownloadFile(new Uri(url), imagePath);
                }

                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.None;
                    bitmapImage.StreamSource = fs;
                    bitmapImage.DecodePixelWidth = wSize;
                    bitmapImage.DecodePixelHeight = hSize;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 내부 리소스를 BitmapImage 형식으로 변경
        /// </summary>
        /// <param name="uri">이미지 URI</param>
        /// <param name="wSize">DecodePixelWidth 설정</param>
        /// <param name="hSize">DecodePixelHeight 설정</param>
        /// <returns></returns>
        public static BitmapImage ResourceToBitmapImage(string uri, int wSize, int hSize)
        {
            try
            {
                using (var grs = Application.GetResourceStream(new Uri(uri, UriKind.RelativeOrAbsolute)).Stream)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.None;
                    bitmapImage.StreamSource = grs;
                    bitmapImage.DecodePixelWidth = wSize;
                    bitmapImage.DecodePixelHeight = hSize;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 애니메이션 폴더 초기화
        /// </summary>
        public static void ClearAnimationDirectory()
        {
            try
            {
                RHYANetwork.UtaitePlayer.DataManager.MusicDataManager musicDataManager = new RHYANetwork.UtaitePlayer.DataManager.MusicDataManager();

                string imageFilePath = System.IO.Path.Combine(musicDataManager.DATA_FILE_SAVE_DIRECTORY_NAME, IMAGE_ANIMATION_DIRECTORY_NAME);

                DirectoryInfo di = new DirectoryInfo(imageFilePath);
                if (!di.Exists) di.Create();

                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                    file.Delete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
