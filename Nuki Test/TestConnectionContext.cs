using Nuki.Communication.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.SemanticTypes;

namespace Nuki_Test
{
    class TestConnectionContext : IConnectionContext
    {
        public ClientPublicKey ClientPublicKey
        {
            get;private set;
        }

        public SharedKey SharedKey
        {
            get; private set;
        }

        public SmartLockNonce SmartLockNonce
        {
            get; private set;
        }

        public SmartLockPublicKey SmartLockPublicKey
        {
            get; private set;
        }

        public UniqueClientID UniqueClientID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TestConnectionContext(byte[] clientPublicKey, byte[] sharedKey, byte[] smartLockNonce, byte[] smartLockPublicKey)
        {
            ClientPublicKey = new ClientPublicKey(clientPublicKey);
            SharedKey = new SharedKey(sharedKey);
            SmartLockNonce = new SmartLockNonce(smartLockNonce);
            SmartLockPublicKey = new SmartLockPublicKey(smartLockPublicKey);
        }

        public ClientNonce CreateNonce()
        {
            return new ClientNonce(new byte[32]);
        }
    }
}
