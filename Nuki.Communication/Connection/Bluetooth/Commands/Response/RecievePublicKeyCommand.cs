using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.SemanticTypes;

namespace Nuki.Communication.Commands.Response
{
    public class RecievePublicKeyCommand : RecieveBaseCommand
    {
        public SmartLockPublicKey PublicKey { get { return GetData<SmartLockPublicKey>(nameof(PublicKey)); } }
        public RecievePublicKeyCommand()
            : base(CommandTypes.PublicKey, InitFields())
        {
        }

        protected static IEnumerable<FieldParserBase> InitFields()
        {
            yield return new FieldParser<SmartLockPublicKey> (nameof(PublicKey), 32, SeperateSemanticType((b) => new SmartLockPublicKey(b)));
        }
    }
}
