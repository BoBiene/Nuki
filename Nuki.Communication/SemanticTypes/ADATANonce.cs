using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class ADATANonce : SemanticByteArray
    {
        public ADATANonce(byte[] clientPublicKey)
            : base(clientPublicKey,24)
        {

        }
    }
}
