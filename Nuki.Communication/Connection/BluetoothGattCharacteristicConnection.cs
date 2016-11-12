using Nuki.Communication.Commands;
using Nuki.Communication.Commands.Request;
using Nuki.Communication.Commands.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection
{
    internal class BluetoothGattCharacteristicConnection
    {
        private GattCharacteristic m_GattCharacteristic = null;
        private TaskCompletionSource<RecieveBaseCommand> m_responseWaitHandle = null;
        private ResponseCommandParser m_commandParser = new ResponseCommandParser();
        private ConcurrentQueue<RecieveBaseCommand> m_recieveQueue = new ConcurrentQueue<RecieveBaseCommand>();

        public BluetoothGattCharacteristicConnection(GattCharacteristic characteristic)
        {
            m_GattCharacteristic = characteristic;
            m_GattCharacteristic.ValueChanged += M_GattCharacteristic_ValueChanged;

        }

        private void M_GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var recieveBuffer = args.CharacteristicValue;
            byte[] byData = new byte[recieveBuffer.Length];
            DataReader.FromBuffer(recieveBuffer).ReadBytes(byData);

            Debug.WriteLine("Got response: " + ByteHelper.ByteArrayToString(byData));
            var cmd  = m_commandParser.Parse(byData);
            if (cmd != null)
            {
                if (m_responseWaitHandle != null)
                {
                    m_responseWaitHandle?.SetResult(cmd);
                }
                else
                {
                    m_recieveQueue.Enqueue(cmd);
                }
            }
            else
            {
                Debug.WriteLine("Recieved unknown command");
            }
        }

        public async Task<bool> Send(SendBaseCommand cmd)
        {
            m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
            if (m_recieveQueue.Count > 0)
                m_recieveQueue = new ConcurrentQueue<RecieveBaseCommand>();
            var writer = new DataWriter();
            writer.WriteBytes(cmd.Serialize().ToArray());
            var result = await m_GattCharacteristic.WriteValueAsync(writer.DetachBuffer());

            return result == GattCommunicationStatus.Success;
        }

        public async Task<RecieveBaseCommand> Recieve(int nTimeout)
        {
            RecieveBaseCommand retCommand = null;
            if (m_recieveQueue.Count <= 0)
            {
                Task completedTask = await Task.WhenAny(m_responseWaitHandle.Task, Task.Delay(nTimeout));

                if (completedTask == m_responseWaitHandle.Task)
                {
                    var recieveBuffer = m_responseWaitHandle.Task.Result;
                }
                else
                {
                    //Timeout
                }

                m_responseWaitHandle = null;
            }
            else
            {
                m_recieveQueue.TryDequeue(out retCommand);
            }
            return retCommand;
        }
    }
}
