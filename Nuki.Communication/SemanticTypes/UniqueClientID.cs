using SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class UniqueClientID : SemanticType<UInt32>
    {
        public UniqueClientID(UInt32 uniqueClientID)
            : base((id) => true, uniqueClientID)
        {

        }
    }
}
