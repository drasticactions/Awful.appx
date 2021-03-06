﻿using Awful.Controls;
using Awful.Parser.Models.Threads;
using Awful.Services;
using Awful.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Awful.Views
{
    public sealed partial class ThreadListPage : Page
    {
        public ThreadListPageViewModel ViewModel => this.DataContext as ThreadListPageViewModel;

        public ThreadListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            //ThreadPageView.MasterDetailViewControl = previewControl;
            ViewModel.MasterDetailViewControl = previewControl;
            //ViewModel.ThreadView = ThreadPageView;
            ViewModel.Tabs = Tabs;
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            previewControl.Loaded();
        }

        private void App_Suspending(object sender, SuspendingEventArgs e)
        {
            previewControl.Unloaded();
        }

        private void App_Resuming(object sender, object e)
        {
            previewControl.Loaded();
        }

        private void RefreshContainer_OnRefreshRequested(
            RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            if (!ViewModel.IsLoading)
                ViewModel.Init();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            previewControl.OnNavigated();
            if (e.Parameter == null) return; 
            ViewModel.Load(e.Parameter as string);
            //if (ViewModel.Selected == null)
            //    await ThreadPageView.LoadBaseView();
            App.ShellViewModel.BackNavigated -= NavigationService.BackRequested;
            App.ShellViewModel.BackNavigated += previewControl.NavigationManager_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            previewControl.FromNavigated();
            App.ShellViewModel.BackNavigated -= previewControl.NavigationManager_BackRequested;
            App.ShellViewModel.BackNavigated += NavigationService.BackRequested;
        }

        private async void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var thread = e.ClickedItem as Thread;
            if (thread == null)
                return;
            ViewModel.Selected = thread;
            ViewModel.Tabs.Items.Add(new TabViewItem()
            {
                Name = thread.ThreadId.ToString(),
                Header = thread.Name,
                Content = new ThreadView()
            });
            //await ThreadPageView.LoadThread(thread);
            ViewModel.IsThreadSelectedAndLoaded = true;
        }

        private async void GoToLastPage(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            ViewModel.Selected = thread;
            //await ThreadPageView.LoadThread(thread, false, true);
            ViewModel.IsThreadSelectedAndLoaded = true;
        }

        private async void AddRemoveBookmark(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            await ViewModel.AddRemoveBookmark(thread);
        }

        private async void Unread(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var thread = imageSource?.CommandParameter as Thread;
            if (thread == null)
                return;
            await ViewModel.UnreadThread(thread);
        }
    }
}
