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
    internal abstract class BluetoothGattCharacteristicConnection
    {
        private object syncroot = new object();
        private GattCharacteristic m_GattCharacteristic = null;
        private TaskCompletionSource<RecieveBaseCommand> m_responseWaitHandle = null;
        private RecieveBaseCommand m_cmdInProgress = null;
        public BluetoothGattCharacteristicConnection()
        {
        }
        public bool IsValid => m_GattCharacteristic != null;

        internal void SetConnection(GattCharacteristic characteristic)
        {
            Debug.WriteLine("SetConnection");
            if (m_GattCharacteristic != null)
                m_GattCharacteristic.ValueChanged -= M_GattCharacteristic_ValueChanged;
            m_GattCharacteristic = characteristic;
            m_GattCharacteristic.ValueChanged += M_GattCharacteristic_ValueChanged;
        }

        ~BluetoothGattCharacteristicConnection()
        {
            m_GattCharacteristic.ValueChanged -= M_GattCharacteristic_ValueChanged;
        }

        protected abstract bool TryGetRecieveBuffer(IBuffer value,out DataReader reader);

        private void M_GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var recieveBuffer = args.CharacteristicValue;
            Debug.WriteLine("M_GattCharacteristic_ValueChanged");
            lock (syncroot)
            {
                DataReader reader = null;
                if (TryGetRecieveBuffer(recieveBuffer, out reader))
                {
                    if (m_cmdInProgress == null)
                        m_cmdInProgress = ResponseCommandParser.Parse(reader);

                    if (m_cmdInProgress != null)
                    {
                        m_cmdInProgress.ProcessRecievedData(reader);
                        if (m_cmdInProgress.Complete)
                        {
                            var cmd = m_cmdInProgress;
                            m_cmdInProgress = null;
                            Debug.WriteLine($"Recieved Command {cmd}...");
                            if (m_responseWaitHandle?.TrySetResult(cmd) != true)
                            {
                                Debug.WriteLine($"Recieved Command {cmd} is not handlet...");
                            }
                            else
                            {
                                Debug.WriteLine("m_responseWaitHandle set");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Command not complete...");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Recieved unknown command");
                    }
                }
                else
                {

                }
            }
        }

        public Task<bool> Send(SendBaseCommand cmd)
        {
            return Send(cmd, 2000);
        }



        public async Task<bool> Send(SendBaseCommand cmd, int nTimeout)
        {
            bool blnRet = false;
            m_responseWaitHandle = new TaskCompletionSource<RecieveBaseCommand>();
            Debug.WriteLine($"Send Command {cmd}...");
            var writer = new DataWriter();
            if (await Send(cmd, writer))
            {
                var result = await m_GattCharacteristic.WriteValueAsync(writer.DetachBuffer());
                blnRet = result == GattCommunicationStatus.Success;
            }
            else { }
            return blnRet;
        }

        protected abstract Task<bool> Send(SendBaseCommand cmd, DataWriter writer);
        

        public async Task<RecieveBaseCommand> Recieve(int nTimeout)
        {
            RecieveBaseCommand retCommand = null;
            Debug.WriteLine("Await response...");
            Task completedTask = await Task.WhenAny(m_responseWaitHandle.Task, Task.Delay(nTimeout));

            if (completedTask == m_responseWaitHandle.Task)
            {
                Debug.WriteLine("Recieved command...");
                retCommand = m_responseWaitHandle.Task.Result;
            }
            else
            {
                Debug.WriteLine("Recieve timed out...");
                //Timeout
            }

            return retCommand;
        }
    }
}
