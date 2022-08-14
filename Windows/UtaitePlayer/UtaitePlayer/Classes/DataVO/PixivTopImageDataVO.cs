using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UtaitePlayer.Classes.DataVO
{
    public class PixivTopImageDataVO
    {
        // 이미지
        public BitmapImage image { get; set; }
        // 이미지 URL
        public string url { get; set; }
    }
}
