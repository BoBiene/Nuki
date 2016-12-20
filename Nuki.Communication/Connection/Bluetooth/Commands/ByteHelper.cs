using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands
{
    public static class ByteHelper
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(SemanticByteArray ba)
        {
            return ByteArrayToString(ba.Value);
        }
        public static string ByteArrayToString(IEnumerable<byte> ba)
        {
            StringBuilder hex = new StringBuilder(ba.Count() * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }

    }
}
