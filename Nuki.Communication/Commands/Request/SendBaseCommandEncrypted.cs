using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public abstract class SendBaseCommandEncrypted : SendBaseCommand
    {
        public byte[] Authenticator { get { return GetData<byte[]>(nameof(Authenticator)); } }
        public SendBaseCommandEncrypted(CommandTypes type, int nNumberOfFields)
            : base(type, nNumberOfFields + 1)
        {
            AddField(nameof(Authenticator), FieldFlags.All, CalculateAuthenticator);
        }

        private byte[] CalculateAuthenticator()
        {
            throw new NotImplementedException();
        }
    }
}
