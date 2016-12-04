using Nuki.Communication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiLockAction : Byte
    {
        Unlock = 0x01,
        Lock = 0x02,
        Unlatch = 0x03,
        Lock_n_go = 0x04,
        Lock_n_go_withunlatch = 0x05,
        FobAction_1 = 0x81,
        FobAction_2 = 0x82,
        FobAction_3 = 0x83,
    }
}
