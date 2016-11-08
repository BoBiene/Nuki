using SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.SemanticTypes
{
    public abstract class SemanticByteArray : SemanticTypeBase<byte[]>, IEquatable<SemanticTypeBase<byte[]>>, IComparable<SemanticTypeBase<byte[]>>
    {
        public SemanticByteArray(byte[] value, int nByteArrayLength) : base((b) => b?.Length == nByteArrayLength, value)
        {
        }

        public static implicit operator byte[] (SemanticByteArray value)
        {
            return value?.Value;
        }
        
        public int CompareTo(SemanticTypeBase<byte[]> other)
        {
            int nret = 0;
            if (other == null)
                return 1;
            if (other.Value == null)
            {
                if (this.Value == null)
                {
                    nret = 0;
                }
                else
                {
                    nret = 0;
                }
            }
            else if (this.Value == null)
            {
                nret = -1;
            }
            else
            {
                for (int i = 0; i < other.Value.Length && nret == 0; ++i)
                    nret = this.Value[i].CompareTo(other.Value[i]);
            }
            return nret;
        }

        public override bool Equals(SemanticTypeBase<byte[]> other)
        {
            return this.Value == other.Value || this.CompareTo(other) == 0;
        }
    }
}
