using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtaitePlayer.Classes.DataVO;

namespace UtaitePlayer.Classes.Utils
{
    public class RHYASearchHelper
    {
        // 검색 결과 - 노래
        public List<string> searchResultForMusic = null;
        // 검색 결과 - 아티스트
        public List<string> searchResultForArtist = null;




        /// <summary>
        /// 생성자
        /// </summary>
        public RHYASearchHelper()
        {
            // 리스트 초기화
            searchResultForMusic = new List<string>();
            searchResultForArtist = new List<string>();
        }
    }
}
