using Nuki.Communication.SemanticTypes;
using SemanticTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public BaseCommand(CommandTypes type, int nNumberOfFields)
        {
            m_mapByPostion = new DataField[nNumberOfFields + 1];
            AddField(nameof(CommandType), type,FieldFlags.PartOfMessage);
        }

        protected BaseCommand(IEnumerable<DataField> data)
        {
            int nPos = -1;
            foreach (var field in data)
            {
                m_mapByFieldName[field.Name] = field;
                nPos = Math.Max(field.Position, nPos);
            }
            m_mapByPostion = m_mapByFieldName.Values.OrderBy((f) => f.Position).ToArray();
            m_nFieldPointer = nPos;
        }

        protected int AddField<T>(string strName, T data)
        {
            return AddField(strName, data, FieldFlags.All);
        }
        protected int AddField<T>(string strName, T data, FieldFlags flags)
        {
            return AddField((nPos) => new DataFieldTypedValue<T>(strName, nPos, data, flags));
        }
        protected int AddField<T>(string strName, T data, int nFieldByteLength, FieldFlags flags)
        {
            return AddField((nPos) => new DataFieldTypedValue<T>(strName, nPos, data,nFieldByteLength, flags));
        }

        protected int AddField<T>(string strName, Func<T> valueGetter,int nFieldByteLength, FieldFlags flags)
        {
            return AddField((nPos) => new DataFieldAction<T>(strName, nPos,flags, nFieldByteLength, valueGetter));
        }

        private int AddField(Func<int, DataField> fieldFactroy)
        {
            int nPos = Interlocked.Increment(ref m_nFieldPointer);
            var field = fieldFactroy(nPos);
            m_mapByPostion[field.Position] = field;
            m_mapByFieldName[field.Name] = field;
            return nPos;
        }
        
        protected void SetData(string strName, object objData)
        {
            m_mapByFieldName[strName].SetData(objData);
        }



        protected T GetData<T>(int nDataPosition)
        {
            return (T)m_mapByPostion[nDataPosition].GetData();
        }

        protected T GetData<T>(string strKey)
        {
            return (T)m_mapByFieldName[strKey].GetData();
        }


        protected IEnumerable<DataField> GetData(FieldFlags flag)
        {
            foreach (var field in m_mapByPostion)
                if (field?.Flags.HasFlag(flag) == true)
                    yield return field;
        }

        protected abstract class DataField
        {
            public string Name { get; private set; }
            public int Position { get; private set; }
            public FieldFlags Flags { get; private set; }
            public abstract int ByteLength { get; }
            public abstract object GetData();
            public abstract void SetData(object objData);

            public DataField(string strName, int nPositon, FieldFlags flags)
            {
                Name = strName;
                Position = nPositon;
                Flags = flags;
            }
        }

        protected class DataFieldAction<T> : DataField
        { 
            private int m_nByteLength = 0;
            public Func<T> ValueGetter { get; private set; }
            public override int ByteLength
            {
                get { return m_nByteLength; }
            }
            public DataFieldAction(string strName, int nPositon, FieldFlags flags, int nByteLength, Func<T> valueGetter) : base(strName, nPositon, flags)
            {

                ValueGetter = valueGetter;
                m_nByteLength = nByteLength;
            }

            public override object GetData()
            {
                return ValueGetter();
            }

            public override void SetData(object objData)
            {
                throw new NotImplementedException();
            }
        }

        protected class DataFieldTypedValue<T> : DataField
        {
            private int m_nByteLength = 0;
            public T Data { get; set; }

            public override int ByteLength
            {
                get
                {
                    return m_nByteLength;
                }
            }
            public DataFieldTypedValue(string strName, int nPositon, T data, FieldFlags flags)
                :this(strName,nPositon,data,FieldSize(),flags)
            {

            }
            public DataFieldTypedValue(string strName, int nPositon, T data, int nFieldSize, FieldFlags flags)
                : base(strName, nPositon, flags)
            {
                Data = data;
                m_nByteLength = nFieldSize;
            
            }

            private static int? s_FieldSize = null;
            private static int FieldSize()
            {
                if (s_FieldSize == null)
                {
                    var typeInfo = typeof(T).GetTypeInfo();
                    if (typeof(T) == typeof(Semantic32ByteArray))
                    {
                        s_FieldSize = 32;
                    }
                    else if (typeInfo.IsEnum)
                    {
                        s_FieldSize = (typeInfo.GetCustomAttribute<EnumBitSizeAttribute>()?.BitSize ?? 32) / 8;
                    }
                    else if (typeInfo.ImplementedInterfaces.Contains(typeof(ISemanticType)))
                    {
                        s_FieldSize = (typeInfo.GetCustomAttribute<SemanticTypeByteSizeAttribute>()?.ByteSize ?? 4);
                    }
                    else
                    {
                        s_FieldSize = Marshal.SizeOf<T>();
                    }
                }
                else { }

                return s_FieldSize.Value;
            }

            public override object GetData()
            {
                return Data;
            }

            public override void SetData(object objData)
            {
                Data =(T) objData;
            }
        }


        protected class DataFieldObjectValue: DataField
        {
            private int m_nByteLength = 0;
            public object Data { get; set; }

            public override int ByteLength
            {
                get
                {
                    return m_nByteLength;
                }
            }
            public DataFieldObjectValue(string strName, int nPositon, object data, int nFieldSize, FieldFlags flags)
                : base(strName, nPositon, flags)
            {
                Data = data;
                m_nByteLength = nFieldSize;

            }


            public override object GetData()
            {
                return Data;
            }

            public override void SetData(object objData)
            {
                Data = objData;
            }
        }
    }
}
