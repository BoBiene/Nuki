using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public class NukiCommandException : INukiErrorMessage
    {
        public Exception Exception { get; private set; }
        public NukiErrorCode ErrorCode => NukiErrorCode.ERROR_UNKNOWN;

        public NukiCommandType FailedCommand { get; private set; }

        public string Message => Exception.Message;

        public NukiCommandException(NukiCommandType failedCommand,Exception ex)
        {
            FailedCommand = failedCommand;
            Exception = ex;
        }
    }
}
