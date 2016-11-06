using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{
    public class RecievePublicKeyCommand : RecieveBaseCommand
    {
        public byte[] PublicKey { get { return GetData<byte[]>(nameof(PublicKey)); } }
        public RecievePublicKeyCommand(byte[] data)
            : base(CommandTypes.PublicKey, data, InitFields())
        {
        }

        protected static IEnumerable<FieldParser> InitFields()
        {
            yield return new FieldParser(nameof(PublicKey), 32, SubArray);
        }
    }
}
