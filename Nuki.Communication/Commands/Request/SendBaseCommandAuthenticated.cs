using Nuki.Communication.Connection;
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
        public byte[] Authenticator { get { return GetData<byte[]>(nameof(Authenticator)); } }
        public SendBaseCommandAuthenticated(CommandTypes type, IConnectionContext connectionContext, int nNumberOfFields)
            : base(type, nNumberOfFields + 1)
        {
            ConnectionContext = connectionContext;
            AddField(nameof(Authenticator), CalculateAuthenticator,32, FieldFlags.PartOfMessage);
        }

        private byte[] CalculateAuthenticator()
        {
            return Sodium.SecretKeyAuth.SignHmacSha256(Serialize(FieldFlags.PartOfAuthentication,false).ToArray(), ConnectionContext.SharedKey);
        }
    }
}
