﻿#pragma checksum "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "65CBCF9CFB0173D3CE653DF1C4AB355C777FCB18F87FE63B2533075CB8A90FF5"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// AnimUploadInfoPage
    /// </summary>
    public partial class AnimUploadInfoPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 42 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl yoilSelectedTabControl;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid noResult;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox animInfolistListBox;
        
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
            System.Uri resourceLocater = new System.Uri("/UtaitePlayer;component/layout/pages/animuploadinfopage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
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
            
            #line 21 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
            ((UtaitePlayer.Layout.Pages.AnimUploadInfoPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.yoilSelectedTabControl = ((System.Windows.Controls.TabControl)(target));
            
            #line 48 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
            this.yoilSelectedTabControl.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.yoilSelectedTabControl_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.noResult = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.animInfolistListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 95 "..\..\..\..\Layout\Pages\AnimUploadInfoPage.xaml"
            this.animInfolistListBox.PreviewMouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.animInfolistListBox_PreviewMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
