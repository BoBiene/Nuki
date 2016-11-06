using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Util
{
    public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }
    /// <summary>
    /// Source: http://www.sanity-free.org/133/crc_16_ccitt_in_csharp.html
    /// </summary>
    public class CRC16
    {
        public static readonly CRC16 NonZero = new CRC16(InitialCrcValue.NonZero1);

        const ushort poly = 4129;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        public ushort ComputeChecksum(IEnumerable<byte> bytes)
        {
            return ComputeChecksum(bytes, -1);
        }
        public ushort ComputeChecksum(IEnumerable<byte> bytes,int nLength)
        {
            int i = 0;
            ushort crc = this.initialValue;
            foreach (byte singleByte in bytes)
            {
                if (nLength < 0 || i++ < nLength)
                    crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & singleByte))]);
                else
                    break;
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(IEnumerable<byte> bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public CRC16(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }
    }
}
