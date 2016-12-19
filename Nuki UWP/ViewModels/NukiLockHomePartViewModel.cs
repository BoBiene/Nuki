using MetroLog;
using Nuki.Communication.API;
using Nuki.Communication.Commands.Response;
using Nuki.Communication.Connection;
using Nuki.Services.SettingsServices;
using Nuki.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Nuki.ViewModels
{
    public class NukiLockHomePartViewModel : NukiLockViewModel.Part
    {
        private NukiLockState m_LockState = NukiLockState.Undefined;
        private NukiState m_NukiState = NukiState.Uninitialized;
        private bool m_blnCriticalBattery = false;
        private string m_strLockRingState = string.Empty;
        private Visibility m_IsFlyoutOpen = Visibility.Collapsed;
        private static ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger(nameof(NukiLockHomePartViewModel));
        public Visibility IsFlyoutOpen { get { return m_IsFlyoutOpen; } set { Set(ref m_IsFlyoutOpen, value); } }
        public string LockRingState { get { return m_strLockRingState; } set { Set(ref m_strLockRingState, value); } }
        public bool CriticalBattery {  get { return m_blnCriticalBattery;  } set { Set(ref m_blnCriticalBattery, value); } }
        public NukiLockState LockState { get { return m_LockState; } set { Set(ref m_LockState, value); } }
        public NukiState NukiState { get { return m_NukiState; } set { Set(ref m_NukiState, value); } }
        public NukiLockHomePartViewModel()
        {

        }
        //public NukiLockHomePartViewModel(NukiLockViewModel baseModel)
        //    : base(baseModel)
        //{

        //}

        
        public DelegateCommand m_OpenFlyoutCommand = null;
        public DelegateCommand OpenFlyoutCommand
            => m_OpenFlyoutCommand ?? (m_OpenFlyoutCommand = new DelegateCommand(() =>
            {
                IsFlyoutOpen = Visibility.Visible;

            }, () => IsFlyoutOpen == Visibility.Collapsed));

        public DelegateCommand m_CloseFlyoutCommand = null;
        public DelegateCommand CloseFlyoutCommand
            => m_CloseFlyoutCommand ?? (m_CloseFlyoutCommand = new DelegateCommand(() =>
            {
                IsFlyoutOpen = Visibility.Collapsed;

            }, () => IsFlyoutOpen == Visibility.Visible));

        public DelegateCommand m_ToggleFlyoutCommand = null;
        public DelegateCommand ToggleFlyoutCommand
            => m_ToggleFlyoutCommand ?? (m_ToggleFlyoutCommand = new DelegateCommand(() =>
            {
                if (IsFlyoutOpen == Visibility.Visible)
                    IsFlyoutOpen = Visibility.Collapsed;
                else
                    IsFlyoutOpen = Visibility.Visible;

            }, () =>true));

        public DelegateCommand m_SendLockCommand = null;
        public DelegateCommand SendLockCommand
            => m_SendLockCommand ?? (m_SendLockCommand = new DelegateCommand(async () =>
            {
                Shell.SetBusy(true, "Locking...");
                await BaseModel.BluetoothConnection.SendLockAction(NukiLockAction.Lock);
                Shell.SetBusy(false);

            }, () => BaseModel.BluetoothConnection?.Connected == true));

        public DelegateCommand m_SendUnlockCommand = null;
        public DelegateCommand SendUnlockCommand
            => m_SendUnlockCommand ?? (m_SendUnlockCommand = new DelegateCommand(async () =>
            {
                Shell.SetBusy(true, "Unlocking...");
                await BaseModel.BluetoothConnection.SendLockAction(NukiLockAction.Unlock,NukiLockActionFlags.ForceUnlock);
                Shell.SetBusy(false);

            }, () => BaseModel.BluetoothConnection?.Connected == true));

        public DelegateCommand m_SendCalibrateCommand = null;
        public DelegateCommand SendCalibrateCommand
            => m_SendCalibrateCommand ?? (m_SendCalibrateCommand = new DelegateCommand(async () =>
            {
                var userPassword = await BaseModel.RequestPassword();
                if (userPassword.Successfull)
                {
                    await BaseModel.BluetoothConnection.SendCalibrateRequest(userPassword.SecurityPIN);
                }
                else { }

            }, () => BaseModel.BluetoothConnection?.Connected == true));

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LockRingState = "Connecting...";
            if (Dispatcher != null)
            {
                var dispatcher = Dispatcher;
                Log.Info("OnNavigatedToAsync");
                BluetoothConnectionMonitor.Start(SettingsService.Instance.PairdLocks, (action) => dispatcher.DispatchAsync(action).AsAsyncAction(), (connection) =>
                 {
                     if (connection == BaseModel.BluetoothConnection)
                         BaseModel.Dispatcher.Dispatch(async () => await RefreshNukiState());
                 });
            }
            else
            {
                Log.Debug("Dispatcher is null...");
            }
          //  await RefreshNukiState();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async Task RefreshNukiState()
        {
            LockRingState = "Requsting state...";
            BaseModel.ShowProgressbar(true);
            RecieveNukiStatesCommand nukiStateCmd = null;
            try
            {
                if (BaseModel?.BluetoothConnection?.Connected == true)
                    nukiStateCmd = await BaseModel.BluetoothConnection.RequestNukiState();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to request Nuki Stat: {0}", ex);
            }

            if (nukiStateCmd != null)
            {
                LockState = nukiStateCmd.LockState;
                NukiState = nukiStateCmd.NukiState;
                CriticalBattery = nukiStateCmd.CriticalBattery;
                LockRingState = LockState.ToString();
            }
            else
            {
                LockState = NukiLockState.Undefined;
            }
            SendCalibrateCommand.RaiseCanExecuteChanged();
            SendLockCommand.RaiseCanExecuteChanged();
            SendUnlockCommand.RaiseCanExecuteChanged();
            BaseModel.ShowProgressbar(false);
        }
    }
}
