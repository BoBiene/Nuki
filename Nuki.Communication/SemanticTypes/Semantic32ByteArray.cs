using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    [SemanticTypeByteSizeAttribute(32)]
    public abstract class Semantic32ByteArray : SemanticByteArray
    {
        public Semantic32ByteArray(byte[] value) : base(value, 32)
        {
        }
    }
}
