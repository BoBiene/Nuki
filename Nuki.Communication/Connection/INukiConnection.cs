using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public interface INukiConnection: INotifyPropertyChanged
    {

        bool Connected { get; }
        string DeviceName { get; }
        INukiDeviceStateMessage LastKnownDeviceState { get; }
        INukiErrorMessage LastError { get; }

        Task<INukiLogEntryCount> RequestNukiLogEntryCount();
        /// <summary>
        /// Request Log Entries
        /// </summary>
        /// <param name="blnMostRecent">Flag indicating whether or not the most recent <paramref name="nCount"/> entries shouldbe retrieved.</param>
        /// <param name="nStartIndex">The index where to start reading log entries.
        /// Ignored if <paramref name="blnMostRecent"/> above is <c>true</c>. </param>
        /// <param name="nCount">The number of log entries to be read, starting atthe specified <paramref name="nStartIndex"/></param>
        /// <param name="nSecurityPIN">The security pin.</param>
        /// <returns></returns>
        Task<IEnumerable<INukiLogEntry>> RequestLogEntries(bool blnMostRecent, UInt16 nStartIndex, UInt16 nCount, UInt16 nSecurityPIN);
        Task<INukiConfigMessage> RequestNukiConfig();
        Task<INukiDeviceStateMessage> RequestNukiState();
        Task<INukiBatteryReport> RequestNukiBatteryReport();
        Task<INukiReturnMessage> SendCalibrateRequest(ushort securityPin);
        Task<INukiReturnMessage> SendLockAction(NukiLockAction lockAction, NukiLockActionFlags flags = NukiLockActionFlags.None);
    }
}
