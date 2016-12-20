using Nuki.Communication.Connection.Bluetooth.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiDSTSetting
    {
        Disabled = 0x00,
        European = 0x01,
    }
}
