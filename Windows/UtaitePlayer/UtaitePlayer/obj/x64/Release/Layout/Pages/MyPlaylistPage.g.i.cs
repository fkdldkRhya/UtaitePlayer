﻿#pragma checksum "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "BF23B519F95D95126A932117DC58420C6157483E76FDC49D086B9EEC4AE0BA26"
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
using WpfToolkit.Controls;


namespace UtaitePlayer.Layout.Pages {
    
    
    /// <summary>
    /// MyPlaylistPage
    /// </summary>
    public partial class MyPlaylistPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 70 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid myPlaylistRootGrid;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid noResult;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox myPlaylistListBox;
        
        #line default
        #line hidden
        
        
        #line 237 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid x_PlaylistInfoLayout_RootPanel;
        
        #line default
        #line hidden
        
        
        #line 320 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image x_PlaylistInfoLayout_PlaylistImage;
        
        #line default
        #line hidden
        
        
        #line 335 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock x_PlaylistInfoLayout_MusicCount;
        
        #line default
        #line hidden
        
        
        #line 346 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock x_PlaylistInfoLayout_PlaylistTitle;
        
        #line default
        #line hidden
        
        
        #line 357 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock x_PlaylistInfoLayout_AccountName;
        
        #line default
        #line hidden
        
        
        #line 367 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock x_PlaylistInfoLayout_AccountID;
        
        #line default
        #line hidden
        
        
        #line 480 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HandyControl.Controls.CircleProgressBar loadingProgressbar;
        
        #line default
        #line hidden
        
        
        #line 488 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid noResultPanelForPlaylist;
        
        #line default
        #line hidden
        
        
        #line 513 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid x_PlaylistInfoLayout_MyPlaylistDataGrid;
        
        #line default
        #line hidden
        
        
        #line 596 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox;
        
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
            System.Uri resourceLocater = new System.Uri("/UtaitePlayer;component/layout/pages/myplaylistpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
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
            
            #line 22 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((UtaitePlayer.Layout.Pages.MyPlaylistPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 49 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CreateMyPlaylistButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.myPlaylistRootGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.noResult = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.myPlaylistListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 107 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            this.myPlaylistListBox.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.myPlaylistListBox_PreviewMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 125 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AllPlayMyPlaylistMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 141 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.EditMyPlaylistMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 155 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteMyPlaylistMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.x_PlaylistInfoLayout_RootPanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            
            #line 265 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MyPlaylistMoreInfoPanelCloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.x_PlaylistInfoLayout_PlaylistImage = ((System.Windows.Controls.Image)(target));
            return;
            case 12:
            this.x_PlaylistInfoLayout_MusicCount = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.x_PlaylistInfoLayout_PlaylistTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this.x_PlaylistInfoLayout_AccountName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 15:
            this.x_PlaylistInfoLayout_AccountID = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 16:
            
            #line 388 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_EditMyPlaylistButton_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 416 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_SaveMyPlaylistButton_Click);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 444 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_PlayMyPlaylistButton_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            this.loadingProgressbar = ((HandyControl.Controls.CircleProgressBar)(target));
            return;
            case 20:
            this.noResultPanelForPlaylist = ((System.Windows.Controls.Grid)(target));
            return;
            case 21:
            this.x_PlaylistInfoLayout_MyPlaylistDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 22:
            
            #line 538 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_MyPlaylistDataGrid_AddMusicMyPlayListMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 552 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_MyPlaylistDataGrid_ShowMusicInfoMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 566 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_MyPlaylistDataGrid_DeleteMusicMyPlayListMenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 26:
            this.x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 598 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            this.x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox.Click += new System.Windows.RoutedEventHandler(this.x_PlaylistInfoLayout_MyPlaylistDataGrid_AllSelectedCheckbox_Click);
            
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
            case 25:
            
            #line 590 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Click += new System.Windows.RoutedEventHandler(this.MyPlaylistInfoPanel_ColumnCheckBox_Click);
            
            #line default
            #line hidden
            break;
            case 27:
            
            #line 674 "..\..\..\..\..\Layout\Pages\MyPlaylistPage.xaml"
            ((System.Windows.Shapes.Path)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.x_PlaylistInfoLayout_PlayButton_MouseLeftButtonDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
