﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Nuki.Pages;
using Nuki.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Threading.Tasks;
using Nuki.Communication.Connection;
using Windows.UI.Core;
using Nuki.Settings;

namespace Nuki
{

    public sealed partial class Shell : UserControl
    {
        public NukiAppSettings AppSettings { get; private set; }
        public Shell()
        {
            this.InitializeComponent();
            
            var vm = new ShellViewModel();
            
            vm.MenuItems.Add(new MenuItem { Icon = (char)0xE115, Title = "Einrichten", PageType = typeof(Pages.Setup.Setup01LandingPage) });
            vm.MenuItems.Add(new MenuItem { Icon = '', Title = "Page 1", PageType = typeof(Page1), LeftMargin = 20 });
            vm.MenuItems.Add(new MenuItem { Icon = '', Title = "Page 2", PageType = typeof(Page2) });
            vm.MenuItems.Add(new MenuItem { Icon = '', Title = "Page 3", PageType = typeof(Page3) });
            vm.MenuItems.Add(new MenuItem { Icon = (char)0xE946, Title = "About", PageType = typeof(AboutPage) });
            // select the first menu item
            vm.SelectedMenuItem = vm.MenuItems.First();

            this.ViewModel = vm;
            // add entry animations
            var transitions = new TransitionCollection { };
            var transition = new NavigationThemeTransition { };
            transitions.Add(transition);
            this.Frame.ContentTransitions = transitions;
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            base.Loading += Shell_Loading;
        }

        private async void Shell_Loading(FrameworkElement sender, object args)
        {
            await BeginLoad();
        }

        private Task m_initTask = null;
        public Task BeginLoad()
        {
            if (m_initTask == null)
            {
                m_initTask = Task.Run(async () =>
                {
                    AppSettings = await NukiAppSettings.Load() ?? new NukiAppSettings();
                    BluetoothConnectionMonitor.Start(AppSettings.PairdLocks);
                });
            }
            else { }
            return m_initTask;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.BackgoundMode))
                this.BackgroundCanvas.Invalidate();
        }

        public static Shell Current
        {
            get { return Window.Current.Content as Shell; }
        }

        public ShellViewModel ViewModel { get; private set; }

        public Frame RootFrame
        {
            get
            {
                return this.Frame;
            }
        }

        CanvasBitmap m_BackgroundImage = null;
        CanvasBitmap m_BackgroundImageInitial = null;
        CanvasBitmap m_BackgroundImageInitialAlpha = null;
        BlendEffect m_BluredBackground = null;
        BlendEffect m_BlendedBackground = null;
        BlendEffect m_ExtremeBlendedBackground = null;

        private byte m_nFadein = 200;
        private ImageLoadStatus m_ImageStatus = ImageLoadStatus.Initial;
        private  enum ImageLoadStatus
        {
            Initial,
            LoadRequested,
            Fading,
            Complete
        }
        private void CreateCanvasResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateInitialResourcesAsync(sender).AsAsyncAction());
          
        }

        private async Task CreateInitialResourcesAsync(CanvasControl sender)
        {
            m_BackgroundImageInitial = await CanvasBitmap.LoadAsync(sender.Device, new Uri("ms-appx:///Assets/nuki_initial.jpg"));
         
        }

        private async Task CreateResourcesAsync(CanvasControl sender)
        {
            m_BackgroundImage = await CanvasBitmap.LoadAsync(sender.Device,new Uri( "ms-appx:///Assets/setup_bg.jpg"));
            m_BackgroundImageInitialAlpha = await CanvasBitmap.LoadAsync(sender.Device, new Uri("ms-appx:///Assets/nuki_initial.png"));
            var blured = new GaussianBlurEffect()
            {
                Source = m_BackgroundImage,
                BlurAmount = 10.0f,
            };

            m_BluredBackground = new BlendEffect()
            {
                Background = blured,
                Foreground = new ColorSourceEffect()
                {
                    Color = Windows.UI.Color.FromArgb(25, 0, 0, 0)
                },
                Mode = BlendEffectMode.Darken
            };

            m_BlendedBackground = new BlendEffect()
            {
                Background = blured,
                Foreground = new ColorSourceEffect()
                {
                    Color = Windows.UI.Color.FromArgb(100, 0, 0, 0)
                },
                Mode = BlendEffectMode.Darken
            };
            m_ImageStatus = ImageLoadStatus.Fading;
            sender.Invalidate();
        }

        private void CanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (m_ImageStatus == ImageLoadStatus.Fading)
            {
                CanvasFadeInLoadedImage(sender, args);
            }
            else
            {
                CanvasDrawBrackground(sender, args);
            }
        }

        private void CanvasFadeInLoadedImage(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var drawRectSmall = GetDrawRect(sender, m_BackgroundImageInitial);
            var drawRectBig = GetDrawRect(sender, m_BackgroundImage);
            var blendEffect = new BlendEffect()
            {
                Background = m_BackgroundImage,
                Foreground = new ColorSourceEffect()
                {
                    Color = Windows.UI.Color.FromArgb(m_nFadein, 0, 0, 0)
                },
                Mode = BlendEffectMode.Darken
            };

            args.DrawingSession.DrawImage(blendEffect, drawRectBig, m_BackgroundImage.Bounds);
            args.DrawingSession.DrawImage(m_BackgroundImageInitialAlpha, drawRectSmall, m_BackgroundImageInitialAlpha.Bounds);
            m_nFadein -= 50;
            if (m_nFadein <= 0)
                m_ImageStatus = ImageLoadStatus.Complete;
            Task.Delay(25).ContinueWith((t) => Dispatcher.RunAsync(CoreDispatcherPriority.Low, sender.Invalidate));
        }

        private void CanvasDrawBrackground(CanvasControl sender, CanvasDrawEventArgs args)
        {
            ICanvasImage img = null;

            Rect drawRect, imageBounds;
            if (m_BackgroundImage == null)
            {
                if (m_ImageStatus == ImageLoadStatus.Initial)
                {
                    m_ImageStatus = ImageLoadStatus.LoadRequested;
                    CreateResourcesAsync(sender);
                }
                else { }
                drawRect = GetDrawRect(sender, m_BackgroundImageInitial);
                img = m_BackgroundImageInitial;
                imageBounds = m_BackgroundImageInitial.Bounds;
            }
            else
            {

                switch (ViewModel.BackgoundMode)
                {
                    case BackgoundMode.CleanImage:
                        img = m_BackgroundImage;
                        break;
                    case BackgoundMode.BluredImage:
                        img = m_BluredBackground;
                        break;
                    case BackgoundMode.BluredDarkImage:
                        img = m_BlendedBackground;
                        break;
                    default:
                    case BackgoundMode.None:
                        break;
                }

                imageBounds = m_BackgroundImage.Bounds;
                drawRect = GetDrawRect(sender, m_BackgroundImage);
            }

            if (img != null)
            {
                args.DrawingSession.DrawImage(img, drawRect, imageBounds);
            }
            else { }
        }

        private Rect GetDrawRect(CanvasControl sender, CanvasBitmap bmp)
        {
          

            int nTopSpace = 120;
            int nBottomSpace = 180;
            int nNeededSpace = nTopSpace + nBottomSpace;

            Rect lockRect = new Rect(1764, 1265, 369, 667);
            if (bmp == m_BackgroundImageInitial)
                lockRect = new Rect(169,98, 369, 667);


            double neededLockHeight = Math.Max(1, sender.ActualHeight - nNeededSpace);
            double factor = neededLockHeight / lockRect.Height;
            double drawWidth = bmp.Bounds.Width * factor;
            double drawHeight = bmp.Bounds.Height * factor;

            Rect drawRect = new Rect((float)((sender.ActualWidth / 2d) - (factor * (lockRect.X + (lockRect.Width / 2d)))),
                                        nTopSpace - (lockRect.Y * factor),
                                        drawWidth, drawHeight);
            return drawRect;
        }
    }
}
