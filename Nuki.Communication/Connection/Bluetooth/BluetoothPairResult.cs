using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection.Bluetooth
{
    public class BluetoothPairResult
    {
        public BlutoothPairStatus Status { get; private set; }
        public NukiConnectionConfig ConnectionInfo { get; private set; }
        public BluetoothPairResult(BlutoothPairStatus status, NukiConnectionConfig connectionInfo)
        {
            Status = status;
            ConnectionInfo = connectionInfo;
        }
    }
}
