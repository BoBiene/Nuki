using Microsoft.Toolkit.Uwp;
using Nuki.Communication.API;
using Nuki.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;

namespace Nuki.ViewModels
{
    public class NukiLockStatusPartViewModel : NukiLockViewModel.Part
    {

        public IncrementalLoadingCollection<LockHistoryList, INukiLogEntry> LockHistory
        {
            get; private set;
        }
      
        public NukiLockStatusPartViewModel()
        {
            LockHistory = null;
        }

        public DelegateCommand m_RequestLogEntriesCommand = null;
        public DelegateCommand RequestLogEntriesCommand
            => m_RequestLogEntriesCommand ?? (m_RequestLogEntriesCommand = new DelegateCommand(async () =>
            {
                var userPassword = await BaseModel.RequestPassword();
                if (userPassword.Successfull)
                {
                    LockHistory = new IncrementalLoadingCollection<LockHistoryList, INukiLogEntry>(new LockHistoryList(BaseModel, userPassword.SecurityPIN), 10);
                    RaisePropertyChanged(nameof(LockHistory));
                }
                else { }

            }, () => BaseModel.NukiConncetion?.Connected == true));

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            await RefreshNukiState();
        }

        private async Task RefreshNukiState()
        {
            BaseModel.ShowProgressbar(true);
            INukiBatteryReport nukiBatteeryReport = null;
            try
            {
                if (BaseModel?.NukiConncetion?.Connected == true)
                    nukiBatteeryReport = await BaseModel.NukiConncetion.RequestNukiBatteryReport();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to request Nuki Stat: {0}", ex);
            }
            BaseModel.ShowProgressbar(false);

        
        }
    }
}
