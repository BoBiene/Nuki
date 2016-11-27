﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.Commands.Request;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection
{
    internal class BluetoothGattCharacteristicConnectionPlain : BluetoothGattCharacteristicConnection
    {
        protected override bool TryGetRecieveBuffer(IBuffer value, out DataReader reader)
        {
            reader = DataReader.FromBuffer(value);
            reader.ByteOrder = ByteOrder.LittleEndian;
            return true;
        }

        protected override Task<bool> Send(SendBaseCommand cmd, DataWriter writer)
        {
            writer.WriteBytes(cmd.Serialize().ToArray());

            return Task.FromResult(true);
        }
    }
}
