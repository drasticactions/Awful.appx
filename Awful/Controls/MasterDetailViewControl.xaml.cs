﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using Awful.Services;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Awful.Controls
{
    public sealed partial class MasterDetailViewControl : UserControl
    {
        private bool isInOnePaneMode = false;
        private double lastWindowWidth = 0;
        private double lastWindowHeight = 0;
        private SystemNavigationManager navigationManager = null;
        private string currentState = "";
        public string MasterViewTitle { get; set; }

        public string PreviewViewTitle { get; set; }

        public MasterDetailViewControl()
        {
            this.InitializeComponent();
        }

        public void Loaded()
        {
            //ApplicationView.GetForCurrentView().VisibleBoundsChanged += OnVisibleBoundsChanged;
            //this.DataContextChanged += MasterDetailViewControl_DataContextChanged;
            //EvaluateLayout();
        }

        public void OnNavigated()
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += OnVisibleBoundsChanged;
            this.DataContextChanged += MasterDetailViewControl_DataContextChanged;
            Window.Current.SizeChanged += Current_SizeChanged;
            EvaluateLayout();
        }

        public void FromNavigated()
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged -= OnVisibleBoundsChanged;
            this.DataContextChanged -= MasterDetailViewControl_DataContextChanged;
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void OnVisibleBoundsChanged(ApplicationView sender, object args)
        {
            EvaluateLayout();
        }

        private void MasterDetailViewControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //may not be needed
            //masterViewContentControl.DataContext = this.DataContext;
            //detailViewContentControl.DataContext = this.DataContext;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            EvaluateLayout();
        }

        public void Unloaded()
        {
            //ApplicationView.GetForCurrentView().VisibleBoundsChanged -= OnVisibleBoundsChanged;
            //this.DataContextChanged -= MasterDetailViewControl_DataContextChanged;
        }

        public void NavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (isInOnePaneMode)
            {
                if (PreviewItem != null)
                {
                    ShowMasterView();
                }
                else
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            }
            else
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            //e.Handled = true;
        }

        public void ShowMasterView()
        {
            App.ShellViewModel.Header = MasterViewTitle;
            if (isInOnePaneMode)
            {
                if (NullifyPreviewItemWhenGoingToMasterView)
                    PreviewItem = null;

                //EvaluateLayout();

                lock (currentState)
                {
                    VisualStateManager.GoToState(this, "OnePaneMasterVisualState", true);
                    currentState = "OnePaneMasterVisualState";
                }

                if (BackButtonVisibilityHinted != null)
                    BackButtonVisibilityHinted(this, new BackButtonVisibilityHintedEventArgs(false));
            }
        }

        public void SetMasterHeaderText(string text)
        {
            App.ShellViewModel.Header = text;
            MasterViewTitle = text;
        }

        public void SetPreviewHeaderText(string text)
        {
            App.ShellViewModel.Header = text;
            PreviewViewTitle = text;
        }

        public void EvaluateLayout()
        {
            double width = Window.Current.Bounds.Width;
            double height;
            height = ApplicationView.GetForCurrentView().VisibleBounds.Height;

            bool isOrientationChange = width == lastWindowHeight && height == lastWindowWidth;

            /* According to https://msdn.microsoft.com/en-us/library/windows/apps/dn997765.aspx - The recommend style is as follows:
             * 320 epx-719 epx (Available window width) = Stacked (Single pane shown at one time)
             * 720 epx or wider (Available window width) = Side-by-Side (Two panes shown at one time)
             */


            if (width > 800)
            {
                isInOnePaneMode = false;
                lock (currentState)
                {
                    VisualStateManager.GoToState(this, "TwoPaneVisualState", true);

                    currentState = "TwoPaneVisualState";
                }

                if (BackButtonVisibilityHinted != null)
                    BackButtonVisibilityHinted(this, new BackButtonVisibilityHintedEventArgs(false));
            }
            else
            {
                isInOnePaneMode = true;
                if (width >= 640)
                {
                    PART_detailViewContentControl.Width = width - 50;
                    PART_masterViewContentControl.Width = width - 50;
                }
                else
                {
                    PART_detailViewContentControl.Width = width;
                    PART_masterViewContentControl.Width = width;
                }

                if (!isOrientationChange)
                {
                    var onePaneModeState = (PreviewItem != null ? "OnePaneDetailVisualState" : "OnePaneMasterVisualState");

                    lock (currentState)
                    {
                        VisualStateManager.GoToState(this, onePaneModeState, true);

                        currentState = onePaneModeState;
                    }

                    if (BackButtonVisibilityHinted != null)
                        BackButtonVisibilityHinted(this, new BackButtonVisibilityHintedEventArgs(onePaneModeState == "OnePaneDetailVisualState"));
                }
            }

            App.ShellViewModel.Header = PreviewItem == null ? MasterViewTitle : PreviewViewTitle;

            if (width >= 640)
            {
                PART_relativePanelParent.Height = height;
            }
            else
            {
                PART_relativePanelParent.Height = height + 10;
            }
            lastWindowHeight = height;
            lastWindowWidth = width;
        }

        public static readonly DependencyProperty MasterViewPaneContentProperty = DependencyProperty.Register("MasterViewPaneContent", typeof(FrameworkElement),
            typeof(MasterDetailViewControl), new PropertyMetadata(null));

        public FrameworkElement MasterViewPaneContent
        {
            get { return (FrameworkElement)GetValue(MasterViewPaneContentProperty); }
            set { SetValue(MasterViewPaneContentProperty, value); }
        }

        public static readonly DependencyProperty DetailViewPaneContentProperty = DependencyProperty.Register("DetailViewPaneContent", typeof(FrameworkElement),
            typeof(MasterDetailViewControl), new PropertyMetadata(null));

        public FrameworkElement DetailViewPaneContent
        {
            get { return (FrameworkElement)GetValue(DetailViewPaneContentProperty); }
            set { SetValue(DetailViewPaneContentProperty, value); }
        }


        public static readonly DependencyProperty PreviewItemProperty = DependencyProperty.Register("PreviewItem", typeof(object),
            typeof(MasterDetailViewControl), new PropertyMetadata(null, new PropertyChangedCallback((control, args) =>
            {
                (control as MasterDetailViewControl).EvaluateLayout();
            })));

        /// <summary>
        /// The item that the preview pane is showing. This MUST be connected to a TwoWay binding.
        /// </summary>
        public object PreviewItem
        {
            get { return GetValue(PreviewItemProperty); }
            set { SetValue(PreviewItemProperty, value); }
        }


        public static readonly DependencyProperty NullifyPreviewItemWhenGoingToMasterViewProperty = DependencyProperty.Register("NullifyPreviewItemWhenGoingToMasterView", typeof(bool),
            typeof(MasterDetailViewControl), new PropertyMetadata(true, new PropertyChangedCallback((control, args) =>
            {
                (control as MasterDetailViewControl).EvaluateLayout();
            })));

        public bool NullifyPreviewItemWhenGoingToMasterView
        {
            get { return (bool)GetValue(NullifyPreviewItemWhenGoingToMasterViewProperty); }
            set { SetValue(NullifyPreviewItemWhenGoingToMasterViewProperty, value); }
        }

        public bool IsShowingDetailView { get { return currentState == "TwoPaneVisualState" || currentState == "OnePaneDetailVisualState"; } }

        public event EventHandler<BackButtonVisibilityHintedEventArgs> BackButtonVisibilityHinted;
    }

    public class BackButtonVisibilityHintedEventArgs : EventArgs
    {
        internal BackButtonVisibilityHintedEventArgs(bool shouldBeVisible)
        {
            BackButtonShouldBeVisible = shouldBeVisible;
        }

        public bool BackButtonShouldBeVisible { get; private set; }
    }
}
