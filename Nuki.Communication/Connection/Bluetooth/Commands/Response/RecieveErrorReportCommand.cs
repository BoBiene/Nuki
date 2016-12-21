using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class RecieveErrorReportCommand : RecieveBaseCommand
    {
        public NukiErrorCode ErrorCode => GetData<NukiErrorCode>(nameof(ErrorCode));
        public CommandTypes FailedCommand => GetData<CommandTypes>(nameof(FailedCommand));
        public RecieveErrorReportCommand() 
            : base(CommandTypes.ErrorReport, CreateFields())
        {
        }

        private static IEnumerable<FieldParserBase> CreateFields()
        {
            yield return new FieldParser<NukiErrorCode>(nameof(ErrorCode), sizeof(NukiErrorCode), (buffer, start, length) => (NukiErrorCode)buffer[start]);
            yield return new FieldParser<CommandTypes>(nameof(FailedCommand), sizeof(CommandTypes), (buffer, start, length) => (CommandTypes)BitConverter.ToUInt16(buffer,start));
        }
    }
}
