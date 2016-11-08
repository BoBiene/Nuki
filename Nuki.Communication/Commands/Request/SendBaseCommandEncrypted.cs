using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Commands.Request
{
    public class SendBaseCommandEncrypted : SendBaseCommandAuthenticated
    {
        public SmartLockNonce SmartLockNonce {  get { return GetData<SmartLockNonce>(nameof(SmartLockNonce)); } }
        public ClientNonce ClientNonce { get { return GetData<ClientNonce>(nameof(ClientNonce)); } }

        public SendBaseCommandEncrypted(CommandTypes type, IConnectionContext connectionContext, int nNumberOfFields) :
            base(type, connectionContext, nNumberOfFields +2)
        {
            AddField(nameof(ClientNonce), connectionContext.CreateNonce(), FieldFlags.All);
            AddField(nameof(SmartLockNonce), connectionContext.SmartLockNonce, FieldFlags.PartOfAuthentication);
        }
    }
}
