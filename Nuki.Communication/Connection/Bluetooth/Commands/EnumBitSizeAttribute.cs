using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands
{
    [System.AttributeUsage(AttributeTargets.Enum, Inherited = true, AllowMultiple = false)]
    sealed class EnumBitSizeAttribute : Attribute
    {
        readonly int m_nBitSize;

        public EnumBitSizeAttribute(int nBitSize)
        {
            m_nBitSize = nBitSize;
            if (nBitSize % 8 != 0)
                throw new ArgumentException("Bit size must be % 8 == 0");
        }

        public int BitSize
        {
            get { return m_nBitSize; }
        }
    }
}
