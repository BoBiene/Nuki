using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Nuki.Communication.SemanticTypes;

namespace Nuki.ViewModels
{
    public class NukiLockSettingsPartViewModel : NukiLockViewModel.Part, INukiConfigMessage
    {
        private bool m_blnAutoUnlatch = false;
        public bool AutoUnlatch { get { return m_blnAutoUnlatch; } set { Set(ref m_blnAutoUnlatch, value); } }

        public bool m_blnButtonEnabled = false;
        public bool ButtonEnabled { get { return m_blnButtonEnabled; } set { Set(ref m_blnButtonEnabled, value); } }

        private NukiTimeStamp m_NukiTimeStamp = null;
        public NukiTimeStamp CurrentTime { get { return m_NukiTimeStamp; } set { Set(ref m_NukiTimeStamp, value); } }

        private NukiDSTSetting m_NukiDSTSetting = NukiDSTSetting.European;
        public NukiDSTSetting DSTMode { get { return m_NukiDSTSetting; } set { Set(ref m_NukiDSTSetting, value); } }

        private NukiFobAction m_FobAction1 = NukiFobAction.Intelligent;
        public NukiFobAction FobAction1 { get { return m_FobAction1; } set { Set(ref m_FobAction1, value); } }
        private NukiFobAction m_FobAction2 = NukiFobAction.Intelligent;
        public NukiFobAction FobAction2 { get { return m_FobAction2; } set { Set(ref m_FobAction2, value); } }
        private NukiFobAction m_FobAction3 = NukiFobAction.Intelligent;
        public NukiFobAction FobAction3 { get { return m_FobAction3; } set { Set(ref m_FobAction3, value); } }
        private bool m_blnHasFob = false;
        public bool HasFob { get { return m_blnHasFob; } set { Set(ref m_blnHasFob, value); } }
        private float m_fLatitude = 0;
        public float Latitude { get { return m_fLatitude; } set { Set(ref m_fLatitude, value); } }
        private byte m_bLEDBrightness = 0;
        public byte LEDBrightness { get { return m_bLEDBrightness; } set { Set(ref m_bLEDBrightness, value); } }
        private bool m_blnLEDEnabled = false;
        public bool LEDEnabled { get { return m_blnLEDEnabled; } set { Set(ref m_blnLEDEnabled, value); } }
        private float m_fLongitude = 0;
        public float Longitude { get { return m_fLongitude; } set { Set(ref m_fLongitude, value); } }
        private string m_strName = string.Empty;
        public string Name { get { return m_strName; } set { Set(ref m_strName, value); } }
        private bool m_blnPairingEnabled = false;
        public bool PairingEnabled { get { return m_blnPairingEnabled; } set { Set(ref m_blnPairingEnabled, value); } }
        
        public UniqueClientID UniqueClientID { get; set; }
        private short m_nUTCOffset = 0;
        public short UTCOffset { get { return m_nUTCOffset; } set { Set(ref m_nUTCOffset, value); } }

        public NukiLockSettingsPartViewModel(NukiLockViewModel baseModel)
            : base(baseModel)
        {

        }
        public NukiLockSettingsPartViewModel()
        {

        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            await RefreshNukiState();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async Task RefreshNukiState()
        {
            INukiConfigMessage nukiConfig = null;
            try
            {
                if (BaseModel?.NukiConncetion?.Connected == true)
                    nukiConfig = await BaseModel.NukiConncetion.RequestNukiConfig();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to request Nuki Stat: {0}", ex);
            }

            if (nukiConfig != null)
            {
                this.AutoUnlatch = nukiConfig.AutoUnlatch;
                this.ButtonEnabled = nukiConfig.ButtonEnabled;
                this.CurrentTime = nukiConfig.CurrentTime;
                this.DSTMode = nukiConfig.DSTMode;
                this.FobAction1 = nukiConfig.FobAction1;
                this.FobAction2 = nukiConfig.FobAction2;
                this.FobAction3 = nukiConfig.FobAction3;
                this.HasFob = nukiConfig.HasFob;
                this.Latitude = nukiConfig.Latitude;
                this.LEDBrightness = nukiConfig.LEDBrightness;
                this.LEDEnabled = nukiConfig.LEDEnabled;
                this.Longitude = nukiConfig.Longitude;
                this.Name = nukiConfig.Name;
                this.PairingEnabled = nukiConfig.PairingEnabled;
                this.UniqueClientID = nukiConfig.UniqueClientID;
                this.UTCOffset = nukiConfig.UTCOffset;
            }
            else { }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            return base.OnNavigatedFromAsync(pageState, suspending);
        }
    }
}
