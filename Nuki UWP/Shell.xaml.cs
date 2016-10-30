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
            this.Frame.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/setup_bg.jpg")),
                Stretch = Stretch.UniformToFill
            };
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
    }
}
