using Windows.UI.Xaml;
using System.Threading.Tasks;
using Nuki.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Nuki.Views;
using Nuki.Communication.Connection;
using Microsoft.HockeyApp;
using MetroLog;
using MetroLog.Targets;

namespace Nuki
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public static readonly SQLiteTarget SQLiteTarget = new SQLiteTarget();
        private static ILogger Log = null;
        public App()
        {
            
            HockeyClient.Current.Configure("bc0c6685cf164f4881b205148b4b9d2e");
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, LogLevel.Fatal, new StreamingFileTarget());

            SQLiteTarget.RetainDays = 2;
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, LogLevel.Fatal, App.SQLiteTarget);
            Log = LogManagerFactory.DefaultLogManager.GetLogger<App>();
            Log.Info("Starting...");
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;
            LogManagerFactory.DefaultConfiguration.IsEnabled = settings.EnableLogging;
            #endregion
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            Log.Info("OnStartAsync...");
            // TODO: add your long-running task here
            var firstLock = SettingsService.Instance.PairdLocks.FirstOrDefault();
            if (firstLock != null)
            {
               
                await NavigationService.NavigateAsync(typeof(NukiLock), firstLock.UniqueClientID.Value);
            }
            else
            {
                await NavigationService.NavigateAsync(typeof(Views.SetupNewLock));
            }
        }
    }
}

