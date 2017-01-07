using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    public interface INukiLogEntry
    {
        /// <summary>
        /// The index of the log entry. 
        /// </summary>
        UInt16 Index { get; }
        NukiTimeStamp Timestamp { get; }
        /// <summary>
        /// The name of the authorization. 
        /// </summary>
        string Name { get; }
        bool LoggingEnabled { get; }
        NukiLockAction LockAction { get; }
        NukiLockStateChangeTrigger Trigger { get; }
        NukiLockActionFlags Flags { get; }
    }
}
