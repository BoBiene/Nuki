using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Commands.Response
{
    public class RecieveAuthorization­IDCommand : RecieveBaseCommandAuthenticated
    {
        public UniqueClientID UniqueClientID { get { return GetData<UniqueClientID>(nameof(UniqueClientID)); } }
        public SmartLockUUID SmartLockUUID { get { return GetData<SmartLockUUID>(nameof(SmartLockUUID)); } }
        public SmartLockNonce SmartLockNonce { get { return GetData<SmartLockNonce>(nameof(SmartLockNonce)); } }
        public RecieveAuthorization­IDCommand(byte[] data)
                : base(CommandTypes.AuthorizationID, data, InitFields())
        {
        }


        protected static IEnumerable<FieldParserBase> InitFields()
        {
            yield return new FieldParser<UniqueClientID>(nameof(UniqueClientID), 4,
                (buffer, start, length) => new UniqueClientID(BitConverter.ToUInt32(buffer, start)));

            yield return new FieldParser<SmartLockUUID>(nameof(SmartLockUUID), 16, 
                SeperateSemanticType((b) => new SmartLockUUID(b)));

            yield return new FieldParser<SmartLockNonce>(nameof(SmartLockNonce), 32,
                SeperateSemanticType((b) => new SmartLockNonce(b)));
        }
    }
}
