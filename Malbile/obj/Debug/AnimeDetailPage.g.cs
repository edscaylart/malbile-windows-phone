﻿#pragma checksum "C:\Users\Ed\Dropbox\Sistemas\Mobile\WindowsPhone\Malbile\Malbile\AnimeDetailPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7468BCF8EC5523C7D6CCC6E5A52832A0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Malbile {
    
    
    public partial class AnimeDetailPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ProgressBar LoadingBar;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.ListPicker listPickerStatus;
        
        internal System.Windows.Controls.Button btnPlusEpisode;
        
        internal System.Windows.Controls.Button btnMinusEpisode;
        
        internal Microsoft.Phone.Controls.Rating ratingScore;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btnAdd;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btnSave;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btnDelete;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Malbile;component/AnimeDetailPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.LoadingBar = ((System.Windows.Controls.ProgressBar)(this.FindName("LoadingBar")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.listPickerStatus = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("listPickerStatus")));
            this.btnPlusEpisode = ((System.Windows.Controls.Button)(this.FindName("btnPlusEpisode")));
            this.btnMinusEpisode = ((System.Windows.Controls.Button)(this.FindName("btnMinusEpisode")));
            this.ratingScore = ((Microsoft.Phone.Controls.Rating)(this.FindName("ratingScore")));
            this.btnAdd = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btnAdd")));
            this.btnSave = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btnSave")));
            this.btnDelete = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btnDelete")));
        }
    }
}

