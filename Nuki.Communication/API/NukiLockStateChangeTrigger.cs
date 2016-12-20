using Nuki.Communication.Connection.Bluetooth.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(8)]
    public enum NukiLockStateChangeTrigger
    {
        /// <summary>
        /// via bluetooth command
        /// </summary>
        System = 0x00,
        /// <summary>
        /// by using a key from outside the door 
        /// or by rotating the wheel on the inside 
        /// </summary>
        Manual = 0x01,
        /// <summary>
        /// by pressing the Smartlocks button 
        /// </summary>
        Button =   0x02,
    }
}
