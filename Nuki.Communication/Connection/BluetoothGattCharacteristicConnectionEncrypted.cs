using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Commands.Request;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection
{
    internal class BluetoothGattCharacteristicConnectionEncrypted : BluetoothGattCharacteristicConnection
    {
        
        protected override Task<bool> Send(SendBaseCommand cmd, DataWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetRecieveBuffer(IBuffer value, out DataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
