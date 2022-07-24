using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RHYANetwork.UtaitePlayer.TrayIcon
{
    public class RhyaTrayIcon
    {
        // TrayIcon 변수
        private NotifyIcon notifyIcon = new NotifyIcon();

        // Instance
        private static RhyaTrayIcon rhyaTrayIcon = null;

        // 음소거 상태 감지 변수
        private bool isMuteMenuClick = false;

        // Action
        // -------------------------------------------------------------------------- //
        // 더블 클릭 이벤트
        public Action mActionForDoubleClick;
        public Action actionForDoubleClick 
        {
            get
            {
                return mActionForDoubleClick; 
            }

            set
            {
                mActionForDoubleClick = value;
                notifyIcon.DoubleClick += delegate (object sender, EventArgs eventArgs)
                {
                    mActionForDoubleClick();
                };
            } 
        }
        // 종료 메뉴 클릭 이벤트
        public Action actionForExitMenu { get; set; } = null;
        // 로그아웃 메뉴 클릭 이벤트
        public Action actionForLogoutMenu { get; set; } = null;
        // 음소거 메뉴 클릭 이벤트
        public Action actionForMutetMenu { get; set; } = null;
        // 음소거 메뉴 클릭 이벤트
        public Action actionForUnMutetMenu { get; set; } = null;
        // -------------------------------------------------------------------------- //




        /// <summary>
        /// 생성자
        /// </summary>
        public RhyaTrayIcon()
        {
            // TrayIcon 설정
            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/UtaitePlayer;component/UtaitePlayer.ico")).Stream;
            notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            notifyIcon.Text = "우타이테 플레이어 (Utaite Player)";

            // Context menu 설정
            ContextMenu contextMenu = new ContextMenu();


            // -------------------------------------- //
            // 사용자 계정 정보
            string userIDValue = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id;
            if (userIDValue.Length > 10)
            {
                userIDValue = userIDValue.Substring(0, 10);
                userIDValue = userIDValue + "...";
            } 
            MenuItem userIDMenu = new MenuItem();
            userIDMenu.Text = userIDValue;
            userIDMenu.Enabled = false;
            contextMenu.MenuItems.Add(userIDMenu);

            // 구분 선
            contextMenu.MenuItems.Add("-");

            // 음소거 메뉴 생성
            MenuItem muteMenu = new MenuItem();
            muteMenu.Text = "음소거";
            muteMenu.Click += delegate (object click, EventArgs eventArgs) {
                if (isMuteMenuClick)
                {
                    isMuteMenuClick = false;
                    muteMenu.Text = "음소거";
                    if (actionForUnMutetMenu != null)
                        actionForUnMutetMenu();
                }
                else
                {
                    isMuteMenuClick = true;
                    muteMenu.Text = "음소거 해제";
                    if (actionForMutetMenu != null)
                        actionForMutetMenu();
                }
            };
            contextMenu.MenuItems.Add(muteMenu);

            // 구분 선
            contextMenu.MenuItems.Add("-");

            // 로그아웃 메뉴 생성
            MenuItem logoutMenu = new MenuItem();
            logoutMenu.Text = "로그아웃";
            logoutMenu.Click += delegate (object click, EventArgs eventArgs) {
                if (actionForLogoutMenu != null)
                    actionForLogoutMenu();
            };
            contextMenu.MenuItems.Add(logoutMenu);

            // 종료 메뉴 생성
            MenuItem exitMenu = new MenuItem();
            exitMenu.Text = "종료";
            exitMenu.Click += delegate (object click, EventArgs eventArgs) {
                if (actionForExitMenu != null)
                    actionForExitMenu();
            };
            contextMenu.MenuItems.Add(exitMenu);
            // -------------------------------------- //


            // 메뉴 설정
            notifyIcon.ContextMenu = contextMenu;
        }



        /// <summary>
        /// Instance 가져오기
        /// </summary>
        /// <returns>RhyaTrayIcon Instance</returns>
        public static RhyaTrayIcon getInstance()
        {
            if (rhyaTrayIcon == null)
                rhyaTrayIcon = new RhyaTrayIcon();

            return rhyaTrayIcon;
        }



        /// <summary>
        /// TrayIcon 보여주기
        /// </summary>
        public void show()
        {
            notifyIcon.Visible = true;
        }



        /// <summary>
        /// TrayIcon 숨기기
        /// </summary>
        public void hide()
        {
            notifyIcon.Visible = false;
        }
    }
}
