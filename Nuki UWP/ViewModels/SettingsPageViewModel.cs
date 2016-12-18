using MetroLog;
using MetroLog.Layouts;
using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Nuki.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private static readonly Brush ErrorBrush = new SolidColorBrush(Colors.DarkRed);
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            AboutPartViewModel.LogContent = await LoadLogFile();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async Task<StackPanel> LoadLogFile()
        {
            var panel = new StackPanel();
            var query = new LogReadQuery();
            query.SetLevels(LogLevel.Trace, LogLevel.Fatal);
            query.Top = 1000;
            query.FromDateTimeUtc = DateTime.UtcNow.AddHours(-2);
            var entries = await App.SQLiteTarget.ReadLogEntriesAsync(query);

            foreach (var entry in entries.Events)
            {

                var textBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = GetFormattedString(entry),
                    FontSize = 12,
                };
                if (entry.Level >= LogLevel.Error)
                    textBlock.Foreground = ErrorBrush;
                panel.Children.Add(textBlock);
            }

            return panel;
        }

        private static string GetFormattedString(LogEventInfoItem info)
        {
            var builder = new StringBuilder();
            builder.Append(info.SequenceId);
            builder.Append("|");
            builder.Append(info.DateTimeUtc.ToLocalTime().ToString());
            builder.Append("|");
            builder.Append(info.Level.ToString().ToUpper());
            builder.Append("|");
            builder.Append(Environment.CurrentManagedThreadId);
            builder.Append("|");
            builder.Append(info.Logger);
            builder.Append("|");
            builder.Append(info.Message);
            if (info.Exception != null)
            {
                builder.Append(" --> ");
                builder.Append(info.Exception);
            }

            return builder.ToString();
        }
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        Services.SettingsServices.SettingsService _settings;

        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        public bool ShowHamburgerButton
        {
            get { return _settings.ShowHamburgerButton; }
            set { _settings.ShowHamburgerButton = value; base.RaisePropertyChanged(); }
        }

        public bool IsFullScreen
        {
            get { return _settings.IsFullScreen; }
            set
            {
                _settings.IsFullScreen = value;
                base.RaisePropertyChanged();
                if (value)
                {
                    ShowHamburgerButton = false;
                }
                else
                {
                    ShowHamburgerButton = true;
                }
            }
        }

        public bool EnableLoggingToggleSwitch
        {
            get { return _settings.EnableLogging; }
            set { _settings.EnableLogging = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        private string _BusyText = "Please wait...";
        public string BusyText
        {
            get { return _BusyText; }
            set
            {
                Set(ref _BusyText, value);
                _ShowBusyCommand.RaiseCanExecuteChanged();
            }
        }

        DelegateCommand _ShowBusyCommand;
        public DelegateCommand ShowBusyCommand
            => _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, _BusyText);
                await Task.Delay(5000);
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));

        public void ClearLocks() => _settings.PairdLocks.Clear();

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        private StackPanel m_LogContent = null;
        public StackPanel LogContent { get { return m_LogContent; } set { Set(ref m_LogContent, value); } }

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}

