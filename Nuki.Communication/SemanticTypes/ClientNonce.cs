using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class ClientNonce : Semantic32ByteArray
    {
        public ClientNonce(byte[] clientNonce)
            : base(clientNonce)
        {

        }
    }
}
    