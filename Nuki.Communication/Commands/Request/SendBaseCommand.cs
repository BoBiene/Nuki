using Nuki.Communication.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Commands.Request
{
    public abstract class SendBaseCommand : BaseCommand
    {


        public SendBaseCommand(CommandTypes type, int nNumberOfFields)
            : base(type, nNumberOfFields)
        {

        }





        public virtual IEnumerable<byte> Serialize()
        {
            return Serialize(FieldFlags.PartOfMessage,true);
        }
        protected virtual IEnumerable<byte> Serialize(FieldFlags flags, bool blnAddCrc)
        {
            List<byte> list = new List<byte>();
            foreach (var field in GetData(flags))
                list.AddRange(Serialize(field));

            if (blnAddCrc)
                list.AddRange(CRC16.NonZero.ComputeChecksumBytes(list));
            return list;
        }



        private static IEnumerable<byte> Serialize(DataField field)
        {
            var fieldData = field.GetData();
            if (fieldData == null)
            {
                return new byte[0];
            }
            else if (fieldData is Enum)
            {
                int nBitSize = fieldData.GetType().GetTypeInfo().
                    GetCustomAttribute<EnumBitSizeAttribute>()?.BitSize ?? 32;


                switch (nBitSize)
                {

                    case 8:
                        return BitConverter.GetBytes((byte)(fieldData));
                    case 16:
                        return BitConverter.GetBytes((UInt16)(fieldData));
                    case 64:
                        return BitConverter.GetBytes((UInt64)(fieldData));
                    case 32:
                        return BitConverter.GetBytes((UInt32)(fieldData));

                }
            }
            else if (fieldData is byte[])
            {
                return (byte[])fieldData;
            }
            else if (fieldData is SemanticByteArray)
            {
                return ((SemanticByteArray)fieldData).Value;
            }

            throw new NotImplementedException();
        }
    }
}
