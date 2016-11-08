using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public static class SemanticTypeExtentions
    {
        public static T ToSemanticType<T>(this byte[] array,Func<byte[], T> factory )
            where T:SemanticByteArray
        {
            return factory(array);
        }
    }
}
