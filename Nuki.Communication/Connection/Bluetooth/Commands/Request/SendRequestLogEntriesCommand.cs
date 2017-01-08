using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.API;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public class SendRequestLogEntriesCommand : SendBaseCommand
    {
        public bool MostRecent { get { return GetData<bool>(nameof(MostRecent)); } }
        public UInt16 StartIndex { get { return GetData<UInt16>(nameof(StartIndex)); } }
        public UInt16 Count { get { return GetData<UInt16>(nameof(Count)); } }
        public UInt16 SecurityPIN { get { return GetData<UInt16>(nameof(SecurityPIN)); } }

        public SmartLockNonce SmartLockNonce { get { return GetData<SmartLockNonce>(nameof(SmartLockNonce)); } }

        public SendRequestLogEntriesCommand(IConnectionContext context, bool blnMostRecent,UInt16 nStartIndex, UInt16 nCount, UInt16 securityPin)
            : base(NukiCommandType.RequestLogEntries,5)
        {
            AddField(nameof(MostRecent), blnMostRecent);
            AddField(nameof(StartIndex), nStartIndex);
            AddField(nameof(Count), nCount);    
            AddField(nameof(SmartLockNonce), context.SmartLockNonce);
            AddField(nameof(SecurityPIN), securityPin);
        }
    }
}
