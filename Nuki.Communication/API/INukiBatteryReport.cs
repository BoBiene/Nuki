using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    public interface INukiBatteryReport
    {
        /// <summary>
        /// The current battery drain in Milliamperes (mA). 
        /// </summary>
        UInt16 BatteryDrain { get; }
        /// <summary>
        /// The current battery voltage in Millivolts (mV). 
        /// </summary>
        UInt16 BatteryVoltage { get; }
        /// <summary>
        /// This flag signals a critical battery state. 
        /// </summary>
        bool CriticalBatteryState { get; }
    }
}
