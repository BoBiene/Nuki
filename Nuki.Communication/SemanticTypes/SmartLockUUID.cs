using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    [SemanticTypeByteSize(16)]
    public class SmartLockUUID : SemanticByteArray
    {
        public SmartLockUUID(byte[] value) : base(value,16)
        {
        }
        private SmartLockUUID()
            : this(new byte[16])
        {

        }

    }
}
