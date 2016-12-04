using Nuki.Communication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiState
    {
        Uninitialized = 0x00,
        PairingMode = 0x01,
        DoorMode = 0x02,
    }
}
