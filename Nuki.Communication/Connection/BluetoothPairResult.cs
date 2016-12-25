using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.Connection
{
    public class BluetoothPairResult
    {
        public BlutoothPairStatus Status { get; private set; }
        public NukiConnectionBinding ConnectionInfo { get; private set; }
        public BluetoothPairResult(BlutoothPairStatus status, NukiConnectionBinding connectionInfo)
        {
            Status = status;
            ConnectionInfo = connectionInfo;
        }
    }
}
