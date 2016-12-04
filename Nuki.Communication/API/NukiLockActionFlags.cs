using Nuki.Communication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [Flags]
    [EnumBitSize(8)]
    public enum NukiLockActionFlags : Byte
    {
        None = 0x00,
        AutoUnlock = 0x01,
        ForceUnlock = 0x02,
    }
}
