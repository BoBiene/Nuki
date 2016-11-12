using SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public class BluetoothCharacteristic : SemanticType<Guid>
    {
        public BluetoothCharacteristic(Guid value) : base((g) => g != Guid.Empty, value)
        {
        }
        public BluetoothCharacteristic(string strID)
            : this(Guid.Parse(strID))
        {

        }
    }
}
