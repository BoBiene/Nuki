using MetroLog;
using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Nuki.ViewModels
{
    public class SettingsPageAboutPartViewModel : SettingsPageViewModel.Part
    {
        private static readonly Brush ErrorBrush = new SolidColorBrush(Colors.DarkRed);
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

        


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LogContent = await LoadLogFile();
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
            int nEntry = 0;
            foreach (var entry in entries.Events)
            {

                var textBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.NoWrap,
                    Text = GetFormattedString(entry),
                    FontSize = 12,
                };
                if (entry.Level >= LogLevel.Error)
                    textBlock.Foreground = ErrorBrush;

                //if (nEntry++ % 2 == 0)//Even Rows
                //{
                //    textBlock. 
                //}
                //else { }

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
}
