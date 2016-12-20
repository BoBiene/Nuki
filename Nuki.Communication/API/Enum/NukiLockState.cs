using Nuki.Communication.Connection.Bluetooth.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiLockState
    {
        Uncalibrated = 0x00,
        Locked = 0x01,
        Unlocking = 0x02,
        Unlocked = 0x03,
        Locking = 0x04,
        Unlatched = 0x05,
        Unlocked_Lock_n_goactive = 0x06,
        Unlatching = 0x07,
        Motorblocked = 0xFE,
        Undefined = 0xFF,
    }
}
