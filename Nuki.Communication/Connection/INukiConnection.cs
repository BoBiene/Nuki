using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    interface INukiConnection
    {

        bool Connected { get; }
        string DeviceName { get; }

        Task<RecieveNukiStatesCommand> RequestNukiState();
        Task<RecieveStatusCommand> SendCalibrateRequest(ushort securityPin);
        Task<RecieveStatusCommand> SendLockAction(NukiLockAction lockAction, NukiLockActionFlags flags = NukiLockActionFlags.None);
    }
}
