using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public class NukiCommandFailed : INukiErrorMessage
    {
        public NukiErrorCode ErrorCode
        {
            get; private set;
        }

        public NukiCommandType FailedCommand
        {
            get;private set;
        }

        public string Message
        {
            get { return string.Empty; }
        }
        public NukiCommandFailed(NukiCommandType requestedCmd, NukiErrorCode statusCode)
        {
            FailedCommand = requestedCmd;
            ErrorCode = statusCode;
        }
    }
}
