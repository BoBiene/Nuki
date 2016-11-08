using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class ClientPrivateKey : Semantic32ByteArray
    {
        public ClientPrivateKey(byte[] clientPublicKey)
            : base(clientPublicKey)
        {

        }
    }
}
