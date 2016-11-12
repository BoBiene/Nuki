using SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class BluetoothServiceUUID : SemanticType<Guid>
    {
        public BluetoothServiceUUID( Guid value) : base((g) => g != Guid.Empty, value)
        {
        }
        public BluetoothServiceUUID(string strID)
            : this(Guid.Parse(strID))
        {

        }
    }
}
