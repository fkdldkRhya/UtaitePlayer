using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.Utils
{
    /// <summary>
    /// Copyright 골드러쉬 (goldrushing). All rights reserved.
    /// https://blog.naver.com/goldrushing/221663952698
    /// 
    /// RHYA.Network Service
    /// </summary>
    public static class RHYAGlobalFunctionManager
    {
        // 키와 함수명을 갖는 전역 Dictionary
        private static IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();
        // 전역함수 호출 키
        public static readonly string FUNCTION_KEY_SHOW_AUTH_CHECK_MANAGER_DIALOG = "_FUNCTION_KEY_SHOW_AUTH_CHECK_MANAGER_DIALOG_"; // 우타이테 플레이어 사용권 확인 필요 Dialog 관리
        public static readonly string FUNCTION_KEY_REFRESH_MY_PLAYLIST = "_FUNCTION_KEY_REFRESH_MY_PLAYLIST_"; // 현재 플레이리스트 새로고침
        public static readonly string FUNCTION_KEY_RESET_MUSIC_PANEL_INFO = "_FUNCTION_KEY_RESET_MUSIC_PANEL_INFO_"; // 현재 재생 중인 노래 정보 재설정
        public static readonly string FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER = "_FUNCTION_KEY_SHOW_MUSIC_INFO_DRAWER_"; // 노래 정보 DRAWER 보여주기
        public static readonly string FUNCTION_KEY_SHOW_EDIT_PLAYLIST_DRAWER = "_FUNCTION_KEY_SHOW_EDIT_PLAYLIST_DRAWER_"; // 플레이리스트 편집 DRAWER 보여주기
        public static readonly string FUNCTION_KEY_SHOW_CREATE_PLAYLIST_DRAWER = "_FUNCTION_KEY_SHOW_CREATE_PLAYLIST_DRAWER_"; // 플레이리스트 생성 DRAWER 보여주기
        public static readonly string FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER = "_FUNCTION_KEY_SHOW_ADD_MUSIC_TO_PLAYLIST_DRAWER_"; // 플레이리스트에 노래 추가 DRAWER 보여주기
        public static readonly string FUNCTION_KEY_MUSIC_ADD_PLAYLIST = "_FUNCTION_KEY_MUSIC_ADD_PLAYLIST_"; // 현재 플레이리스트에 노래 추가하기
        public static readonly string FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY = "_FUNCTION_KEY_MUSIC_ADD_PLAYLIST_FOR_ARRAY_"; // 현재 플레이리스트에 노래 추가하기 (배열)
        public static readonly string FUNCTION_KEY_PLAY_MUSIC = "_FUNCTION_KEY_PLAY_MUSIC_"; // 노래 재생하기
        public static readonly string FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS = "_FUNCTION_KEY_SHOW_GROWL_MESSAGE_FOR_SUCCESS_"; // GROWL 보여주기 - Success
        public static readonly string FUNCTION_KEY_SHOW_LOADING_DIALOG = "_FUNCTION_KEY_SHOW_LOADING_DIALOG_"; // LOADING DAILOG 보여주기
        public static readonly string FUNCTION_KEY_HIDE_LOADING_DIALOG = "_FUNCTION_KEY_HIDE_LOADING_DIALOG_"; // LOADING DAILOG 숨기기
        public static readonly string FUNCTION_KEY_SEARCH_FOR_TEXT = "_FUNCTION_KEY_SEARCH_FOR_TEXT_"; // 데이터 검색 진행 (문자열로 검색)
        public static readonly string FUNCTION_KEY_MAIN_WINDOW_TOP_MOST_SETTING = "_FUNCTION_KEY_MAIN_WINDOW_TOP_MOST_SETTING_"; // 메인 화면 Top most 설정
        public static readonly string FUNCTION_KEY_SHOW_DIALOG_YES_OR_NO = "_FUNCTION_KEY_SHOW_DIALOG_YES_OR_NO_"; // Dialog 보여주기 - Yes or No
        public static readonly string FUNCTION_KEY_HIDE_DIALOG_YES_OR_NO = "_FUNCTION_KEY_HIDE_DIALOG_YES_OR_NO_"; // Dialog 숨기기 - Yes or No



        /// <summary>
        /// 전역함수 등록
        /// </summary>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        static public void Register(string token, Action<object> callback)
        {
            if (!pl_dict.ContainsKey(token))
            {
                var list = new List<Action<object>>();
                list.Add(callback);
                pl_dict.Add(token, list);
            }
            else
            {
                bool found = false;
                foreach (var item in pl_dict[token])
                    if (item.Method.ToString() == callback.Method.ToString())
                        found = true;
                if (!found)
                    pl_dict[token].Add(callback);
            }
        }



        /// <summary>
        /// 전역함수 해제
        /// </summary>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        static public void Unregister(string token, Action<object> callback)
        {
            if (pl_dict.ContainsKey(token))
                pl_dict[token].Remove(callback);
        }



        /// <summary>
        /// 전역함수 호출
        /// </summary>
        /// <param name="token"></param>
        /// <param name="args"></param>
        static public void NotifyColleagues(string token, object args)
        {
            if (pl_dict.ContainsKey(token))
                foreach (var callback in pl_dict[token])
                    callback(args);
        }
    }
}
