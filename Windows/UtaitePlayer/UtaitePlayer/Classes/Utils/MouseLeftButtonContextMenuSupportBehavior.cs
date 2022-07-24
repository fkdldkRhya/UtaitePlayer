using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace UtaitePlayer.Classes.Utils
{
    /// <summary>
    /// 마우스 왼쪽 버튼 컨텍스트 메뉴 지원 동작
    /// </summary>
    public class MouseLeftButtonContextMenuSupportBehavior : Behavior<UIElement>
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 마우스 왼쪽 버튼 DOWN 허용 여부 첨부 속성 - AllowMouseLeftMouseDownProperty

        /// <summary>
        /// 마우스 왼쪽 버튼 DOWN 허용 여부 속성
        /// </summary>
        public static readonly DependencyProperty AllowMouseLeftMouseDownProperty = DependencyProperty.RegisterAttached
        (
            "AllowMouseLeftMouseDown",
            typeof(bool),
            typeof(MouseLeftButtonContextMenuSupportBehavior),
            new FrameworkPropertyMetadata(false)
        );

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 마우스 왼쪽 버튼 DOWN 허용 여부 구하기 - GetAllowMouseLeftMouseDown(element)

        /// <summary>
        /// 마우스 왼쪽 버튼 DOWN 허용 여부 구하기
        /// </summary>
        /// <param name="element">UI 엘리먼트</param>
        /// <returns>마우스 왼쪽 버튼 DOWN 허용 여부</returns>
        public static bool GetAllowMouseLeftMouseDown(UIElement element)
        {
            return (bool)element.GetValue(AllowMouseLeftMouseDownProperty);
        }

        #endregion
        #region 마우스 왼쪽 버튼 DOWN 허용 여부 설정하기 - SetAllowMouseLeftMouseDown(element, value)

        /// <summary>
        /// 마우스 왼쪽 버튼 DOWN 허용 여부 설정하기
        /// </summary>
        /// <param name="element">UI 엘리먼트</param>
        /// <param name="value">값</param>
        public static void SetAllowMouseLeftMouseDown(UIElement element, bool value)
        {
            element.SetValue(AllowMouseLeftMouseDownProperty, value);
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Instance
        //////////////////////////////////////////////////////////////////////////////// Protected

        #region 접착시 처리하기 - OnAttached()

        /// <summary>
        /// 접착시 처리하기
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.AddHandler
                (
                    UIElement.PreviewMouseLeftButtonDownEvent,
                    new RoutedEventHandler(UIElement_PreviewMouseLeftButtonDown)
                );
            }
        }

        #endregion
        #region 탈착시 처리하기 - OnDetaching()

        /// <summary>
        /// 탈착시 처리하기
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.RemoveHandler
                (
                    UIElement.PreviewMouseLeftButtonDownEvent,
                    new RoutedEventHandler(UIElement_PreviewMouseLeftButtonDown)
                );
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////// Private

        #region  UI 엘리먼트 프리뷰 마우스 왼쪽 버튼 DOWN 처리하기 - UIElement_PreviewMouseLeftButtonDown(sender, e)

        /// <summary>
        /// UI 엘리먼트 프리뷰 마우스 왼쪽 버튼 DOWN 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void UIElement_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            UIElement element = e.Source as UIElement;

            if (element != null)
            {
                if (true.Equals(element.GetValue(MouseLeftButtonContextMenuSupportBehavior.AllowMouseLeftMouseDownProperty)))
                {
                    ContextMenu contextMenu = ContextMenuService.GetContextMenu(element);

                    if (contextMenu != null)
                    {
                        MouseButtonEventArgs eventArgs = new MouseButtonEventArgs
                        (
                            Mouse.PrimaryDevice,
                            Environment.TickCount,
                            MouseButton.Right
                        );

                        eventArgs.RoutedEvent = Mouse.MouseUpEvent;
                        eventArgs.Source = element;

                        InputManager.Current.ProcessInput(eventArgs);

                        e.Handled = true;
                    }
                }
            }
        }

        #endregion
    }
}