using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class SmartLockPublicKey : SemanticByteArray
    {
        public SmartLockPublicKey(byte[] clientPublicKey)
            : base(clientPublicKey,32)
        {

        }
    }
}
