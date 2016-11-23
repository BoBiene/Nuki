using Nuki.Communication.Connection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRtUtility;

namespace Nuki.Settings
{
    public class NukiAppSettings
    {
        private static ObjectStorageHelper<NukiAppSettings> s_StorageHelper = new ObjectStorageHelper<NukiAppSettings>(StorageType.Local);
        private static NukiAppSettings s_Settings = null;

        #region Load / Save
        public static Task<NukiAppSettings> Load()
        {
            if (s_Settings != null)
                return Task.FromResult(s_Settings);
            else
                return s_StorageHelper.LoadAsync().ContinueWith((t) => (!t.IsFaulted) ? t.Result : null ?? new NukiAppSettings());
        }

        public Task Save()
        {
            return s_StorageHelper.SaveAsync(this);
        }
        #endregion


        public ObservableCollection<BluetoothConnectionInfo> PairdLocks
        {
            get; private set;
        } = new ObservableCollection<BluetoothConnectionInfo>();

        public NukiAppSettings()
        {
            
        }
    }
}
