using Nuki.Communication.Connection;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Settings
{
    public class NukiDeviceSetting
    {
        public string Name { get; set; }
        public BluetoothConnectionInfo ConnectionInfo { get; set; }

    }
}
