using Nuki.Communication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{

    public class RecieveBaseCommand : BaseCommand
    {
        public UInt16 CRC { get { return GetData<UInt16>(nameof(CRC)); } }
        protected RecieveBaseCommand(CommandTypes type, byte[] data, IEnumerable<FieldParser> fields)
            : base(BuildFields(type,data, fields))
        {

        }

        private static IEnumerable<DataField> BuildFields(CommandTypes type, byte[] data, IEnumerable<FieldParser> fields)
        {
            int nArrayParsePos = 0;
            int nFieldPos = 0;
            int nFieldLength = 2;
            yield return new DataField(nameof(CommandType), nFieldPos++, type, FieldFlags.All);
            nArrayParsePos += nFieldLength;

            foreach (var field in fields)
            {
                yield return new DataField(field.FieldName, nFieldPos++, field.GetValue(data, nArrayParsePos), FieldFlags.All);
                nArrayParsePos += field.ByteLength;
            }

            UInt16 nCrcMessage = BitConverter.ToUInt16(data, nArrayParsePos);
            UInt16 nCalculated = CRC16.NonZero.ComputeChecksum(data, nArrayParsePos);
            if (nCrcMessage != nCalculated)
                throw new CrcMissmatchException();

            yield return new DataField(nameof(CRC), nFieldPos++, nCrcMessage, FieldFlags.All);
            nArrayParsePos += 2;

            if (nArrayParsePos != data.Length)
                throw new MessageParseException();
        }

        public static byte[] SubArray(byte[] data, int nStart, int nLength)
        {
            byte[] b = new byte[nLength];
            for(int i=0; i< nLength;++i)
            {
                b[i] = data[nStart + i];
            }

            return b;
        }

        protected class FieldParser
        {
            public delegate object ParseValueDelegate(byte[] data, int start, int length);
            public virtual int ByteLength { get; private set; }
            protected ParseValueDelegate ParseValue { get; private set; }
            public string FieldName { get; private set; }

            public FieldParser(string strName, int nByteLength, ParseValueDelegate parseValue )
            {
                FieldName = strName;
                ByteLength = nByteLength;
                ParseValue = parseValue;
            }

            public object GetValue(byte[] data, int nOffset)
            {
                return ParseValue(data, nOffset, ByteLength);
            }
        }
    }
}
