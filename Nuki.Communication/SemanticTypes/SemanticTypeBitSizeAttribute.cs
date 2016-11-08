using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class SemanticTypeByteSizeAttribute : Attribute
    {
        readonly int m_nByteSize;
        public SemanticTypeByteSizeAttribute(int nByteSize)
        {
            m_nByteSize = nByteSize;
        }

        public int ByteSize
        {
            get { return m_nByteSize; }
        }

    }
}
