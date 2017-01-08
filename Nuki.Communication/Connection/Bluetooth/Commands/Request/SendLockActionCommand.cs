using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Connection;
using Nuki.Communication.API;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Request
{
    public class SendLockActionCommand : SendBaseCommandWithContext
    {

        public NukiLockAction LockAction { get { return GetData<NukiLockAction>(nameof(LockAction)); } }
        public UniqueClientID UniqueClientID { get { return GetData<UniqueClientID>(nameof(UniqueClientID)); } }
        public NukiLockActionFlags Flags { get { return GetData<NukiLockActionFlags>(nameof(Flags)); } }

        public SmartLockNonce SmartLockNonce { get { return GetData<SmartLockNonce>(nameof(SmartLockNonce)); } }


        public SendLockActionCommand(NukiLockAction lockAction, NukiLockActionFlags flags, IConnectionContext connectionContext) :
            base(NukiCommandType.LockAction, connectionContext, 4)
        {
            AddField(nameof(LockAction), lockAction);
            AddField(nameof(UniqueClientID), connectionContext.UniqueClientID);
            AddField(nameof(Flags), flags);
            AddField(nameof(SmartLockNonce), connectionContext.SmartLockNonce);

        }
    }
}
