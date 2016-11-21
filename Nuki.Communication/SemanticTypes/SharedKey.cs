using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class SharedKey : Semantic32ByteArray
    {
        public SharedKey(byte[] clientPublicKey)
            : base(clientPublicKey)
        {

        }
        private SharedKey()
            : base(new byte[32])
        {

        }
    }
}
