using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class MessageAuthentication : Semantic32ByteArray
    {
        public MessageAuthentication(byte[] value) : base(value)
        {
        }
    }
}
