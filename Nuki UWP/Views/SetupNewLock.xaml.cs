using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Nuki.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Nuki.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SetupNewLock : Page
    {
        public static SetupNewLock Current { get; private set; }
        public SetupNewLock()
        {
            ViewModel = new SetupNewLockViewModel();
            this.InitializeComponent();
            Current = this;
            // add entry animations
            var transitions = new TransitionCollection { };
            var transition = new NavigationThemeTransition { };
            transitions.Add(transition);
            this.Frame.ContentTransitions = transitions;
            base.DataContext = ViewModel;
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public SetupNewLockViewModel ViewModel
        {
            get;private set;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.BackgoundMode))
                this.BackgroundCanvas.Invalidate();
        }



        public Frame RootFrame
        {
            get
            {
                return this.Frame;
            }
        }

        CanvasBitmap m_BackgroundImage = null;
        CanvasBitmap m_BackgroundImageInitial = null;
        //CanvasBitmap m_BackgroundImageInitialAlpha = null;
        BlendEffect m_BluredBackground = null;
        BlendEffect m_BlendedBackground = null;
        BlendEffect m_ExtremeBlendedBackground = null;

        private float m_nFadein = 0f;
        private ImageLoadStatus m_ImageStatus = ImageLoadStatus.Initial;
        private enum ImageLoadStatus
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
            m_BackgroundImage = await CanvasBitmap.LoadAsync(sender.Device, new Uri("ms-appx:///Assets/setup_bg.jpg"));
            //m_BackgroundImageInitialAlpha = await CanvasBitmap.LoadAsync(sender.Device, new Uri("ms-appx:///Assets/nuki_initial.png"));
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
           
            
            args.DrawingSession.DrawImage(m_BackgroundImageInitial, drawRectSmall, m_BackgroundImageInitial.Bounds);
            args.DrawingSession.DrawImage(m_BackgroundImage, drawRectBig, m_BackgroundImage.Bounds, m_nFadein);

            m_nFadein += 0.1f;
            if (m_nFadein >= 1)
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

            Rect lockRect = new Rect(1763, 1264, 370, 680);
            if (bmp == m_BackgroundImageInitial)
                lockRect = new Rect(170, 95, 369, 680);


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
