using Nuki.Communication.Connection.Bluetooth.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    [EnumBitSize(16)]
    public enum NukiCommandType : UInt16
    {
        RequestData = 0x0001,
        PublicKey = 0x0003,
        Challenge = 0x0004,
        AuthorizationAuthenticator = 0x0005,
        AuthorizationData = 0x0006,
        Authorization­ID = 0x0007,
        RemoveUserAuthorization = 0x0008,
        RequestAuthorizationEntries = 0x0009,
        AuthorizationEntry = 0x000A,
        AuthorizationData_Invite = 0x000B,
        NukiStates = 0x000C,
        LockAction = 0x000D,
        Status = 0x000E,
        MostRecentCommand = 0x000F,
        OpeningsClosingsSummary = 0x0010,
        BatteryReport = 0x0011,
        ErrorReport = 0x0012,
        SetConfig = 0x0013,
        RequestConfig = 0x0014,
        Config = 0x0015,
        SetSecurityPIN = 0x0019,
        RequestCalibration = 0x001A,
        RequestReboot = 0x001D,
        Authorization­IDConfirmation = 0x001E,
        Authorization­ID_Invite = 0x001F,
        VerifySecurityPIN = 0x0020,
        UpdateTime = 0x0021,
        EnableLogging = 0x0022,
        RequestLogEntries = 0x0023,
        LogEntry = 0x0024,
        UpdateUserAuthorization = 0x0025,
        LogEntryCount = 0x0026,
        AuthorizationEntryCount = 0x0027,
    }
}
