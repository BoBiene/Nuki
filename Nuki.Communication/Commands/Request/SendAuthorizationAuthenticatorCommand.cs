using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public class SendAuthorizationAuthenticatorCommand : SendBaseCommandEncrypted
    {
        public ClientPublicKey ClientPublicKey { get { return GetData<ClientPublicKey>(nameof(ClientPublicKey)); } }
        public SmartLockPublicKey SmartLockPublicKey { get { return GetData<SmartLockPublicKey>(nameof(SmartLockPublicKey)); } }
        public SmartLockNonce ChallengeNonce { get { return GetData<SmartLockNonce>(nameof(ChallengeNonce)); } }

        /// <summary>
        /// Public Key (0x0003) 
        /// </summary>
        /// <param name="byPublicKey">The public key of the sender. </param>
        public SendAuthorizationAuthenticatorCommand(IConnectionContext context)
            : base(CommandTypes.AuthorizationAuthenticator,context, 3)
        {
            AddField(nameof(ClientPublicKey), context.ClientPublicKey, FieldFlags.PartOfAuthentication);
            AddField(nameof(SmartLockPublicKey), context.SmartLockPublicKey, FieldFlags.PartOfAuthentication);
            AddField(nameof(ChallengeNonce), context.SmartLockNonce, FieldFlags.PartOfAuthentication);
        }
    }
}
