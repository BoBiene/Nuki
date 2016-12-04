using Nuki.Communication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiFobAction
    {
        NoAction = 0x00,
        Unlock = 0x01,
        Lock = 0x02,
        Lock_n_go = 0x03,
        /// <summary>
        /// unlock if locked, lock if unlocked
        /// </summary>
        Intelligent = 0x04,
    }
}
