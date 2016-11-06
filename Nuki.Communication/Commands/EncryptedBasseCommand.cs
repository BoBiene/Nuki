using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands
{
    public abstract class EncryptedBasseCommand : BaseCommand
    {
        public byte[] Authenticator { get; private set; }
        public EncryptedBasseCommand(byte[] byAuthenticator, CommandTypes type, int nNumberOfFields)
            : base(type, nNumberOfFields + 1)
        {
            Authenticator = byAuthenticator;
        }
    }
}
