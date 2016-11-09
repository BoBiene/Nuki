using Nuki.Communication.SemanticTypes;
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
        protected byte[] Data { get; private set; }
        public UInt16 CRC { get { return GetData<UInt16>(nameof(CRC)); } }
        protected RecieveBaseCommand(CommandTypes type, byte[] data, IEnumerable<FieldParserBase> fields)
            : base(BuildFields(type,data, fields))
        {
            Data = data;
        }

        private static IEnumerable<DataField> BuildFields(CommandTypes type, byte[] data, IEnumerable<FieldParserBase> fields)
        {
            int nArrayParsePos = 0;
            int nFieldPos = 0;
            int nFieldLength = 2;
            yield return new DataFieldObjectValue(nameof(CommandType), nFieldPos++, type, nFieldLength, FieldFlags.All);
            nArrayParsePos += nFieldLength;

            foreach (var field in fields)
            {
                yield return new DataFieldObjectValue(field.FieldName, nFieldPos++, field.GetValue(data, nArrayParsePos), field.ByteLength, FieldFlags.All);
                nArrayParsePos += field.ByteLength;
            }

            UInt16 nCrcMessage = BitConverter.ToUInt16(data, nArrayParsePos);
            UInt16 nCalculated = CRC16.NonZero.ComputeChecksum(data, nArrayParsePos);
            if (nCrcMessage != nCalculated)
                throw new CrcMissmatchException();

            yield return new DataFieldObjectValue(nameof(CRC), nFieldPos++, nCrcMessage,2, FieldFlags.All);
            nArrayParsePos += 2;

            if (nArrayParsePos != data.Length)
                throw new MessageParseException();
        }

        public static byte[] SeperateByteArray(byte[] data, int nStart, int nLength)
        {
            byte[] b = new byte[nLength];
            for(int i=0; i< nLength;++i)
            {
                b[i] = data[nStart + i];
            }

            return b;
        }
        protected static FieldParser<T>.ParseValueDelegate SeperateSemanticType<T>(Func<byte[], T> factory)
            where T: SemanticByteArray
        {
            return (data, nStart, nLength) => factory(SeperateByteArray(data, nStart, nLength));
        }
        public virtual bool IsValid()
        {
            return true; //TODO Validate CRC
        }


        protected abstract class FieldParserBase
        {
            
            public virtual int ByteLength { get; private set; }
            
            public string FieldName { get; private set; }

            public FieldFlags FieldFlags { get; set; } = FieldFlags.All;
            public FieldParserBase(string strName, int nByteLength )
            {
                FieldName = strName;
                ByteLength = nByteLength;
            }

            public abstract object GetValue(byte[] data, int nOffset);
    
        }
        protected class FieldParser<T> : FieldParserBase
        {
            public delegate T ParseValueDelegate(byte[] data, int start, int length);
            protected ParseValueDelegate ParseValue { get; private set; }
            public FieldParser(string strName, int nByteLength, ParseValueDelegate parseValue)
                : base(strName,nByteLength)

            {
                ParseValue = parseValue;
            }
            public override object GetValue(byte[] data, int nOffset)
            {
                return ParseValue(data, nOffset, ByteLength);
            }
        }
    
    }
}
