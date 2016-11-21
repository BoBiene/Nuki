using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class ClientPublicKey : Semantic32ByteArray
    {
        public ClientPublicKey(byte[] clientPublicKey)
            : base(clientPublicKey)
        {

        }
        private ClientPublicKey()
            : base(new byte[32])
        {

        }
    }
}
