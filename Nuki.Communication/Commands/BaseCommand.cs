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
            int nPos = Interlocked.Increment(ref m_nFieldPointer);
            DataField field = new DataField(strName, nPos, data, flags);
            m_mapByPostion[field.Position] = field;
            m_mapByFieldName[field.Name] = field;
            return nPos;
        }
        
        protected void SetData(string strName, object objData)
        {
            m_mapByFieldName[strName].Data = objData;
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
        }
    }
}
