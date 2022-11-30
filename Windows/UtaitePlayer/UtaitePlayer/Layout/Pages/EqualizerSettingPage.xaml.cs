using Newtonsoft.Json.Linq;
using RHYANetwork.UtaitePlayer.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtaitePlayer.Classes.Core;
using UtaitePlayer.Classes.DataVO;
using UtaitePlayer.Classes.Utils;

namespace UtaitePlayer.Layout.Pages
{
    /// <summary>
    /// EqualizerSettingPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EqualizerSettingPage : Page
    {
        // 데이터 로딩 감지
        public bool isLoaded = false;
        // Equalizer 설정 데이터
        private List<EqualizerSettingDataVO> equalizerSettingDataVOs = new List<EqualizerSettingDataVO>();




        /// <summary>
        /// 생성자
        /// </summary>
        public EqualizerSettingPage()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 페이지 로딩 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            reload();
        }



        /// <summary>
        /// 데이터 로딩
        /// </summary>
        public async void reload()
        {
            try
            {
                if (isLoaded) return;

                // 새로고침
                await Task.Run(() => Refresh());

                isLoaded = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }



        /// <summary>
        /// 플레이리스트 데이터 다시 로딩
        /// </summary>
        public void Refresh()
        {
            try
            {
                // UI 설정
                Application.Current.Dispatcher.Invoke(() =>
                {
                    equalizerSettingDataListBox.Visibility = Visibility.Hidden;
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_SHOW_LOADING_DIALOG, "Loading...");
                });

                // EQ 데이터 초기화
                Application.Current.Dispatcher.Invoke(() =>
                {
                    equalizerSettingDataVOs.Clear();
                    equalizerSettingDataListBox.ItemsSource = equalizerSettingDataVOs;
                });

                // 데이터 다시 불러오기
                try
                {
                    RHYANetwork.UtaitePlayer.Registry.RegistryManager registryManager = new RHYANetwork.UtaitePlayer.Registry.RegistryManager();
                    RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient utaitePlayerClient = new RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient();

                    string result = utaitePlayerClient.winEQSettingManager(
                        registryManager.getAuthToken().ToString(),
                        RHYANetwork.UtaitePlayer.Client.UtaitePlayerClient.EQSettingDataMode.EQ_GET_ALL,
                        null,
                        -1,
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                    // 서버 응답 확인
                    JObject jObjectForGetUserMoreInfoJsonValue = JObject.Parse(result);
                    if (jObjectForGetUserMoreInfoJsonValue.ContainsKey("result"))
                    {
                        if (((string)jObjectForGetUserMoreInfoJsonValue["result"]).Equals("fail"))
                        {
                            // 예외 처리
                            ExceptionManager.getInstance().showMessageBox("사용자 EQ 설정 데이터 JSON 구문을 분석하는 도중 알 수 없는 오류가 발생하였습니다. 프로그램을 종료 후 다시 실행하여 주십시오.");

                            return;
                        }
                        else
                        {
                            string account = RHYANetwork.UtaitePlayer.DataManager.UserResourcesVO.getInstance().userInfoVO.id;
                            JArray array = (JArray)jObjectForGetUserMoreInfoJsonValue.GetValue("message");
                            foreach (object obj in array)
                            {
                                JObject data = JObject.Parse(((JObject)obj).ToString());

                                EqualizerSettingDataVO equalizerSettingDataVO = new EqualizerSettingDataVO((int)data["eq_id"], (string)data["eq_setting_name"], (string)data["eq_setting_date"]);
                                equalizerSettingDataVO.eq_value_1 = Convert.ToDouble(data["eq_value_60"]);
                                equalizerSettingDataVO.eq_value_2 = Convert.ToDouble(data["eq_value_170"]);
                                equalizerSettingDataVO.eq_value_3 = Convert.ToDouble(data["eq_value_300"]);
                                equalizerSettingDataVO.eq_value_4 = Convert.ToDouble(data["eq_value_600"]);
                                equalizerSettingDataVO.eq_value_5 = Convert.ToDouble(data["eq_value_1000"]);
                                equalizerSettingDataVO.eq_value_6 = Convert.ToDouble(data["eq_value_3000"]);
                                equalizerSettingDataVO.eq_value_7 = Convert.ToDouble(data["eq_value_6000"]);
                                equalizerSettingDataVO.eq_value_8 = Convert.ToDouble(data["eq_value_12000"]);
                                equalizerSettingDataVO.eq_value_9 = Convert.ToDouble(data["eq_value_14000"]);
                                equalizerSettingDataVO.eq_value_10 = Convert.ToDouble(data["eq_value_16000"]);
                                equalizerSettingDataVO.account = account;

                                equalizerSettingDataVOs.Add(equalizerSettingDataVO);
                            }
                        }
                    }
                    // 데이터 설정
                }
                catch (Exception ex)
                {
                    ExceptionManager.getInstance().showMessageBox(ex);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (equalizerSettingDataVOs.Count <= 0)
                    {
                        noResult.Visibility = Visibility.Visible;
                        equalizerSettingDataListBox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        noResult.Visibility = Visibility.Collapsed;
                        equalizerSettingDataListBox.Visibility = Visibility.Visible;
                    }

                    equalizerSettingDataListBox.Items.Refresh();

                    // 전역 Dialog 설정
                    RHYAGlobalFunctionManager.NotifyColleagues(RHYAGlobalFunctionManager.FUNCTION_KEY_HIDE_LOADING_DIALOG, null);

                    // UI 설정
                    myPlaylistRootGrid.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                ExceptionManager.getInstance().showMessageBox(ex);
            }
        }

        private void equalizerSettingDataListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
