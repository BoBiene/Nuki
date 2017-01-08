using Nuki.Communication.Connection;
using Nuki.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Template10.Common;
using Template10.Services.NavigationService;
using Windows.UI.Core;

namespace Nuki.ViewModels
{
    public partial class NukiLockViewModel : PivotBaseViewModel<NukiLockViewModel>
    {
        private NukiConnectionConfig m_NukiConnectionBinding = null;
        private int m_nProgressRequests = 0;
        private Visibility m_ProgressbarVisibility = Visibility.Collapsed;
        private Visibility m_ErrorbarVisibility = Visibility.Collapsed;
        private string m_strErrorText = string.Empty;

        public string SelectedLock
        {
            get { return NukiConnectionConfig?.ConnectionName ?? string.Empty; }

        }

        public Visibility ErrorbarVisibility => m_ErrorbarVisibility;
        public string ErrorbarText => m_strErrorText;
        public Visibility ProgressbarVisibility
        {
            get { return m_ProgressbarVisibility; }

        }

        public void ShowProgressbar(bool blnVisibility)
        {
            if (blnVisibility)
                ++m_nProgressRequests;
            else
                --m_nProgressRequests;

            if (blnVisibility)
                m_ProgressbarVisibility = Visibility.Visible;
            else
                m_ProgressbarVisibility = Visibility.Collapsed;


            RaisePropertyChanged(nameof(ProgressbarVisibility));
        }

        public void ShowError(string strText)
        {
            try
            {
                Log.Trace("Show error: " + strText);
                var func = new Action(async () =>
                 {
                     m_strErrorText = strText;
                     m_ErrorbarVisibility = Visibility.Visible;
                     RaisePropertyChanged(nameof(ErrorbarText));
                     RaisePropertyChanged(nameof(ErrorbarVisibility));
                     await Task.Delay(10000);
                     m_ErrorbarVisibility = Visibility.Collapsed;
                     RaisePropertyChanged(nameof(ErrorbarVisibility));
                 });
                if (Dispatcher != null)
                    Dispatcher.DispatchAsync(func, 0, CoreDispatcherPriority.Low);
                else
                    func();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to display error: " + strText, ex);
            }
        }

        public INukiConnection NukiConncetion { get; private set; }

        public NukiConnectionConfig NukiConnectionConfig
        {
            get { return m_NukiConnectionBinding; }
            set
            {
                Set(ref m_NukiConnectionBinding, value);
                RaisePropertyChanged(nameof(SelectedLock));
            
            }
        }


        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (NukiConncetion != null)
                NukiConncetion.PropertyChanged -= NukiConncetion_PropertyChanged;
            return base.OnNavigatingFromAsync(args);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            ShowProgressbar(true);

            NukiConnectionConfig = SettingsService.Instance.PairdLocks.Where((l) => l.UniqueClientID.Value == parameter as uint?).FirstOrDefault();
            await TryToConnect();
            ShowProgressbar(false);
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async Task<bool> TryToConnect(int nTryCount = 0)
        {
            bool blnRet = false;
            var connectResult = await NukiConnectionFactory.TryConnect(NukiConnectionConfig, (action) => Dispatcher.DispatchAsync(action, priority: CoreDispatcherPriority.Low).AsAsyncAction());
            if (connectResult.Successfull)
            {
                if (NukiConncetion != null)
                    NukiConncetion.PropertyChanged -= NukiConncetion_PropertyChanged;

                NukiConncetion = connectResult.Connection;
                
                NukiConncetion.PropertyChanged += NukiConncetion_PropertyChanged;
                RaisePropertyChanged(nameof(NukiConncetion));
            }
            else if (nTryCount < 5 )
            {
                blnRet = await Task.Delay(1000).ContinueWith((t) => TryToConnect(nTryCount + 1)).Unwrap();
            }
            else
            {
                //Failed
            }

            return blnRet;
        }

        private void NukiConncetion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof( NukiConncetion.LastError))
            {
                ShowError($"Command <{NukiConncetion.LastError.FailedCommand}> errocode: {NukiConncetion.LastError.ErrorCode} {NukiConncetion.LastError.Message}");
            }
            else {  }
        }

        public struct PasswordRequestResult
        {
            public UInt16 SecurityPIN { get; private set; }
            public bool Successfull { get; private set; }
            public PasswordRequestResult(UInt16 securityPin, bool blnSuccessfull)
            {
                SecurityPIN = securityPin;
                Successfull = blnSuccessfull;
            }
        }

        public async Task<PasswordRequestResult> RequestPassword()
        {
            var dlg = new Views.dialogRequestPassword();
            var result = await dlg.ShowAsync();
            PasswordRequestResult returnValue = new PasswordRequestResult();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                ushort pin = 0;
                if (ushort.TryParse(dlg.UserInputPassword, out pin))
                    returnValue = new PasswordRequestResult(pin, true);

            }
            else
            {

            }

            return returnValue;
        }

    }
}
