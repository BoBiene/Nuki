using Nuki.Communication.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{
    public class RecieveNukiStatesCommand : RecieveBaseCommand
    {
        public NukiState NukiState { get { return GetData<NukiState>(nameof(NukiState)); } }
        public NukiLockState LockState { get { return GetData<NukiLockState>(nameof(LockState)); } }
        public NukiLockStateChangeTrigger Trigger { get { return GetData<NukiLockStateChangeTrigger>(nameof(Trigger)); } }

        public NukiTimeStamp CurrentTime { get { return GetData<NukiTimeStamp>(nameof(CurrentTime)); } }
        public Int16 UTCOffset { get { return GetData<Int16>(nameof(UTCOffset)); } }
        public bool CriticalBattery { get { return GetData<bool>(nameof(CriticalBattery)); } }

        public RecieveNukiStatesCommand()
            : base(CommandTypes.NukiStates, InitFields())
        {
        }

        protected static IEnumerable<FieldParserBase> InitFields()
        {
            yield return new FieldParser<NukiState>(nameof(NukiState), 1, (b, s, l) => (NukiState)b[s]);
            yield return new FieldParser<NukiLockState>(nameof(LockState), 1, (b, s, l) => (NukiLockState)b[s]);
            yield return new FieldParser<NukiLockStateChangeTrigger>(nameof(Trigger), 1, (b, s, l) => (NukiLockStateChangeTrigger)b[s]);
            yield return new FieldParser<NukiTimeStamp>(nameof(CurrentTime), 7, (b, s, l) => NukiTimeStamp.FromBytes(b, s));
            yield return new FieldParser<Int16>(nameof(UTCOffset), 2, (b, s, l) => BitConverter.ToInt16(b,s));
            yield return new FieldParser<bool>(nameof(CriticalBattery), 1, (b, s, l) => b[s] != 0);
        }
    }
}
