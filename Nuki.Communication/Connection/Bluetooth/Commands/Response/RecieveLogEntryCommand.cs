using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class RecieveLogEntryCommand : RecieveBaseCommand,INukiLogEntry
    {
        public NukiLockActionFlags Flags { get { return GetData<NukiLockActionFlags>(nameof(Flags)); } }

        public ushort Index { get { return GetData<ushort>(nameof(Index)); } }
        public byte Type { get { return GetData<byte>(nameof(Type)); } }
        public NukiLockAction LockAction { get { return GetData<NukiLockAction>(nameof(LockAction)); } }

        public bool LoggingEnabled { get { return GetData<bool>(nameof(LoggingEnabled)); } }

        public string Name { get { return GetData<string>(nameof(Name)); } }

        public NukiTimeStamp Timestamp { get { return GetData<NukiTimeStamp>(nameof(Timestamp)); } }

        public NukiLockStateChangeTrigger Trigger { get { return GetData<NukiLockStateChangeTrigger>(nameof(Trigger)); } }

        public RecieveLogEntryCommand()
            : base(NukiCommandType.LogEntry, CreateFields())
        {
        }

        private static IEnumerable<FieldParserBase> CreateFields()
        {
            yield return new FieldParser<ushort>(nameof(Index), sizeof(ushort), (buffer, start, length) => BitConverter.ToUInt16(buffer, start));
            yield return new FieldParser<NukiTimeStamp>(nameof(Timestamp), 7, (buffer, start, length) => NukiTimeStamp.FromBytes(buffer, start));
            yield return new FieldParser<string>(nameof(Name), 32, (buffer, start, length) => Encoding.ASCII.GetString(buffer, start, length));
            yield return new FieldParser<byte>(nameof(Type), 1, (buffer, start, length) => buffer[start]);
            yield return new FieldParser<NukiLockAction>(nameof(LockAction), 1, (buffer, start, length) => (NukiLockAction)buffer[start]);
            yield return new FieldParser<NukiLockStateChangeTrigger>(nameof(Trigger), 1, (buffer, start, length) => (NukiLockStateChangeTrigger)buffer[start]);
            yield return new FieldParser<NukiLockActionFlags>(nameof(Flags), 1, (buffer, start, length) => (NukiLockActionFlags)buffer[start]);
        }
    }
}
