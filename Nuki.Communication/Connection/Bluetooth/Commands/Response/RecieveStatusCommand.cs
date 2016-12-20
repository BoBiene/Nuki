using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class RecieveStatusCommand : RecieveBaseCommand
    {
        public NukiErrorCode StatusCode => GetData<NukiErrorCode>(nameof(StatusCode));
        public RecieveStatusCommand()
            : base(CommandTypes.Status, CreateFields())
        {
        }

        private static IEnumerable<FieldParserBase> CreateFields()
        {
            yield return new FieldParser<NukiErrorCode>(nameof(StatusCode), sizeof(NukiErrorCode), (buffer, start, length) => (NukiErrorCode)buffer[start]);
        }
    }
}
