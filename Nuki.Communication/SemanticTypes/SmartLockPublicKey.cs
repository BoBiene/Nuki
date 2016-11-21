using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class SmartLockPublicKey : Semantic32ByteArray
    {
        public SmartLockPublicKey(byte[] clientPublicKey)
            : base(clientPublicKey)
        {

        }
        private SmartLockPublicKey()
            : base(new byte[32])
        {

        }
    }
}
