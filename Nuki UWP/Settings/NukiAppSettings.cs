using System;
using System.Collections.Generic;
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
                return s_StorageHelper.LoadAsync().ContinueWith((t) => t.Result ?? new NukiAppSettings());
        }

        public Task Save()
        {
            return s_StorageHelper.SaveAsync(this);
        }
        #endregion


        public List<NukiDeviceSetting> PairdLocks
        {
            get; private set;
        } = new List<NukiDeviceSetting>();

        public NukiAppSettings()
        {
            
        }
    }
}
