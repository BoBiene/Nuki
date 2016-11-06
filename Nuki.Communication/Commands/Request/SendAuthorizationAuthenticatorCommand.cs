using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public class SendAuthorizationAuthenticatorCommand : SendBaseCommandEncrypted
    {
        public byte[] ClientPublicKey { get { return GetData<byte[]>(nameof(ClientPublicKey)); } }
        public byte[] SmartLockPublicKey { get { return GetData<byte[]>(nameof(SmartLockPublicKey)); } }
        public byte[] ChallengeNonce { get { return GetData<byte[]>(nameof(ChallengeNonce)); } }

        /// <summary>
        /// Public Key (0x0003) 
        /// </summary>
        /// <param name="byPublicKey">The public key of the sender. </param>
        public SendAuthorizationAuthenticatorCommand(byte[] byAuthenticator, byte[] byClientPublicKey, byte[] bySmartLockPublicKey, byte[] byChallengeNonce)
            : base(CommandTypes.AuthorizationAuthenticator, 3)
        {
            AddField(nameof(ClientPublicKey), byClientPublicKey, FieldFlags.PartOfAuthentication);
            AddField(nameof(SmartLockPublicKey), bySmartLockPublicKey, FieldFlags.PartOfAuthentication);
            AddField(nameof(ChallengeNonce), byChallengeNonce, FieldFlags.PartOfAuthentication);
        }
    }
}
