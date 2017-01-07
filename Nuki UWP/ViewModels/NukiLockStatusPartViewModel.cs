using Microsoft.Toolkit.Uwp;
using Nuki.Communication.API;
using Nuki.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            LockHistory = new IncrementalLoadingCollection<LockHistoryList, INukiLogEntry>(new LockHistoryList(BaseModel));
            RaisePropertyChanged(nameof(LockHistory));
        }
    }
}
