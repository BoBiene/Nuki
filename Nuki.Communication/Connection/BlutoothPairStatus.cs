using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public enum BlutoothPairStatus : byte
    {
        NoCharateristic = 0,
        MissingCharateristic = 1,
        Successfull = 2,
        Failed = 255,
        Timeout = 3,
        PairingNotActive = 4,
    }
}
