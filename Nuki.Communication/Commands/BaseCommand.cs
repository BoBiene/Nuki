using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            AddField(nameof(CommandType), type);
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

        protected int AddField(string strName, object data)
        {
            return AddField(strName, data, FieldFlags.All);
        }
        protected int AddField(string strName, object data, FieldFlags flags)
        {
            return AddField((nPos) => new DataFieldObjectValue(strName, nPos, data, flags));
        }

        protected int AddField(string strName, FieldFlags flags, Func<object> valueGetter)
        {
            return AddField((nPos) => new DataFieldAction(strName, nPos,flags,valueGetter));
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
                if (field.Flags.HasFlag(flag))
                    yield return field;
        }

        protected abstract class DataField
        {
            public string Name { get; private set; }
            public int Position { get; private set; }
            public FieldFlags Flags { get; private set; }

            public abstract object GetData();
            public abstract void SetData(object objData);

            public DataField(string strName, int nPositon, FieldFlags flags)
            {
                Name = strName;
                Position = nPositon;
                Flags = flags;
            }
        }

        protected class DataFieldAction : DataField
        {
            public Func<object> ValueGetter { get; private set; }
            public DataFieldAction(string strName, int nPositon, FieldFlags flags, Func<object> valueGetter) : base(strName, nPositon, flags)
            {
                ValueGetter = valueGetter;
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

        protected class DataFieldObjectValue : DataField
        {
            public object Data { get; set; }
            
            public DataFieldObjectValue(string strName, int nPositon, object data, FieldFlags flags)
                : base(strName,nPositon,flags)
            {
                Data = data;
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
