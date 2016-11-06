using Nuki.Communication.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

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
            List<byte> list = new List<byte>();
            foreach (var field in GetData(FieldFlags.PartOfMessage))
                list.AddRange(Serialize(field));

            list.AddRange(CRC16.NonZero.ComputeChecksumBytes(list));
            return list;
        }



        private static IEnumerable<byte> Serialize(DataField field)
        {
            if (field.Data == null)
            {
                return new byte[0];
            }
            else if (field.Data is Enum)
            {
                int nBitSize = field.Data.GetType().GetTypeInfo().
                    GetCustomAttribute<EnumBitSizeAttribute>()?.BitSize ?? 32;


                switch (nBitSize)
                {

                    case 8:
                        return BitConverter.GetBytes((byte)(field.Data));
                    case 16:
                        return BitConverter.GetBytes((UInt16)(field.Data));
                    case 64:
                        return BitConverter.GetBytes((UInt64)(field.Data));
                    case 32:
                        return BitConverter.GetBytes((UInt32)(field.Data));

                }
            }
            else if (field.Data is byte[])
            {
                return (byte[])field.Data;
            }

            throw new NotImplementedException();
        }
    }
}
