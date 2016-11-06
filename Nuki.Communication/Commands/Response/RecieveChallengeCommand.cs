using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{
    public class RecieveChallengeCommand : RecieveBaseCommand
    {
        public byte[] Nonce { get { return GetData<byte[]>(nameof(Nonce)); } }
        public RecieveChallengeCommand(byte[] data)
            : base(CommandTypes.Challenge, data, InitFields())
        {
        }

        protected static IEnumerable<FieldParser> InitFields()
        {
            yield return new FieldParser(nameof(Nonce), 32, SubArray);
        }
    }
}
