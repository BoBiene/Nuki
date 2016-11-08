using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public interface IConnectionContext
    {
        ClientPublicKey ClientPublicKey { get; }
        SmartLockPublicKey SmartLockPublicKey { get; }
        SharedKey SharedKey { get; }
        SmartLockNonce SmartLockNonce { get;}

        UniqueClientID UniqueClientID { get; }
        ClientNonce CreateNonce();
    }
}
