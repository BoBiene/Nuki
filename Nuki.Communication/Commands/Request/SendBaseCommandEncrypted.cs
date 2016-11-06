using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Request
{
    public abstract class SendBaseCommandEncrypted : SendBaseCommand
    {
        public byte[] Authenticator { get; private set; }
        public SendBaseCommandEncrypted(byte[] byAuthenticator, CommandTypes type, int nNumberOfFields)
            : base(type, nNumberOfFields + 1)
        {
            Authenticator = byAuthenticator;
        }
    }
}
