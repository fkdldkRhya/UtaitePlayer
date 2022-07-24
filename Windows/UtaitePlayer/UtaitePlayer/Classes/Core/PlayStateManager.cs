using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.Core
{
    public class PlayStateManager
    {
        // Instance
        private static PlayStateManager playStateManager;

        // =====================================
        // Play state 변수
        // =====================================

        // 셔플 활성화, 비활성화
        // -------------------------------------
        public bool isShfful { get; set; } = false;
        // -------------------------------------

        // 반복 재생 활성화, 비활성화
        // -------------------------------------
        public enum RepeatState
        {
            NO_REPEAT = 0,
            ALL_REPEAT = 1,
            ONLY_ONE_REPEAT = 2
        }
        public RepeatState isRepeat { get; set; } = RepeatState.NO_REPEAT;
        // -------------------------------------

        // =====================================
        // =====================================

        // 노래 볼륨
        private float _MusicVolume = 1;
        public float musicVolume 
        {
            get { return _MusicVolume; }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    _MusicVolume = value;
                }
            }
        }




        /// <summary>
        /// Instance 가져오기
        /// </summary>
        /// <returns>MusicResourcesVO Instance</returns>
        public static PlayStateManager getInstance()
        {
            if (playStateManager == null)
                playStateManager = new PlayStateManager();

            return playStateManager;
        }
    }
}
