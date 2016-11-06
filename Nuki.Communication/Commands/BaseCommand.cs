using Nuki.Communication.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Nuki.Communication.Commands
{
    public abstract class BaseCommand
    {
        [Flags]
        protected enum FieldFlags
        {
            PartOfMessage = 0x01,
            PartOfAuthentication = 0x02,
            All = 0xFF
        }
        private int m_nFieldPointer = -1;
        private ConcurrentDictionary<string, DataField> m_mapByFieldName = new ConcurrentDictionary<string, DataField>(StringComparer.OrdinalIgnoreCase);
        private DataField[] m_mapByPostion = null;
        public CommandTypes CommandType { get { return GetData<CommandTypes>(nameof(CommandType)); } }

        public BaseCommand(CommandTypes type,int nNumberOfFields)
        {
            m_mapByPostion = new DataField[nNumberOfFields+1];
            AddField(nameof(CommandType), type);
        }

        protected int AddField(string strName, object data)
        {
            return AddField(strName, data, FieldFlags.All);
        }
        protected int AddField(string strName, object data, FieldFlags flags)
        {
            int nPos = Interlocked.Increment(ref m_nFieldPointer);
            DataField field = new DataField(strName, nPos, data,flags);
            m_mapByPostion[nPos] = field;
            m_mapByFieldName[strName] = field;
            return nPos;
        }

        protected void SetData(string strName, IEnumerable<byte> byData)
        {
            m_mapByFieldName[strName].Data = byData;
        }



        protected T GetData<T>(int nDataPosition)
        {
            return (T)m_mapByPostion[nDataPosition].Data;
        }

        protected T GetData<T>(string strKey)
        {
            return (T)m_mapByFieldName[strKey].Data;
        }


        protected IEnumerable<DataField> GetData(FieldFlags flag)
        {
            foreach (var field in m_mapByPostion)
                if (field.Flags.HasFlag(flag))
                    yield return field;
        }

        protected class DataField
        {
            public string Name { get; private set; }
            public int Position { get; private set; }
            public object Data { get; set; }
            public FieldFlags Flags { get; private set; }

            public DataField(string strName, int nPositon, object data, FieldFlags flags)
            {
                Name = strName;
                Position = nPositon;
                Data = data;
                Flags = flags;
            }

            public IEnumerable<byte> Serialize()
            {
                if (Data == null)
                    return new byte[0];
                else if (Data is Enum)
                {
                    int nBitSize = Data.GetType().GetTypeInfo().
                        GetCustomAttribute<EnumBitSizeAttribute>()?.BitSize ?? 32;

                    
                    switch (nBitSize)
                    {
                        
                        case 8:
                            return BitConverter.GetBytes((byte)(Data));
                        case 16:
                            return BitConverter.GetBytes((UInt16)(Data));
                        case 64:
                            return BitConverter.GetBytes((UInt64)(Data));
                        case 32:
                            return BitConverter.GetBytes((UInt32)(Data));

                    }
                }

                throw new NotImplementedException();
            }
        }

        


        public virtual IEnumerable<byte> Serialize()
        {
            List<byte> list = new List<byte>();
            foreach (var field in GetData(FieldFlags.PartOfMessage))
                list.AddRange(field.Serialize());
            
            list.AddRange(CRC16.NonZero.ComputeChecksumBytes(list));
            return list;
        }

    }
}
