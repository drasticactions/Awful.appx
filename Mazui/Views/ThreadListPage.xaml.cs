﻿using AmazingPullToRefresh.Controls;
using Mazui.Core.Models.Threads;
using Mazui.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Template10.Services.WindowService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mazui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThreadListPage : Page
    {
        public static ThreadListPage Instance { get; set; }
        public ThreadListPage()
        {
            Instance = this;
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        // strongly-typed view models enable x:bind
        public ThreadListPageViewModel ViewModel => this.DataContext as ThreadListPageViewModel;

        private async void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var thread = e.ClickedItem as Thread;
            await ViewModel.NavigateToThread(thread);
        }

        private async void OpenInNewWindow(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            ViewModel.Selected = thread;
            await WindowService.Instance.ShowAsync<ThreadPage>(JsonConvert.SerializeObject(thread));
        }

        private async void GoToLastPage(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            ViewModel.Selected = thread;
            await ViewModel.NavigateToThread(thread);
        }

        private void AddRemoveBookmark(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            ViewModel.AddRemoveBookmark(thread);
        }

        private void Unread(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            ViewModel.UnreadThread(thread);
        }

        private void PullToRefreshExtender_RefreshRequested(object sender, RefreshRequestedEventArgs e)
        {
            ViewModel.Init();
        }
    }
}
