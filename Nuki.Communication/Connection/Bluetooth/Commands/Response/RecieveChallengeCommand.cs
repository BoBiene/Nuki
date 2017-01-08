using Nuki.Communication.API;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth.Commands.Response
{
    public class RecieveChallengeCommand : RecieveBaseCommand
    {
        public SmartLockNonce Nonce { get { return GetData<SmartLockNonce>(nameof(Nonce)); } }
        public RecieveChallengeCommand()
            : base(NukiCommandType.Challenge, InitFields())
        {
        }

        protected static IEnumerable<FieldParserBase> InitFields()
        {
            yield return new FieldParser<SmartLockNonce>(nameof(Nonce), 32, SeperateSemanticType((b) => new SmartLockNonce(b)) );
        }
    }
}
