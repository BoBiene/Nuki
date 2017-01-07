using MetroLog;
using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
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
using Template10.Services.NavigationService;

namespace Nuki.ViewModels
{
    public class NukiLockHomePartViewModel : NukiLockViewModel.Part
    {
        private NukiLockState m_LockState = NukiLockState.Undefined;
        private NukiState m_NukiState = NukiState.Uninitialized;
        private bool m_blnCriticalBattery = false;
        private string m_strLockRingState = string.Empty;
        private Visibility m_IsFlyoutOpen = Visibility.Collapsed;
        
        public Visibility IsFlyoutOpen { get { return m_IsFlyoutOpen; } set { Set(ref m_IsFlyoutOpen, value); } }
        public string LockRingState { get { return m_strLockRingState; } set { Set(ref m_strLockRingState, value); } }
        public bool CriticalBattery {  get { return m_blnCriticalBattery;  } set { Set(ref m_blnCriticalBattery, value); } }
        public NukiLockState LockState { get { return m_LockState; } set { Set(ref m_LockState, value); } }
        public NukiState NukiState { get { return m_NukiState; } set { Set(ref m_NukiState, value); } }
        public NukiLockHomePartViewModel()
        {

        }

        
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
                await BaseModel.NukiConncetion.SendLockAction(NukiLockAction.Lock);
                Shell.SetBusy(false);

            }, () => BaseModel.NukiConncetion?.Connected == true));

        public DelegateCommand m_SendUnlockCommand = null;
        public DelegateCommand SendUnlockCommand
            => m_SendUnlockCommand ?? (m_SendUnlockCommand = new DelegateCommand(async () =>
            {
                Shell.SetBusy(true, "Unlocking...");
                await BaseModel.NukiConncetion.SendLockAction(NukiLockAction.Unlock,NukiLockActionFlags.ForceUnlock);
                Shell.SetBusy(false);

            }, () => BaseModel.NukiConncetion?.Connected == true));

        public DelegateCommand m_SendCalibrateCommand = null;
        public DelegateCommand SendCalibrateCommand
            => m_SendCalibrateCommand ?? (m_SendCalibrateCommand = new DelegateCommand(async () =>
            {
                var userPassword = await BaseModel.RequestPassword();
                if (userPassword.Successfull)
                {
                    await BaseModel.NukiConncetion.SendCalibrateRequest(userPassword.SecurityPIN);
                }
                else { }

            }, () => BaseModel.NukiConncetion?.Connected == true));

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (BaseModel?.NukiConncetion != null)
            {
                BaseModel.NukiConncetion.PropertyChanged -= NukiConncetion_PropertyChanged;
            }
            else { }
            return base.OnNavigatingFromAsync(args);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LockRingState = "Connecting...";
            Log.Info("OnNavigatedToAsync");
            if(BaseModel?.NukiConncetion != null)
            {
                BaseModel.NukiConncetion.PropertyChanged -= NukiConncetion_PropertyChanged;
                BaseModel.NukiConncetion.PropertyChanged += NukiConncetion_PropertyChanged;
            }
            else { }
            await base.OnNavigatedToAsync(parameter, mode, state);
            await RefreshNukiState();
        }

        private void NukiConncetion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BaseModel.NukiConncetion.LastKnownDeviceState))
            {
                UpdateNukiDeviceState(BaseModel.NukiConncetion.LastKnownDeviceState).GetAwaiter();
            }
            else { }
        }

        private async Task RefreshNukiState()
        {
            BaseModel.ShowProgressbar(true);
            LockRingState = "Requsting state...";
            INukiDeviceStateMessage nukiStateCmd = null;
            try
            {
                if (BaseModel?.NukiConncetion?.Connected == true)
                    nukiStateCmd = await BaseModel.NukiConncetion.RequestNukiState();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to request Nuki Stat: {0}", ex);
            }
            await UpdateNukiDeviceState(nukiStateCmd);
            SendCalibrateCommand.RaiseCanExecuteChanged();
            SendLockCommand.RaiseCanExecuteChanged();
            SendUnlockCommand.RaiseCanExecuteChanged();
            BaseModel.ShowProgressbar(false);
        }

        private async Task UpdateNukiDeviceState(INukiDeviceStateMessage nukiStateCmd)
        {
            var action = new Action(() =>
             {
                 if (nukiStateCmd != null)
                 {
                     LockState = nukiStateCmd.LockState;
                     NukiState = nukiStateCmd.NukiState;
                     CriticalBattery = nukiStateCmd.CriticalBattery;
                 }
                 else
                 {
                     LockState = NukiLockState.Undefined;
                 }
                 LockRingState = LockState.ToString();
             });
            if (Dispatcher != null)
            {
                await Dispatcher.DispatchAsync(action);
            }
            else
            {
                action();
            }
        }
    }
}
