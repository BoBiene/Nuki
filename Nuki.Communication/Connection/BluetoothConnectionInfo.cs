using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public class BluetoothConnectionInfo
    {
        public ClientPublicKey ClientPublicKey { get; set; }
        public SharedKey SharedKey { get; set; }
        public UniqueClientID UniqueClientID { get; set; }
        public SmartLockPublicKey SmartLockPublicKey { get; set; }
        public SmartLockUUID SmartLockUUID { get; set; }

        public string DeviceID { get; set; }
    }
}
