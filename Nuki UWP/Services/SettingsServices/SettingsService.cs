using Nuki.Communication.Connection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

namespace Nuki.Services.SettingsServices
{
    public class SettingsService
    {
        private static XmlSerializer s_XmlSerializer = new XmlSerializer(typeof(NukiConnectionBinding[]));
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;
        
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
            PairdLocks.AddRange(LoadBluetoothConnectionInfo());
            PairdLocks.CollectionChanged += PairdLocks_CollectionChanged;
        }

        private NukiConnectionBinding[] LoadBluetoothConnectionInfo()
        {
            NukiConnectionBinding[] retValues = new NukiConnectionBinding[0];
            try
            {
                string strLockSettings = _helper.Read<string>(nameof(PairdLocks), string.Empty);
                if (!string.IsNullOrEmpty(strLockSettings))
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        using (StreamWriter w = new StreamWriter(mem, Encoding.UTF8, 128, true))
                            w.Write(strLockSettings);

                        mem.Position = 0;
                        retValues = s_XmlSerializer.Deserialize(mem) as NukiConnectionBinding[];
                    }
                }
                else { }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Load BluetoothConnectionInfo failed: {0}", ex);
            }
            return retValues;
        }

        private void SaveBluetoothConnectionInfo()
        {
            try
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    s_XmlSerializer.Serialize(mem, PairdLocks.ToArray());
                    mem.Position = 0;
                    using (StreamReader r = new StreamReader(mem,Encoding.UTF8,true,128,true))
                        _helper.Write(nameof(PairdLocks), r.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Save BluetoothConnectionInfo failed: {0}", ex);
            }
        }

        private void PairdLocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SaveBluetoothConnectionInfo();
        }


        public ObservableCollection<NukiConnectionBinding> PairdLocks { get; private set; } 
            = new ObservableCollection<NukiConnectionBinding>();

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Dark;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value, true);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public bool ShowHamburgerButton
        {
            get { return _helper.Read<bool>(nameof(ShowHamburgerButton), true); }
            set
            {
                _helper.Write(nameof(ShowHamburgerButton), value);
                Views.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsFullScreen
        {
            get { return _helper.Read<bool>(nameof(IsFullScreen), false); }
            set
            {
                _helper.Write(nameof(IsFullScreen), value);
                Views.Shell.HamburgerMenu.IsFullScreen = value;
            }
        }
    }
}

