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
        private NukiConnectionBinding m_NukiConnectionBinding = null;
        private Visibility m_ProgressbarVisibility = Visibility.Collapsed;
      

        public string SelectedLock
        {
            get { return NukiConncection?.ConnectionName ?? string.Empty; }

        }


        public Visibility ProgressbarVisibility => m_ProgressbarVisibility;

        public void ShowProgressbar(bool blnVisibility)
        {
            if (blnVisibility)
                m_ProgressbarVisibility = Visibility.Visible;
            else
                m_ProgressbarVisibility = Visibility.Collapsed;

            RaisePropertyChanged(nameof(ProgressbarVisibility));
        }


        public INukiConnection NukiConncetion { get; private set; }

        public NukiConnectionBinding NukiConncection
        {
            get { return m_NukiConnectionBinding; }
            set
            {
                Set(ref m_NukiConnectionBinding, value);
                RaisePropertyChanged(nameof(SelectedLock));
            
            }
        }


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            NukiConncection = SettingsService.Instance.PairdLocks.Where((l) => l.UniqueClientID.Value == parameter as uint?).FirstOrDefault();

            var connectResult = await NukiConnectionFactory.TryConnect(NukiConncection, (action) => Dispatcher.DispatchAsync(action,priority: CoreDispatcherPriority.Low).AsAsyncAction());

            NukiConncetion = connectResult.Connection;
            RaisePropertyChanged(nameof(NukiConncetion));

            await base.OnNavigatedToAsync(parameter, mode, state);
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
