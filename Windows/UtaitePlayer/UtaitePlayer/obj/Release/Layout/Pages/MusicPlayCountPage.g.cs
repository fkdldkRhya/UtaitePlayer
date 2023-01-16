﻿#pragma checksum "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "96AD13CBDC5C917F8FE3A76D2EF727E2FC88AD79B7FF65178934DC9A3A5413B3"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Expression.Media;
using HandyControl.Expression.Shapes;
using HandyControl.Interactivity;
using HandyControl.Media.Animation;
using HandyControl.Media.Effects;
using HandyControl.Properties.Langs;
using HandyControl.Themes;
using HandyControl.Tools;
using HandyControl.Tools.Converter;
using HandyControl.Tools.Extension;
using LiveCharts.Wpf;
using SharpVectors.Converters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using UtaitePlayer.Layout.Pages;


namespace UtaitePlayer.Layout.Pages {
    
    
    /// <summary>
    /// MusicPlayCountPage
    /// </summary>
    public partial class MusicPlayCountPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 24 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar LoadingProgressBar;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock searchResultTitleForMusic;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid noResult;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LiveCharts.Wpf.PieChart pieChartForMusicTag;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid musicCountDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/UtaitePlayer;component/layout/pages/musicplaycountpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 20 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
            ((UtaitePlayer.Layout.Pages.MusicPlayCountPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LoadingProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 3:
            this.searchResultTitleForMusic = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.noResult = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.pieChartForMusicTag = ((LiveCharts.Wpf.PieChart)(target));
            return;
            case 6:
            this.musicCountDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            
            #line 137 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddMusicMyPlayListMenuItemForMusicCountDataGrid_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 151 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddMusicCustomMyPlayListMenuItemForMusicCountDataGrid_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 165 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ShowMusicInfoMenuItemForMusicCountDataGrid_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 10:
            
            #line 260 "..\..\..\..\Layout\Pages\MusicPlayCountPage.xaml"
            ((System.Windows.Shapes.Path)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.PlayButtonForMusicCountDataGrid_MouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
