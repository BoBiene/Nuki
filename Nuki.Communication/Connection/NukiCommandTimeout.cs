using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public class NukiCommandTimeout : INukiErrorMessage
    {
        public NukiErrorCode ErrorCode
        {
            get
            {
                return NukiErrorCode.ERROR_COMMAND_TIMEOUT;
            }
        }

        public NukiCommandType FailedCommand
        {
            get;
            private set;
        }

        public string Message { get; private set; }
        public int TimeoutTime { get; private set; }

        internal NukiCommandTimeout(SendBaseCommand bleCommand, int nTimeoutTime)
            : this(bleCommand.CommandType, nTimeoutTime)
        {
            if (bleCommand is SendRequestDataCommand)
                Message = $"Requested Command {((SendRequestDataCommand)bleCommand).ReqestedCommand }";
            else
                Message = string.Empty;
        } 
        public NukiCommandTimeout(NukiCommandType failedCommand, int nTimeoutTime)
        {
            FailedCommand = failedCommand;
            TimeoutTime = nTimeoutTime;

        }
    }
}
