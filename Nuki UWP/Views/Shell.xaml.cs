using System.ComponentModel;
using System.Linq;
using System;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Mvvm;
using System.Collections.ObjectModel;

namespace Nuki.Views
{
    public sealed partial class Shell : Page, INotifyPropertyChanged
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        public ObservableCollection<HamburgerButtonInfo> HamburgerMenuPrimaryButtons { get; private set; } =
            new ObservableCollection<HamburgerButtonInfo>();
        Services.SettingsServices.SettingsService _settings;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = Services.SettingsServices.SettingsService.Instance;
            _settings.PairdLocks.CollectionChanged += PairdLocks_CollectionChanged;
            BuildPimaryButtons();
        }

        private void BuildPimaryButtons()
        {
            int n = 0;

            foreach (var nuki in _settings.PairdLocks)
            {
                HamburgerButtonInfo btn = null;
                if (n < HamburgerMenuPrimaryButtons.Count)
                    btn = HamburgerMenuPrimaryButtons[n++];
                else
                {
                    btn = new HamburgerButtonInfo();
                    HamburgerMenuPrimaryButtons.Add(btn);
                    ++n;
                }
                btn.Visibility = Visibility.Visible;
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,

                };
                panel.Children.Add(new SymbolIcon { Width = 48, Height = 48, Symbol = nuki.Icon });
                panel.Children.Add(new TextBlock
                {
                    Margin = new Thickness(12, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = nuki.ConnectionName
                });
                btn.Content = panel;
                btn.ClearHistory = true;
                btn.PageType = typeof(NukiLock);
                btn.PageParameter = nuki.UniqueClientID.Value;
            }

            while (n < HamburgerMenuPrimaryButtons.Count)
                HamburgerMenuPrimaryButtons.RemoveAt(n);
        }

        private void PairdLocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            BuildPimaryButtons();
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility = _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool IsBusy { get; set; } = false;
        public string BusyText { get; set; } = "Please wait...";
        public event PropertyChangedEventHandler PropertyChanged;

        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                if (busy)
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                else
                    BootStrapper.Current.UpdateShellBackButton();

                Instance.IsBusy = busy;
                Instance.BusyText = text;

                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(IsBusy)));
                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(BusyText)));
            });
        }
    }
}

