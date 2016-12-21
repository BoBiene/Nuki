using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public interface INukiConnection
    {

        bool Connected { get; }
        string DeviceName { get; }

        Task<INukiDeviceStateMessage> RequestNukiState();
        Task<INukiReturnMessage> SendCalibrateRequest(ushort securityPin);
        Task<INukiReturnMessage> SendLockAction(NukiLockAction lockAction, NukiLockActionFlags flags = NukiLockActionFlags.None);
    }
}
