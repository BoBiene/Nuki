using Nuki.Communication.API;
using Nuki.Communication.SemanticTypes;
using Nuki.Communication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{

    public class RecieveBaseCommand : BaseCommand
    {
        private byte[] m_byData = null;
        private int m_nRecievePointer = 0;
        private FieldParserBase[] m_fields = null;
        public UInt16 CRC { get { return GetData<UInt16>(nameof(CRC)); } }
        protected RecieveBaseCommand(NukiCommandType type, IEnumerable<FieldParserBase> fields)
           :this(type,AddCRCField( fields))
        {
         
        }

        private static FieldParserBase[] AddCRCField(IEnumerable<FieldParserBase> fields)
        {
            List<FieldParserBase> tmp = new List<FieldParserBase>();
            foreach (var field in fields)
                tmp.Add( field);
            tmp.Add(new FieldParser<UInt16>(nameof(CRC), sizeof(UInt16), (buffer, pos, length) => BitConverter.ToUInt16(buffer, pos)));

            return tmp.ToArray() ;
        }

        private RecieveBaseCommand(NukiCommandType type, FieldParserBase[] fields)
            : base(type,fields.Length)
        {
            m_fields = fields;
            int nBytesCount = 0;
            foreach(var field in fields)
            {
                nBytesCount += field.ByteLength;
            }
            m_byData = new byte[nBytesCount];
        }

        public int BytesRecieved => m_nRecievePointer;
        public int BytesTotal => m_byData.Length;
        public bool Complete => m_nRecievePointer >= m_byData.Length;
        protected byte[] Data => m_byData;

        public void ProcessRecievedData(IDataReader reader)
        {
            if (!Complete)
            {
                byte[] buffer = new byte[(int)reader.UnconsumedBufferLength];
                reader.ReadBytes(buffer);
                ProcessRecievedData(buffer);
              
            }
            else { }
        }

        public void ProcessRecievedData(byte[] buffer)
        {
            if (!Complete)
            {
                Array.Copy(buffer, 0, m_byData, m_nRecievePointer, Math.Min(buffer.Length, m_byData.Length - m_nRecievePointer));
                m_nRecievePointer += buffer.Length;
                if (Complete)
                {
                    AddFields(BuildFields(FieldPointer, m_byData, m_fields));

                }
                else { }
            }
            else { }
        }
        private static IEnumerable<DataField> BuildFields(int nFieldPos, byte[] data, IEnumerable<FieldParserBase> fields)
        {
            int nArrayParsePos = 0;
            foreach (var field in fields)
            {
                yield return new DataFieldObjectValue(field.FieldName, nFieldPos++, field.GetValue(data, nArrayParsePos), field.ByteLength, FieldFlags.All);
                nArrayParsePos += field.ByteLength;
            }

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
            //UInt16 nCalculated = CRC16.NonZero.ComputeChecksum(data, nArrayParsePos - 2);

            return Complete; //TODO Validate CRC
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
