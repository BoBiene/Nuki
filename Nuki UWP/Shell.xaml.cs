using Microsoft.Graphics.Canvas;
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

namespace Nuki
{

    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();
            
            var vm = new ShellViewModel();
            
            vm.MenuItems.Add(new MenuItem { Icon = (char)0xE115, Title = "Einrichten", PageType = typeof(Pages.Setup.SetupLandingPage) });
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
        BlendEffect m_BluredBackground = null;
        BlendEffect m_BlendedBackground = null;
        private void CreateCanvasResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task CreateResourcesAsync(CanvasControl sender)
        {
            m_BackgroundImage = await CanvasBitmap.LoadAsync(sender.Device,new Uri( "ms-appx:///Assets/setup_bg.jpg"));
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
        }

        private void CanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            int nTopSpace = 120;
            int nBottomSpace = 180;
            int nNeededSpace = nTopSpace + nBottomSpace;
            Rect lockRect = new Rect(1764, 1265, 369, 667);


            double neededLockHeight = Math.Max(1, sender.ActualHeight - nNeededSpace);
            double factor = neededLockHeight / lockRect.Height;
            double drawWidth = m_BackgroundImage.Bounds.Width * factor;
            double drawHeight = m_BackgroundImage.Bounds.Height * factor;

            Rect drawRect = new Rect((float)((sender.ActualWidth / 2d) - (factor * (lockRect.X + (lockRect.Width / 2d)))),
                                        nTopSpace - (lockRect.Y * factor),
                                        drawWidth, drawHeight);

            ICanvasImage img = null;
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
            if (img != null)
                args.DrawingSession.DrawImage(img, drawRect, m_BackgroundImage.Bounds);
        }
    }
}
