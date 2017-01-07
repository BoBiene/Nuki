using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public interface INukiConnection: INotifyPropertyChanged
    {

        bool Connected { get; }
        string DeviceName { get; }
        INukiDeviceStateMessage LastKnownDeviceState { get; }
        INukiErrorMessage LastError { get; }

        Task<INukiConfigMessage> RequestNukiConfig();
        Task<INukiDeviceStateMessage> RequestNukiState();
        Task<INukiBatteryReport> RequestNukiBatteryReport();
        Task<INukiReturnMessage> SendCalibrateRequest(ushort securityPin);
        Task<INukiReturnMessage> SendLockAction(NukiLockAction lockAction, NukiLockActionFlags flags = NukiLockActionFlags.None);
    }
}
