using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class RecieveErrorReportCommand : RecieveBaseCommand, INukiErrorMessage
    {
        public NukiErrorCode ErrorCode => GetData<NukiErrorCode>(nameof(ErrorCode));
        public NukiCommandType FailedCommand => GetData<NukiCommandType>(nameof(FailedCommand));

        public RecieveErrorReportCommand() 
            : base(NukiCommandType.ErrorReport, CreateFields())
        {
        }

        private static IEnumerable<FieldParserBase> CreateFields()
        {
            yield return new FieldParser<NukiErrorCode>(nameof(ErrorCode), sizeof(NukiErrorCode), (buffer, start, length) => (NukiErrorCode)buffer[start]);
            yield return new FieldParser<NukiCommandType>(nameof(FailedCommand), sizeof(NukiCommandType), (buffer, start, length) => (NukiCommandType)BitConverter.ToUInt16(buffer,start));
        }
    }
}
