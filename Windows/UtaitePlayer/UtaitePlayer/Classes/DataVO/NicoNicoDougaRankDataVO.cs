using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.DataVO
{
    public class NicoNicoDougaRankDataVO
    {
        // 니코니코 동화 순위
        public int rank { get; private set; }
        // 니코니코 동화 콘텐츠 ID
        public string contentId { get; private set; }
        // 니코니코동 동화 제목
        public string title { get; private set; }
        // 니코니코 동화 조회수
        public int viewCounter { get; private set; }
        // 니코니코 동화 좋아요 개수
        public int likeCounter { get; private set; }




        /// <summary>
        /// 니코니코 동화 정보 생성자
        /// </summary>
        /// <param name="rank">니코니코 동화 순위></param>
        /// <param name="contentId">니코니코 동화 콘텐츠 ID</param>
        /// <param name="title">니코니코동 동화 제목</param>
        /// <param name="viewCounter">니코니코 동화 조회수</param>
        /// <param name="likeCounter">니코니코 동화 좋아요 개수</param>
        public NicoNicoDougaRankDataVO(int rank, string contentId, string title, int viewCounter, int likeCounter)
        {
            try
            {
                this.rank = rank;
                this.contentId = contentId;
                this.title = title;
                this.viewCounter = viewCounter;
                this.likeCounter = likeCounter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
