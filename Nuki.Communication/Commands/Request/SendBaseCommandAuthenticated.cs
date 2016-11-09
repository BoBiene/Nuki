using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public abstract class SendBaseCommandAuthenticated : SendBaseCommand
    {
        public IConnectionContext ConnectionContext { get; private set; }
        public MessageAuthentication Authenticator { get { return GetData<MessageAuthentication>(nameof(Authenticator)); } }
        public SendBaseCommandAuthenticated(CommandTypes type, IConnectionContext connectionContext, int nNumberOfFields)
            : base(type, nNumberOfFields + 1)
        {
            ConnectionContext = connectionContext;
            AddField(nameof(Authenticator), CalculateAuthenticator,32, FieldFlags.PartOfMessage);
        }

        private MessageAuthentication CalculateAuthenticator()
        {
            return new MessageAuthentication(Sodium.SecretKeyAuth.SignHmacSha256(Serialize(FieldFlags.PartOfAuthentication, false).ToArray(), ConnectionContext.SharedKey));
        }
    }
}
