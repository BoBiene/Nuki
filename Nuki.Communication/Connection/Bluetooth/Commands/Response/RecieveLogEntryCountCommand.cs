using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
   public class RecieveLogEntryCountCommand : RecieveBaseCommand, INukiLogEntryCount
    {
        public bool LoggingEnabled { get { return GetData<bool>(nameof(LoggingEnabled)); } }
        public UInt16    Count { get { return GetData<UInt16>(nameof(Count)); } }

        public RecieveLogEntryCountCommand()
            : base(NukiCommandType.LogEntryCount, InitFields())
        {
        }

        protected static IEnumerable<FieldParserBase> InitFields()
        {
            yield return new FieldParser<bool>(nameof(LoggingEnabled), 1, (b, s, l) => b[s] != 0);
            yield return new FieldParser<UInt16>(nameof(Count), 1, (b, s, l) => BitConverter.ToUInt16(b, s));
        }
    }
}
