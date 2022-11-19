using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RHYANetwork.UtaitePlayer.Setup.Layout.Windows
{
    /// <summary>
    /// SplashWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplashWindow : Window
    {
        // 창 닫기 확인 변수
        private bool isEndAnimation = true;



        /// <summary>
        /// 생성자
        /// </summary>
        public SplashWindow()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 창 로드 완료 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 애니메이션 설정 변수
            const double ANIM_DURATION = 0.6;
            const string ANIM_TARGETNAME = "AnimationGrid";
            
            // 변수 초기화
            isEndAnimation = true;

            // 창 비활성화 설정
            rootGrid.Visibility = Visibility.Hidden;
            // 창 위치 조절
            double setTop = (System.Windows.SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = setTop + setTop / 2;
            // 창 비활성화 해제
            rootGrid.Visibility = Visibility.Visible;
            // 시작 애니메이션
            ThicknessAnimation thicknessAnimation1 = new ThicknessAnimation();
            thicknessAnimation1.Duration = TimeSpan.FromSeconds(ANIM_DURATION);
            thicknessAnimation1.From = new Thickness(270, 0, 0, 0);
            thicknessAnimation1.To = new Thickness(0, 0, 0, 0);
            Storyboard.SetTargetName(thicknessAnimation1, ANIM_TARGETNAME);
            Storyboard.SetTargetProperty(thicknessAnimation1, new PropertyPath(Grid.MarginProperty));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(thicknessAnimation1);
            // 시작 애니메이션 종료 이벤트
            thicknessAnimation1.Completed += async (o1, s1) => {
                // 2.5초 대기
                await Task.Run(() => Thread.Sleep(2500));
                // 종료 애니메이션 실행
                ThicknessAnimation thicknessAnimation2 = new ThicknessAnimation();
                thicknessAnimation2.Duration = TimeSpan.FromSeconds(ANIM_DURATION);
                thicknessAnimation2.From = new Thickness(0, 0, 0, 0);
                thicknessAnimation2.To = new Thickness(270, 0, 0, 0);
                Storyboard.SetTargetName(thicknessAnimation2, ANIM_TARGETNAME);
                Storyboard.SetTargetProperty(thicknessAnimation2, new PropertyPath(Grid.MarginProperty));
                storyboard.Children.Clear();
                storyboard.Children.Add(thicknessAnimation2);
                // 시작 애니메이션 종료 이벤트
                thicknessAnimation2.Completed += (o2, s2) =>
                {
                    isEndAnimation = false;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    this.Close();
                };
                // 애니메이션 시작
                storyboard.Begin(AnimationGrid);
            };
            // 애니메이션 시작
            storyboard.Begin(AnimationGrid);
        }




        /// <summary>
        /// 창 닫기 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = isEndAnimation;
        }
    }
}
